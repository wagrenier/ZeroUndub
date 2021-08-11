using System.Buffers.Binary;
using System.Linq;

namespace ZeroUndubProcess
{
    public sealed class PssAudioHandler
    {
        private readonly ArrayAsStream _original;
        private readonly ArrayAsStream _target;

        private PssAudioHandler(byte[] original, byte[] target)
        {
            _original = new ArrayAsStream(original);
            _target = new ArrayAsStream(target);
        }

        public static byte[] SwitchPssAudio(byte[] original, byte[] target)
        {
            var pssHandler = new PssAudioHandler(original, target);
            pssHandler.TransferAudio();
            return pssHandler._target.GetBuffer();
        }

        private void TransferAudio()
        {
            var originalAudioBuffer = _original.BuildFullAudioBuffer();

            _original.SeekNextAudioBlock();
            _target.SeekNextAudioBlock();

            var originalCurrentBlockSize = _original.InitialAudioBlock(out var originalAudioSize);
            var targetCurrentBlockSize = _target.InitialAudioBlock(out var targetAudioSize);
            
            _original.SeekRelative(originalCurrentBlockSize);
            
            _target.Write(originalAudioBuffer.Read(targetCurrentBlockSize));

            while (true)
            {
                var isDoneOriginal = _original.SeekNextAudioBlock();
                var isDoneTarget = _target.SeekNextAudioBlock();

                if (isDoneOriginal || isDoneTarget)
                {
                    break;
                }

                originalCurrentBlockSize = _original.AudioBlock();
                targetCurrentBlockSize = _target.AudioBlock();
                
                _original.SeekRelative(originalCurrentBlockSize);
                
                _target.Write(originalAudioBuffer.Read(targetCurrentBlockSize));
            }
        }
    }

    public static class ArrayAsStreamExtensionMethods
    {
        public static bool SeekNextAudioBlock(this ArrayAsStream pssBuffer)
        {
            while (true)
            {
                var blockId = pssBuffer.Read(0x4);

                if (blockId.SequenceEqual(PssConstants.PackStart))
                {
                    pssBuffer.SeekRelative(0xa);
                }
                else if (blockId.SequenceEqual(PssConstants.AudioSegment))
                {
                    return false;
                }
                else if (blockId.SequenceEqual(PssConstants.EndFile))
                {
                    return true;
                }
                else
                {
                    var blockSize = BinaryPrimitives.ReadUInt16BigEndian(pssBuffer.Read(0x2));
                    pssBuffer.SeekRelative(blockSize);
                }
            }
        }

        public static int InitialAudioBlock(this ArrayAsStream pssBuffer, out uint totalSize)
        {
            var blockSize = BinaryPrimitives.ReadUInt16BigEndian(pssBuffer.Read(0x2));
            pssBuffer.SeekRelative(0x3b - 0x6);

            totalSize = BinaryPrimitives.ReadUInt32LittleEndian(pssBuffer.Read(0x4));
            var dataSize = blockSize - PssConstants.FirstHeaderSize + 0x6;
            return dataSize;
        }

        public static int AudioBlock(this ArrayAsStream pssBuffer)
        {
            var blockSize = BinaryPrimitives.ReadUInt16BigEndian(pssBuffer.Read(0x2));
            
            pssBuffer.SeekRelative(-0x6);
            pssBuffer.SeekRelative(PssConstants.HeaderSize);
            
            var dataSize = blockSize - PssConstants.HeaderSize + 0x6;
            return dataSize;
        }

        public static ArrayAsStream BuildFullAudioBuffer(this ArrayAsStream pssBuffer)
        {
            pssBuffer.SeekNextAudioBlock();
            var currentBlockSize = pssBuffer.InitialAudioBlock(out var totalSize);

            var fullAudio = new ArrayAsStream(new byte[totalSize]);
            
            fullAudio.Write(pssBuffer.Read(currentBlockSize));

            while (true)
            {
                var isDone = pssBuffer.SeekNextAudioBlock();

                if (isDone)
                {
                    break;
                }

                currentBlockSize = pssBuffer.AudioBlock();
                
                fullAudio.Write(pssBuffer.Read(currentBlockSize));
            }
            
            fullAudio.SeekAbsolute(0);
            pssBuffer.SeekAbsolute(0);
            
            return fullAudio;
        }
    }
}