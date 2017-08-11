using System;

namespace DeSmuME_Watch
{
    public interface IWatch32Bit
    {
        uint[] offsets { get; }

        int type { get; }

        int cValAsInt { get; }
        int pValAsInt { get; }
        int getDiffAsInt();

        void updateValue(int newValue);
    }

    static class WatchTypes
    {
        public const int T_INVALID = 0x00;
        public const int T_BYTE = 0x01;
        public const int T_SHORT = 0x02;
        public const int T_INT = 0x04;
        public const int T_FLOAT = 0x08;
    }
}
