using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Reports.Statements.T133.Defaults
{
    public class DefaultFStatement133L01 : AuditedEntity<string>
    {
        public int? Ord { get; set; }
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Title { get; set; }
    }
}
