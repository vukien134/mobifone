using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.ProductVouchers
{
    public class ProductVoucherCustomineDto
    {
        public int Year { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        public string BusinessCode { get; set; }
        public string BusinessAcc { get; set; }
        public string VoucherNumber { get; set; }
        public string InvoiceNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        public string PaymentTermsCode { get; set; }
        public string PartnerCode0 { get; set; }
        public string PartnerName0 { get; set; }
        public string Representative { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public string Place { get; set; }
        public string OriginVoucher { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? TotalAmountWithoutVatCur { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? TotalDiscountAmountCur { get; set; }
        public decimal? TotalDiscountAmount { get; set; }
        public decimal? TotalVatAmountCur { get; set; }
        public decimal? TotalVatAmount { get; set; }
        public string DebitOrCredit { get; set; }
        public decimal? TotalAmountCur { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalProductAmountCur { get; set; }
        public decimal? TotalProductAmount { get; set; }
        public decimal? TotalExciseTaxAmountCur { get; set; }
        public decimal? TotalExciseTaxAmount { get; set; }
        public decimal? TotalQuantity { get; set; }
        public string ExportNumber { get; set; }
        public decimal? TotalExpenseAmountCur0 { get; set; }
        public decimal? TotalExpenseAmount0 { get; set; }
        public decimal? TotalImportTaxAmountCur { get; set; }
        public decimal? TotalImportTaxAmount { get; set; }
        public decimal? TotalExpenseAmountCur { get; set; }
        public decimal? TotalExpenseAmount { get; set; }
        public string EmployeeCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; }
        public string PaymentTermsId { get; set; }
        public string SalesChannelCode { get; set; }
        public string BillNumber { get; set; }
        public decimal? DevaluationPercentage { get; set; }
        public decimal? TotalDevaluationAmountCur { get; set; }
        public decimal? TotalDevaluationAmount { get; set; }
        public string PriceDebitAcc { get; set; }
        public string PriceCreditAcc { get; set; }
        public string PriceDecreasingDescription { get; set; }
        public bool IsCreatedEInvoice { get; set; }
        public string InfoFilter { get; set; }
        public int? RefType { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ImportDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ExportDate { get; set; }
        //Phương tiện
        public string Vehicle { get; set; }
        //Phòng ban
        public string OtherDepartment { get; set; }
        public string CommandNumber { get; set; }
        public decimal? TotalExpenseAmountCur1 { get; set; }
        public decimal? TotalExpenseAmount1 { get; set; }

        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }

        public string ProductCode { get; set; }
        public string TransProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductName0 { get; set; }
        public string UnitCode { get; set; }
        public string TransUnitCode { get; set; }
        public string WarehouseCode { get; set; }
        public string TransWarehouseCode { get; set; }
        public decimal? TrxQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public decimal? TrxAmountCur { get; set; }
        public decimal? TrxAmount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public bool FixedPrice { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string ProductLotCode { get; set; }
        public string TransProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string TransProductOriginCode { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string ContractCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public decimal? PriceCur2 { get; set; }
        public decimal? Price2 { get; set; }
        public decimal? AmountCur2 { get; set; }
        public decimal? Amount2 { get; set; }
        public string DebitAcc2 { get; set; }
        public string CreditAcc2 { get; set; }
        public string TaxCategoryCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? InsuranceDate { get; set; }
        public string Note { get; set; }
        public string NoteE { get; set; }
        public decimal? HTPercentage { get; set; }
        public decimal? RevenueAmount { get; set; }
        public decimal? VatPriceCur { get; set; }
        public decimal? VatPrice { get; set; }
        //PT_GG

        //GIA_GG_NT
        public decimal? DevaluationPriceCur { get; set; }
        //GIA_GG
        public decimal? DevaluationPrice { get; set; }
        public decimal? DevaluationAmountCur { get; set; }
        public decimal? DevaluationAmount { get; set; }
        public decimal? VarianceAmount { get; set; }
        public string RefId { get; set; }
        public string ProductVoucherDetailId { get; set; }
        public decimal? VatTaxAmount { get; set; }
        //% Giảm trừ
        public decimal? DecreasePercentage { get; set; }
        // Tiền giảm trừ
        public decimal? DecreaseAmount { get; set; }
        public decimal? AmountWithVat { get; set; }
        //TIEN_ST tiền sau giảm trừ
        public decimal? AmountAfterDecrease { get; set; }
        public decimal? VatAmount { get; set; }
        public string AttachProductLot { get; set; }
        public string AttachProductOrigin { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmountCur { get; set; }
        public decimal? DiscountAmount { get; set; }

    }
}
