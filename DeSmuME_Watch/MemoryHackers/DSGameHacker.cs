using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace DeSmuME_Watch
{
    public class DSGameHacker : IMemoryHacker32Bit
    {
        long _cfPtr = 0x4FF5368; // Current Frame
        long _sslOff = 0x5EB7758; // Save State Load
        public int CurrentFrame
        {
            get { return mem.ReadInteger(new IntPtr(_cfPtr + mem.TargetProcess.MainModule.BaseAddress.ToInt64())); }
            set { mem.WriteInteger(new IntPtr(_cfPtr + (long)mem.TargetProcess.MainModule.BaseAddress), value); }
        }
        public bool justLoadedState
        {
            get { return mem.ReadInteger(new IntPtr(_sslOff + (long)mem.TargetProcess.MainModule.BaseAddress)) == 0; }
        }
        private void unsetJustLoadedState()
        {
            mem.WriteInteger(new IntPtr(mem.TargetProcess.MainModule.BaseAddress.ToInt64() + _sslOff), 1);
        }
        private int lastFrame = -1;

        public event Action Updated;

        public MemoryHackerWindows mem;
        public long gameMemory { get; private set; }

        public string DeSmuMEVersion { get; private set; }
        public void FindMemory(string ver = "9")
        {
            DeSmuMEVersion = ver;
            long emuPtr = 0xEE3FE0;
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
            else if (ver == "12")
            {
                emuPtr = 0x3a40300;
                _cfPtr = 0x4f4bbf0;
                ver = "DeSmuME_0.9.12.1";
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
            gameMemory = emuPtr + mem.TargetProcess.MainModule.BaseAddress.ToInt64();

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

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public int GetAddress(IWatch32Bit watch)
        {
            int address = (int)watch.offsets[0];
            for (int i = 1; i < watch.offsets.Length; i++)
                address = ReadInteger(address) + watch.offsets[i];

            return address;
        }
        public long GetCEAddress(IWatch32Bit watch)
        {
            return DSAddressToCEAddress(GetAddress(watch));
        }

        private long DSAddressToCEAddress(int emuPtr)
        {
            // These ranges are in the emulator's memory in order of 0x01 range, 0x027 range, 0x02 range. Nothing between them.
            if (emuPtr >= 0x02000000 && emuPtr < 0x02400000)
                return emuPtr + gameMemory;
            else if (emuPtr >= 0x01000000 && emuPtr < 0x01008000)
                return emuPtr + gameMemory + 0xFF4000;
            else if (emuPtr >= 0x027E0000 && emuPtr < 0x027E4000)
                return emuPtr + gameMemory - 0x7E4000;
            else
                return -1;
        }

        public int ReadInteger(int p)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr == -1) return 0;
            return mem.ReadInteger((IntPtr)ptr);
        }
        public void WriteInteger(int p, int val)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr != -1)
                mem.WriteInteger((IntPtr)ptr, val);
        }
        public uint ReadUInteger(int p)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr == -1) return 0;
            return mem.ReadUInteger((IntPtr)ptr);
        }
        public void WriteUInteger(int p, uint val)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr != -1)
                mem.WriteUInteger((IntPtr)ptr, val);
        }

        public short ReadShort(int p)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr == -1) return 0;
            return mem.ReadShort((IntPtr)ptr);
        }
        public void WriteShort(int p, short val)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr != -1)
                mem.WriteShort((IntPtr)ptr, val);
        }

        public byte ReadByte(int p)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr == -1) return 0;
            return mem.ReadByte((IntPtr)ptr);
        }
        public void WriteByte(int p, byte val)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr != -1)
                mem.WriteByte((IntPtr)ptr, val);
        }

        public byte[] ReadBytes(int p, int len)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr == -1) return null;
            return mem.ReadMemory((IntPtr)ptr, len);
        }
        public void WriteBytes(int p, byte[] val)
        {
            long ptr = DSAddressToCEAddress(p);
            if (ptr != -1)
                mem.WriteMemory((IntPtr)ptr, val);
        }

        public void Dispose()
        {
            mem.Dispose();
        }
    }
}
