using System;
using Accounting.BaseDtos;

namespace Accounting.Reports.Financials.Tenant
{
    public class CrudFStatement200L09Dto : CruOrgBaseDto
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
        public decimal? HH1 { get; set; }
        public decimal? HH2 { get; set; }
        public decimal? HH3 { get; set; }
        public decimal? HH4 { get; set; }
        public decimal? HH5 { get; set; }
        public decimal? HH6 { get; set; }
        public decimal? HH7 { get; set; }
        public decimal? Total { get; set; }
        public string Title { get; set; }
    }
}

