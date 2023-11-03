using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.Printings
{
    public class PrintingAccVoucherDetail
    {
        public string Id { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string Note { get; set; }
    }
}
