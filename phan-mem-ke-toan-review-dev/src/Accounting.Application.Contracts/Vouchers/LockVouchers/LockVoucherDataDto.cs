using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.ResetVoucherNumbers
{
    public class LockVoucherDataDto : CruOrgBaseDto
    {
        public string VoucherCode { get; set; }
        public string VoucherName { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? BookClosingDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? BusinessBeginningDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? BusinessEndingDate { get; set; }
    }
    
}
