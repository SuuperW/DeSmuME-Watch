using System;

namespace DeSmuME_Watch
{
    public interface IMemoryHacker32Bit : IDisposable
    {
        byte[] ReadBytes(uint MemoryAddress, int Count);
        void WriteBytes(uint MemoryAddress, byte[] val);

        byte ReadByte(uint MemoryAddress);
        void WriteByte(uint MemoryAddress, byte val);

        short ReadShort(uint MemoryAddress);
        void WriteShort(uint MemoryAddress, short val);

        int ReadInteger(uint MemoryAddress);
        void WriteInteger(uint MemoryAddress, int val);
        uint ReadUInteger(uint MemoryAddress);
        void WriteUInteger(uint MemoryAddress, uint val);

        event Action Updated;
    }
}
