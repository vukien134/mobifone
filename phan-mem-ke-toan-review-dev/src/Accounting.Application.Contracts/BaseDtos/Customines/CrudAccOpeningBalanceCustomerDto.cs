using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class CrudAccOpeningBalanceCustomineDto
    {
        public int Year { get; set; }
        public string AccCode { get; set; }
        public string OrgCode { get; set; }
        public string AccName { get; set; }
        public string CurrencyCode { get; set; }
        public string AttachCurrency { get; set; }
        public string AttachPartner { get; set; }
        public string AttachContract { get; set; }
        public string AttachProductCost { get; set; }
        public string AttachAccSection { get; set; }
        public string AttachWorkPlace { get; set; }
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
