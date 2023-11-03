using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ImportExcelTemplate : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string WindowId { get; set; }
        public string UrlApi { get; set; }
        public int? RowBegin { get; set; }
        public Window Window { get; set; }
        public string HelpContent { get; set; }
        public ICollection<ImportExcelTemplateColumn> ImportExcelTemplateColumns { get; set; }
    }
}
