using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class VoucherPaymentBeginningDetail : TenantOrgEntity
    {
        public string VoucherPaymentBeginningId { get; set; }
        public DateTime? DeadlinePayment { get; set; }
        public decimal Amount { get; set; }
        public int Times { get; set; }
        public VoucherPaymentBeginning VoucherPaymentBeginning { get; set; }
    }
}
