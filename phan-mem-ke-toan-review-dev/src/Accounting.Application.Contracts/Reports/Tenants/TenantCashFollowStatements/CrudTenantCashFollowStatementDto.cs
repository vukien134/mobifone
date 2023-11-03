using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Tenants.TenantCashFollowStatements
{
    public class CrudTenantCashFollowStatementDto : CruOrgBaseDto
    {
        public int? Year { get; set; }
        public int? Ord { get; set; }
        public int? UsingDecision { get; set; }
        public string Printable { get; set; }
        public string Bold { get; set; }
        public string NumberCode { get; set; }
        public int? Rank { get; set; }
        public string Formular { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        //TH_MINH
        public string TargetCode { get; set; }
        public int? Htkt { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        //KY_TRUOC
        public decimal? LastPeriod { get; set; }
        //KY_NAY
        public decimal? ThisPeriod { get; set; }
        //LUY_KEKT
        public decimal? AccumulatedLastPeriod { get; set; }
        //LUY_KEKN
        public decimal? AccumulatedThisPeriod { get; set; }
        public string CarryingCurrency { get; set; }
        public string IsSummary { get; set; }
        public string Edit { get; set; }
        //SO_AM
        public int? Negative { get; set; }
    }
}
