using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Reports.Statements.T200.Defaults
{
    public class DefaultFStatement200L04 : AuditedEntity<string>
    {
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
