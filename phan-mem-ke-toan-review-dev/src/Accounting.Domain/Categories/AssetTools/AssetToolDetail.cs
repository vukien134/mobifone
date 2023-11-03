using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolDetail : TenantOrgEntity
    {
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string UpDownCode { get; set; }
        public DateTime? UpDownDate { get; set; }
        public string Number { get; set; }
        public string DepartmentCode { get; set; }
        //Mã nguồn vốn
        public string CapitalCode { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Impoverishment { get; set; }
        public decimal? MonthNumber0 { get; set; }
        public string IsCalculating { get; set; }
        public decimal? Remaining { get; set; }
        public DateTime? BeginDate { get; set; }
        //Giá trị tính KH
        public decimal? CalculatingAmount { get; set; }
        public decimal? MonthNumber { get; set; }
        public decimal? DepreciationAmount { get; set; }
        public DateTime? EndDate { get; set; }
        public string DepreciationDebitAcc { get; set; }
        public string DepreciationCreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string Note { get; set; }
        public AssetTool AssetTool { get; set; }
    }
}
