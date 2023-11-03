using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.GeneralDiaries
{
    public class AccountTDto
    {
        public string Sort { get; set; }
        public int Sort1 { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string Acc { get; set; }
        public string AccName { get; set; }
        public string ReciprocalAcc { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
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
        public decimal? Balance { get; set; }
        public decimal? BalanceCur { get; set; }
    }
}
