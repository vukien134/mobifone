using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolDetailDepreciation : TenantOrgEntity
    {
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string Ord0 { get; set; }
        public DateTime DepreciationBeginDate { get; set; }
        public decimal DepreciationAmount { get; set; }
    }
}
