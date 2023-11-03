using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.HouseholdBusiness
{
    public class ReducingVatDto
    {
        public int Ord { get; set; }
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? ReductionPercent { get; set; }
        public decimal? Turnover { get; set; }
        public decimal? VatReduction { get; set; }
        public string CareerCode { get; set; }
        public string CareerName { get; set; }
    }
}
