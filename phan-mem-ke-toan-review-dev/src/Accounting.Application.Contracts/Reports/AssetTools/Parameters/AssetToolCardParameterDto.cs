using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class AssetToolCardParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        public string AssetToolCode { get; set; } = "";
        public string AssetOrTool { get; set; } = "TS";
    }
}
