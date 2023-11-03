using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ConfigForwardOrg : AuditedEntity<string>
    {
        public string TableName { get; set; }
    }
}
