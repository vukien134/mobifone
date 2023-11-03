using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Reports.Statements.T200.Defaults
{
    public class DefaultFStatement200L05 : AuditedEntity<string>
    {
        public int? Sort { get; set; }
        public int? UsingDecision { get; set; }
        public string Bold { get; set; }
        public int? Ord { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Printable { get; set; }
    }
}
