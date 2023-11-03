using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Statements.T200.Tenants
{
    public class TenantFStatement200L10 : TenantOrgEntity
    {
        public int? Year { get; set; }
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string NumberCode { get; set; }
        public int? Rank { get; set; }
        public string Formular { get; set; }
        public string FieldName { get; set; }
        public string Condition { get; set; }
        public decimal? VH1 { get; set; }
        public decimal? VH2 { get; set; }
        public decimal? VH3 { get; set; }
        public decimal? VH4 { get; set; }
        public decimal? VH5 { get; set; }
        public decimal? VH6 { get; set; }
        public decimal? VH7 { get; set; }
        public decimal? Total { get; set; }
        public string Title { get; set; }
    }
}
