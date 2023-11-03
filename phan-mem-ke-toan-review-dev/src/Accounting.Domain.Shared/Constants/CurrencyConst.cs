using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Constants
{
    public class CurrencyConst
    {
        public static List<string> ListFormatNumberFields = new()
        {
            "TIEN",
            "TIEN_NT",
            "GIA",
            "GIA_NT",
            "SL",
            "PT",
            "TY_GIA"
        };
        public const string SymbolSeparateGroupDigit = "SYMBOL_GROUP_DIGIT";
        public const string SymbolSeparateDecimal = "SYMBOL_DECIMAL";
    }
}
