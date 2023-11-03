using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Taxes
{
    public class SalesTaxListDto
    {
        public string Bold { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? InvoiceDate { get; set; }
        public string PartnerName { get; set; }
        public string TaxCode { get; set; }
        public string ProductName { get; set; }
        public decimal? AmountWithoutVatCur { get; set; }
        public decimal? AmountWithoutVat { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalAmountCur { get; set; }
        public decimal? TotalAmount { get; set; }
        public string InvoiceLink { get; set; }
        public string TaxCategoryCode { get; set; }
        public int? Deduct { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string InvoiceGroup { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
    }
}
