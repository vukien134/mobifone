using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Products
{
    public class ProductDetailBookDto
    {
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string Description { get; set; }
        public string AccCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ImportQuantityCur { get; set; }
        public decimal? ImportAmount { get; set; }
        public decimal? ImportAmount1 { get; set; }
        public decimal? ImportAmount2 { get; set; }
        public decimal? ImportAmountCur { get; set; }
        public decimal? ImportAmountCur1 { get; set; }
        public decimal? ImportAmountCur2 { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ExportQuantityCur { get; set; }
        public decimal? ExportAmount { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public decimal? RemainingQuantity { get; set; }       
        public decimal? RemainingAmount { get; set; }
        public decimal? RemainingAmountCur { get; set; }
        public string ProductCode { get; set; }
        public string Note { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOrigin { get; set; }
        public decimal? ImportQuantity1 { get; set; }
        public decimal? ImportQuantity2 { get; set; }
        public int VoucherGroup { get; set; }
        public int Sort0 { get; set; }
    }
}
