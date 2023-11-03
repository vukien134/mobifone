using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class IncreaseReducedAssetToolDto
    {
        public string Bold { get; set; }
        public string Sort { get; set; }
        public string Ord0 { get; set; }
        public string IncreaseOrReduced { get; set; }
        public string OrgCode { get; set; }
        public string AssetToolCode { get; set; }
        public string AssetToolName { get; set; }
        public string PurposeCode { get; set; }
        public string AssetToolAcc { get; set; }
        public string Wattage { get; set; }
        public string Country { get; set; }
        public string CalculatingMethod { get; set; }
        public int? ProductionYear { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? DepreciationBeginDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? UpDownDate { get; set; }
        public string Note { get; set; }
        public string AssetToolGroupCode { get; set; }
        public int AssetToolRank { get; set; }
        public string AssetToolOrdGroup { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ReduceDate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? MonthNumberDepreciation { get; set; }
        public decimal? MonthNumber0 { get; set; }
        public decimal? MonthNumber { get; set; }
        public decimal? DepreciationAccumulated { get; set; }
        public decimal? DepreciationAmount { get; set; }
        public decimal? DepreciationUpAmount { get; set; }
        public decimal? DepreciationDownAmount { get; set; }
        public decimal? OriginalPrice2 { get; set; }
        public decimal? ImpoverishmentPrice2 { get; set; }
        public decimal? RemainingPrice2 { get; set; }
        public decimal? OriginalPrice1 { get; set; }
        public decimal? ImpoverishmentPrice1 { get; set; }
        public decimal? OriginalPriceIncreased { get; set; }
        public decimal? OriginalPriceReduced { get; set; }
        public decimal? ImpoverishmentIncrease { get; set; }
        public decimal? ImpoverishmentReduced { get; set; }
        public string AssetToolNameHTML
        {
            get
            {
                return this.AssetToolName.Replace(" ", "&nbsp;");
            }
        }
    }
}
