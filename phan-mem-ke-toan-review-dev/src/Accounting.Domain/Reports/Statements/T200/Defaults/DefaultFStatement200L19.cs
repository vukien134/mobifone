using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Reports.Statements.T200.Defaults
{
    public class DefaultFStatement200L19 : AuditedEntity<string>
    {
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string DebitOrCredit { get; set; }        
        public string NumberCode { get; set; }
        public int? Rank { get; set; }
        public string Formular { get; set; }
        public string Acc { get; set; }
        public decimal? NV1 { get; set; }
        public decimal? NV2 { get; set; }
        public decimal? NV3 { get; set; }
        public decimal? NV4 { get; set; }
        public decimal? NV5 { get; set; }
        public decimal? NV6 { get; set; }
        public decimal? NV7 { get; set; }
        public decimal? NV8 { get; set; }
        public decimal? Total { get; set; }
        public string Title { get; set; }
    }
}
