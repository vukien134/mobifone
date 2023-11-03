using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class GetQuantityFProductFilterDto
    {
        public int Year { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string LstVoucherCode { get; set; } // Danh sách mã chứng từ
        public string ProductionPeriodCode { get; set; } // mã sản xuất
    }
}
