using System;

namespace ZeroUndubProcess
{
    public class SubtitleHandler
    {
        private readonly ArrayAsStream _subtitleFile;

        public SubtitleHandler(byte[] subtitleFileContent)
        {
            _subtitleFile = new ArrayAsStream(subtitleFileContent);
        }
        
        public void WriteSubtitleNewText(ZeroFile subtitleFile, int subtitleOverallOffset, byte[] bytes)
        {
            SeekSubtitleAddressText(subtitleFile, subtitleOverallOffset);
            _subtitleFile.Write(bytes);
            _subtitleFile.Write(new byte[]{0xFF});
        }
        
        public void WriteSubtitleNewAddress(ZeroFile subtitleFile, int subtitleId, int newAddress)
        {
            SeekSubtitleAddressIndex(subtitleFile, subtitleId);
            _subtitleFile.Write(BitConverter.GetBytes(newAddress + EuIsoConstants.SubtitleTextOffset));
        }

        public byte[] GetFileContent()
        {
            return _subtitleFile.GetBuffer();
        }
        
        private void SeekSubtitleAddressText(ZeroFile subtitleFile, int subtitleOverallOffset)
        {
            SeekFile(subtitleFile);
            var textFileOffset = subtitleOverallOffset + EuIsoConstants.SubtitleTextOffset;

            _subtitleFile.SeekRelative(textFileOffset);
        }

        private void SeekSubtitleAddressIndex(ZeroFile subtitleFile, int subtitleId)
        {
            SeekFile(subtitleFile);
            var textFileOffset = subtitleId * 0x4 + EuIsoConstants.SubtitleIndexOffset;

            _subtitleFile.SeekRelative(textFileOffset);
        }

        private void SeekFile(ZeroFile subtitleFile)
        {
            _subtitleFile.SeekAbsolute(0);
        }
    }
}