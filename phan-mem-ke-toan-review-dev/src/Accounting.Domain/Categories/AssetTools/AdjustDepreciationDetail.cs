using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AdjustDepreciationDetail : TenantOrgEntity
    {
        public string AdjustDepreciationId { get; set; }
        public string AssetToolDetailId { get; set; }
        public decimal? Amount { get; set; }
        public AdjustDepreciation AdjustDepreciation { get; set; }
    }
}
