using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Util
{
    public class ConversionUtil
    {
        public static int ToInt(Object value)
        {
            if (value is int)
            {
                return ((int)value);
            }
            else if (value is long)
            {
                return (int)((long)value);
            }
            else
            {
                return int.Parse(value.ToString());
            }
        }

        internal static bool ToBoolean(object value)
        {
            throw new NotImplementedException();
        }
    }
}
