using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class GetIncurredExpensesFilterDto
    {
        public int Year { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string AccCode { get; set; }
        public string SectionCode { get; set; }
        public int? DebitCredit { get; set; }
        public string OrdRec { get; set; }
        public int ForwardType { get; set; }
        public string FProductWork { get; set; }
        public string AttachProduct { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string Round { get; set; } = "K";
        public string NC { get; set; }

    }
}
