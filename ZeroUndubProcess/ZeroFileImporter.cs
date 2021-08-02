using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ZeroUndubProcess
{
    public sealed class ZeroFileImporter
    {
        public int UndubbedFiles { get; private set; }
        public bool IsCompleted { get; private set; } = false;
        public bool IsSuccess { get; private set; } = false;
        public string ErrorMessage { get; private set; }
        private FileInfo JpIsoFile { get; set; }
        private FileInfo EuIsoFileRead { get; set; }
        private FileInfo EuIsoFileWrite { get; set; }
        private Options UndubOptions { get; set; }
        private ReaderIsoHandler _jpReaderHandler { get; set; }
        private ReaderIsoHandler _euReaderHandler { get; set; }
        private WriterIsoHandler _euWriterHandler { get; set; }
        
        public ZeroFileImporter(string euIsoFile, string jpIsoFile, Options options)
        {
            UndubOptions = options;
            EuIsoFileRead = new FileInfo(euIsoFile);
            
            File.Copy(EuIsoFileRead.FullName, $"{EuIsoFileRead.DirectoryName}/pz_restored.iso");
            EuIsoFileWrite = new FileInfo($"{EuIsoFileRead.DirectoryName}/pz_restored.iso");
            JpIsoFile = new FileInfo(jpIsoFile);

            _jpReaderHandler = new ReaderIsoHandler(JpIsoFile, JpIsoConstants.ImgHdBinStartAddress,
                JpIsoConstants.ImgBdBinStartAddress);
            
            _euReaderHandler = new ReaderIsoHandler(EuIsoFileRead, EuIsoConstants.ImgHdBinStartAddress,
                EuIsoConstants.ImgBdBinStartAddress);
            
            _euWriterHandler = new WriterIsoHandler(EuIsoFileWrite, EuIsoConstants.ImgHdBinStartAddress,
                EuIsoConstants.ImgBdBinStartAddress);
        }

        public void RestoreGame()
        {
            try
            {
                for (var i = 0; i < EuIsoConstants.NumberFiles; i++)
                {
                    UndubbedFiles = i;
                    var zeroFile = _euReaderHandler.ExtractFileInfo(i);
                    Console.WriteLine($"FileId: {zeroFile.FileId}, Offset: {zeroFile.Offset}, Size: {zeroFile.Size}");

                    if (UndubOptions.IsUndub)
                    {
                        this.AudioUndub(zeroFile);
                        this.PssUndub(zeroFile);
                    }

                    if (UndubOptions.IsModelImport)
                    {
                        this.SwapModels(zeroFile);
                    }
                    
                    if (UndubOptions.IsSubtitleInject)
                    {
                        this.InjectNewSubtitles(zeroFile);
                    }
                }

                IsSuccess = true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                IsSuccess = false;
            }

            IsCompleted = true;
        }

        private void InjectNewSubtitles(ZeroFile zeroFile)
        {
            if (!UndubOptions.IsSubtitleInject || zeroFile.FileId != 40)
            {
                return;
            }
            
            this.PatchBinarySubtitle();
            
            var subtitleOverallOffset = 0x0;
            var subtitles = JsonSerializer.Deserialize<List<SubtitleFile>>(File.ReadAllText("transcribe_real.json"));

            for (var i = 0; i < EuIsoConstants.NumberSubtitles + 46; i++)
            {
                var textInject = "";
                if (i > EuIsoConstants.NumberSubtitles - 1)
                {
                    textInject = subtitles[0].Text;
                }
                else
                {
                    textInject = subtitles[i].Text;
                }
                
                textInject = TextUtils.LineSplit(textInject);
                _euWriterHandler.WriteSubtitleNewAddress(zeroFile, i, subtitleOverallOffset);
                
                var strBytes = TextUtils.ConvertToBytes(textInject);
                
                _euWriterHandler.WriteSubtitleNewText(zeroFile, subtitleOverallOffset, strBytes);
                subtitleOverallOffset += strBytes.Length + 1;
            }
        }

        private void PssUndub(ZeroFile zeroFile)
        {
            if (zeroFile.FileId != 0)
            {
                return;
            }

            var pss = JsonSerializer.Deserialize<List<PssInfo>>(File.ReadAllText("pss_info.json"));

            var pssLength = pss.Count;

            for (var i = 0; i < pssLength; i++)
            {
                if (pss[i].JpSize > pss[i].EuSize)
                {
                    continue;
                }
                
                var jpBuffer = this._jpReaderHandler.ExtractFileFromAbsoluteAddress(pss[i].JpOffset, (int) pss[i].JpSize);
                
                this._euWriterHandler.PatchBytesAtAbsoluteOffset(pss[i].EuOffset, jpBuffer);
                this._euWriterHandler.PatchBytesAtAbsoluteOffset(pss[i].EuSizeOffset, BitConverter.GetBytes(pss[i].JpSize));
            }
        }

        private void PatchBinarySubtitle()
        {
            _euWriterHandler.PatchByteAtAbsoluteOffset(0x25711B, 0x14);
            _euWriterHandler.PatchByteAtAbsoluteOffset(0x257153, 0x14);
            _euWriterHandler.PatchByteAtAbsoluteOffset(0x257313, 0x10);
            _euWriterHandler.PatchByteAtAbsoluteOffset(0x261BB3, 0x14);
        }

        private void SwapModels(ZeroFile euFile)
        {
            var jpFileIndex = 0;

            switch (euFile.FileId)
            {
                case 214:
                    jpFileIndex = 78;
                    break;
                case 814:
                    jpFileIndex = 495;
                    break;
                case 1062:
                    jpFileIndex = 743;
                    break;
                case 1063:
                    jpFileIndex = 744;
                    break;
                case 1064:
                    jpFileIndex = 745;
                    break;
                case 1065:
                    jpFileIndex = 746;
                    break;
                case 1066:
                    jpFileIndex = 747;
                    break;
                case 1067:
                    jpFileIndex = 748;
                    break;
                default:
                    return;
            }

            var file_buffer = _jpReaderHandler.ExtractFileContent(_jpReaderHandler.ExtractFileInfo(jpFileIndex));

            _euWriterHandler.OverwriteFile(euFile, file_buffer);
        }

        private void AudioUndub(ZeroFile euFile)
        {
            if (euFile.FileId < EuIsoConstants.AudioStartIndex || euFile.FileId == EuIsoConstants.NumberFiles - 1)
            {
                return;
            }
            
            var jpFileIndex = euFile.FileId - (EuIsoConstants.AudioStartIndex - JpIsoConstants.AudioStartIndex);
            var file_buffer = _jpReaderHandler.ExtractFileContent(_jpReaderHandler.ExtractFileInfo(jpFileIndex));

            _euWriterHandler.OverwriteFile(euFile, file_buffer);
        }
    }
}