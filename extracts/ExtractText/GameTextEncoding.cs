using System.Text;

namespace TacticsogreExtracts
{
    public class GameTextEncoding : Encoding
    {
        public override int GetByteCount(char[] chars, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotImplementedException();
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            var ret = 0;
            var i = index;
            var end = index + count;
            while (i < end)
            {
                if (i + 3 < end && bytes[i] == 0xEF && bytes[i + 1] == 0xA3 && bytes[i + 2] == 0xBF)
                {
                    i += 3;
                    var opCode = bytes[i] - 0x80;
                    var opSize = operationSizes[opCode];
                    i += opSize;
                    if (!forHumanReading || operationHumanReadables[opCode] == 1)
                    {
                        ret += 2 + opSize * 2;
                    }
                    if (forHumanReading && (opCode == 0x00 || opCode == 0x06))
                    {
                        ret += 1;
                    }
                }
                else
                {
                    var length = GetCodepointLength(bytes[i]);
                    i += length;
                    ret += length == 4 ? 2 : 1;
                }
            }
            return ret;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            var i = byteIndex;
            var bi = byteIndex;
            var end = byteIndex + byteCount;
            var ci = charIndex;
            while (i < end)
            {
                if (i + 3 < end && bytes[i] == 0xEF && bytes[i + 1] == 0xA3 && bytes[i + 2] == 0xBF)
                {
                    if (i != bi)
                    {
                        ci += UTF8.GetChars(bytes, bi, i - bi, chars, ci);
                    }

                    i += 3;
                    var opCode = bytes[i] - 0x80;
                    var opSize = operationSizes[opCode];
                    if (!forHumanReading || operationHumanReadables[opCode] == 1)
                    {
                        chars[ci] = '{'; ci++;
                        for (var j = 0; j < opSize; j++)
                        {
                            var hex = bytes[i + j].ToString("X2");
                            chars[ci] = hex[0]; ci++;
                            chars[ci] = hex[1]; ci++;
                        }
                        chars[ci] = '}'; ci++;
                    }
                    if (forHumanReading && (opCode == 0x00 || opCode == 0x06))
                    {
                        chars[ci] = '\n'; ci++;
                    }
                    i += opSize;

                    bi = i;
                }
                else
                {
                    i += GetCodepointLength(bytes[i]);
                }
            }
            if (i != bi)
            {
                ci += UTF8.GetChars(bytes, bi, i - bi, chars, ci);
            }
            return ci - charIndex;
        }

        public override int GetMaxByteCount(int charCount)
        {
            throw new NotImplementedException();
        }

        public override int GetMaxCharCount(int byteCount)
        {
            throw new NotImplementedException();
        }

        static int GetCodepointLength(byte byte0) => byte0 switch
        {
            < 0b10000000 => 1,
            < 0b11000000 => throw new InvalidDataException(),
            < 0b11100000 => 2,
            < 0b11110000 => 3,
            < 0b11111000 => 4,
            _ => throw new InvalidDataException(),
        };

        static readonly int[] operationSizes =
        {
            1, 3, 5, 6, 5, 3, 1, 2, 2, 2, 2, 3, 2, 3, 3, 3,
            3, 1, 1, 1, 1, 4, 1, 1, 3, 3, 2, 1, 3, 2, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 3, 1, 3, 3,
            1, 1, 4, 4, 3, 3,
        };

        static readonly int[] operationHumanReadables =
        {
            0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1,
            1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1,
            0, 0, 1, 1, 1, 1, 1,
        };

        private readonly bool forHumanReading;

        public GameTextEncoding(bool forHumanReading = false)
        {
            this.forHumanReading = forHumanReading;
        }
    }
}
