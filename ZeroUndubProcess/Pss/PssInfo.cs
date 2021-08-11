namespace ZeroUndubProcess
{
    public sealed class PssInfo
    {
        public string Filename { get; set; }
        public long EuOffset { get; set; }
        public long EuSizeOffset { get; set; }
        public long EuSize { get; set; }
        public long JpSize { get; set; }
        public long JpOffset { get; set; }
    }
}