using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ExportExcelTemplate : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string WindowId { get; set; }
        public string UrlApi { get; set; }
        public Window Window { get; set; }
        public ICollection<ExportExcelTemplateColumn> ExportExcelTemplateColumns { get; set; }
    }
}
