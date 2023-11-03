using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class AssetToolCardDto
    {
        public string AssetToolCard { get; set; }
        public string AssetToolCode { get; set; }
        public string AssetToolName { get; set; }
        public string Country { get; set; }
        public int? ProductionYear { get; set; }
        public string Wattage { get; set; }
        public string DepartmentCode { get; set; }
        public string ReduceDetail { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ReduceDate { get; set; }
        public string Content { get; set; }
        public string Ord0 { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? DepreciationBeginDate { get; set; }
        public string Description { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int Year { get; set; }
        public decimal? ImpoverishmentPrice { get; set; }
        public decimal? Cumulative { get; set; } //CONG_DON
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public string AssetToolAcc { get; set; }
        public string PurposeCode { get; set; }
        public string DepreciationType { get; set; }
        public string Note { get; set; }
        public decimal? Remaining { get; set; }
        public decimal? Impoverishment { get; set; }
        public string FollowDepreciation { get; set; }
        public decimal? DepreciationAmount0 { get; set; }
        public decimal? DepreciationAmount { get; set; }
    }
}
