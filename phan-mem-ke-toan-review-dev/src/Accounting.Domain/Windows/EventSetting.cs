using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class EventSetting : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string TypeEvent { get; set; }
        public string EventObject { get; set; }
        public ICollection<RegisterEvent> RegisterEvents { get; set; }
    }
}
