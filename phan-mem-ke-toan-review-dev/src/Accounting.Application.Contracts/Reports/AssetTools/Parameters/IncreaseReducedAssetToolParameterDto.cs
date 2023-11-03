using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class IncreaseReducedAssetToolParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string PurposeCode { get; set; } = "";
        public string DepartmentCode { get; set; } = "";
        public string AssetToolAcc { get; set; } = "";
        public string AssetGroupCode { get; set; } = "";
        public string AssetToolCode { get; set; } = "";
        public string AssetOrTool { get; set; } = "TS";
        public string ReasonType { get; set; } = "T";
        public string UpDownCode { get; set; }
    }
}
