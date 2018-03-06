using System;

namespace DeSmuME_Watch
{
    public interface IMemoryHacker32Bit : IDisposable
    {
        byte[] ReadBytes(int MemoryAddress, int Count);
        void WriteBytes(int MemoryAddress, byte[] val);

        byte ReadByte(int MemoryAddress);
        void WriteByte(int MemoryAddress, byte val);

        short ReadShort(int MemoryAddress);
        void WriteShort(int MemoryAddress, short val);

        int ReadInteger(int MemoryAddress);
        void WriteInteger(int MemoryAddress, int val);
        uint ReadUInteger(int MemoryAddress);
        void WriteUInteger(int MemoryAddress, uint val);

        event Action Updated;
    }
}
