using System;
using System.Collections.Generic;
using System.Text;

namespace EatOut.Common
{
    public static class Validate
    {
        public static void IsNotNull<T>(T o, string paramName)
            where T : class
        {
            if (o is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
        public static void IsNotEmpty(string s, string paramName)
        {
            if (s == string.Empty)
            {
                throw new ArgumentException("The string is empty.", paramName);
            }
        }

        internal static void IsNotNullOrWhiteSpace(string s, string paramName)
        {
            IsNotNull(s, paramName);
            IsNotEmpty(s, paramName);

            for (var i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                {
                    return;
                }
            }

            throw new ArgumentException("The string only contains whitespace", paramName);
        }
    }
}
