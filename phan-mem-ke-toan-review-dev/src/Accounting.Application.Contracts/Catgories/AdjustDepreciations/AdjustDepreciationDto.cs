using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.AdjustDepreciations
{
    public class AdjustDepreciationDto : TenantOrgDto
    {
        public string AssetOrTool { get; set; }
        public string AssetToolCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }
        public string Note { get; set; }
    }
}
