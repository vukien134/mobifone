using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class InfoWindowDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TypeInfoWindow { get; set; }
        public string UrlApi { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? MaxRowInForm { get; set; }
        public string WindowId { get; set; }
        public string ReportTemplateId { get; set; }
        public string AfterLoad { get; set; }
        public List<InfoWindowDetailDto> InfoWindowDetails { get; set; }
    }
}
