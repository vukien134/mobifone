using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Categories.Menus
{
    public class MenuAccounting : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string Url { get; set; }
        public string Detail { get; set; }
        public string Icon { get; set; }
        public string Caption { get; set; }
        public string windowId { get; set; }
        public string LastModifierName { get; set; }
        public string CreatorName { get; set; }
        public int? Order { get; set; }
        public string JavaScriptCode { get; set; }
        public int? TenantType { get; set; }
        public string ViewPermission { get; set; }
    }
}
