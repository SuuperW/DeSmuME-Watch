using System;

using NLua;

// This namespace will be exposed to Lua scripts.
namespace DeSmuME_Watch.Lua
{
    public class Lua
    {
        public Lua(WatchManager32Bit watchManager)
        {
            this.watchManager = watchManager;
        }

        private WatchManager32Bit watchManager;

        public int GetWatchValueAsInt(int watchID)
        {
            return watchManager.GetWatch(watchID).cValAsInt;
        }
        public float GetWatchValueAsFixedPoint(int watchID)
        {
            return new DSFixedPoint4(watchManager.GetWatch(watchID).cValAsInt);
        }

        public string FormatIntegerValues(int currentValue, int change, bool displayChange = true, bool asHex = false, bool asSigned = true)
        {
            return SimpleWatchDisplay.FormatIntegerValues(currentValue, change, displayChange, asHex, asSigned);
        }
        public string FormatFixedPointValues(DSFixedPoint4 currentValue, DSFixedPoint4 change, bool displayChange = true, bool exactDisplay = true, int digitsPastRadix = 0)
        {
            return SimpleWatchDisplay.FormatFixedPointValues(currentValue, change, displayChange, exactDisplay, digitsPastRadix);
        }
        public string FormatFixedPointValues(float currentValue, float change, bool displayChange = true, bool exactDisplay = true, int digitsPastRadix = 0)
        {
            return SimpleWatchDisplay.FormatFixedPointValues(currentValue, change, displayChange, exactDisplay, digitsPastRadix);
        }
    }
}
