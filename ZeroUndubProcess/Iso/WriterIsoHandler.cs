using System.IO;
using System.Runtime.Intrinsics.X86;

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

        public void AppendFile(ZeroFile zeroFile, byte[] fileContent)
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
        
        public void WriteSubtitleNewText(ZeroFile subtitleFile, int subtitleOverallOffset, byte[] bytes)
        {
            SeekSubtitleAddressText(subtitleFile, subtitleOverallOffset);
            _writer.Write(bytes);
            _writer.Write(new byte[]{0xFF});
        }
        
        public void WriteSubtitleNewAddress(ZeroFile subtitleFile, int subtitleId, int newAddress)
        {
            SeekSubtitleAddressIndex(subtitleFile, subtitleId);
            _writer.Write(newAddress + EuIsoConstants.SubtitleTextOffset);
        }
        
        private void SeekSubtitleAddressText(ZeroFile subtitleFile, int subtitleOverallOffset)
        {
            SeekFile(subtitleFile);
            var textFileOffset = subtitleOverallOffset + EuIsoConstants.SubtitleTextOffset;

            _writer.Seek(textFileOffset, SeekOrigin.Current);
        }

        private void SeekSubtitleAddressIndex(ZeroFile subtitleFile, int subtitleId)
        {
            SeekFile(subtitleFile);
            var textFileOffset = subtitleId * 0x4 + EuIsoConstants.SubtitleIndexOffset;

            _writer.BaseStream.Seek(textFileOffset, SeekOrigin.Current);
        }

        public void WriteNewSizeFile(ZeroFile zeroFile, int newSize)
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

        public void FillIso()
        {
            _writer.Seek(0x0, SeekOrigin.End);

            var blankBytes = Ps2Constants.SectorSize - _writer.BaseStream.Position % Ps2Constants.SectorSize;
            
            WriteEmptyByte((int)blankBytes);
        }
    }
}