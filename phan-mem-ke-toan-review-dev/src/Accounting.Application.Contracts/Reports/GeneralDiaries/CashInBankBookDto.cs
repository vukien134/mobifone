
using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.GeneralDiaries
{
    public class CashInBankBookDto
    {
        public string Sort0 { get; set; }
        public int Sort1 { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherId { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string Note { get; set; }
        public string Acc { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public string Representative { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerCode0 { get; set; }
        public string PartnerName { get; set; }
        public decimal? Debit { get; set; }
        public decimal? DebitCur { get; set; }
        public string ReciprocalAcc { get; set; }
        public string NOT_PRINT { get; set; }
    }
}

