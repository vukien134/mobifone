using Accounting.BaseDtos;
using System;

namespace Accounting.Catgories.Accounts.AccOpeningBalances
{
    public class CrudAccOpeningBalanceDto : CruOrgBaseDto
    {
        public int Year { get; set; }
        public string AccCode { get; set; }
        public string CurrencyCode { get; set; }
        public string PartnerCode { get; set; }
        public string ContractCode { get; set; }
        public string FProductWorkCode { get; set; }
        //Mã khoản mục
        public string AccSectionCode { get; set; }
        //Mã phân xưởng
        public string WorkPlaceCode { get; set; }
        //Dư nợ
        public decimal Debit { get; set; }
        //Dư nợ ngoại tệ
        public decimal DebitCur { get; set; }
        //Dư có
        public decimal Credit { get; set; }
        //Dư có ngoại tệ
        public decimal CreditCur { get; set; }
        //Dư nợ lũy kế
        public decimal DebitCum { get; set; }
        //Dư nợ lũy kế ngoại tệ
        public decimal DebitCumCur { get; set; }
        //Dư có lũy kế
        public decimal CreditCum { get; set; }
        //Dư có lũy kế ngoại tệ
        public decimal CreditCumCur { get; set; }
    }
}
