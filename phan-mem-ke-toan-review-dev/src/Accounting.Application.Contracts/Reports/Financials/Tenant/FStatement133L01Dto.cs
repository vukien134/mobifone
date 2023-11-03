using System;
using Accounting.BaseDtos;

namespace Accounting.Reports.Financials.Tenant
{
    public class FStatement133L01Dto : TenantOrgDto
    {
        public int? Year { get; set; }
        public int? Ord { get; set; }
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Title { get; set; }
    }
}

