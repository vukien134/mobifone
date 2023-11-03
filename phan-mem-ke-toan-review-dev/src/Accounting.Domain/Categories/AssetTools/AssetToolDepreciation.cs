using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolDepreciation : TenantOrgEntity
    {
        //TS_CC
        public string AssetToolCode { get; set; }
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime? DepreciationDate { get; set; }
        public DateTime? UpDownDate { get; set; }
        public DateTime? DepreciationBeginDate { get; set; }
        public string DepartmentCode { get; set; }
        //Mã nguồn vốn
        public string CapitalCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public decimal? DepreciationAmount { get; set; }
        public decimal? DepreciationUpAmount { get; set; }
        public decimal? DepreciationDownAmount { get; set; }
        public string Note { get; set; }
        //SUA_KH
        public string DepreciationEdit { get; set; }
        public string Edit { get; set; }
        public AssetTool AssetTool { get; set; }
    }
}
