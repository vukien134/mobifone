using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.Others
{
    public class BalanceAccDto
    {
        public int Sort { get; set; }
        public string Status { get; set; }
        public int Sort0 { get; set; }
        public int Sort1 { get; set; }
        public string Ord0 { get; set; }
        public string VoucherId { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? PriceCur { get; set; }
        public string BusinessAcc { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? VatAmountCur { get; set; }
        public decimal? Price0 { get; set; }
        public decimal? PriceCur0 { get; set; }
        public decimal? ExportAmount { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public decimal? VatAmount { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? Amount0 { get; set; }
        public decimal? AmountCur0 { get; set; }

        public string Id { get; set; }
        public string FProductWorkCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceNumber { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string TransWarehouseCode { get; set; }
        public string CreditAcc2 { get; set; }
        public decimal? Price { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public int VoucherGroup { get; set; }
        public string GroupCode { get; set; }
        public string CaseCode { get; set; }
        public string CaseName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string AccName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string FProductWorkName { get; set; }
        public string CheckDuplicate { get; set; }
        public string ProductGroupName { get; set; }
        public string SectionCode { get; set; }
        public string DebitContractCode { get; set; }
        public string DebitWorkPlaceCode { get; set; }
        public string DebitFProductWorkCode { get; set; }
        public string DebitSectionCode { get; set; }
        public decimal? DebitAmountCur { get; set; }
        public decimal? CreditAmountCur { get; set; }
        public decimal? QuanltityDky { get; set; }
        public decimal? AmountDky { get; set; }
        public decimal QuanltityCky { get; set; }
        public decimal AmountCky { get; set; }
        public string AccCode { get; set; }
        public string ContractCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode0 { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public DateTime? CreationTime { get; set; }
        public string CheckDuplicate0 { get; set; }
        public decimal Residual { get; set; }
        public decimal ResidualCur { get; set; }
        public string CurrencyCode { get; set; }
    }
}

