using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Dtos.Vouchers
{
    public class ReportTest
    {
        public string VoucherNumber { get; set; }
        public List<ReportTestDetail> ReportTestDetails { get; set; }
    }
    public class ReportTestDetail
    {
        public string DebitAcc { get; set; }
    }
}
