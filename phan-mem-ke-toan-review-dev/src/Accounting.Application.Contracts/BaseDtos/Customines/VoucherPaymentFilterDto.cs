using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class VoucherPaymentFilterDto
    {
        public string PartnerCode { get; set; } = "";
        public string AccCode { get; set; } = "";
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        
    }
}
