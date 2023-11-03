using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class InfoWindowDetail : AuditedEntity<string>
    {
        public int? Ord { get; set; }
        public string InfoWindowId { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string TypeEditor { get; set; }
        public string LabelPosition { get; set; }
        public string LabelWidth { get; set; }
        public string Format { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string ReferenceId { get; set; }
        public string DefaultValue { get; set; }
        public bool? Hidden { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public bool? Required { get; set; }
        public InfoWindow InfoWindow { get; set; }
        public string ValueChange { get; set; }
    }
}
