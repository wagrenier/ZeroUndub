namespace ZeroUndubProcess
{
    public static class Ps2Constants
    {
        public const int SectorSize = 0x800;
    }

    public static class EuIsoConstants
    {
        public const int NumberFiles = 0x879;
        public const int SubtitleIndexOffset = 0x1EA13;
        public const int SubtitleTextOffset = 0x1ED97 + 184;
        public const int NumberSubtitles = 225;
        public const int AudioStartIndex = 1622;
        public const long ImgHdBinStartAddress = 0xA63000;
        public const long ImgBdBinStartAddress = 0x384A7800;
    }

    public static class JpIsoConstants
    {
        public const int NumberFiles = 0x73A;
        public const int AudioStartIndex = 1303;
        public const long ImgHdBinStartAddress = 0x20829000;
        public const long ImgBdBinStartAddress = 0x2082D000;
    }
}