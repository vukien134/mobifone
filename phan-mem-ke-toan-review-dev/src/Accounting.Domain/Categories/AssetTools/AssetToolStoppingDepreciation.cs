using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolStoppingDepreciation : TenantOrgEntity
    {
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string Ord0 { get; set; }
        public DateTime? BeginDate { get; set; }
        public  DateTime? EndDate { get; set; }
        public string Note { get; set; }
        public AssetTool AssetTool { get; set; }
    }
}
