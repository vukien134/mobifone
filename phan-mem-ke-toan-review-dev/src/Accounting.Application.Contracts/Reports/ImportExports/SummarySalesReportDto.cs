using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.ImportExports
{
    public class SummarySalesReportDto
    {
        public string Bold { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Capital { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? GrossProfit
        {
            get
            {
                decimal revenue = this.Revenue == null ? 0 : this.Revenue.Value;
                decimal returnAmount = this.ReturnAmount == null ? 0 : this.ReturnAmount.Value;
                //decimal discountAmount = this.DiscountAmount == null ? 0 : this.DiscountAmount.Value;
                //decimal exportAmount = this.ExportAmount == null ? 0 : this.ExportAmount.Value;
                //decimal importAmount = this.ImportAmount == null ? 0 : this.ImportAmount.Value;
                decimal Capital = this.Capital == null ? 0 : this.Capital.Value;
                return -Capital + revenue - returnAmount;
            }
        }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? ReturnQuantity { get; set; }
        public decimal? ReturnAmount { get; set; }
        public decimal? ReturnCapital { get; set; }
        public decimal? RealQuantity
        {
            get
            {
                if (this.ReturnQuantity == null) return this.Quantity;
                decimal quantity = this.Quantity == null ? 0 : this.Quantity.Value;
                return quantity - this.ReturnQuantity.Value;
            }
        }
        public decimal? NetRevenue
        {
            get
            {
                decimal revenue = this.Revenue == null ? 0 : this.Revenue.Value;
                decimal returnAmount = this.ReturnAmount == null ? 0 : this.ReturnAmount.Value;
                return revenue - returnAmount;
            }
        }
        public decimal? CapitalProfit
        {
            get
            {
                decimal revenue = this.Revenue == null ? 0 : this.Revenue.Value;
                decimal exportAmount = this.ExportAmount == null ? 0 : this.ExportAmount.Value;
                decimal importAmount = this.ImportAmount == null ? 0 : this.ImportAmount.Value;
                decimal amount = exportAmount - importAmount;
                if (amount == 0) return null;
                return 100 * ((revenue - exportAmount - importAmount) / amount);
            }
        }
        public decimal? RevenueProfit
        {
            get
            {
                decimal revenue = this.Revenue == null ? 0 : this.Revenue.Value;
                if (revenue == 0) return null;
                decimal exportAmount = this.ExportAmount == null ? 0 : this.ExportAmount.Value;
                decimal importAmount = this.ImportAmount == null ? 0 : this.ImportAmount.Value;
                return 100 * ((revenue - exportAmount - importAmount) / revenue);
            }
        }
        public string FProductWorkCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public decimal? ImportAmount { get; set; }
        public decimal? ExportAmount { get; set; }
        public string ProductGroupCode { get; set; }
        public string OrgCode { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatAmountCur { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount2 { get; set; }
        public decimal? Amount2Cur { get; set; }
        public decimal? TotalAmount
        {
            get
            {
                decimal amount2 = this.Amount2 == null ? 0 : this.Amount2.Value;
                decimal discountAmount = this.DiscountAmount == null ? 0 : this.DiscountAmount.Value;
                decimal vatAmount = this.VatAmount == null ? 0 : this.VatAmount.Value;
                return amount2 - discountAmount + vatAmount;
            }
        }
        public decimal? TotalAmountCur
        {
            get
            {
                decimal amount2 = this.Amount2Cur == null ? 0 : this.Amount2Cur.Value;
                decimal discountAmount = this.DiscountAmountCur == null ? 0 : this.DiscountAmountCur.Value;
                decimal vatAmount = this.VatAmountCur == null ? 0 : this.VatAmountCur.Value;
                return amount2 - discountAmount + vatAmount;
            }
        }
        public decimal? Price2 { get; set; }

        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherId { get; set; }
        public int? VoucherGroup { get; set; }
        public string WarehouseCode { get; set; }
        public string Acc { get; set; }
        public string ReciprocalAcc { get; set; }
        public decimal? Price { get; set; }
    }
}
