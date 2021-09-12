using System.Collections.Generic;
using System.Linq;
using ZeroUndubProcess.GameText;

namespace ZeroUndubProcess
{
    public class ArrayAsStream
    {
        private byte[] _stream;
        private List<byte> _tempBuffer = new List<byte>(); 
        private int _position = 0;

        public ArrayAsStream(byte[] buffer)
        {
            _stream = buffer;
        }

        public void SeekAbsolute(int position)
        {
            _position = position;
        }

        public void SeekRelative(int position)
        {
            _position += position;
        }

        public void Write(byte[] bufferWrite)
        {
            var overflow = false;
            foreach (var byteToWrite in bufferWrite)
            {
                if (_position >= _stream.Length)
                {
                    overflow = true;
                    _tempBuffer.Add(byteToWrite);
                    _position += 1;
                    continue;
                }
                
                _stream[_position] = byteToWrite;
                _position += 1;
            }

            if (!overflow)
            {
                return;
            }

            _stream = _stream.Concat(_tempBuffer).ToArray();
            _tempBuffer.Clear();
        }

        public byte[] Read(int size)
        {
            var returnValue = _stream.SubArray(_position, size);
            _position += size;
            
            return returnValue;
        }

        public byte[] GetBuffer()
        {
            return _stream;
        }
    }
}