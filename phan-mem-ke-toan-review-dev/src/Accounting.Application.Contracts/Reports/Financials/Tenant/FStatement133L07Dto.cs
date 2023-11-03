using System;
using Accounting.BaseDtos;

namespace Accounting.Reports.Financials.Tenant
{
    public class FStatement133L07Dto : TenantOrgDto
    {
        public int? Year { get; set; }
        public int? UsingDecision { get; set; }
        public int? Ord { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string DebitOrCredit { get; set; }
        public string Type { get; set; }
        //TK_DU
        public string Acc { get; set; }
        public string NumberCode { get; set; }
        public string Formular { get; set; }
        public int? Rank { get; set; }
        public string Title { get; set; }
        public decimal? Amount1 { get; set; }
        public decimal? Amount2 { get; set; }
        public decimal? Amount3 { get; set; }
        public decimal? Amount4 { get; set; }
        public decimal? Amount5 { get; set; }
        public decimal? Amount6 { get; set; }
        public decimal? Amount7 { get; set; }
        public decimal? Amount8 { get; set; }
        public decimal? Total { get; set; }
    }
}

