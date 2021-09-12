using System.IO;

namespace ZeroUndubProcess
{
    public sealed class WriterIsoHandler
    {
        private readonly long _imgBdBinStartAddress;
        private readonly long _imgHdBinStartAddress;
        private long FileArchiveEndIsoAddress;
        private long FileArchiveEndAddress;
        private readonly BinaryWriter _writer;

        public WriterIsoHandler(FileInfo isoFile, long ImgHdBinStartAddress, long ImgBdBinStartAddress)
        {
            _writer = new BinaryWriter(File.OpenWrite(isoFile.FullName));
            FileArchiveEndIsoAddress = EuIsoConstants.IsoEndAddress;
            FileArchiveEndAddress = EuIsoConstants.ImgBdSize + EuIsoConstants.IsoEndAddress - EuIsoConstants.ImgBdBinEndAddress;
            _imgHdBinStartAddress = ImgHdBinStartAddress;
            _imgBdBinStartAddress = ImgBdBinStartAddress;
        }

        public void Close()
        {
            _writer.Close();
        }

        public void PatchByteAtAbsoluteOffset(long offset, byte patchByte)
        {
            _writer.BaseStream.Seek(offset, SeekOrigin.Begin);
            _writer.Write(patchByte);

            // 4th writing does not work if this is not here, wtf
            _writer.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public void PatchBytesAtAbsoluteOffset(long offset, byte[] patchByte)
        {
            _writer.BaseStream.Seek(offset, SeekOrigin.Begin);
            _writer.Write(patchByte);

            // 4th writing does not work if this is not here, wtf
            _writer.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public void OverwriteFile(ZeroFile zeroFile, byte[] buffer)
        {
            var newFileSize = buffer.Length;

            if (newFileSize > (int) zeroFile.Size)
            {
                AppendFile(zeroFile, buffer);
                return;
            }

            WriteNewSizeFile(zeroFile, newFileSize);

            SeekFile(zeroFile);
            _writer.Write(buffer);
        }

        private void AppendFile(ZeroFile zeroFile, byte[] fileContent)
        {
            _writer.BaseStream.Seek(FileArchiveEndIsoAddress, SeekOrigin.Begin);
            var startAddress = (uint) FileArchiveEndAddress / Ps2Constants.SectorSize;
            
            _writer.Write(fileContent);
            
            var blankBytes = Ps2Constants.SectorSize - _writer.BaseStream.Position % Ps2Constants.SectorSize;
            
            WriteEmptyByte((int) blankBytes);

            FileArchiveEndIsoAddress += fileContent.Length + blankBytes;
            FileArchiveEndAddress += fileContent.Length + blankBytes;
            
            WriteNewAddressFile(zeroFile, startAddress);
            WriteNewSizeFile(zeroFile, fileContent.Length);
        }

        private void WriteNewSizeFile(ZeroFile zeroFile, int newSize)
        {
            var fileSizeInfoOffset = zeroFile.FileId * 0x8 + 0x4;
            SeekHdOffset(fileSizeInfoOffset);
            _writer.Write(newSize);
        }
        
        private void WriteNewAddressFile(ZeroFile zeroFile, uint address)
        {
            var fileSizeInfoOffset = zeroFile.FileId * 0x8;
            SeekHdOffset(fileSizeInfoOffset);
            _writer.Write(address);
        }

        private void SeekHdOffset(long offset)
        {
            _writer.BaseStream.Seek(_imgHdBinStartAddress, SeekOrigin.Begin);
            _writer.BaseStream.Seek(offset, SeekOrigin.Current);
        }

        private void SeekFile(ZeroFile zeroFile)
        {
            _writer.BaseStream.Seek(_imgBdBinStartAddress, SeekOrigin.Begin);
            _writer.BaseStream.Seek(zeroFile.Offset, SeekOrigin.Current);
        }
        
        private void WriteEmptyByte(int numBlankBytes)
        {
            for (var i = 0; i < numBlankBytes; i++)
            {
                _writer.Write(0x0);
            }
        }
    }
}