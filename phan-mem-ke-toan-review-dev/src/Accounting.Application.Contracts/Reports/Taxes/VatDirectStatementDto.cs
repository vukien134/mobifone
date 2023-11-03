using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Taxes
{
    public class VatDirectStatementDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string Ord { get; set; }
        public string CareerGroup { get; set; }
        public string NumberCode1 { get; set; }
        public decimal? Turnover1 { get; set; }
        public string NumberCode2 { get; set; }
        public decimal? Turnover2 { get; set; }
        public string TurnoverRatio { get; set; }
        public string NumberCode3 { get; set; }
        public decimal? Turnover3 { get; set; }
    }
}
