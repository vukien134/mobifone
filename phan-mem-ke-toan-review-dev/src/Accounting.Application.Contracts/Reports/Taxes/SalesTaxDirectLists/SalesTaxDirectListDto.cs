using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Taxes.SalesTaxDirectLists
{
    public class SalesTaxDirectListDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string DocId { get; set; }
        public string VoucherCode { get; set; }
        public int Deduct { get; set; }
        public int ViewType { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? TotalAmountVat { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? InvoiceDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? VoucherDate { get; set; }
        public decimal? VatPercentage { get; set; }
        public string VoucherNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string TaxCode { get; set; }
        public string InvoiceSerial { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string ProductName { get; set; }
        public decimal? AmountWithoutVat { get; set; }
        public decimal? Amount { get; set; }
    }
}
