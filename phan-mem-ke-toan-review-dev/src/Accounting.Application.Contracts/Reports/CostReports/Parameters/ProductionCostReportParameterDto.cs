using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class ProductionCostReportParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string WorkPlaceCode { get; set; } = "";
        public string ProductionPeriodCode { get; set; } = "";
        public string FProductWorkCode { get; set; } = "";
    }
}
