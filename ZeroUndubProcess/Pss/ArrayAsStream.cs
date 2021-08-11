namespace ZeroUndubProcess
{
    public class ArrayAsStream
    {
        private byte[] _stream;
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
            foreach (var byteToWrite in bufferWrite)
            {
                _stream[_position] = byteToWrite;
                _position += 1;
            }
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