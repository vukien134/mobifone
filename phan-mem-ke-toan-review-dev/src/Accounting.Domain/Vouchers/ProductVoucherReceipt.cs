using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class ProductVoucherReceipt : TenantOrgEntity
    {
        public string ProductVoucherId { get; set; }
        public int Year { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string DiscountDebitAcc { get; set; }
        public string DiscountCreditAcc { get; set; }
        public string DiscountDescription { get; set; }
        public string DiscountDebitAcc0 { get; set; }
        public string DiscountCreditAcc0 { get; set; }
        public decimal? DiscountAmountCur0 { get; set; }
        public decimal? DiscountAmount0 { get; set; }
        public string DiscountDescription0 { get; set; }
        public decimal? ImportTaxPercentage { get; set; }
        public string ImportDebitAcc { get; set; }
        public string ImportCreditAcc { get; set; }
        public string ImportDescription { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
        public string ExciseTaxDebitAcc { get; set; }
        public string ExciseTaxCreditAcc { get; set; }
        public string ExciseTaxDescription { get; set; }
        public ProductVoucher ProductVoucher { get; set; }
    }
}
