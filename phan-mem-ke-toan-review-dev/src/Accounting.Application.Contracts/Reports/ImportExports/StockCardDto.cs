using System;
using System.Collections.Generic;
using System.Text;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class StockCardDto
    {
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int? VoucherGroup { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherId { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string ImportVoucherNumber { get; set; }
        public string ExportVoucherNumber { get; set; }
        public string Description { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public string FProductWorkCode { get; set; }
        public string FProducWorkName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string WarehouseCode { get; set; }
        public string UnitCode { get; set; }
    }
}
