using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos
{
    public abstract class TenantOrgDto : TenantAuditedEntityDto<string>
    {
        public string OrgCode { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }
    }
}
