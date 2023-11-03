using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class AccountBalanceDto
    {
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public string FProductCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string CurrencyCode { get; set; }
        public string ContractCode { get; set; }
        public decimal? DebitCur { get; set; }
        public decimal? Debit { get; set; }
        public decimal? CreditCur { get; set; }
        public decimal? Credit { get; set; }
        public decimal? CreditIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? DebitIncurredCur { get; set; }
    }
}
