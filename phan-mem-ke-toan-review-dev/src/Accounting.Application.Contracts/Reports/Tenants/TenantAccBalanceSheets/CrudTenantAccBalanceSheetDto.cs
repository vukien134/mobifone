using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Tenants.TenantAccBalanceSheets
{
    public class CrudTenantAccBalanceSheetDto : CruOrgBaseDto
    {
        public int? Year { get; set; }
        public int? Ord { get; set; }
        public int? UsingDecision { get; set; }
        public string Printable { get; set; }
        public string Bold { get; set; }
        public string DebitOrCredit { get; set; }
        public string Type { get; set; }
        public string Acc { get; set; }
        public string NumberCode { get; set; }
        public int? Rank { get; set; }
        public string Formular { get; set; }
        //TH_MINH
        public string TargetCode { get; set; }
        public int? Htkt { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public decimal? OpeningAmount { get; set; }
        public decimal? EndingAmount { get; set; }
        public decimal? OpeningAmountCur { get; set; }
        public decimal? EndingAmountCur { get; set; }
        public string CarryingCurrency { get; set; }
        public string IsSummary { get; set; }
        public string Edit { get; set; }
    }
}
