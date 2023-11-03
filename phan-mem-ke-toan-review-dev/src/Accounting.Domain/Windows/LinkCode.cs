using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class LinkCode : AuditedEntity<string>
    {
        public int? Ord { get; set; }
        public string FieldCode { get; set; }
        public string RefTableName { get; set; }
        public string RefFieldCode { get; set; }
        public bool? IsChkDel { get; set; }
        public bool? AttachYear { get; set; }
    }
}
