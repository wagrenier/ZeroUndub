namespace ZeroUndubProcess
{
    public interface IIsoHandler
    {
        public void SeekFile(ZeroFile zeroFile);
        public ZeroFile ExtractFileInfo(int fileId);
        public char[] ExtractFileContent(ZeroFile zeroFile);
        public void OverwriteFile(ZeroFile zeroFile, char[] buffer);
    }
}