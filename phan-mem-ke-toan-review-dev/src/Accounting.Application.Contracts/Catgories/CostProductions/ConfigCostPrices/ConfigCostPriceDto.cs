using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.CostProductions
{
    public class ConfigCostPriceDto : TenantOrgDto
    {
        public int Type { get; set; }
        public int Fifo { get; set; }
        public int ConsecutiveMonth { get; set; }
    }
}
