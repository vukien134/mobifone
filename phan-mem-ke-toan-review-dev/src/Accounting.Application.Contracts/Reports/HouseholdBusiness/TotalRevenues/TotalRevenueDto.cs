using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.HouseholdBusiness
{
    public class TotalRevenueDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string CareerCode { get; set; }
        public string CareerName { get; set; }
        public string CareerName0 { get; set; }
        public int? Ord { get; set; }
        public string NumberCode { get; set; }
        public decimal? Turnover { get; set; }
        public decimal? Vat { get; set; }
        public decimal? TurnoverPersonal { get; set; }
        public decimal? VatPersonal { get; set; }
    }
}
