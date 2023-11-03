using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.GeneralDiaries
{
    public class CashSpendDiaryDto
    {
        public string Sort { get; set; }
        public int Sort0 { get; set; }
        public int Sort1 { get; set; }
        public string VoucherId { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string Note { get; set; }
        public string Acc { get; set; }
        public string ReciprocalAcc { get; set; }
        public string ReciprocalAccOther { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public decimal? TotalCreditIncurredCur { get; set; }
        public decimal? TotalCreditIncurred { get; set; }
        public decimal? ReciprocalIncurredCur1 { get; set; }
        public decimal? ReciprocalIncurred1 { get; set; }
        public decimal? ReciprocalIncurredCur2 { get; set; }
        public decimal? ReciprocalIncurred2 { get; set; }
        public decimal? ReciprocalIncurredCur3 { get; set; }
        public decimal? ReciprocalIncurred3 { get; set; }
        public decimal? ReciprocalIncurredCur4 { get; set; }
        public decimal? ReciprocalIncurred4 { get; set; }
        public decimal? ReciprocalIncurredCur5 { get; set; }
        public decimal? ReciprocalIncurred5 { get; set; }
        public decimal? ReciprocalIncurredCurOther { get; set; }
        public decimal? ReciprocalIncurredOther { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ToDate { get; set; }
        public string IncurredAcc { get; set; }
        public string Acc1 { get; set; }
        public string Acc2 { get; set; }
        public string Acc3 { get; set; }
        public string Acc4 { get; set; }
        public string Acc5 { get; set; }

        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? RecordingVoucherDate
        {
            get
            {
                return this.VoucherDate;
            }
        }
        public decimal? DebitCur
        {
            get
            {
                if (this.BalanceCur == null) return null;
                if (this.BalanceCur >= 0) return Math.Abs(this.BalanceCur.Value);
                return null;
            }
        }
        public decimal? Debit
        {
            get
            {
                if (this.Balance == null) return null;
                if (this.Balance >= 0) return Math.Abs(this.Balance.Value);
                return null;
            }
        }
        public decimal? CreditCur
        {
            get
            {
                if (this.BalanceCur == null) return null;
                if (this.BalanceCur < 0) return Math.Abs(this.BalanceCur.Value);
                return null;
            }
        }
        public decimal? Credit
        {
            get
            {
                if (this.Balance == null) return null;
                if (this.Balance < 0) return Math.Abs(this.Balance.Value);
                return null;
            }
        }
        public string Representative { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public decimal? Balance { get; set; }
        public decimal? BalanceCur { get; set; }
    }
}
