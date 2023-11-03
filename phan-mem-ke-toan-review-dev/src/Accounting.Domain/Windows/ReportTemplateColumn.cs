using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ReportTemplateColumn : AuditedEntity<string>
    {
        public int Ord { get; set; }
        public string ReportTemplateId { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public int? Width { get; set; }
        public string FieldType { get; set; }
        public string Format { get; set; }
        public bool? Hidden { get; set; }
        public string VndNt { get; set; }
        public ReportTemplate ReportTemplate { get; set; }
    }
}
