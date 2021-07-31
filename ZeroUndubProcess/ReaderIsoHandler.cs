namespace ZeroUndubProcess
{
    public class ReaderIsoHandler : IIsoHandler
    {
        public void SeekFile(ZeroFile zeroFile)
        {
            throw new System.NotImplementedException();
        }

        public ZeroFile ExtractFileInfo(int fileId)
        {
            throw new System.NotImplementedException();
        }

        public char[] ExtractFileContent(ZeroFile zeroFile)
        {
            throw new System.NotImplementedException();
        }

        public void OverwriteFile(ZeroFile zeroFile, char[] buffer)
        {
            throw new System.NotImplementedException();
        }
    }
}