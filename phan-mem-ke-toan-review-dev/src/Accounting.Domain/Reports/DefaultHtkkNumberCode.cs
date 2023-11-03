using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Reports
{
    public class DefaultHtkkNumberCode : AuditedEntity<string>
    {
        public string CircularCode { get; set; }
        public string NumberCode { get; set; }
    }
}
