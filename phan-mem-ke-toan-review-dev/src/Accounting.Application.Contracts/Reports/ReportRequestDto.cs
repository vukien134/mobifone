using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class ReportRequestDto<T>  where T: class
    {
        public string ReportTemplateCode { get; set; }
        public string InfoId { get; set; }
        public string Type { get; set; }
        public string VndNt { get; set; }
        public T Parameters { get; set; }
    }
}
