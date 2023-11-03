using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class SummaryOfDebtToInvoiceDto
    {
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public decimal Remain { get; set; }
        public decimal D0to15 { get; set; }
        public decimal D16to30 { get; set; }
        public decimal D31to60 { get; set; }
        public decimal D61to90 { get; set; }
        public decimal D91to120 { get; set; }
        public decimal DOver120 { get; set; }
        public int Sort { get; set; }
        public string Id { get; set; }
        public string DocumentId { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string AccCode { get; set; }
        public int Year { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? DeadlinePayment { get; set; }
        public string OrgCode { get; set; }
        public string AccType { get; set; }
        public string Address { get; set; }
        public string Representative { get; set; }
        public int Times { get; set; }
        public string Description { get; set; }
        public decimal VatAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountReceivable { get; set; }
        public decimal AmountReceived { get; set; }
    }
}

