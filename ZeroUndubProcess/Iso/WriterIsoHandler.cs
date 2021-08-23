using System;
using System.IO;
using ZeroUndubProcess.GameText;

namespace ZeroUndubProcess
{
    public sealed class WriterIsoHandler
    {
        private readonly long _imgBdBinStartAddress;
        private readonly long _imgHdBinStartAddress;
        private readonly BinaryWriter _writer;

        public WriterIsoHandler(FileInfo isoFile, long ImgHdBinStartAddress, long ImgBdBinStartAddress)
        {
            _writer = new BinaryWriter(File.OpenWrite(isoFile.FullName));
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

        public void WriteSubtitleNewText(ZeroFile subtitleFile, int subtitleOverallOffset, byte[] bytes)
        {
            SeekSubtitleAddressText(subtitleFile, subtitleOverallOffset);
            _writer.Write(bytes);
            _writer.Write(0xFF);
        }

        private void SeekSubtitleAddressText(ZeroFile subtitleFile, int subtitleOverallOffset)
        {
            SeekFile(subtitleFile);
            var textFileOffset = subtitleOverallOffset + EuIsoConstants.SubtitleTextOffset;

            _writer.BaseStream.Seek(textFileOffset, SeekOrigin.Current);
        }

        public void WriteSubtitleNewAddress(ZeroFile subtitleFile, int subtitleId, int newAddress)
        {
            SeekSubtitleAddressIndex(subtitleFile, subtitleId);
            _writer.Write(newAddress + EuIsoConstants.SubtitleTextOffset);
        }

        private void SeekSubtitleAddressIndex(ZeroFile subtitleFile, int subtitleId)
        {
            SeekFile(subtitleFile);
            var textFileOffset = subtitleId * 0x4 + EuIsoConstants.SubtitleIndexOffset;

            _writer.BaseStream.Seek(textFileOffset, SeekOrigin.Current);
        }

        public void OverwriteFile(ZeroFile zeroFile, byte[] buffer, ZeroFile nextFile = null)
        {
            var newFileSize = buffer.Length;

            if (newFileSize > (int) zeroFile.Size)
            {
                var availableBytes = Ps2Constants.SectorSize - zeroFile.Size % Ps2Constants.SectorSize;
                var fileSizeDifference = newFileSize - zeroFile.Size;

                if (availableBytes < fileSizeDifference)
                {
                    var nextFileOffset = nextFile.Offset / Ps2Constants.SectorSize;

                    var currFileEndSector =
                        (zeroFile.Offset + availableBytes + zeroFile.Size) / Ps2Constants.SectorSize;

                    var availableSectors = nextFileOffset - currFileEndSector;

                    var fullAvailableBytes = availableSectors * Ps2Constants.SectorSize;

                    if (fullAvailableBytes < fileSizeDifference)
                    {
                        var whiteSpace = 0;
                        for (var i = buffer.Length - 1; i > 0; i--)
                        {
                            if (buffer[i] != '\x00')
                            {
                                break;
                            }

                            whiteSpace += 1;
                        }

                        fullAvailableBytes += whiteSpace;

                        if (fullAvailableBytes < fileSizeDifference)
                        {
                            Console.WriteLine($"File {zeroFile.FileId} cannot be undubbed. Difference of {fileSizeDifference}, Full: {fullAvailableBytes}, Fits: {fullAvailableBytes >= fileSizeDifference}");
                    
                            // Force write, results unknown
                            return;
                        }

                        buffer = buffer.SubArray(0, buffer.Length - whiteSpace);
                        newFileSize = buffer.Length;
                    }
                }
            }

            WriteNewSizeFile(zeroFile, newFileSize);

            SeekFile(zeroFile);
            _writer.Write(buffer);
        }

        public void WriteNewSizeFile(ZeroFile zeroFile, int newSize)
        {
            var fileSizeInfoOffset = zeroFile.FileId * 0x8 + 0x4;
            SeekHdOffset(fileSizeInfoOffset);
            _writer.Write(newSize);
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
    }
}