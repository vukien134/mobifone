using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines.AssetTool
{
    public class DataAssetToolDetailDto
    {
        public string Id { get; set; }
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string OrgCode { get; set; }
        public string AssetToolCode { get; set; }
        public string Ord0 { get; set; }
        public string Ord00 { get; set; }
        //Mã nguồn vốn
        public string CapitalCode { get; set; }
        public string DepartmentCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? UpDownDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ReduceDate { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Impoverishment { get; set; }
        public decimal? CalculatingAmount { get; set; }
        public decimal? DepreciationAmount { get; set; }
        public decimal? DepreciationAmountT { get; set; }
        public string ReasonType { get; set; }
        public decimal? DepreciationAmount0 { get; set; }
        public decimal? AmountRemaining { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate0 { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate0 { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? DepreciationBeginDate0 { get; set; }
        public string IsStoppingDepreciation { get; set; }
        public string Recalculate { get; set; }
        public int DayNum { get; set; }
        public decimal MonthNumber0 { get; set; }
        public decimal MonthNumber { get; set; }
        public decimal RemainingMonth { get; set; }
        public decimal RemainingMonth0 { get; set; }
        public decimal RemainingMonth1 { get; set; }
    }
}
