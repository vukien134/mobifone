using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class ProductBalanceDto
    {
        public string OrgCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string AccCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public decimal? OpeningQuantity { get; set; }
        public decimal? OpeningAmount { get; set; }
        public decimal? OpeningAmountCur { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ImportAmount { get; set; }
        public decimal? ImportAmountCur { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ExportAmount { get; set; }
        public decimal? ExportAmountCur { get; set; } 
        public decimal? BalanceQuantity
        {
            get
            {
                decimal _importQuantity = this.ImportQuantity == null ? 0 : this.ImportQuantity.Value;
                decimal _exportQuantity = this.ExportQuantity == null ? 0 : this.ExportQuantity.Value;
                return _importQuantity - _exportQuantity;
            }
        }
        public decimal? BalanceAmount
        { 
            get
            {
                decimal _importAmount = this.ImportAmount == null ? 0 : this.ImportAmount.Value;
                decimal _exportAmount = this.ExportAmount == null ? 0 : this.ExportAmount.Value;
                return _importAmount - _exportAmount;
            }
        }
        public decimal? BalanceAmountCur
        {
            get
            {
                decimal _importAmountCur = this.ImportAmountCur == null ? 0 : this.ImportAmountCur.Value;
                decimal _exportAmountCur = this.ExportAmountCur == null ? 0 : this.ExportAmountCur.Value;
                return _importAmountCur - _exportAmountCur;
            } 
        }
    }
}
