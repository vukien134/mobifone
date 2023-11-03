using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.InventoryRecords
{
    public class ConfirmInventoryParameterDto
    {
        public List<ConfirmInventoryDataDto> Data { get; set; }
    }
    public class ConfirmInventoryDataDto
    {
        public string Id0 { get; set; }
        public string Id { get; set; }
        public int Year { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime VoucherDate { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
    }
}
