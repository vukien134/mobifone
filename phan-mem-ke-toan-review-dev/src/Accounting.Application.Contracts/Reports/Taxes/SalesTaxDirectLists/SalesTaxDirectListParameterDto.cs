using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Taxes.NewFolder
{
    public class SalesTaxDirectListParameterDto
    {
        public string FProducWorkCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string AccCode { get; set; }
        public string Sort { get; set; }
        public string CurrencyCode { get; set; }
    }
}
