using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Reports.Statements.T133.Defaults
{
    public class DefaultFStatement133L04 : AuditedEntity<string>
    {
        public int? UsingDecision { get; set; }
        public int? Ord { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
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
        //PS_TANG
        public decimal? IncreaseAmount { get; set; }
        //PS_GIAM
        public decimal? DecreaseAmount { get; set; }
    }
}
