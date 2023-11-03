using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Taxes
{
    public class DeclarationExciseTaxDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string Ord { get; set; }
        public string Ord0 { get; set; }
        public string ExciseTaxCode { get; set; }
        public string ProductName { get; set; }
        public string UnitCode { get; set; }
        public decimal? QuantityConsume { get; set; }
        public decimal? TurnoverWithoutVat { get; set; }
        public decimal? TurnoverExcise { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
        public decimal? ExciseTaxDeduct { get; set; }
        public decimal? AmountExciseTax { get; set; }
        public string HtkkName { get; set; }
        public string Htkk { get; set; }
        public string Htkk0 { get; set; }
        public string Type { get; set; }
    }
}
