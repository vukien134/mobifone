using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class RatioDto
    {
        public string Id { get; set; }
        public string GroupCoefficientDetailId { get; set; }
        public string OrgCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string ProductCode { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal? QuantityNorm { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? AllotmentValue { get; set; }
        public decimal? Ratio { get; set; }
        public decimal? RatioAll { get; set; }
        public decimal? Coefficient { get; set; }
    }
}
