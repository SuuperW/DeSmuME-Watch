using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeSmuME_Watch
{
    public class WatchManager32Bit
    {
        public WatchManager32Bit(IMemoryHacker32Bit memoryHacker)
        {
            memHacker = memoryHacker;
        }

        private IMemoryHacker32Bit memHacker;
        private SortedDictionary<int, IWatch32Bit> vars = new SortedDictionary<int, IWatch32Bit>();
        private int _nextWatchID = 0;

        public int AddWatch(IWatch32Bit w)
        {
            vars.Add(_nextWatchID, w);
            _nextWatchID++;
            return _nextWatchID - 1;
        }
        public IWatch32Bit GetWatch(int handle) { return vars[handle]; }
        public void RemoveWatch(int handle) { vars.Remove(handle); }
        public bool ContainsWatch(int handle) { return vars.ContainsKey(handle); }

        private uint GetAddress(IWatch32Bit watch)
        {
            uint address = watch.offsets[0];
            for (int i = 1; i < watch.offsets.Length; i++)
                address = memHacker.ReadUInteger(address) + watch.offsets[i];

            return address;
        }

        // Start looking at variables on a separate thread.
        public void Start()
        {
            memHacker.Updated += UpdateWatches;
        }

        public event Action Updated;
        public void UpdateWatches()
        {
            for (int i = 0; i < vars.Count; i++)
                UpdateValue(vars.ElementAt(i).Value);

            if (Updated != null)
                Updated();
        }
        public void UpdateValue(IWatch32Bit watch)
        {
            watch.updateValue(memHacker.ReadInteger(GetAddress(watch)));
        }
        public bool WriteValueToWatch(int handle, double val)
        {
            IWatch32Bit watch = GetWatch(handle);
            if (watch.type == WatchTypes.T_INT)
                memHacker.WriteInteger(GetAddress(watch), (int)val);
            else if (watch.type == WatchTypes.T_SHORT)
                memHacker.WriteShort(GetAddress(watch), (short)val);
            else if (watch.type == WatchTypes.T_BYTE)
                memHacker.WriteShort(GetAddress(watch), (byte)val);
            else if (watch.type == WatchTypes.T_FLOAT)
                memHacker.WriteInteger(GetAddress(watch), (int)(val * 4096));
            else
                return false;

            return true;
        }

        // Stop so the program can close
        public void Stop()
        {
            memHacker.Updated -= UpdateWatches;
        }

    }
}
