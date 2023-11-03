using System;
namespace Accounting.Reports.DebitBooks
{
    public class BalanceSheetAccDto
    {
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public string PartnerCode { get; set; }
        public string FProductCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string CurrencyCode { get; set; }
        public string ContractCode { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public decimal? DebitCur1 { get; set; }
        public decimal? Debit1 { get; set; }
        public decimal? CreditCur1 { get; set; }
        public decimal? Credit1 { get; set; }
        public decimal? DebitCur2 { get; set; }
        public decimal? Debit2 { get; set; }
        public decimal? CreditCur2 { get; set; }
        public decimal? Credit2 { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string IsBalanceSheetAcc { get; set; }
        public string AccNameTemp { get; set; }
        public string AccNameTempE { get; set; }
        public string AccType { get; set; }
        public decimal? AccumulationDebitCur { get; set; }
        public decimal? AccumulationDebit { get; set; }
        public decimal? AccumulationCreditCur { get; set; }
        public decimal? AccumulationCredit { get; set; }
        public int Sort { get; set; }
        public decimal? Debit11 { get; set; }
        public decimal? DebitCur11 { get; set; }
        public decimal? Credit11 { get; set; }
        public decimal? CreditCur11 { get; set; }
        public string Bold { get; set; }
        public string SortPath { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public int? Rank { get; set; }
        //Theo đối tượng, đối tác
        public string AttachPartner { get; set; }
        //Theo hợp đồng
        public string AttachContract { get; set; }
        //Theo Khoản mục
        public string AttachAccSection { get; set; }
        //Mã khoản mục
        public string AccSectionCode { get; set; }
        //Theo ngoại tệ
        public string AttachCurrency { get; set; }
        //Theo phân xưởng
        public string AttachWorkPlace { get; set; }
        //Theo giá thành, chi phí sản phẩm
        public string AttachProductCost { get; set; }
    }
}

