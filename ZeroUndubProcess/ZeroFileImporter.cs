using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ZeroUndubProcess
{
    public sealed class ZeroFileImporter
    {
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

        public int UndubbedFiles { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        private FileInfo JpIsoFile { get; }
        private FileInfo EuIsoFileRead { get; }
        private FileInfo EuIsoFileWrite { get; }
        private Options UndubOptions { get; }
        private ReaderIsoHandler _jpReaderHandler { get; }
        private ReaderIsoHandler _euReaderHandler { get; }
        private WriterIsoHandler _euWriterHandler { get; }

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
                        AudioUndub(zeroFile);
                        PssUndub(zeroFile);
                    }

                    if (UndubOptions.IsModelImport)
                    {
                        SwapModels(zeroFile);
                    }

                    if (UndubOptions.IsSubtitleInject)
                    {
                        InjectNewSubtitles(zeroFile);
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

            PatchBinarySubtitle();

            var subtitleOverallOffset = 0x0;
            var subtitles = JsonSerializer.Deserialize<List<SubtitleFile>>(File.ReadAllText("transcribe_real.json"));

            for (var i = 0; i < EuIsoConstants.NumberSubtitles + 46; i++)
            {
                var textInject = subtitles[i].Text;

                textInject = TextUtils.LineSplit(textInject);
                _euWriterHandler.WriteSubtitleNewAddress(zeroFile, i, subtitleOverallOffset);

                var strBytes = TextUtils.ConvertToBytes(textInject);

                _euWriterHandler.WriteSubtitleNewText(zeroFile, subtitleOverallOffset, strBytes);
                subtitleOverallOffset += strBytes.Length + 1;
            }

            _euWriterHandler.WriteNewSizeFile(zeroFile, (int) zeroFile.Size + 100000);
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
                var jpBuffer = _jpReaderHandler.ExtractFileFromAbsoluteAddress(pss[i].JpOffset, (int) pss[i].JpSize);

                if (pss[i].JpSize <= pss[i].EuSize)
                {
                    _euWriterHandler.PatchBytesAtAbsoluteOffset(pss[i].EuOffset, jpBuffer);
                    _euWriterHandler.PatchBytesAtAbsoluteOffset(pss[i].EuSizeOffset,
                        BitConverter.GetBytes(pss[i].JpSize));
                    continue;
                }

                CreateFile($"{EuIsoFileRead.DirectoryName}/{i}_jp.PSS", jpBuffer);

                var euBuffer = _euReaderHandler.ExtractFileFromAbsoluteAddress(pss[i].EuOffset, (int) pss[i].EuSize);
                CreateFile($"{EuIsoFileRead.DirectoryName}/{i}_eu.PSS", euBuffer);

                ExternalProcess.PssSwitchAudio(i, EuIsoFileRead.DirectoryName);

                var newVideoBuffer = File.ReadAllBytes($"{EuIsoFileRead.DirectoryName}/{i}.PSS");

                File.Delete($"{EuIsoFileRead.DirectoryName}/{i}.PSS");

                if (newVideoBuffer.Length > pss[i].EuSize)
                {
                    Console.WriteLine($"File {pss[i].Filename} is too big to be imported");
                }

                _euWriterHandler.PatchBytesAtAbsoluteOffset(pss[i].EuOffset, newVideoBuffer);
                _euWriterHandler.PatchBytesAtAbsoluteOffset(pss[i].EuSizeOffset,
                    BitConverter.GetBytes(newVideoBuffer.Length));
            }
        }

        private void CreateFile(string fileName, byte[] buffer)
        {
            var file = File.Create(fileName);
            file.Write(buffer);
            file.Close();
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
            if (euFile.FileId < EuIsoConstants.AudioStartIndex ||
                euFile.FileId == EuIsoConstants.NumberFiles - 1)
            {
                return;
            }

            var jpFileIndex = euFile.FileId - (EuIsoConstants.AudioStartIndex - JpIsoConstants.AudioStartIndex);
            var file_buffer = _jpReaderHandler.ExtractFileContent(_jpReaderHandler.ExtractFileInfo(jpFileIndex));

            _euWriterHandler.OverwriteFile(euFile, file_buffer);
        }
    }
}