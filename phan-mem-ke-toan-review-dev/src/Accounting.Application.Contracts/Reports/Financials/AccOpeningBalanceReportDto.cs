using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Financials
{
    public class AccOpeningBalanceReportDto
    {
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public int Tag { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string CurrencyCode { get; set; }
        public string ContractCode { get; set; }
        public decimal? DebitCur1 { get; set; }
        public decimal? Debit1 { get; set; }
        public decimal? CreditCur1 { get; set; }
        public decimal? Credit1 { get; set; }
        public decimal? _DebitCur1 { get; set; }
        public decimal? _Debit1 { get; set; }
        public decimal? _CreditCur1 { get; set; }
        public decimal? _Credit1 { get; set; }
        public decimal? DebitCur2 { get; set; }
        public decimal? Debit2 { get; set; }
        public decimal? CreditCur2 { get; set; }
        public decimal? Credit2 { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public decimal? DebitAccumulationCur { get; set; }
        public decimal? DebitAccumulation { get; set; }
        public decimal? CreditAccumulationCur { get; set; }
        public decimal? CreditAccumulation { get; set; }
        public string AttachPartner { get; set; }
        public string AttachContract { get; set; }
        public string AttachAccSection { get; set; }
        public string AttachWorkPlace { get; set; }
        public string AttachProductCost { get; set; }
        public string IsBalanceSheetAcc { get; set; }
        public string AttachCurrency { get; set; }
        public int? AccPattern { get; set; }
    }
}
