using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class ReportTemplateDto : BaseDto
    {
        public int Ord { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FileTemplate { get; set; }
        public string WindowId { get; set; }
        public string UrlApiData { get; set; }
        public string ReportType { get; set; }
        public string GridType { get; set; }
        public string InfoWindowId { get; set; }
        public string ViewType { get; set; }
    }
}
