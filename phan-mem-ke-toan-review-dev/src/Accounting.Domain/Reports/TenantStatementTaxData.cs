using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports
{
    public class TenantStatementTaxData : TenantOrgEntity
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Extend { get; set; }
        public decimal? DeductPre { get; set; }
        public decimal? IncreasePre { get; set; }
        public decimal? ReducePre { get; set; }
        public decimal? SuggestionReturn { get; set; }
    }
}
