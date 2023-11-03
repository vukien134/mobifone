using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class SalesTransactionListDto
    {
        public int Tag { get; set; }
        public string SortProductGroup { get; set; }
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
        public decimal InventoryAmount { get; set; }
        public decimal InventoryAmountCur { get; set; }
        public decimal InventoryQuantity { get; set; }
        public int Grp1 { get; set; }
        public string GroupName { get; set; }
        public decimal? TotalAmount2 { get; set; }
        public decimal? AmountFundsCur { get; set; }
        public decimal? AmountFunds { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate01 { get; set; }
        public string Description { get; set; }
        public string PartnerCode { get; set; }
        public string OrdVoucher { get; set; }
        public string Note { get; set; }
        public string Unit { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ImportAmount { get; set; }
        public decimal? ImportAmountCur { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public decimal? Quantity { get; set; }
        public string ReciprocalAcc { get; set; }
        public string VoucherNumber1 { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? PriceCur { get; set; }
        public string BusinessAcc { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? VatAmountCur { get; set; }
        public string Representative { get; set; }
        public decimal? Price0 { get; set; }
        public decimal? PriceCur0 { get; set; }
        public decimal? ExportAmount { get; set; }
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
        public string DebitAcc2 { get; set; }
        public string CreditAcc2 { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string SalesChannelName { get; set; }
        public int RankProduct { get; set; }
        public string Address { get; set; }

        public string TransWarehouseCode { get; set; }
        public string SectionName { get; set; }
        public decimal? DebitAcc111 { get; set; }
        public string CurrencyName { get; set; }
        public decimal? DebitAcc131 { get; set; }
        public decimal? Price { get; set; }
        public string DepartmentCode { get; set; }
        public string PartnerName { get; set; }
        public int VoucherGroup { get; set; }
        public decimal? ExpenseAmount1 { get; set; }
        public decimal? ExprenseAmountCur { get; set; }
        public decimal? AmountVat { get; set; }
        public decimal? AmountPay { get; set; }
        public decimal? AmountPayCur { get; set; }
        public decimal? AmountVatCur { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public decimal? ExpenseAmountCur1 { get; set; }
        public string GroupCode { get; set; }
        public decimal? ExpenseAmount0 { get; set; }
        public string CaseCode { get; set; }
        public string CaseName { get; set; }
        public decimal? DevaluationAmountCur { get; set; }
        public decimal? ExpenseAmountCur0 { get; set; }
        public decimal? DevaluationAmount { get; set; }
        public decimal? AmountCur2 { get; set; }
        public decimal? Amount2 { get; set; }
        public string DescriptionE { get; set; }
        public decimal? PriceCur2 { get; set; }
        public decimal? Price2 { get; set; }
        public string ProductOriginCode { get; set; }
        public string CurrencyCode { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string AccName { get; set; }
        public string SalesChannelCode { get; set; }
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
        public decimal? Payments { get; set; }
        public decimal? PaymentsCur { get; set; }
        public decimal? Profit { get; set; }
    }
}

