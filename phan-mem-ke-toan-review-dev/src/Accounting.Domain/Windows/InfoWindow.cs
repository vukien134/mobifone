using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class InfoWindow : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TypeInfoWindow { get; set; }
        public string UrlApi { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? MaxRowInForm { get; set; }        
        public string ReportTemplateId { get; set; }
        public string AfterLoad { get; set; }
        public ReportTemplate ReportTemplate { get; set; }
        public ICollection<InfoWindowDetail> InfoWindowDetails { get; set; }
    }
}
