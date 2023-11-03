using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ReferenceDetail : AuditedEntity<string>
    {
        public int? Ord { get; set; }
        public string ReferenceId { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public int? Width { get; set; }
        public string Format { get; set; }
        public string Template { get; set; }
        public Reference Reference { get; set; }
    }
}
