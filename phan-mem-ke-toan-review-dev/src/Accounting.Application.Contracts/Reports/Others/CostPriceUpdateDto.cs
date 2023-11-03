using System;
using NPOI.SS.Formula.Functions;

namespace Accounting.Reports.Others
{
    public class CostPriceUpdateDto
    {

        // public string Id { get; set; }
        public int Year { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        public string VoucherNumber { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string UnitCode { get; set; }
    }
}

