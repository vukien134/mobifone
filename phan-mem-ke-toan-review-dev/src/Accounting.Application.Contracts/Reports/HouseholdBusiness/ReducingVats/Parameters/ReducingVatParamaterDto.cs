using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.HouseholdBusiness
{
    public class ReducingVatParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerGroupCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductGroupCode { get; set; }
    }
}
