using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Licenses
{
    public class TenantLicense : TenantAuditedEntity<string>
    {
        public string LicXml { get; set; }
        public string Key { get; set; }
    }
}
