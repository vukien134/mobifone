using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class UpdatePriceUnfinishedInventoryDto
    {
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public Decimal? ExportQuantity { get; set; }
        public Decimal? ExportAmount { get; set; }
        public Decimal? BeginQuantity { get; set; }
        public Decimal? BeginAmount { get; set; }
        public Decimal? ExportPrice { get; set; }
        public Decimal? BeginPrice { get; set; }
    }
}
