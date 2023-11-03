using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ReportMenuShortcut : AuditedEntity<string>
    {
        public string Caption { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public string VisibleWhen { get; set; }
        public string Parameter { get; set; }
        public string OriginReportId { get; set; }
        public string ReferenceReportId { get; set; }
        public string ReferenceWindowId { get; set; }
    }
}
