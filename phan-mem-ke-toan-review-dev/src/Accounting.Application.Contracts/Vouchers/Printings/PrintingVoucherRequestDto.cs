using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.Printings
{
    public class PrintingVoucherRequestDto
    {
        public string VoucherTemplateId { get; set; }
        public string TypePrint { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string[] LstVoucherId { get; set; }
    }
}
