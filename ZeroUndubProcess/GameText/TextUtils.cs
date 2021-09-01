using System;
using System.Linq;
using System.Text;

namespace ZeroUndubProcess.GameText
{
    public static class TextUtils
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            var result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
        
        public static T[] SubArray<T>(this T[] array, uint offset, uint length)
        {
            var result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
        
        public static byte[] ConvertToBytes(string strConvert)
        {
            return strConvert.Select(ConvertChar).ToArray();
        }

        public static string LineSplit(string subtitleText, bool isRadioSubtitle = false)
        {
            var maxLineLength = isRadioSubtitle ? 39 : 25;
            var screenMaxLineLength = isRadioSubtitle ? 43 : 43;
            
            if (subtitleText.Length < maxLineLength)
            {
                return subtitleText;
            }

            var doLineSplit = false;
            var returnStr = new StringBuilder(subtitleText);
            var currentLineIndex = -1;

            for (var i = 0; i < subtitleText.Length; i++)
            {
                currentLineIndex += 1;
                if (currentLineIndex != 0 && currentLineIndex % maxLineLength == 0)
                {
                    doLineSplit = true;
                }

                if (subtitleText[i] != ' ' || !doLineSplit)
                {
                    continue;
                }

                if (currentLineIndex >= screenMaxLineLength)
                {
                    for (var k = i - 1; k > 0; k--)
                    {
                        if (subtitleText[k] != ' ')
                        {
                            continue;
                        }

                        i = k;
                        break;
                    }
                }
                
                returnStr[i] = '\n';
                doLineSplit = false;
                currentLineIndex = -1;
            }

            return returnStr.ToString();
        }

        private static byte ConvertChar(char charConvert)
        {
            return charConvert switch
            {
                ' ' => 0x00,
                'A' => 0x01,
                'B' => 0x02,
                'C' => 0x03,
                'D' => 0x04,
                'E' => 0x05,
                'F' => 0x06,
                'G' => 0x07,
                'H' => 0x08,
                'I' => 0x09,
                'J' => 0x0A,
                'K' => 0x0B,
                'L' => 0x0C,
                'M' => 0x0D,
                'N' => 0x0E,
                'O' => 0x0F,
                'P' => 0x10,
                'Q' => 0x11,
                'R' => 0x12,
                'S' => 0x13,
                'T' => 0x14,
                'U' => 0x15,
                'V' => 0x16,
                'W' => 0x17,
                'X' => 0x18,
                'Y' => 0x19,
                'Z' => 0x1A,
                'a' => 0x1B,
                'b' => 0x1C,
                'c' => 0x1D,
                'd' => 0x1E,
                'e' => 0x1F,
                'f' => 0x20,
                'g' => 0x21,
                'h' => 0x22,
                'i' => 0x23,
                'j' => 0x24,
                'k' => 0x25,
                'l' => 0x26,
                'm' => 0x27,
                'n' => 0x28,
                'o' => 0x29,
                'p' => 0x2A,
                'q' => 0x2B,
                'r' => 0x2C,
                's' => 0x2D,
                't' => 0x2E,
                'u' => 0x2F,
                'v' => 0x30,
                'w' => 0x31,
                'x' => 0x32,
                'y' => 0x33,
                'z' => 0x34,
                '0' => 0x3F,
                '1' => 0x40,
                '2' => 0x41,
                '3' => 0x42,
                '4' => 0x43,
                '5' => 0x44,
                '6' => 0x45,
                '7' => 0x46,
                '8' => 0x47,
                '9' => 0x48,
                '"' => 0x8A,
                '\'' => 0x8B,
                '(' => 0x8C,
                ')' => 0x8D,
                '-' => 0x8E,
                '?' => 0x8F,
                '/' => 0x90,
                '’' => 0x91,
                '、' => 0x92,
                ';' => 0x93,
                ':' => 0x94,
                ',' => 0x95,
                '.' => 0x96,
                '!' => 0x97,
                '\n' => 0xFE,
                _ => 0x0
            };
        }
    }
}