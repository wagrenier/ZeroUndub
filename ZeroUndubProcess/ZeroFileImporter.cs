using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ZeroUndubProcess.GameText;

namespace ZeroUndubProcess
{
    public sealed class ZeroFileImporter
    {
        public int TotalFiles { get; private set; }
        public int FilesCompleted { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        private FileInfo JpIsoFile { get; }
        private FileInfo EuIsoFileRead { get; }
        private FileInfo EuIsoFileWrite { get; }
        private Options UndubOptions { get; }
        private ReaderIsoHandler JpReaderHandler { get; }
        private ReaderIsoHandler EuReaderHandler { get; }
        private WriterIsoHandler EuWriterHandler { get; }
        private List<PssInfo> PssInfos { get; }
        
        public ZeroFileImporter(string euIsoFile, string jpIsoFile, Options options)
        {
            UndubOptions = options;
            EuIsoFileRead = new FileInfo(euIsoFile);

            File.Copy(EuIsoFileRead.FullName, $"{EuIsoFileRead.DirectoryName}/pz_redux.iso");
            EuIsoFileWrite = new FileInfo($"{EuIsoFileRead.DirectoryName}/pz_redux.iso");
            JpIsoFile = new FileInfo(jpIsoFile);

            JpReaderHandler = new ReaderIsoHandler(JpIsoFile, JpIsoConstants.ImgHdBinStartAddress,
                JpIsoConstants.ImgBdBinStartAddress);

            EuReaderHandler = new ReaderIsoHandler(EuIsoFileRead, EuIsoConstants.ImgHdBinStartAddress,
                EuIsoConstants.ImgBdBinStartAddress);

            EuWriterHandler = new WriterIsoHandler(EuIsoFileWrite, EuIsoConstants.ImgHdBinStartAddress,
                EuIsoConstants.ImgBdBinStartAddress);
            
            PssInfos = JsonSerializer.Deserialize<List<PssInfo>>(PssConstants.PssIsoData);

            TotalFiles = PssInfos.Count + EuIsoConstants.NumberFiles;
            FilesCompleted = 0;
        }

        public void RestoreGame()
        {
            try
            {
                if (UndubOptions.IsUndub)
                {
                    PssUndub();
                }
                
                for (var i = 0; i < EuIsoConstants.NumberFiles; i++)
                {
                    var zeroFile = EuReaderHandler.ExtractFileInfo(i);

                    if (zeroFile.FileId == 660)
                    {
                        // Patch the title screen
                        EuWriterHandler.PatchBytesAtAbsoluteOffset(0x4179F830, IntroSplashScreen.NewSplashScreen);
                    }

                    if (UndubOptions.IsUndub)
                    {
                        AudioUndub(zeroFile);
                    }

                    if (UndubOptions.IsModelImport)
                    {
                        SwapModels(zeroFile);
                    }

                    if (UndubOptions.IsSubtitleInject)
                    {
                        InjectNewSubtitles(zeroFile);
                    }
                    
                    FilesCompleted += 1;
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
            EuReaderHandler.Close();
            EuWriterHandler.Close();
            JpReaderHandler.Close();
        }

        private void InjectNewSubtitles(ZeroFile zeroFile)
        {
            if (!UndubOptions.IsSubtitleInject || zeroFile.FileId != 40)
            {
                return;
            }

            PatchBinarySubtitle();

            var subtitleOverallOffset = 0x0;
            var subtitles = JsonSerializer.Deserialize<List<SubtitleFile>>(File.ReadAllText("transcribe.json"));

            for (var i = 0; i < EuIsoConstants.NumberSubtitles + 46; i++)
            {
                var textInject = subtitles[i].Text;

                var isRadioSubtitle = subtitles[i].Id is > 154 and < 199;

                textInject = TextUtils.LineSplit(textInject, isRadioSubtitle);
                EuWriterHandler.WriteSubtitleNewAddress(zeroFile, i, subtitleOverallOffset);

                var strBytes = TextUtils.ConvertToBytes(textInject);

                EuWriterHandler.WriteSubtitleNewText(zeroFile, subtitleOverallOffset, strBytes);
                subtitleOverallOffset += strBytes.Length + 1;
            }

            EuWriterHandler.WriteNewSizeFile(zeroFile, (int) zeroFile.Size + 100000);
        }

        private void PssUndub()
        {
            foreach (var currentPss in PssInfos)
            {
                FilesCompleted += 1;
                
                var jpBuffer = JpReaderHandler.ExtractFileFromAbsoluteAddress(currentPss.JpOffset, (int) currentPss.JpSize);

                if (currentPss.JpSize <= currentPss.EuSize && UndubOptions.IsModelImport)
                {
                    EuWriterHandler.PatchBytesAtAbsoluteOffset(currentPss.EuOffset, jpBuffer);
                    EuWriterHandler.PatchBytesAtAbsoluteOffset(currentPss.EuSizeOffset,
                        BitConverter.GetBytes(currentPss.JpSize));
                    continue;
                }

                var euBuffer = EuReaderHandler.ExtractFileFromAbsoluteAddress(currentPss.EuOffset, (int) currentPss.EuSize);

                var newVideoBuffer = PssAudioHandler.SwitchPssAudio(jpBuffer, euBuffer);

                EuWriterHandler.PatchBytesAtAbsoluteOffset(currentPss.EuOffset, newVideoBuffer);
                EuWriterHandler.PatchBytesAtAbsoluteOffset(currentPss.EuSizeOffset, BitConverter.GetBytes(newVideoBuffer.Length));
            }
        }

        private void PatchBinarySubtitle()
        {
            EuWriterHandler.PatchByteAtAbsoluteOffset(0x25711B, 0x14);
            EuWriterHandler.PatchByteAtAbsoluteOffset(0x257153, 0x14);
            EuWriterHandler.PatchByteAtAbsoluteOffset(0x257313, 0x10);
            EuWriterHandler.PatchByteAtAbsoluteOffset(0x261BB3, 0x14);
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
                case 170:
                    jpFileIndex = 66;
                    break;
                case 214:
                    jpFileIndex = 78;
                    break;
                case 491:
                case 492:
                case 493:
                case 494:
                case 495:
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
                case 1068:
                    jpFileIndex = 749;
                    break;
                case 1069:
                    jpFileIndex = 750;
                    break;
                case 1070:
                    jpFileIndex = 751;
                    break;
                case 1071:
                    jpFileIndex = 752;
                    break;
                default:
                    return;
            }

            var fileBuffer = JpReaderHandler.ExtractFileContent(JpReaderHandler.ExtractFileInfo(jpFileIndex));

            EuWriterHandler.OverwriteFile(euFile, fileBuffer);
        }

        private void SwapHomeMenu(ZeroFile zeroFile, int jpFileIndex)
        {
            var fileBufferJp = JpReaderHandler.ExtractFileContent(JpReaderHandler.ExtractFileInfo(jpFileIndex));
            var fileBufferEu = EuReaderHandler.ExtractFileContent(zeroFile);

            var newBuffer = fileBufferEu.SubArray(0, fileBufferEu.Length);

            for (var i = 0; i < 11; i++)
            {
                var offsetEu = BitConverter.ToUInt32(fileBufferEu.SubArray(0x10 + i * 0x4, 4));

                var offsetJp = BitConverter.ToUInt32(fileBufferJp.SubArray(0x10 + i * 0x4, 4));
                var sizeJp = BitConverter.ToUInt32(fileBufferJp.SubArray(offsetJp + 0x10, 4));

                for (var k = 0; k < sizeJp; k++)
                {
                    newBuffer[k + offsetEu] = fileBufferJp[k + offsetJp];
                }
            }
            
            EuWriterHandler.OverwriteFile(zeroFile, newBuffer);
        }

        private void AudioUndub(ZeroFile euFile)
        {
            if (euFile.FileId < EuIsoConstants.AudioStartIndex || euFile.FileId == EuIsoConstants.NumberFiles - 1)
            {
                return;
            }

            var jpFileIndex = euFile.FileId - (EuIsoConstants.AudioStartIndex - JpIsoConstants.AudioStartIndex);
            var fileContent = JpReaderHandler.ExtractFileContent(JpReaderHandler.ExtractFileInfo(jpFileIndex));

            EuWriterHandler.OverwriteFile(euFile, fileContent);
        }
    }
}