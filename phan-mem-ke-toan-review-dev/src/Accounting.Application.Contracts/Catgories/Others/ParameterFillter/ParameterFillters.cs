using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.Others.ParameterFillter
{
    public class ParameterFillters
    {

        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
    }
}
