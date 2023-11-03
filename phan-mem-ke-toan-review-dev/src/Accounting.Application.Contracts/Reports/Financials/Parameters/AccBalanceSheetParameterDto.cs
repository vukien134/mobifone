using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class AccBalanceSheetParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string LstOrgCode { get; set; }
        public int? IsSummary { get; set; } = 0;
        public int? ClearingIncurred { get; set; } = 0; // psbt
        public int? ClearingIncurredFP { get; set; } = 1; // psbtcs
        public int? UsingDecision { get; set; } = null; // QD_TC
        public string Check { get; set; } = "K"; // Check
        public DateTime? FromDate0
        {
            get
            {
                return this.FromDate;
            }
        }

        public DateTime? ToDate0
        {
            get
            {
                return this.ToDate;
            }
        }
    }
}
