using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class RecordingVoucherBook : TenantOrgEntity
    {
        public int Year { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Description { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string LstVoucherCode { get; set; }
        public int TypeDescription { get; set; }
        public int TypeFilter { get; set; }
    }
}
