using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class StatementOfValueAddedTaxParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string AccBuy { get; set; } = "133";
        public string AccSell { get; set; } = "33311";
        public string Extend { get; set; }
        public decimal? DeductPre { get; set; }
        public decimal? IncreasePre { get; set; }
        public decimal? ReducePre { get; set; }
        public decimal? SuggestionReturn { get; set; }
    }
}
