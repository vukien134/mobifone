using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Extensions
{
    public static class StringExtension
    {
        public static string UpperFirstChar(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (value.Length == 1) return value.ToUpper();
            return string.Concat(value.First().ToString().ToUpper(), value.Substring(1));
        }
    }
}
