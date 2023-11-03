using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Tenants
{
    public class TenantStatementTaxDataDto : CruOrgBaseDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        public string Extend { get; set; }
        public decimal? DeductPre { get; set; }
        public decimal? IncreasePre { get; set; }
        public decimal? ReducePre { get; set; }
        public decimal? SuggestionReturn { get; set; }
    }
}
