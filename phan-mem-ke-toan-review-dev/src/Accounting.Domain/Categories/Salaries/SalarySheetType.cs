using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Salaries
{
    public class SalarySheetType : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string VoucherCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public ICollection<SalarySheetTypeDetail> SalarySheetTypeDetails { get; set; }
    }
}
