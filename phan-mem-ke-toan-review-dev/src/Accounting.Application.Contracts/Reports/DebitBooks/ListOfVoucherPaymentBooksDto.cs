using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.DebitBooks
{
    public class ListOfVoucherPaymentBooksDto
    {
        public string DocumentId { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherNumber0 { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public decimal? AmountReceivable { get; set; }
        public decimal? AmountReceived { get; set; }
        public decimal? Amount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string PaymentTermCode { get; set; }
        public string Description { get; set; }
        public int Sort1 { get; set; }
        public int Sort2 { get; set; }
        public string Bold { get; set; }
        public string Note { get; set; }
        public decimal? AmountRemaining { get; set; }
    }
}

