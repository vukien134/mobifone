using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class SpreadsheetDepreciationParameterAMDto
    {
        public string PurposeCode { get; set; } = "";
        public string DepartmentCode { get; set; } = "";
        public string AssetToolAcc { get; set; } = "";
        public string AssetGroupCode { get; set; } = "";
        public string AssetToolCode { get; set; } = "";
        public string CapitalCode { get; set; } = "";
        public string AssetOrTool { get; set; } = "TS";
    }
}
