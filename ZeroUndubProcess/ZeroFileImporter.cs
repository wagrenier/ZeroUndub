using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ZeroUndubProcess
{
    public sealed class ZeroFileImporter
    {
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
        
        public ZeroFileImporter(string euIsoFile, string jpIsoFile, Options options)
        {
            UndubOptions = options;
            EuIsoFileRead = new FileInfo(euIsoFile);

            File.Copy(EuIsoFileRead.FullName, $"{EuIsoFileRead.DirectoryName}/pz_redux.iso");
            EuIsoFileWrite = new FileInfo($"{EuIsoFileRead.DirectoryName}/pz_redux.iso");
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

                    if (zeroFile.FileId == 660)
                    {
                        // Patch the title screen
                        _euWriterHandler.PatchBytesAtAbsoluteOffset(0x4179F830, TextUtils.NewSplashScreen);
                    }

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
            CloseFiles();
        }
        
        private void CloseFiles()
        {
            _euReaderHandler.Close();
            _euWriterHandler.Close();
            _jpReaderHandler.Close();
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
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                    jpFileIndex = 39;
                    SwapHomeMenu(euFile, jpFileIndex);
                    return;
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                    jpFileIndex = 40;
                    SwapHomeMenu(euFile, jpFileIndex);
                    return;
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                    jpFileIndex = 41;
                    SwapHomeMenu(euFile, jpFileIndex);
                    return;
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                    jpFileIndex = 42;
                    SwapHomeMenu(euFile, jpFileIndex);
                    return;
                case 214:
                    jpFileIndex = 78;
                    break;
                case 491:
                case 492:
                case 493:
                case 494:
                case 495:
                case 496:
                    jpFileIndex = 259;
                    SwapHomeMenu(euFile, jpFileIndex);
                    return;
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

        private void SwapHomeMenu(ZeroFile zeroFile, int jpFileIndex)
        {
            var file_buffer_jp = _jpReaderHandler.ExtractFileContent(_jpReaderHandler.ExtractFileInfo(jpFileIndex));
            var file_buffer_eu = _euReaderHandler.ExtractFileContent(zeroFile);

            var newBuffer = file_buffer_eu.SubArray(0, file_buffer_eu.Length);

            for (var i = 0; i < 11; i++)
            {
                var offsetEu = BitConverter.ToUInt32(file_buffer_eu.SubArray(0x10 + i * 0x4, 4));

                var offsetJp = BitConverter.ToUInt32(file_buffer_jp.SubArray(0x10 + i * 0x4, 4));
                var sizeJp = BitConverter.ToUInt32(file_buffer_jp.SubArray(offsetJp + 0x10, 4));

                for (var k = 0; k < sizeJp; k++)
                {
                    newBuffer[k + offsetEu] = file_buffer_jp[k + offsetJp];
                }
            }
            
            _euWriterHandler.OverwriteFile(zeroFile, newBuffer);
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