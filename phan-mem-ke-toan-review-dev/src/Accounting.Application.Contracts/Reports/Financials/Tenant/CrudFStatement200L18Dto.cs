using System;
using Accounting.BaseDtos;

namespace Accounting.Reports.Financials.Tenant
{
    public class CrudFStatement200L18Dto : CruOrgBaseDto
    {
        public int? Year { get; set; }
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }
}

