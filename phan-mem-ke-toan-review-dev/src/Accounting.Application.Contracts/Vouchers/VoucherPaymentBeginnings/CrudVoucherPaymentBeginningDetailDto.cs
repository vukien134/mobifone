using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.VoucherPaymentBeginnings
{
    public class CrudVoucherPaymentBeginningDetailDto : CruOrgBaseDto
    {
        public string VoucherPaymentBeginningId { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime DeadlinePayment { get; set; }
        public decimal Amount { get; set; }
        public int Times { get; set; }
    }
}
