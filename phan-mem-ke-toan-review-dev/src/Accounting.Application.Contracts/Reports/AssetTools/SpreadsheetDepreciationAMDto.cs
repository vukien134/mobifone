using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class SpreadsheetDepreciationAMDto
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
        public decimal? Depreciation { get; set; } // KHAU_HAO
        public decimal? MonthNumber0 { get; set; } // SO_THANG0
        public decimal? MonthNumber { get; set; } // SO_THANG
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
        public decimal? DepreciationAmount { get; set; } //GT_KH
        public decimal? OriginalPrice { get; set; }
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
        public decimal? DepreciationRate { get; set; } // TY_LE_KH
        public decimal? DepreciationAmount01 { get; set; }
        public decimal? DepreciationAmount02 { get; set; }
        public decimal? DepreciationAmount03 { get; set; }
        public decimal? DepreciationAmount04 { get; set; }
        public decimal? DepreciationAmount05 { get; set; }
        public decimal? DepreciationAmount06 { get; set; }
        public decimal? DepreciationAmount07 { get; set; }
        public decimal? DepreciationAmount08 { get; set; }
        public decimal? DepreciationAmount09 { get; set; }
        public decimal? DepreciationAmount10 { get; set; }
        public decimal? DepreciationAmount11 { get; set; }
        public decimal? DepreciationAmount12 { get; set; }

        public string AssetToolNameHTML
        {
            get
            {
                return this.AssetToolName.Replace(" ", "&nbsp;");
            }
        }
    }
}
