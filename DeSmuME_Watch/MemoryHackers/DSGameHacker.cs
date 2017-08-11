using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace DeSmuME_Watch
{
    public class DSGameHacker : IMemoryHacker32Bit
    {

        int _cfPtr = 0x4FF5368; // Current Frame
        int _sslOff = 0x5EB7758; // Save State Load
        public int CurrentFrame
        {
            // v10: 4FF4808
            // v9: 4FF5368
            get { return mem.ReadInteger((IntPtr)_cfPtr + (int)mem.TargetProcess.MainModule.BaseAddress); }
            set { mem.WriteInteger((IntPtr)_cfPtr + (int)mem.TargetProcess.MainModule.BaseAddress, value); }
        }
        public bool justLoadedState
        {
            get { return mem.ReadInteger((IntPtr)_sslOff + (int)mem.TargetProcess.MainModule.BaseAddress) == 0; }
        }
        private void unsetJustLoadedState()
        {
            mem.WriteInteger(mem.TargetProcess.MainModule.BaseAddress + _sslOff, 1);
        }
        private int lastFrame = -1;

        public event Action Updated;

        public MemoryHackerWindows mem;
        public uint gameMemory { get; private set; }

        public string DeSmuMEVersion { get; private set; }
        public void FindMemory(string ver = "9")
        {
            DeSmuMEVersion = ver;
            uint emuPtr = 0xEE3FE0;
            _cfPtr = 0x4FF5368;
            if (ver == "9")
                ver = "DeSmuME_0.9.9_x86";
            else if (ver == "10")
            {
                emuPtr = 0xEEB4B8;
                _cfPtr = 0x4FF4808;
                ver = "DeSmuME_0.9.10_x86";
            }
            else if (ver == "11")
            {
                emuPtr = 0xEA0EB8;
                _cfPtr = 0x4FAA208;
                ver = "DeSmuME_0.9.11_x86";
            }
            else
            {
                emuPtr = 0x2CFF600;
                _cfPtr = 0x4CF35FC;
                ver = "DeSmuME_X432R_AVI_x86";
            }
            Process[] emus = Process.GetProcessesByName(ver);
            if (emus.Length == 0)
                MessageBox.Show("Could not find" + ver + ". Please open the program, start a movie, and try again.");

            mem = new MemoryHackerWindows(emus[0]);
            gameMemory = emuPtr + (uint)mem.TargetProcess.MainModule.BaseAddress;

            StartFrameAndStateChecks();
        }

        private void StartFrameAndStateChecks()
        {
            Thread varThread = new Thread(FrameAndStateChecks);
            varThread.Start();
        }
        private void FrameAndStateChecks()
        {
            do
            {
                WaitForFrameOrState();
            } while (!stop);
        }
        private void WaitForFrameOrState()
        {
            do
            {
                Thread.Yield();
                Thread.Sleep(2);
                // State loaded?
                if (justLoadedState)
                {
                    //Debug.Print("Loaded. Frame is " + CurrentFrame);
                    unsetJustLoadedState();
                    // wait until 32D7FA4 turns to 1
                    while (CurrentFrame == 0)
                        Thread.Sleep(1);
                    //Debug.Print("Frame is now " + CurrentFrame);
                    break;
                }
                // Frame advanced? Assume that if CurrentFrame is 0, a state is being loaded.
                if (CurrentFrame != lastFrame && CurrentFrame != 0)
                {
                    //Debug.Print(CurrentFrame.ToString());
                    break;
                }
            } while (!forceUpdate && !stop);
            lastFrame = CurrentFrame;
            if (Updated != null && !stop)
                Updated();
            forceUpdate = false;
        }
        private bool forceUpdate = false;
        public void ForceUpdate()
        { forceUpdate = true; }

        // Stop so the program can close
        bool stop = false;
        public void Stop()
        {
            stop = true;
        }

        public uint GetAddress(IWatch32Bit watch)
        {
            uint address = (uint)watch.offsets[0];
            for (int i = 1; i < watch.offsets.Length; i++)
                address = ReadUInteger(address) + watch.offsets[i];

            return address;
        }
        public uint GetCEAddress(IWatch32Bit watch)
        { return GetAddress(watch) + gameMemory; }

        private uint ConvertFromEmuPtr(uint emuPtr)
        {
            // These ranges are in the emulator's memory in order of 0x01 range, 0x027 range, 0x02 range. Nothing between them.
            if (emuPtr >= 0x02000000 && emuPtr < 0x02400000)
                return emuPtr + gameMemory;
            else if (emuPtr >= 0x01000000 && emuPtr < 0x01008000)
                return emuPtr + gameMemory + 0xFF4000;
            else if (emuPtr >= 0x027E0000 && emuPtr < 0x027E4000)
                return emuPtr + gameMemory - 0x7E4000;
            else
                throw new Exception("Attempted to read an emulated address that does not exist!");
        }

        public int ReadInteger(uint p)
        {
            p += gameMemory;
            return mem.ReadInteger((IntPtr)p);
        }
        public void WriteInteger(uint p, int val)
        {
            p += gameMemory;
            mem.WriteInteger((IntPtr)p, val);
        }
        public uint ReadUInteger(uint p)
        {
            p += gameMemory;
            return mem.ReadUInteger((IntPtr)p);
        }
        public void WriteUInteger(uint p, uint val)
        {
            p += gameMemory;
            mem.WriteUInteger((IntPtr)p, val);
        }

        public short ReadShort(uint p)
        {
            p += gameMemory;
            return mem.ReadShort((IntPtr)p);
        }
        public void WriteShort(uint p, short val)
        {
            p += gameMemory;
            mem.WriteShort((IntPtr)p, val);
            return;
        }

        public byte ReadByte(uint p)
        {
            p += gameMemory;
            return mem.ReadByte((IntPtr)p);
        }
        public void WriteByte(uint p, byte val)
        {
            p += gameMemory;
            mem.WriteByte((IntPtr)p, val);
            return;
        }

        public byte[] ReadBytes(uint p, int len)
        {
            p += gameMemory;
            return mem.ReadMemory((IntPtr)p, len);
        }
        public void WriteBytes(uint p, byte[] val)
        {
            p += gameMemory;
            mem.WriteMemory((IntPtr)p, val);
        }

        public void Dispose()
        {
            mem.Dispose();
        }
    }
}
