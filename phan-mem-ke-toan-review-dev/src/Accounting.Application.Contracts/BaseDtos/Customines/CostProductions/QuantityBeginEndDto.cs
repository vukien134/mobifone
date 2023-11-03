using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class QuantityBeginEndDto
    {
        public string WorkPlaceCode { get; set; }
        public string ProductCode { get; set; }
        public decimal? UnfinishedQuantity { get; set; }
        public decimal? CompletedQuantity { get; set; }
        public decimal? HTPercentage { get; set; }
    }
}
