namespace ZeroUndubProcess
{
    public sealed class InfoReporter
    {
        public int TotalFiles { get; set; }
        public int FilesCompleted { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}