using System;
namespace Accounting.Vouchers.Ledgers
{
    public class LedgerMultiDto
    {
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        public string BusinessAcc { get; set; }
        public string BusinessCode { get; set; }
        public string VoucherNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime VoucherDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public string PartnerCode0 { get; set; }
        public string Representative { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string OriginVoucher { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
        public string Ord0 { get; set; }
        public string ContractCode { get; set; }
        public string ContractCodeBT { get; set; }
        public string CreditAcc { get; set; }
        public string DebitAcc { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string FProductWorkCodeBT { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerCodeBt { get; set; }
        public string SectionCode { get; set; }
        public string SectionCodeBt { get; set; }
        public string CaseCode { get; set; }
        public string TaxCategoryCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceSerial { get; set; }
        public string InvoiceNumbers { get; set; }
        public decimal? VatPercentage { get; set; }
        public string PartnerNameBt { get; set; }
        public string AddressInv { get; set; }
        public string TaxCode { get; set; }
        public string CurrencyCodeDebit { get; set; }
        public decimal ExchangeRateDebit { get; set; }
        public decimal? AmountCurDebit { get; set; }
        public string PartnerCodeDebit { get; set; }
        public string ContractCodeDebit { get; set; }
        public string FProductWorkCodeDebit { get; set; }
        public string SectionCodeDebit { get; set; }
        public string WorkPlaceCodeDebit { get; set; }
        public string CurrencyCodeCredit { get; set; }
        public decimal? ExchangeRateCredit { get; set; }
        public decimal? AmountCurCredit { get; set; }
        public string PartnerCodeCredit { get; set; }
        public string ContractCodeCredit { get; set; }
        public string FProductWorkCodeCredit { get; set; }
        public string SectionCodeCredit { get; set; }
        public string WorkPlaceCodeCredit { get; set; }
        public string CheckDuplicate { get; set; }
        public string WarehouseCode { get; set; }
        public string AssemblyWarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public string ProductName0 { get; set; }
    }
}
