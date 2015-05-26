using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Util
{
    public class BytesEncoder
    {
        public static char[] digitChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        public static int toDigits(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            else if (c >= 'a' && c <= 'f')
            {
                return c - 'a' + 10;
            }
            else if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
            }
            return 0;
        }
        public static byte[] DecodeBytesFromString(string str)
        {
            if (str == null) return null;
            if ((str.Length % 2) != 0) throw new InvalidDataException("length = " + str.Length);
            int length = str.Length / 2;
            byte[] buf = new byte[length];
            char[] charArray = str.ToCharArray();
            for (int i = 0; i < length; i++)
            {
                int d1 = toDigits(charArray[i * 2]);
                int d0 = toDigits(charArray[i * 2 + 1]);
                buf[i] = (byte)((d1 << 4) | d0);
            }
            return buf;
        }
    }
}
