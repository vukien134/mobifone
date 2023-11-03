using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class VoucherPaymentBook : TenantOrgEntity
    {
        public string AccVoucherId { get; set; }
        public string DocumentId { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public decimal Amount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DeadlinePayment { get; set; }
        public decimal AmountReceivable { get; set; }
        public int Times { get; set; }
        public decimal AmountReceived { get; set; }
        public int Year { get; set; }
        public string AccType { get; set; }
    }
}
