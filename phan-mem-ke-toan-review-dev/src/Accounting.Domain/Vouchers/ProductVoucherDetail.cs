using Accounting.JsonConverters;
using Accounting.TenantEntities;
using System;
using System.Collections.Generic;

namespace Accounting.Vouchers
{
    public class ProductVoucherDetail : TenantOrgEntity
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
        public string SalechannelCode { get;set; } 
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
        public decimal? DevaluationPercentage { get; set; }
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
        public ProductVoucher ProductVoucher { get; set; }
        public ICollection<ProductVoucherDetailReceipt> ProductVoucherDetailReceipts { get; set; }
    }
}
