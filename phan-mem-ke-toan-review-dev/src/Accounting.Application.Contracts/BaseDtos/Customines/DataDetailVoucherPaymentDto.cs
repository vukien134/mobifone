using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class DetailDataVoucherPaymentDto
    {
        public string Id { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public decimal? Amount { get; set; }
        public decimal Residual { get; set; }
    }
}
