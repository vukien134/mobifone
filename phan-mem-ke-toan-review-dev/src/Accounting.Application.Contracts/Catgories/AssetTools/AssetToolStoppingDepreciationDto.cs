using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.AssetTools
{
    public class AssetToolStoppingDepreciationDto : TenantOrgDto
    {
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string Ord0 { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        public string Note { get; set; }
    }
}
