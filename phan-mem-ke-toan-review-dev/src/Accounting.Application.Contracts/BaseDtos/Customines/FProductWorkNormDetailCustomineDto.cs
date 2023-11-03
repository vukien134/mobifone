using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.Customines
{
    public class FProductWorkNormDetailCustomineDto : TenantOrgDto
    {
        public string FProductWorkNormId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime EndDate { get; set; }
        public string Ord0 { get; set; }
        public string AccCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOrigin { get; set; }
        public string ProductOriginCode { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuantityLoss { get; set; }
        public decimal? PercentLoss { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ApplicableDate1 { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ApplicableDate2 { get; set; }
        public string ProductName { get; set; }
    }
}
