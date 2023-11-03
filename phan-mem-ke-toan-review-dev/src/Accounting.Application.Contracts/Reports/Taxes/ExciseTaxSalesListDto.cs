
using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.Taxes
{
    public class ExciseTaxSalesListDto
    {
        public int Sort { get; set; }
        public string Bold { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSerial { get; set; }
        public string InvoiceSymbol { get; set; }
        public string PartnerCode { get; set; }
        public string ProductCode0 { get; set; }
        public string ProductName { get; set; }
        public string ProductName0 { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceCur { get; set; }
        public string UnitCode { get; set; }
        public decimal? ExciseTaxAmount { get; set; }
        public decimal? ExciseTaxAmountCur { get; set; }
        public decimal? AmountWithoutTax { get; set; }
        public decimal? AmountWithoutTaxCur { get; set; }
        public string PartnerName { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? InvoiceDate { get; set; }
        public string ExciseTaxName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalAmountCur { get; set; }

    }
}

