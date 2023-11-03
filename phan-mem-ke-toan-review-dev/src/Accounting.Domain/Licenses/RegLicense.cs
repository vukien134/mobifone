using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Licenses
{
    public class RegLicense : AuditedEntity<string>
    {
        public string TaxCode { get; set; }
        public string Name { get; set; }
        public int? TypeLic { get; set; }
        public int? Month { get; set; }
        public int? CompanyQuantity { get; set; }
        public string LicXml { get; set; }
        public Guid? CustomerTenantId { get; set; }
        public bool? IsApproval { get; set; }
        public string Key { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? UserQuantity { get; set; }
        public string CompanyType { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
