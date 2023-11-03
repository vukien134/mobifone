using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.AdjustDepreciations
{
    public class CrudAdjustDepreciationDto : CruOrgBaseDto
    {
        public string AssetOrTool { get; set; }
        public string AssetToolCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }
        public string Note { get; set; }
        public List<CrudAdjustDepreciationDetailDto> AdjustDepreciationDetails { get; set; }
    }
}
