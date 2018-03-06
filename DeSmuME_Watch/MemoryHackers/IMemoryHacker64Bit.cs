using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSmuME_Watch
{
    public interface IMemoryHacker64Bit : IDisposable
    {
        byte[] ReadBytes(long MemoryAddress, int Count);
        void WriteBytes(long MemoryAddress, byte[] val);

        byte ReadByte(long MemoryAddress);
        void WriteByte(long MemoryAddress, byte val);

        short ReadShort(long MemoryAddress);
        void WriteShort(long MemoryAddress, short val);

        int ReadInteger(long MemoryAddress);
        void WriteInteger(long MemoryAddress, int val);
        uint ReadUInteger(long MemoryAddress);
        void WriteUInteger(long MemoryAddress, uint val);

        event Action Updated;
    }
}
