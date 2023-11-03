using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Vouchers.VoucherPaymentBeginnings
{
    public class VoucherPaymentBeginningDto : TenantOrgDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public decimal TotalAmountWithoutVat { get; set; }
        public decimal TotalAmountDiscount { get; set; }
        public decimal TotalAmountVat { get; set; }
        public decimal TotalAmount { get; set; }
        public int Year { get; set; }
        public string AccCode { get; set; }
        public string PaymentType { get; set; }
        public string Description { get; set; }
    }
}
