using System;
using Accounting.BaseDtos;

namespace Accounting.Reports.Financials.Tenant
{
    public class CrudFStatement200L16Dto : CruOrgBaseDto
    {
        public int? Year { get; set; }
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string DebitOrCredit { get; set; }
        public string Type { get; set; }
        public string NumberCode { get; set; }
        public int? Rank { get; set; }
        public string Formular { get; set; }
        //TK_GT
        public string ValueAcc { get; set; }
        //TK_DP
        public string PreventiveAcc { get; set; }
        public decimal? ValueAmount1 { get; set; }
        public decimal? PreventiveAmount1 { get; set; }
        public decimal? ValueAmount2 { get; set; }
        public decimal? PreventiveAmount2 { get; set; }
        public string Title { get; set; }
    }
}

