using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class PriceFProductDto
    {
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? CompletedQuantity2 { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalZ { get; set; }
    }
}
