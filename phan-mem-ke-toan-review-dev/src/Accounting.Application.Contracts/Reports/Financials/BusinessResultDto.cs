using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Financials
{
    public class BusinessResultDto
    {
        public string Id { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public int? Ord { get; set; }
        public int? UsingDecision { get; set; }
        public string Printable { get; set; }
        public string Bold { get; set; }
        public string NumberCode { get; set; }
        public int Rank { get; set; }
        public string Formular { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string FProductWorkCode { get; set; }
        public string DebitSectionCode { get; set; }
        public string CreditSectionCode { get; set; }
        public string TargetCode { get; set; }
        public string Method { get; set; }
        public int? Htkt { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        //KY_TRUOC
        public decimal? LastPeriod { get; set; }
        //KY_NAY
        public decimal? ThisPeriod { get; set; }
        //KY_TRUOC_NT
        public decimal? LastPeriodCur { get; set; }
        //KY_NAY_NT
        public decimal? ThisPeriodCur { get; set; }
        //LUY_KEKT
        public decimal? AccumulatedLastPeriod { get; set; }
        //LUY_KEKN
        public decimal? AccumulatedThisPeriod { get; set; }
        //LK_KT_NT
        public decimal? AccumulatedLastPeriodCur { get; set; }
        //LK_KN_NT
        public decimal? AccumulatedThisPeriodCur { get; set; }
        public string CarryingCurrency { get; set; }
        public string IsSummary { get; set; }
        public string Edit { get; set; }
        //SO_AM
        public int? Negative { get; set; }
        public int Tag { get; set; }
        public string NameV { get; set; }
        public string NameE { get; set; }
        public string Choose { get; set; }
        public string ModeL { get; set; }
        public string TaxCode { get; set; }
        public string TaxAuthorityCode { get; set; }
        public string TaxAuthorityName { get; set; }
        public string Signee { get; set; }
        public string SubmittingOrganiztion { get; set; }
        public string SubmittingAddress { get; set; }
        public string Wards { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string ChiefAccountant { get; set; }
        public string Director { get; set; }
        public string DescriptionHTML
        {
            get
            {
                return this.Description.Replace(" ", "&nbsp;");
            }
        }
        public string DescriptionEHTML
        {
            get
            {
                return this.DescriptionE.Replace(" ", "&nbsp;");
            }
        }
        public decimal? Amount0 { get; set; }
        public decimal? AmountCur0 { get; set; }
        public string SectionCode0 { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
    }
}
