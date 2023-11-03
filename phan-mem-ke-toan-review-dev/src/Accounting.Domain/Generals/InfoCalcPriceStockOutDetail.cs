using Accounting.JsonConverters;
using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Generals
{
    public class InfoCalcPriceStockOutDetail : TenantOrgEntity
    {
        public string Status { get; set; }
        public string InfoCalcPriceStockOutId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        public bool? IsError { get; set; }
        public string ErrorMsg { get; set; }
        public InfoCalcPriceStockOut InfoCalcPriceStockOut { get; set; }
    }
}
