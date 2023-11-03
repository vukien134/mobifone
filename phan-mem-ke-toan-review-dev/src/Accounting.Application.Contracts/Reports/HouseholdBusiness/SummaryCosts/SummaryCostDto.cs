using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.HouseholdBusiness
{
    public class SummaryCostDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string NumberCode { get; set; }
        public string CurrencyName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
    }
}
