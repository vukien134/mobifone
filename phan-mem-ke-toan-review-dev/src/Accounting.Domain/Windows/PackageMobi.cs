using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class PackageMobi : AuditedEntity<string>
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public int? UserQuantity { get; set; }
        public int? CompanyQuantity { get; set; }
        public string CompanyType { get; set; }
        public int Type { get; set; }
    }
}

