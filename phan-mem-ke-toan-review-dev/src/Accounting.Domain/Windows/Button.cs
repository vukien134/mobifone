using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class Button : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public int? Width { get; set; }
        public string Caption { get; set; }
        public string OnClick { get; set; }
        public string WindowId { get; set; }
        public string ReportTemplateId { get; set; }
        public string MenuClick { get; set; }
        public string IsGroup { get; set; }
        public string ShortCut { get; set; }
        public Window Window { get; set; }
        public ReportTemplate ReportTemplate { get; set; }
    }
}
