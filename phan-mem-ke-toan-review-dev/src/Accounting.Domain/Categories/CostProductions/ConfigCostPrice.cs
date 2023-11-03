using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.CostProductions
{
    public class ConfigCostPrice : TenantOrgEntity
    {
        public int Type { get; set; }
        public int Fifo { get; set; }
        public int ConsecutiveMonth { get; set; }
    }
}
