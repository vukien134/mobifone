using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Tenants
{
    public class TenantExtendInfo : AuditedEntity<string>
    {
        public Guid? TenantId { get; set; }
        public int? TenantType { get; set; }
        public string LicenseXml { get; set; }
        public string CompanyType { get; set; }
        public int? RegNumUser { get; set; }
        public int? RegNumMonth { get; set; }
        public int? RegNumCompany { get; set; }
    }
}
