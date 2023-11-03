using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports
{
    public class TenantThnvnn : TenantOrgEntity
    {
        public int Year { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string Bold { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public int? Rank { get; set; }
        public string NumberCode { get; set; }
        public string Formular { get; set; }
        public string Method { get; set; }
        public string Acc { get; set; }
        public string Condition { get; set; }
        public decimal? DebitBalance1 { get; set; }
        public decimal? CreditBalance1 { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? DebitBalance2 { get; set; }
        public decimal? CreditBalance2 { get; set; }
    }
}
