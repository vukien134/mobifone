using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class VoucherNumber : TenantOrgEntity
    {
        public string VoucherCode { get; set; }
        public string BusinessCode { get; set; }
        public int TotalNumberRecord { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
