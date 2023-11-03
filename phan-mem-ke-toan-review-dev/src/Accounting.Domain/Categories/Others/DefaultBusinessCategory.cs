using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Categories.Others
{
    public class DefaultBusinessCategory : AuditedEntity<string>
    {
        public string VoucherCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public bool IsAccVoucher { get; set; }
        public bool IsProductVoucher { get; set; }
        public string Prefix { get; set; }
        public string Separator { get; set; }
        public string Suffix { get; set; }
        public int? TenantType { get; set; }
    }
}
