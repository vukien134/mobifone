using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.RefVouchers
{
    public class RefVoucherComboItemDto : BaseComboItemDto
    {
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
    }
}
