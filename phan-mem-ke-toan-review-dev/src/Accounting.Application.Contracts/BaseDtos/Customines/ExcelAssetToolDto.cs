using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class ExcelAssetToolDto
    {
        public string AssetGroupCode { get; set; }
        public string Code { get; set; }
        public string AssetToolCard { get; set; }
        public string Name { get; set; }
        public string UnitCode { get; set; }
        public string Country { get; set; }
        public int ProductionYear { get; set; }
        public string Wattage { get; set; }
        public decimal Quantity { get; set; }
        public string PurposeCode { get; set; }
        public string Note { get; set; }
        public string DepreciationDebitAcc { get; set; }
        public string DepreciationCreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        public string UpDownCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime UpDownDate { get; set; }
        public string DepartmentCode { get; set; }
        public string CapitalCode { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal Impoverishment { get; set; }
        public int MonthNumber0 { get; set; }
        public string IsCalculating { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime BeginDate { get; set; }
        public decimal CalculatingAmount { get; set; }
        public int MonthNumber { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal Remaining { get; set; }
        public string CalculatingMethod { get; set; }
        public string NoteDetail { get; set; }
        public string AssetOrTool { get; set; }
        public string ReasonType { get; set; }
        public string DepreciationType { get; set; }
        public string Number { get; set; }
        public string FollowDepreciation { get; set; }
    }
}
