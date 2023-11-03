using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Statements.T133.Tenants
{
    public class TenantFStatement133L01 : TenantOrgEntity
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
