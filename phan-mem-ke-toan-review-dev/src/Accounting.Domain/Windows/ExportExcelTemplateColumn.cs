using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ExportExcelTemplateColumn : AuditedEntity<string>
    {
        public int? Ord { get; set; }
        public string ExportExcelTemplateId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string Caption { get; set; }
        public string Format { get; set; }
        public ExportExcelTemplate ExportExcelTemplate { get; set; }
    }
}
