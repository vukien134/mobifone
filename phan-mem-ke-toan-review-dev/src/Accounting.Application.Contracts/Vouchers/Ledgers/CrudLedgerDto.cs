using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.Ledgers
{
    public class CrudLedgerDto : CruOrgBaseDto
    {
        public string VoucherId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        //REC_HT0
        public string Ord0Extra { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        public string BusinessCode { get; set; }
        public string BusinessAcc { get; set; }
        public string CheckDuplicate { get; set; }
        public string VoucherNumber { get; set; }
        public string InvoiceNbr { get; set; }
        //Số CTGS
        public string RecordingVoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        //Số ngày
        public int? Days { get; set; }
        public string PaymentTermsCode { get; set; }
        public string ContractCode { get; set; }
        public string ClearingContractCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string PartnerCode0 { get; set; }
        public string PartnerName0 { get; set; }
        public string Representative { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public string OriginVoucher { get; set; }
        public string DebitAcc { get; set; }
        public decimal? DebitExchangeRate { get; set; }
        public string DebitCurrencyCode { get; set; }
        public string DebitPartnerCode { get; set; }
        public string DebitContractCode { get; set; }
        public string DebitFProductWorkCode { get; set; }
        public string DebitSectionCode { get; set; }
        public string DebitWorkPlaceCode { get; set; }
        public decimal? DebitAmountCur { get; set; }
        public string CreditAcc { get; set; }
        public string CreditCurrencyCode { get; set; }
        public decimal? CreditExchangeRate { get; set; }
        public string CreditPartnerCode { get; set; }
        public string CreditContractCode { get; set; }
        public string CreditFProductWorkCode { get; set; }
        public string CreditSectionCode { get; set; }
        public string CreditWorkPlaceCode { get; set; }
        public decimal? CreditAmountCur { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string NoteE { get; set; }
        public string FProductWorkCode { get; set; }
        public string PartnerCode { get; set; }
        public string SectionCode { get; set; }
        public string ClearingFProductWorkCode { get; set; }
        public string ClearingPartnerCode { get; set; }
        public string ClearingSectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string CaseCode { get; set; }
        public string WarehouseCode { get; set; }
        public string TransWarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public string UnitCode { get; set; }
        //Số lượng giao dịch
        public decimal? TrxQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TrxPromotionQuantity { get; set; }
        public decimal? PromotionQuantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public string TaxCategoryCode { get; set; }
        public decimal? VatPercentage { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSymbol { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? InvoiceDate { get; set; }
        public string InvoicePartnerName { get; set; }
        public string InvoicePartnerAddress { get; set; }
        public string TaxCode { get; set; }
        //N_C
        public string DebitOrCredit { get; set; }
        //CK_KT0
        public string CheckDuplicate0 { get; set; }
        public string SalesChannelCode { get; set; }
        public string ProductName0 { get; set; }
        //Mã bảo mật
        public string SecurityNo { get; set; }
        public string Status { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
