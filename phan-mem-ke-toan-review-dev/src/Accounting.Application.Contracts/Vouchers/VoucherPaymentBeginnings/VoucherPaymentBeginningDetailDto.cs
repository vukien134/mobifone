using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.VoucherPaymentBeginnings
{
    public class VoucherPaymentBeginningDetailDto : TenantOrgDto
    {
        public string VoucherPaymentBeginningId { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime DeadlinePayment { get; set; }
        public decimal Amount { get; set; }
        public int Times { get; set; }
    }
}
