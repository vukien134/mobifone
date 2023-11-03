using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class ReturnToSupplierListDto
    {
        public int Tag { get; set; }
        public int Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string GroupCode { get; set; }
        public string Description { get; set; }
        public string PartnerCode { get; set; }
        public string Note { get; set; }
        public string DepartmentCode { get; set; }
        public string Unit { get; set; }
        public string WarehouseCode { get; set; }
        public string BusinessAcc { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public string PartnerName { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatAmountCur { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public decimal? ExpenseAmountCur { get; set; }
        public string ProductCode { get; set; }
        public decimal? TurnoverAmount { get; set; }
        public decimal? TurnoverAmountCur { get; set; }
    }
}

