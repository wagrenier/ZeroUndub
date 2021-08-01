using System;
using System.IO;

namespace ZeroUndubProcess
{
    public sealed class WriterIsoHandler
    {
        private readonly long _imgHdBinStartAddress;
        private readonly long _imgBdBinStartAddress;
        private readonly BinaryWriter _writer;

        public WriterIsoHandler(FileInfo isoFile, long ImgHdBinStartAddress, long ImgBdBinStartAddress)
        {
            _writer = new BinaryWriter(File.OpenWrite(isoFile.FullName));
            _imgHdBinStartAddress = ImgHdBinStartAddress;
            _imgBdBinStartAddress = ImgBdBinStartAddress;
        }

        public void PatchByteAtAbsoluteOffset(long offset, byte patchByte)
        {
            this._writer.BaseStream.Seek(offset, SeekOrigin.Begin);
            this._writer.Write(patchByte);
            
            // 4th writing does not work if this is not here, wtf
            this._writer.BaseStream.Seek(0, SeekOrigin.Begin);
        }
        
        public void PatchBytesAtAbsoluteOffset(long offset, byte[] patchByte)
        {
            this._writer.BaseStream.Seek(offset, SeekOrigin.Begin);
            this._writer.Write(patchByte);
            
            // 4th writing does not work if this is not here, wtf
            this._writer.BaseStream.Seek(0, SeekOrigin.Begin);
        }
        
        public void WriteSubtitleNewText(ZeroFile subtitleFile, int subtitleOverallOffset, byte[] bytes)
        {
            this.SeekSubtitleAddressText(subtitleFile, subtitleOverallOffset);
            this._writer.Write(bytes);
            this._writer.Write(0xFF);
        }
        
        private void SeekSubtitleAddressText(ZeroFile subtitleFile, int subtitleOverallOffset)
        {
            this.SeekFile(subtitleFile);
            var textFileOffset = subtitleOverallOffset + EuIsoConstants.SubtitleTextOffset;

            this._writer.BaseStream.Seek(textFileOffset, SeekOrigin.Current);
        }

        public void WriteSubtitleNewAddress(ZeroFile subtitleFile, int subtitleId, int newAddress)
        {
            this.SeekSubtitleAddressIndex(subtitleFile, subtitleId);
            this._writer.Write(newAddress + EuIsoConstants.SubtitleTextOffset);
        }

        private void SeekSubtitleAddressIndex(ZeroFile subtitleFile, int subtitleId)
        {
            this.SeekFile(subtitleFile);
            var textFileOffset = subtitleId * 0x4 + EuIsoConstants.SubtitleIndexOffset;

            this._writer.BaseStream.Seek(textFileOffset, SeekOrigin.Current);
        }
        
        public void OverwriteFile(ZeroFile zeroFile, byte[] buffer)
        {
            var newFileSize = buffer.Length;

            if (newFileSize > (int) zeroFile.Size)
            {
                Console.WriteLine($"File {zeroFile.FileId} cannot be undubbed.");
            }
            
            this.WriteNewSizeFile(zeroFile, newFileSize);
            
            this.SeekFile(zeroFile);
            this._writer.Write(buffer);
        }
        
        private void SeekHdOffset(long offset)
        {
            this._writer.BaseStream.Seek(_imgHdBinStartAddress, SeekOrigin.Begin);
            this._writer.BaseStream.Seek(offset, SeekOrigin.Current);
        }

        private void SeekFile(ZeroFile zeroFile)
        {
            this._writer.BaseStream.Seek(_imgBdBinStartAddress, SeekOrigin.Begin);
            this._writer.BaseStream.Seek(zeroFile.Offset, SeekOrigin.Current);
        }
        
        private void WriteNewSizeFile(ZeroFile zeroFile, int newSize)
        {
            var fileSizeInfoOffset = zeroFile.FileId * 0x8 + 0x4;
            this.SeekHdOffset(fileSizeInfoOffset);
            this._writer.Write(newSize);
        }
    }
}