using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class ProductionCostBookParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string FProductWork { get; set; } // S thành phẩm, C dự án
        public string FProductWorkCode { get; set; } = "";
    }
}
