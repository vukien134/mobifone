using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class ExciseTax : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Htkk { get; set; }
        public string Htkk0 { get; set; }
        public string HtkkName { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
    }
}
