using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class DirectExportImportListDto
    {
        public int Tag { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int Sort { get; set; }
        public string SortProductGroup { get; set; }
        public string VoucherId { get; set; }
        public string Id { get; set; }
        public string Ord0 { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PartnerCode { get; set; }
        public string GroupCode { get; set; }
        public string Unit { get; set; }
        public string SectionCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string CaseCode { get; set; }
        public string DepartmentCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherNumber1 { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public string AccCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceNumber { get; set; }
        public string ReciprocalAcc { get; set; }
        public string Representative { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatAmountCur { get; set; }
        public string CaseName { get; set; }
        public string PartnerName { get; set; }
        public string SectionName { get; set; }
        public string DepartmentName { get; set; }
        public string FProductWorkName { get; set; }
        public string CurrencyName { get; set; }
        public string AccName { get; set; }
        public int VoucherGroup { get; set; }
        public decimal? VatPercentage { get; set; }
        public string Address { get; set; }
        public string WarehouseName { get; set; }
        public string GroupName { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountCur { get; set; }
    }
}
