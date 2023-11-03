using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.ProductVouchers
{
    public class ProductVoucherVatDto : TenantOrgDto
    {
        public string ProductVoucherId { get; set; }
        public int Year { get; set; }
        public string TaxCategoryCode { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceSerial { get; set; }
        public string InvoiceNumber { get; set; }
        public string VatAcc { get; set; }
        public string VatProductName { get; set; }
        public string TaxCode { get; set; }
        public string VatPartnerName { get; set; }
        public string BuyerBankNumber { get; set; }
        public string SellerBankNumber { get; set; }

    }
}
