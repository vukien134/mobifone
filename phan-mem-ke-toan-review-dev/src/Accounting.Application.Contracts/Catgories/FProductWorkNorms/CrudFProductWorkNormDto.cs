using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.FProductWorkNorms
{
    public class CrudFProductWorkNormDto : CruOrgBaseDto
    {
        public int Year { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal Quantity { get; set; }
        public string Note { get; set; }
        public string FProductWorkName { get; set; }
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
        public string UnitCode { get; set; }
        public decimal QuantityLoss { get; set; }
        public decimal QuantityDetail { get; set; }
        public decimal PercentLoss { get; set; }
        public decimal PriceCur { get; set; }
        public decimal Price { get; set; }
        public decimal AmountCur { get; set; }
        public decimal Amount { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ApplicableDate1 { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ApplicableDate2 { get; set; }
        public List<CrudFProductWorkNormDetailDto> FProductWorkNormDetails { get; set; }
    }
}
