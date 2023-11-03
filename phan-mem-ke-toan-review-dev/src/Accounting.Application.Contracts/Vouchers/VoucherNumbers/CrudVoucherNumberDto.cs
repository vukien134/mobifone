using Accounting.BaseDtos;
using System;

namespace Accounting.Vouchers.VoucherNumbers
{
    public class CrudVoucherNumberDto : CruOrgBaseDto
    {
        public string VoucherCode { get; set; }
        public string BusinessCode { get; set; }
        public int TotalNumberRecord { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
