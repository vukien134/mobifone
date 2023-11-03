using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Financials
{
    public class NTFS200LedgerDto
    {
        public string OrgCode { get; set; }
        public string AccCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string DebitPartnerCode { get; set; }
        public string CreditPartnerCode { get; set; }
        public string DebitFProductWorkCode { get; set; }
        public string CreditFProductWorkCode { get; set; }
        public int Year { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
        public decimal? DebitCurPre { get; set; }
        public decimal? DebitPre { get; set; }
        public decimal? CreditCurPre { get; set; }
        public decimal? CreditPre { get; set; }
        public decimal? DebitAccumulateCurPre { get; set; }
        public decimal? DebitAccumulatePre { get; set; }
        public decimal? CreditAccumulateCurPre { get; set; }
        public decimal? CreditAccumulatePre { get; set; }
        public decimal? DebitCurPre2 { get; set; }
        public decimal? DebitPre2 { get; set; }
        public decimal? CreditCurPre2 { get; set; }
        public decimal? CreditPre2 { get; set; }
        public decimal? DebitCur { get; set; }
        public decimal? Debit { get; set; }
        public decimal? CreditCur { get; set; }
        public decimal? Credit { get; set; }
        public decimal? DebitAccumulateCur { get; set; }
        public decimal? DebitAccumulate { get; set; }
        public decimal? CreditAccumulateCur { get; set; }
        public decimal? CreditAccumulate { get; set; }
        public decimal? DebitCur2 { get; set; }
        public decimal? Debit2 { get; set; }
        public decimal? CreditCur2 { get; set; }
        public decimal? Credit2 { get; set; }
        public decimal? DebitAmountCur { get; set; }
        public decimal? CreditAmountCur { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? AmountCurPre { get; set; }
        public decimal? AmountPre { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
    }
}
