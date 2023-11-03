using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ReportTemplate : AuditedEntity<string>
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
        public bool? IsDynamic { get; set; }
        public string ViewType { get; set; }
        public ICollection<ReportTemplateColumn> ReportTemplateColumns { get; set; }
        public ICollection<InfoWindow> InfoWindows { get; set; }
        public ICollection<Button> Buttons { get; set; }
    }
}
