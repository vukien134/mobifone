using Accounting.JsonConverters;
using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Accounting.Generals
{
    public class InfoCalcPriceStockOut : TenantOrgEntity
    {
        public int Year { get; set; }
        public string Status { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public string ExcutionUser { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string WarehouseCose { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string CalculatingMethod { get; set; }
        public bool Continuous { get; set; }
        public ICollection<InfoCalcPriceStockOutDetail> InfoCalcPriceStockOutDetails { get; set; }
    }
}
