using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class SpreadsheetDepreciationDto
    {
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string AssetToolCode { get; set; }
        public string AssetToolName { get; set; }
        public string AssetOrTool { get; set; }
        public int Year { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? DepreciationBeginDate { get; set; }
        public string AssetGroupCode { get; set; }
        public int AssetToolRank { get; set; }
        public string AssetToolOrd { get; set; }
        public decimal? OriginalPrice2 { get; set; } // NG_GIA2
        public decimal? RemainingPrice2 { get; set; } // GT_CL2
        public decimal? Depreciation { get; set; } // KHAU_HAO
        public decimal? MonthNumber0 { get; set; } // SO_THANG0
        public decimal? MonthNumber { get; set; } // SO_THANG
        public decimal? MonthNumberDepreciation { get; set; } // SO_T_KH
        public decimal? OriginalPriceTotal { get; set; }//NG_G_TC
        public decimal? DepreciationTotal { get; set; } // KHAU_HAOTC
        public decimal? DepreciationAccumulatedTotal { get; set; } // KH_LK_TC
        public decimal? RemainingTotal { get; set; } // CON_LAI_TC
        public decimal? OriginalPriceReduced1 { get; set; } // NG_G1
        public decimal? Depreciation1 { get; set; } // KHAU_HAO1
        public decimal? DepreciationAccumulated1 { get; set; } // KH_LK1
        public decimal? Remaining1 { get; set; } // CON_LAI1
        public decimal? OriginalPriceReduced2 { get; set; } // NG_G2
        public decimal? Depreciation2 { get; set; } // KHAU_HAO2
        public decimal? DepreciationAccumulated2 { get; set; } // KH_LK2
        public decimal? Remaining2 { get; set; } // CON_LAI3
        public decimal? OriginalPriceReduced3 { get; set; } // NG_G3
        public decimal? Depreciation3 { get; set; } // KHAU_HAO3
        public decimal? DepreciationAccumulated3 { get; set; } // KH_LK3
        public decimal? Remaining3 { get; set; } // CON_LAI3
        public decimal? OriginalPriceReduced4 { get; set; } // NG_G4
        public decimal? Depreciation4 { get; set; } // KHAU_HAO4
        public decimal? DepreciationAccumulated4 { get; set; } // KH_LK4
        public decimal? Remaining4 { get; set; } // CON_LAI4
        public string DepartmentCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string AssetToolId { get; set; }
        public decimal? Quantity { get; set; }
        public string Ord0 { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ReduceDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? UpDownDate { get; set; }
        public string UpDownCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? BeginDate { get; set; }
        public decimal? DepreciationAmount { get; set; }
        public decimal? OriginalPrice1 { get; set; }
        public decimal? ImpoverishmentPrice1 { get; set; }
        public decimal? RemainingPrice1 { get; set; } // GT_CL1
        public string CapitalCode { get; set; }
        public string IncreaseReduced { get; set; }
        public decimal? OriginalPriceIncreased { get; set; } // NG_TANG
        public decimal? ImpoverishmentIncrease { get; set; } // HM_TANG
        public decimal? DepreciationUpAmount { get; set; } // KH_TANG
        public decimal? OriginalPriceReduced { get; set; } // NG_GIAM
        public decimal? ImpoverishmentReduced { get; set; } // HM_GIAM
        public decimal? DepreciationDownAmount { get; set; } // KH_GIAM
        public decimal? ImpoverishmentPrice2 { get; set; } // GT_HM2
        public decimal? DepreciationAccumulated { get; set; } // kh_lk
        public string AssetToolNameHTML
        {
            get
            {
                return this.AssetToolName.Replace(" ", "&nbsp;");
            }
        }
    }
}
