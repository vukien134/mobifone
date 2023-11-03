using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.DebitBooks
{
    public class ListOfDebtsAccordingToInvoicesDto
    {
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string PartnerCode0 { get; set; }
        public string PartnerName { get; set; }
        public string PaymentTermCode { get; set; }
        public string Description { get; set; }
        public decimal? AmountReceivable { get; set; }
        public decimal? AmountReceived { get; set; }
        public decimal? AmountRemaining { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
    }
}

