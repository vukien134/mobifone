using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.RecordingVouchers
{
    public class RecordingVoucherReportDto
    {
        public string Bold { get; set; }
        public string Sort0 { get; set; }
        public string Sort1 { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string RecordingVoucherNumber { get; set; }
        public string Description { get; set; }
        public string DescriptionRV { get; set; }
        public string Acc { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? Incurred { get; set; }
        public string ReciprocalAcc { get; set; }
    }
}
