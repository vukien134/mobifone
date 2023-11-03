using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AdjustDepreciation : TenantOrgEntity
    {
        public string AssetOrTool { get; set; }
        public string AssetToolCode { get; set; }
        public DateTime? BeginDate { get; set; }
        public string Note { get; set; }
        public ICollection<AdjustDepreciationDetail> AdjustDepreciationDetails { get; set; }
    }
}
