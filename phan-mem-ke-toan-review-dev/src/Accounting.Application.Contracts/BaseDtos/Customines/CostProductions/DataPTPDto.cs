using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class DataPTPDto
    {
        public string Id { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
    }
}
