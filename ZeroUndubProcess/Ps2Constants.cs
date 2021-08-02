namespace ZeroUndubProcess
{
    public static class Ps2Constants
    {
        public static int SectorSize = 0x800;
    }

    public static class EuIsoConstants
    {
        public static int NumberFiles = 0x879;
        public static int SubtitleIndexOffset = 0x1EA13;
        public static int SubtitleTextOffset = 0x1ED97 + 184;
        public static int NumberSubtitles = 225;
        public static int AudioStartIndex = 1622;
        public static long ImgHdBinStartAddress = 0xA63000;
        public static long ImgBdBinStartAddress = 0x384A7800;
    }

    public static class JpIsoConstants
    {
        public static int NumberFiles = 0x73A;
        public static int AudioStartIndex = 1303;
        public static long ImgHdBinStartAddress = 0x20829000;
        public static long ImgBdBinStartAddress = 0x2082D000;
    }
}