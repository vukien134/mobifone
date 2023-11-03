using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.ExportXmls
{
    public class ExportXmlHKDParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string ProductCode { get; set; }
        public string PartnerCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string PartnerGroupCode { get; set; }
    }
}
