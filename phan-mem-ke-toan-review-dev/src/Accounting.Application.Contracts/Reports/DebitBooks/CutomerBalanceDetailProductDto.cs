using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.DebitBooks
{
    public class CutomerBalanceDetailProductDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string NotPrint { get; set; }
        public string AccNumber { get; set; }
        public string PartnerCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string FProductWorkCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherId { get; set; }
        public string VoucherOrd { get; set; }
        public string Ord0 { get; set; }
        public string Note { get; set; }
        public string Acc { get; set; }
        public string ReciprocalAcc { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ToDate { get; set; }
        public string IncurredAcc { get; set; }

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
        public string PartnerName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string InvoicePartnerName { get; set; }
        public string InvoiceNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? InvoiceDate { get; set; }
        public string ContractCode { get; set; }
        public string FProductWorkName { get; set; }
        public string UnitCode { get; set; }
        public decimal? Balance { get; set; }
        public decimal? BalanceCur { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
