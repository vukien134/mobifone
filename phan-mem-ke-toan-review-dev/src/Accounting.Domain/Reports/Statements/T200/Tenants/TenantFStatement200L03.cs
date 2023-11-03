using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Statements.T200.Tenants
{
    public class TenantFStatement200L03 : TenantOrgEntity
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
        public string OriginalPriceAcc { get; set; }
        public string RecordingPriceAcc { get; set; }
        public string PreventivePriceAcc { get; set; }
        public decimal? OriginalPrice2 { get; set; }
        public decimal? RecordingPrice2 { get; set; }
        public decimal? PreventivePrice2 { get; set; }
        public decimal? OriginalPrice1 { get; set; }
        public decimal? RecordingPrice1 { get; set; }
        public decimal? PreventivePrice1 { get; set; }
        public string Title { get; set; }
    }
}
