using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.GeneralDiaries
{
    public class GeneralDiaryBookDto
    {
        public int Sort0 { get; set; }
        public int Sort1 { get; set; }
        public string OrgCode { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string Acc { get; set; }
        public string ReciprocalAcc { get; set; }
        public string Note { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public string Bold { get; set; }
        public string Representation { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public int? RowOrd { get; set; }
    }
}
