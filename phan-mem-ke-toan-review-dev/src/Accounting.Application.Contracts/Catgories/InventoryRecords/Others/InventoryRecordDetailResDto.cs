using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.InventoryRecords
{
    public class InventoryRecordDetailResDto
    {
        public string Id { get; set; }
        public string InventoryRecordId { get; set; }
        public string Ord0 { get; set; }
        public int? Year { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? AuditQuantity { get; set; }
        public decimal? AuditAmount { get; set; }
        public decimal? InventoryQuantity { get; set; }
        public decimal? InventoryAmount { get; set; }
        public decimal? OverQuantity { get; set; }
        public decimal? OverAmount { get; set; }
        public decimal? ShortQuantity { get; set; }
        public decimal? ShortAmount { get; set; }
        public int? Quality1 { get; set; }
        public int? Quality2 { get; set; }
        public int? Quality3 { get; set; }
        public string Note { get; set; }
        public string Acc { get; set; }
        public string FProductWorkCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string ProductName { get; set; }
        public string AttachProductLot { get; set; }
        public string AttachProductOrigin { get; set; }
    }
}
