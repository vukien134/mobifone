using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.DebitBooks
{
    public class CompareConfirmBebtDto
    {
        public string Sort { get; set; }
        public string Status { get; set; }
        public int Sort2 { get; set; }
        public string Sort1 { get; set; }
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
        public string CurrencyCode { get; set; }
        public string InvoicePartnerName { get; set; }
        public string CreditPartnerCode { get; set; }
        public string DebitPartnerCode { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? Debit { get; set; }
        public decimal? DebitCur { get; set; }
        public decimal? Credit { get; set; }
        public decimal? CreditCur { get; set; }
        public int Grp1 { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string Description { get; set; }
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
        public decimal? Balance { get; set; }
        public decimal? BalanceCur { get; set; }
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
        public string TaxCode { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public decimal? TotalDebit { get; set; }
        public decimal? TotalCredit { get; set; }
    }
}

