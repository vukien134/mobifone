using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.GetFixDataVouchers
{
    public class FixDataParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime ToDate { get; set; }
    }
}
