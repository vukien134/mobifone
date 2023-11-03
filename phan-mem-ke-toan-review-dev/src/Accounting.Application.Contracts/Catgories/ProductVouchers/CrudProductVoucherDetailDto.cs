using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.ProductVouchers
{
    public class CrudProductVoucherDetailDto : CruOrgBaseDto
    {
        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string ProductCode { get; set; }
        public string TransProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductName0 { get; set; }
        public string UnitCode { get; set; }
        public string TransUnitCode { get; set; }
        public string WarehouseCode { get; set; }
        public string TransWarehouseCode { get; set; }
        public decimal? TrxQuantity { get; set; } = 0;
        public decimal? Quantity { get; set; } = 0;
        public decimal? PriceCur { get; set; } = 0;
        public decimal? Price { get; set; } = 0;
        public decimal? TrxAmountCur { get; set; } = 0;
        public decimal? TrxAmount { get; set; } = 0;
        public decimal? AmountCur { get; set; } = 0;
        public decimal? Amount { get; set; } = 0;
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
        public decimal? PriceCur2 { get; set; } = 0;
        public decimal? Price2 { get; set; } = 0;
        public decimal? AmountCur2 { get; set; } = 0;
        public decimal? Amount2 { get; set; } = 0;
        public string DebitAcc2 { get; set; }
        public string CreditAcc2 { get; set; }
        public string TaxCategoryCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? InsuranceDate { get; set; }
        public string Note { get; set; }
        public string NoteE { get; set; }
        public decimal? HTPercentage { get; set; } = 0;
        public decimal? RevenueAmount { get; set; } = 0;
        public decimal? VatPriceCur { get; set; } = 0;
        public decimal? VatPrice { get; set; } = 0;
        //PT_GG
        public decimal? DevaluationPercentage { get; set; } = 0;
        //GIA_GG_NT
        public decimal? DevaluationPriceCur { get; set; } = 0;
        //GIA_GG
        public decimal? DevaluationPrice { get; set; } = 0;
        public decimal? DevaluationAmountCur { get; set; } = 0;
        public decimal? DevaluationAmount { get; set; } = 0;
        public decimal? VarianceAmount { get; set; } = 0;
        public string RefId { get; set; }
        public string ProductVoucherDetailId { get; set; }
        public decimal? VatTaxAmount { get; set; } = 0;
        //% Giảm trừ
        public decimal? DecreasePercentage { get; set; } = 0;
        // Tiền giảm trừ
        public decimal? DecreaseAmount { get; set; } = 0;
        public decimal? AmountWithVat { get; set; } = 0;
        //TIEN_ST tiền sau giảm trừ
        public decimal? AmountAfterDecrease { get; set; }
        public decimal? VatAmount { get; set; }


        public decimal? VatPercentage { get; set; } = 0;
        public decimal? VatAmountCur { get; set; } = 0;

        public decimal? DiscountPercentage { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? DiscountAmountCur { get; set; } = 0;
        public decimal? ImportTaxPercentage { get; set; } = 0;
        public decimal? ImportTaxAmountCur { get; set; } = 0;
        public decimal? ImportTaxAmount { get; set; } = 0;
        public decimal? ExciseTaxPercentage { get; set; } = 0;
        public decimal? ExciseTaxAmountCur { get; set; } = 0;
        public decimal? ExciseTaxAmount { get; set; } = 0;
        public decimal? ExpenseAmountCur0 { get; set; } = 0;
        public decimal? ExpenseAmount0 { get; set; } = 0;
        public decimal? ExpenseAmountCur1 { get; set; } = 0;
        public decimal? ExpenseAmount1 { get; set; } = 0;
        public decimal? ExpenseAmountCur { get; set; } = 0;
        public decimal? ExpenseAmount { get; set; } = 0;
        public decimal? ExpenseImportTaxAmountCur { get; set; } = 0;
        public decimal? ExpenseImportTaxAmount { get; set; } = 0;
        public decimal? ExciseAmountCur { get; set; } = 0;
        public decimal? ExciseAmount { get; set; } = 0;
        public string AttachProductLot { get; set; }
        public string AttachProductOrigin { get; set; }
        public string ProductAcc { get; set; }
        public string ProductType { get; set; }
        public string ImportAcc { get; set; }
        public string ExportAcc { get; set; }
        public decimal? TrxImportQuantity { get; set; } = 0;
        public decimal? TrxExportQuantity { get; set; } = 0;
        public decimal? ImportQuantity { get; set; } = 0;
        public decimal? ImportAmountCur { get; set; } = 0;
        public decimal? ImportAmount { get; set; } = 0;
        public decimal? ExportQuantity { get; set; } = 0;
        public decimal? ExportAmountCur { get; set; } = 0;
        public decimal? ExportAmount { get; set; } = 0;
        public decimal? TrxPrice { get; set; } = 0;
        public decimal? TrxPriceCur { get; set; } = 0;
        public string WareHouseAcc { get; set; }
        public decimal? TrxQuantity2 { get; set; } = 0;
        public decimal? TrxPrice2 { get; set; } = 0;
        public decimal? TrxPriceCur2 { get; set; } = 0;
        public decimal? QuantityTrx { get; set; } = 0;
        public decimal? Price0 { get; set; } = 0;
        public decimal? PriceCur0 { get; set; } = 0;
        public List<CrudProductVoucherDetailReceiptDto> ProductVoucherDetailReceipts { get; set; }
    }
}
