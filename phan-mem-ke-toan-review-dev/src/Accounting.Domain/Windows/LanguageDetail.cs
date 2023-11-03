using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class LanguageDetail : AuditedEntity<string>
    {
        public string LanguageId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Language Language { get; set; }
    }
}
