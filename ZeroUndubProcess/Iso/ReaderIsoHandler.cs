using System.IO;

namespace ZeroUndubProcess
{
    public sealed class ReaderIsoHandler
    {
        private readonly long _imgBdBinStartAddress;
        private readonly long _imgHdBinStartAddress;
        private readonly BinaryReader _reader;

        public ReaderIsoHandler(FileInfo isoFile, long ImgHdBinStartAddress, long ImgBdBinStartAddress)
        {
            _reader = new BinaryReader(File.OpenRead(isoFile.FullName));
            _imgHdBinStartAddress = ImgHdBinStartAddress;
            _imgBdBinStartAddress = ImgBdBinStartAddress;
        }

        public void Close()
        {
            _reader.Close();
        }

        private void SeekHdOffset(long offset)
        {
            _reader.BaseStream.Seek(_imgHdBinStartAddress, SeekOrigin.Begin);
            _reader.BaseStream.Seek(offset, SeekOrigin.Current);
        }

        private void SeekFile(ZeroFile zeroFile)
        {
            _reader.BaseStream.Seek(_imgBdBinStartAddress, SeekOrigin.Begin);
            _reader.BaseStream.Seek(zeroFile.Offset, SeekOrigin.Current);
        }

        public ZeroFile ExtractFileInfo(int fileId)
        {
            var fileInfoOffset = fileId * 0x8;
            SeekHdOffset(fileInfoOffset);
            var fileStartAddress = _reader.ReadUInt32() * Ps2Constants.SectorSize;
            var fileSize = _reader.ReadUInt32();

            return new ZeroFile
            {
                FileId = fileId,
                Offset = fileStartAddress,
                Size = fileSize
            };
        }

        public byte[] ExtractFileContent(ZeroFile zeroFile)
        {
            SeekFile(zeroFile);
            return _reader.ReadBytes((int) zeroFile.Size);
        }

        public byte[] ExtractFileFromAbsoluteAddress(long offset, int size)
        {
            _reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            return _reader.ReadBytes(size);
        }
    }
}