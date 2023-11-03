using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class ProductVoucherDetailReceipt : TenantOrgEntity
    {
        public string ProductVoucherDetailId { get; set; }
        public int Year { get; set; }
        public string TaxCategoryCode { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? VatAmountCur { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? ImportTaxPercentage { get; set; }
        public decimal? ImportTaxAmountCur { get; set; }
        public decimal? ImportTaxAmount { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
        public decimal? ExciseTaxAmountCur { get; set; }
        public decimal? ExciseTaxAmount { get; set; }
        public decimal? ExpenseAmountCur0 { get; set; }
        public decimal? ExpenseAmount0 { get; set; }
        public decimal? ExpenseAmountCur1 { get; set; }
        public decimal? ExpenseAmount1 { get; set; }
        public decimal? ExpenseAmountCur { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public ProductVoucherDetail ProductVoucherDetail { get; set; }
    }
}
