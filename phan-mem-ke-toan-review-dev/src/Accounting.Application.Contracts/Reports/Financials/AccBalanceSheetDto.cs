using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Financials
{
    public class AccBalanceSheetDto
    {
        public string Id { get; set; }
        public int? Ord { get; set; }
        public int? UsingDecision { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        public string Bold { get; set; }
        public string CurrencyCode { get; set; }
        public string ContractCode { get; set; }
        public string PrintTable { get; set; }
        public string DebitCredit { get; set; }
        public string Type { get; set; }
        public string Acc { get; set; }
        public string NumberCode { get; set; }
        public int Rank { get; set; }
        public string ExchangeMethod { get; set; }
        public string TargetCode { get; set; }
        public string HTKK { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public decimal? OpeningAmount { get; set; }
        public decimal? EndingAmount { get; set; }
        public decimal? OpeningAmountCur { get; set; }
        public decimal? EndingAmountCur { get; set; }
        public string CarryingCurrency { get; set; }
        public string IsSummary { get; set; }
        public string Edit { get; set; }
        public string Tag { get; set; }
        public string NameV { get; set; }
        public string NameE { get; set; }
        public string Choose { get; set; }
        public string ModeL { get; set; }
        public string TaxCode { get; set; }
        public string TaxAuthorityCode { get; set; }
        public string TaxAuthorityName { get; set; }
        public string Signee { get; set; }
        public string SubmittingOrganiztion { get; set; }
        public string SubmittingAddress { get; set; }
        public string Wards { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string ChiefAccountant { get; set; }
        public string Director { get; set; }
        public string DescriptionEHTML
        {
            get
            {
                return this.DescriptionE.Replace(" ", "&nbsp;");
            }
        }
        public string DescriptionHTML
        {
            get
            {
                return this.Description.Replace(" ", "&nbsp;");
            }
        }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherId { get; set; }
        public string Note { get; set; }
        public string ReciprocalAcc { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
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
