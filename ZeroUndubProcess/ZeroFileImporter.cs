using System.IO;

namespace ZeroUndubProcess
{
    public sealed class ZeroFileImporter
    {
        public int UndubbedFiles { get; private set; }
        public bool IsCompleted { get; private set; } = false;
        public bool IsSuccess { get; private set; } = false;
        private bool IsUndub { get; set; } = false;
        private bool IsModelImport { get; set; } = false;
        private bool IsSubtitleInject { get; set; } = false;
        public string ErrorMessage { get; private set; }
        private FileInfo JpIsoFile { get; set; }
        private FileInfo EuIsoFile { get; set; }
        
        public ZeroFileImporter(string euIsoFile, string jpIsoFile, bool isUndub, bool isModelImport, bool isSubtitleInject)
        {
            IsUndub = isUndub;
            IsModelImport = isModelImport;
            IsSubtitleInject = isSubtitleInject;
            var tempFile = new FileInfo(euIsoFile);
            File.Copy(tempFile.FullName, $"{tempFile.DirectoryName}/ff_ultimate.iso");
            EuIsoFile = new FileInfo($"{tempFile.DirectoryName}/ff_ultimateundub.iso");
            JpIsoFile = new FileInfo(jpIsoFile);
            
            //jpIsoData = new BinaryReader(File.OpenRead(JpIsoFile.FullName));
            //euIsoData = new BinaryWriter(File.OpenWrite(UsIsoFile.FullName));
        }
    }
}