using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.VoucherNumbers
{
    public class CrudVoucherPaymentBookDto : CruOrgBaseDto
    {
        public string AccVoucherId { get; set; }
        public string DocumentId { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public decimal? Amount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? DeadlinePayment { get; set; }
        public decimal AmountReceivable { get; set; }
        public int Times { get; set; }
        public decimal? AmountReceived { get; set; }
        public int Year { get; set; }
        public string AccType { get; set; }
    }
}
