using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class ConfigForwardYear : AuditedEntity<string>
    {
        public int? Ord { get; set; }
        public string TableName { get; set; }
        public string FieldNot { get; set; }
        public string FieldValues { get; set; }
        public string Title { get; set; }
        public int? BusinessType { get; set; }
    }
}
