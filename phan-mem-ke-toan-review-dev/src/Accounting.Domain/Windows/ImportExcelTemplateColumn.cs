using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ImportExcelTemplateColumn : AuditedEntity<string>
    {
        public int Ord { get; set; }
        public string ImportExcelTemplateId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string ExcelCol { get; set; }
        public string DefaultValue { get; set; }
        public string Caption { get; set; }
        public ImportExcelTemplate ImportExcelTemplate { get; set; }
    }
}
