using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.CostProductions
{
    public class CrudConfigCostPriceDto : CruOrgBaseDto
    {
        public int Type { get; set; }
        public int Fifo { get; set; }
        public int ConsecutiveMonth { get; set; }
    }
}
