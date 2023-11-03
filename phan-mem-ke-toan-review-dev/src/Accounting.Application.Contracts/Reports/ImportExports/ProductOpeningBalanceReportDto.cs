using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.ImportExports
{
    public class ProductOpeningBalanceReportDto
    {
        public string OrgCode { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string WarehouseCode { get; set; }
        public string AccCode { get; set; }
        public string ProductCode { get; set; }
        //Mã lô hàng
        public string ProductLotCode { get; set; }
        //Mã nguồn
        public string ProductOriginCode { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ImportAmount { get; set; }
        public decimal? ImportAmountCur { get; set; }
        public decimal? ImportQuantity1 { get; set; }
        public decimal? ImportAmount1 { get; set; }
        public decimal? ImportAmountCur1 { get; set; }
        public decimal? ImportQuantity2 { get; set; }
        public decimal? ImportAmount2 { get; set; }
        public decimal? ImportAmountCur2 { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ExportAmount { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public decimal? Amount2 { get; set; }
        public decimal? AmountCur2 { get; set; }
    }
}
