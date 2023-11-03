using Accounting.JsonConverters;
using System;
namespace Accounting.Reports.Others
{
    public class ImportTempVoucherDto
    {
        public string DocId { get; set; }
        public string VoucherId { get; set; }

        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string Description { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalAmountCur { get; set; }
        public string Status { get; set; }
        public string VoucherCode { get; set; }
    }
}

