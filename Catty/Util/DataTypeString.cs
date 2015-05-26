using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Util
{
    public class DataTypeString
    {
        public static String StringFromBytes(byte[] b)
        {
            return System.Text.Encoding.UTF8.GetString(b);
        }
        public static String StringFromBytes(byte[] b, int index, int count)
        {
            return System.Text.Encoding.UTF8.GetString(b, index, count);
        }
        public static byte[] BytesFromString(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}
