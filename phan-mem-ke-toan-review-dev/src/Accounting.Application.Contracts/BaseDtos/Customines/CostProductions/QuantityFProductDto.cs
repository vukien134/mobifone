using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class QuantityFProductDto
    {
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string ProductCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnfinishedQuantity { get; set; }
        public decimal? CompletedQuantity { get; set; }
        public decimal? HTPercentage { get; set; }
        public decimal? UnfinishedQuantity2 { get; set; }
        public decimal? CompletedQuantity2 { get; set; }
        public decimal? HTPercentage2 { get; set; }
    }
}
