using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RKernel.HSMEngine
{
    internal class VirtualMemory
    {
        private List<byte> _memory;
        private bool[] _validtable;
        public VirtualMemory(int capacity)
        {
            _memory = Enumerable.Repeat((byte)0x00, capacity).ToList();
            _validtable = Enumerable.Repeat(false, capacity).ToArray();
        }
        public byte this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_memory.Count)
                    throw new ArgumentOutOfRangeException();
                if (_validtable[index])
                    return _memory[index];
                else
                    throw new Exception("Cannot read from empty memory cell");
            }
            set
            {
                if ((uint)index >= (uint)_memory.Count)
                    throw new ArgumentOutOfRangeException();
                _memory[index] = value;
                _validtable[index] = true;
            }
        }
        public void Empty(int index) => _validtable[index] = false;
        public void Empty(int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
                _validtable[i] = false;
        }
        public void Load(List<byte> buffer, int startIndex)
        {
            int i = 0;
            int j = startIndex;
            for (; i < buffer.Count; i++, j++)
            {
                _memory[j] = buffer[i];
                _validtable[j] = true;
            }
        }
        public void Load(byte[] buffer, int startIndex)
        {
            int i = 0;
            int j = startIndex;
            for (; i < buffer.Length; i++, j++)
            {
                _memory[j] = buffer[i];
                _validtable[j] = true;
            }
        }
    }
}
