using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolAccessory : TenantOrgEntity
    {
        public string Ord0 { get; set; }
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public int Year { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public AssetTool AssetTool { get; set; }
    }
}
