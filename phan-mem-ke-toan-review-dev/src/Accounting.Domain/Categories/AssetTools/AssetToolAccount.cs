using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolAccount : TenantOrgEntity
    {
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime? DepreciationDate { get; set; }
        public string DepartmentCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public string Note { get; set; }
        public AssetTool AssetTool { get; set; }
    }
}
