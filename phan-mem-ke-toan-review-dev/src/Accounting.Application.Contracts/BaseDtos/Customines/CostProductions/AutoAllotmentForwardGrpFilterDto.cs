using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class AutoAllotmentForwardGrpFilterDto
    {
        public int Year { get; set; }
        public int DecideApply { get; set; }     // Quyết định áp dụng, Nếu null sẽ lấy theo khai báo trông hệ thống
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string FProductWork { get; set; }    // A: phân bổ kết chuyển, S: Thành phẩm,C: công trình, dự án
        public string OrdGrp { get; set; }
        public string Type { get; set; }
        public string ProductionPeriodCode { get; set; }

    }
}
