using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Configs
{
    public class DbServer : AuditedEntity<string>
    {
        public int? Ord { get; set; }
        public string Name { get; set; }
        public int? Port { get; set; }
        public string DatabaseName { get; set; }
        public bool? IsActive { get; set; }
        public int? SchemaOrd { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? IsDemo { get; set; }
    }
}
