using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class CreateVoucherDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public int Year { get; set; }
        public string FProductWork { get; set; }
        public string OrdGrp { get; set; }
        public string Type { get; set; }
        public string ProductionPeriodCode { get; set; }
    }
}
