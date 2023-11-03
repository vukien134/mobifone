using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Categories.Others
{
    public class Circulars : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string TitleV { get; set; }
        public string TitleE { get; set; }
    }
}
