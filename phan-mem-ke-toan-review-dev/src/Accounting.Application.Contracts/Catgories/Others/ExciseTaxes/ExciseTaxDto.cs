using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.ExciseTaxes
{
    public class ExciseTaxDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Htkk { get; set; }
        public string Htkk0 { get; set; }
        public string HtkkName { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
    }
}
