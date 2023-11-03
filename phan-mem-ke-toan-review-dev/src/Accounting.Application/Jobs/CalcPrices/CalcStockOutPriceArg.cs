using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace Accounting.Jobs.CalcPrices
{
    [BackgroundJobName("calcStockOutPrice")]
    public class CalcStockOutPriceArg
    {
        public Guid TenantId { get; set; }
        public string InfoCalcStockOutId { get; set; }
        public int Year { get; set; }
    }
}
