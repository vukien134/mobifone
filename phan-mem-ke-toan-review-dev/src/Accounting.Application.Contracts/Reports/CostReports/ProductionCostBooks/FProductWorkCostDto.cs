using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.CostReports.ProductionCostBooks
{
    public class FProductWorkCostDto
    {
        public string Bold { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherId { get; set; }
        public int? VoucherGroup { get; set; }
        public string VoucherCode { get; set; }
        public string OrgCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitCode { get; set; }
        public string Note { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? Ps621 { get; set; }
        public decimal? Ps622 { get; set; }
        public decimal? Ps623 { get; set; }
        public decimal? Ps627 { get; set; }
        public decimal? Ps511 { get; set; }
        public decimal? TotalExpenseAmount 
        {
            get
            {
                decimal _ps621 = this.Ps621 == null ? 0 : this.Ps621.Value;
                decimal _ps622 = this.Ps622 == null ? 0 : this.Ps622.Value;
                decimal _ps623 = this.Ps623 == null ? 0 : this.Ps623.Value;
                decimal _ps627 = this.Ps627 == null ? 0 : this.Ps627.Value;

                return _ps621 + _ps622 + _ps623 + _ps627;
            }
        }
        public decimal? TotalCost { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TotalZ { get; set; }
        public decimal? Amount { get; set; }
        public string CreditFProductWorkCode { get; set; }
        public string DebitFProductWorkCode { get; set; }
        public decimal? Opening154 { get; set; }
        public decimal? Ending154 { get; set; }
        public decimal? Balance { get; set; }
    }
}
