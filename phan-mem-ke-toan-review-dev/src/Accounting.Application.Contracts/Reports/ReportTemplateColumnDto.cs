using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class ReportTemplateColumnDto
    {
        public string ReportTemplateId { get; set; }
        public int Ord { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public int? Width { get; set; }
        public string VndNt { get; set; }
        public bool? Hidden { get; set; }
        public string FieldType { get; set; }
        public string Format { get; set; }
    }
}
