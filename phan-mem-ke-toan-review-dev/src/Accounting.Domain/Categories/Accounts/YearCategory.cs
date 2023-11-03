using Accounting.TenantEntities;
using System;

namespace Accounting.Categories.Accounts
{
    public class YearCategory : TenantOrgEntity
    {
        public int Year { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UsingDecision { get; set; }
    }
}
