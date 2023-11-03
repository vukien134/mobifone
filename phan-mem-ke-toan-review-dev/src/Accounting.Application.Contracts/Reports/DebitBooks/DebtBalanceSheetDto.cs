using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.DebitBooks
{
    public class DebtBalanceSheetDto
    {
        public string Sort { get; set; }
        public string CSS { get; set; }
        public string Tag { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public int RankPartnerGroup { get; set; }
        public string OrdPartnerGroup { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string PartnerGroupCode { get; set; }
        public string SectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string PartnerName { get; set; }
        public string AttachPartner { get; set; }
        public string AttachProductCost { get; set; }
        public decimal? DebitCur1 { get; set; }
        public decimal? Debit1 { get; set; }
        public decimal? CreditCur1 { get; set; }
        public decimal? Credit1 { get; set; }
        public decimal? _DebitCur1 { get; set; }
        public decimal? _Debit1 { get; set; }
        public decimal? _CreditCur1 { get; set; }
        public decimal? _Credit1 { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public decimal? DebitCur2 { get; set; }
        public decimal? Debit2 { get; set; }
        public decimal? CreditCur2 { get; set; }
        public decimal? Credit2 { get; set; }
        public decimal? DebtCeiling { get; set; }
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
        public string PartnerNameHTML
        {
            get
            {
                return this.PartnerName.Replace(" ", "&nbsp;");
            }
        }
        public string Representative { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string InvoicePartnerName { get; set; }
        public string InvoiceNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? InvoiceDate { get; set; }
        public string ContractCode { get; set; }
        public string FProductWorkName { get; set; }
        public decimal? Balance { get; set; }
        public decimal? BalanceCur { get; set; }
        public string SortPath { get; set; }
        public string ParentId { get; set; }
        public string PartnerGroupId { get; set; }
    }
}
