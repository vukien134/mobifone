using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class ProductionCostCalculationFilterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string FProductWork { get; set; }    // A: phân bổ kết chuyển, S: Thành phẩm,C: công trình, dự án
        public int AttachMonth { get; set; }     // 1: Theo tháng, 0: cả kỳ
        public string LstOrdGrp { get; set; }       // Danh sách những group được kết chuyển - phân bổ
        public int? DecideApply { get; set; }     // Quyết định áp dụng, Nếu null sẽ lấy theo khai báo trông hệ thống
        public int CostCalculation { get; set; } // 1: Có tính giá vốn, 0: Không tính giá vốn
        public string LstProductionPeriodCode { get; set; } // Danh sách mã sản xuất để tính 'GD1,GD2,GD3,...'
    }
}
