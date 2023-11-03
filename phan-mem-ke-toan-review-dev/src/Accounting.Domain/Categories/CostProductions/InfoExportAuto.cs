using Accounting.JsonConverters;
using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.CostProductions
{
    public class InfoExportAuto : TenantOrgEntity
    {
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime EndDate { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string FProductWork { get; set; }
        public string OrdGrp { get; set; }
        public string Type { get; set; }
        public string OrdRec { get; set; }
    }
}
