using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class AssetBookDto
    {
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string AssetToolId { get; set; }
        public string AssetToolCode { get; set; }
        public string AssetToolName { get; set; }
        public int Year { get; set; }
        public string AssetOrTool { get; set; }
        public decimal? Quantity { get; set; }
        public string AssetGroupCode { get; set; }
        public int AssetToolRank { get; set; }
        public string AssetToolOrd { get; set; }
        public string Ord0 { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ReduceDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? UpDownDate { get; set; }
        public string UpDownCode { get; set; }
        public string ReduceCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? DepreciationBeginDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? BeginDate { get; set; }
        public decimal? MonthNumber { get; set; } // SO_T_KH
        public decimal? DepreciationAmount { get; set; }
        public string DepartmentCode { get; set; }
        public string CapitalCode { get; set; }
        public string IncreaseReduced { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? MonthNumber0 { get; set; } // SO_THANG
        public decimal? OriginalPrice1 { get; set; } // NG_GIA1
        public decimal? ImpoverishmentPrice1 { get; set; }
        public decimal? RemainingPrice1 { get; set; } // GT_CL1
        public decimal? Depreciation { get; set; }
        public decimal? OriginalPriceIncreased { get; set; } // NG_TANG
        public decimal? ImpoverishmentIncrease { get; set; } // HM_TANG
        public decimal? DepreciationUpAmount { get; set; } // KH_TANG
        public decimal? OriginalPriceReduced { get; set; } // NG_GIAM
        public decimal? ImpoverishmentReduced { get; set; } // HM_GIAM
        public decimal? DepreciationDownAmount { get; set; } // KH_GIAM
        public decimal? OriginalPrice2 { get; set; } // NG_GIA2
        public decimal? ImpoverishmentPrice2 { get; set; } // GT_HM2
        public decimal? RemainingPrice2 { get; set; } // GT_CL2
        public decimal? DepreciationAccumulated { get; set; } // kh_lk
        public string Country { get; set; }
        public string UpDownCode0 { get; set; }
        public string ReduceDetail { get; set; }
        public DateTime? ReduceDate0 { get; set; }
        public string Content { get; set; }
        public string AssetToolNameHTML
        {
            get
            {
                if (this.AssetToolName == null)
                {
                    return "";
                }
                else
                {
                    return this.AssetToolName.Replace(" ", "&nbsp;");
                }
            }
        }
    }
}
