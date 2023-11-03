using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.Printings
{
    public class PrintingVoucherAccount
    {
        public string AccCode { get; set; }
        public decimal? Amount { get; set; }
        public string Type { get; set; }
    }
}
