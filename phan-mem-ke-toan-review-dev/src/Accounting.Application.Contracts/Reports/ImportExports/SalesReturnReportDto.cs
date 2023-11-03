using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class SalesReturnReportDto
    {
        public int Sort { get; set; }
        public int Sort0 { get; set; }
        public int Sort1 { get; set; }
        public string VoucherId { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string Note { get; set; }
        public string GroupName { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }

        public string Id { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ExportAmount { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public string WorkPlaceCode { get; set; }
        public string BusinessCode { get; set; }
        public decimal? TurnoverAmount { get; set; }
        public decimal? TurnoverAmountCur { get; set; }
        public decimal? AmountFunds { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
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
        public string ProductGroupName { get; set; }
        public int Tag { get; set; }
        public string SortGroupProduct { get; set; }
        public string SectionCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string ProductLotCode { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatAmountCur { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public decimal? ExpenseAmountCur { get; set; }
        public decimal? ImportTaxAmount { get; set; }
        public decimal? ImportTaxAmountCur { get; set; }
        public decimal? AmountPay { get; set; }
        public decimal? AmountPayCur { get; set; }
        public string AccCode { get; set; }
        public string ReciprocalAcc { get; set; }
        public string SectionName { get; set; }
        public string ProductOriginName { get; set; }
        public string ProductLotName { get; set; }
        public string CurrencyName { get; set; }
    }

}

