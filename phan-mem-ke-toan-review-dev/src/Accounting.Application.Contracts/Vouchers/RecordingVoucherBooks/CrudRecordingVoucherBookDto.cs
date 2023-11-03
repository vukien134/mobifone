using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.RecordingVoucherBooks
{
    public class CrudRecordingVoucherBookDto : CruOrgBaseDto
    {
        public int Year { get; set; }
        public string VoucherNumber { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }

        public string Description { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string LstVoucherCode { get; set; }
        public int TypeDescription { get; set; }
        public int TypeFilter { get; set; }
    }
}
