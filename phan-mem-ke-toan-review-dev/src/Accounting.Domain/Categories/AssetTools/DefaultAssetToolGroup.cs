using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Categories.AssetTools
{
    public class DefaultAssetToolGroup : AuditedEntity<string>
    {
        public string AssetOrTool { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
    }
}
