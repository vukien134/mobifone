using Accounting.JsonConverters;
using Accounting.TenantEntities;
using System;

namespace Accounting.Vouchers
{
    public class WarehouseBook : TenantOrgEntity
    {
        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }
        //REC_HT
        public string Ord0Extra { get; set; }
        public int Year { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        public string BusinessCode { get; set; }
        public string BusinessAcc { get; set; }
        public string VoucherNumber { get; set; }
        
        public DateTime VoucherDate { get; set; }
        public string PaymentTermsCode { get; set; }
        public string ContractCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string PartnerCode0 { get; set; }
        public string Representative { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        //Địa điểm
        public string Place { get; set; }
        public string OriginVoucher { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public string ProductCode { get; set; }
        public string TransProductCode { get; set; }
        public string UnitCode { get; set; }
        public string TransferingUnitCode { get; set; }
        public string WarehouseCode { get; set; }
        public string TransWarehouseCode { get; set; }
        public string ProductLotCode { get; set; }
        public string TransProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string TransProductOriginCode { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public decimal? TrxQuantity { get; set; }
        public decimal? TrxPrice { get; set; }
        public decimal? TrxPriceCur { get; set; }
        public decimal? QuantityCur { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        //T_CP_NT0
        public decimal? ExpenseAmountCur0 { get; set; }
        //T_CP0
        public decimal? ExpenseAmount0 { get; set; }
        //T_CP_NT1
        public decimal? ExpenseAmountCur1 { get; set; }
        //T_CP0
        public decimal? ExpenseAmount1 { get; set; }
        public decimal? ExprenseAmountCur { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? VatAmountCur { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ImportTaxPercentage { get; set; }
        public decimal? ImportTaxAmountCur { get; set; }
        public decimal? ImportTaxAmount { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
        public decimal? ExciseTaxAmountCur { get; set; }
        public decimal? ExciseTaxAmount { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        //GIA_GD_NT2
        public decimal? TrxPriceCur2 { get; set; }
        public decimal? TrxPrice2 { get; set; }
        public decimal? PriceCur2 { get; set; }
        public decimal? Price2 { get; set; }
        public decimal? AmountCur2 { get; set; }
        public decimal? Amount2 { get; set; }

        public string DebitAcc2 { get; set; }
        public string CreditAcc2 { get; set; }

        public string Note { get; set; }
        public string NoteE { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceCur0 { get; set; }
        public decimal? Price0 { get; set; }
        public string ImportAcc { get; set; }
        public decimal? TrxImportQuantity { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ImportAmountCur { get; set; }
        public decimal? ImportAmount { get; set; }
        public string ExportAcc { get; set; }
        public decimal? TrxExportQuantity { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public decimal? ExportAmount { get; set; }
        public bool FixedPrice { get; set; }
        public bool CalculateTransfering { get; set; }
        public string DebitOrCredit { get; set; }
        public string TaxCode { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoicePartnerName { get; set; }
        public string InvoicePartnerAddress { get; set; }
        
        public DateTime? InvoiceDate { get; set; }
        public string SalesChannelCode { get; set; }
        public string BillNumber { get; set; }
        public decimal? VatPriceCur { get; set; }
        public decimal? VatPrice { get; set; }
        public decimal? DevaluationPercentage { get; set; }
        public decimal? DevaluationPrice { get; set; }
        public decimal? DevaluationPriceCur { get; set; }
        public decimal? DevaluationAmountCur { get; set; }
        public decimal? DevaluationAmount { get; set; }
        public decimal? VarianceAmount { get; set; }
        public string ProductName0 { get; set; }
        public decimal? TotalAmount2 { get; set; }
        public decimal? TotalDiscountAmount { get; set; }
        public string Status { get; set; }
        public string ChannelCode { get; set; }
        public ProductVoucher ProductVoucher { get; set; }
        public bool? Locked { get; set; }
    }
}
