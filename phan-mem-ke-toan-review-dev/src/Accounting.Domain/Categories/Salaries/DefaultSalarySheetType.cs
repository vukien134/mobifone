using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Categories.Salaries
{
    public class DefaultSalarySheetType : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string VoucherCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public ICollection<DefaultSalarySheetTypeDetail> SalarySheetTypeDetails { get; set; }
    }
}
