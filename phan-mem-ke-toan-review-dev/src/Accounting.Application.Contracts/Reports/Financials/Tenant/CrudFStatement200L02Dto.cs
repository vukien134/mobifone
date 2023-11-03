using System;
using Accounting.BaseDtos;

namespace Accounting.Reports.Financials.Tenant
{
    public class CrudFStatement200L02Dto : CruOrgBaseDto
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
        public string Acc { get; set; }
        public string NumberCode { get; set; }
        public string Formular { get; set; }
        public string Method { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? OpeningAmount { get; set; }
        public decimal? ClosingAmount { get; set; }
        public int? Rank { get; set; }
        public string Title { get; set; }
    }
}

