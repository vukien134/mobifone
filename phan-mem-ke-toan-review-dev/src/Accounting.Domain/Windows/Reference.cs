using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class Reference : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string RefType { get; set; }
        public string ValueField { get; set; }
        public string DisplayField { get; set; }
        public string UrlApiData { get; set; }
        public string WindowId { get; set; }
        public string ListValue { get; set; }
        public string ListType { get; set; }
        public bool? FetchData { get; set; }
        public ICollection<ReferenceDetail> ReferenceDetails { get; set; }
    }
}
