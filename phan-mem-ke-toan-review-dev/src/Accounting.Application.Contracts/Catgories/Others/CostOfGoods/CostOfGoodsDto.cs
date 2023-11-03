using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.Others.CostOfGoods
{
    public class CostOfGoodsDto
    {
        public string OrdCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string Type { get; set; }
        public string WareHouseCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductGroup { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string ProductType { get; set; }
        public string Fifo { get; set; }
        public int ConsecutiveMonth { get; set; }
        public bool Continuous { get; set; }
        public int Year { get; set; }

    }
}
