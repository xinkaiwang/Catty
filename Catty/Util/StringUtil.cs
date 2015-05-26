using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Util
{
    public class StringUtil
    {
        public static String StripControlCharacters(object inObj)
        {
            if (inObj == null)
                return null;
            return StripControlCharacters(inObj.ToString());
        }

        public static String StripControlCharacters(String inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];

                if (!char.IsControl(ch))
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        }
    }
}
