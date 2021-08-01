using System.IO;

namespace ZeroUndubProcess
{
    public sealed class ReaderIsoHandler
    {
        private readonly long _imgHdBinStartAddress;
        private readonly long _imgBdBinStartAddress;
        private readonly BinaryReader _reader;

        public ReaderIsoHandler(FileInfo isoFile, long ImgHdBinStartAddress, long ImgBdBinStartAddress)
        {
            _reader = new BinaryReader(File.OpenRead(isoFile.FullName));
            _imgHdBinStartAddress = ImgHdBinStartAddress;
            _imgBdBinStartAddress = ImgBdBinStartAddress;
        }

        private void SeekHdOffset(long offset)
        {
            this._reader.BaseStream.Seek(_imgHdBinStartAddress, SeekOrigin.Begin);
            this._reader.BaseStream.Seek(offset, SeekOrigin.Current);
        }

        private void SeekFile(ZeroFile zeroFile)
        {
            this._reader.BaseStream.Seek(_imgBdBinStartAddress, SeekOrigin.Begin);
            this._reader.BaseStream.Seek(zeroFile.Offset, SeekOrigin.Current);
        }

        public ZeroFile ExtractFileInfo(int fileId)
        {
            var fileInfoOffset = fileId * 0x8;
            this.SeekHdOffset(fileInfoOffset);
            var fileStartAddress = this._reader.ReadUInt32() * Ps2Constants.SectorSize;
            var fileSize = this._reader.ReadUInt32();
            
            return new ZeroFile
            {
                FileId = fileId,
                Offset = fileStartAddress,
                Size = fileSize
            };
        }

        public byte[] ExtractFileContent(ZeroFile zeroFile)
        {
            this.SeekFile(zeroFile);
            return this._reader.ReadBytes((int) zeroFile.Size);
        }

        public byte[] ExtractFileFromAbsoluteAddress(long offset, int size)
        {
            this._reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            return this._reader.ReadBytes(size);
        }
    }
}