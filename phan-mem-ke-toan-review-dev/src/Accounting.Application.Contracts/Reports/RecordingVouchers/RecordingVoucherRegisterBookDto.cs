using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.RecordingVouchers
{
    public class RecordingVoucherRegisterBookDto
    {
        public string Bold { get; set; }
        public string Sort { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string Acc { get; set; }
        public string RecordingVoucherNumber { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? RecordingVoucherDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? ToDate { get; set; }
    }
}
