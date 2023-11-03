using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.RecordingVouchers
{
    public class RecordingVoucherReportParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public int RankAccCode { get; set; }
        public string FromNumber { get; set; }
        public string ToNumber { get; set; }
    }
}
