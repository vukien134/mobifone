using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.GetFixDataVouchers
{
    public class GetErrorDataParameterDto
    {
        public string Type { get; set; }
        public string LstError { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime ToDate { get; set; }
    }
}
