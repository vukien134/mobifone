using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Accounting.Generals.PriceStockOuts
{
    public class CrudInfoCalcPriceStockOutDto : CruOrgBaseDto
    {
        public int Year { get; set; }
        public string Status { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        public string ExcutionUser { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string CalculatingMethod { get; set; }
        public bool Continuous { get; set; }
        public List<CrudInfoCalcPriceStockOutDetailDto> InfoCalcPriceStockOutDetails { get; set; }
    }
}