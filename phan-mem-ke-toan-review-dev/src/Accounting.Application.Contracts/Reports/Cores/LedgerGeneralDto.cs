using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class LedgerGeneralDto
    {
        public string IncurredType { get; set; }
        public string Id { get; set; }
        public string VoucherId { get; set; }

        public string Ord0 { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        public string BusinessCode { get; set; }
        public string BusinessAcc { get; set; }
        //CK_KT
        public string CheckDuplicate { get; set; }
        public string VoucherNumber { get; set; }
        public string InvoiceNbr { get; set; }
        public string RecordingVoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public int? Days { get; set; }
        public string PaymentTermsCode { get; set; }
        public string Representative { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public string OriginVoucher { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string Status { get; set; }
        public string DebitAcc { get; set; }
        public string DebitCurrencyCode { get; set; }
        public decimal? DebitExchangeRate { get; set; }
        public string DebitPartnerCode { get; set; }
        public string DebitContractCode { get; set; }
        public string DebitFProductWorkCode { get; set; }
        public string DebitSectionCode { get; set; }
        public string DebitWorkPlaceCode { get; set; }
        public decimal? DebitAmountCur { get; set; }
        public string Acc { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string PartnerCode { get; set; }
        public string ContractCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string CreditAcc { get; set; }
        public string CreditCurrencyCode { get; set; }
        public decimal? CreditExchangeRate { get; set; }
        public string CreditPartnerCode { get; set; }
        public string CreditContractCode { get; set; }
        public string CreditFProductWorkCode { get; set; }
        public string CreditSectionCode { get; set; }
        public string CreditWorkPlaceCode { get; set; }
        public decimal? CreditAmountCur { get; set; }
        //TK_DU
        public string ReciprocalAcc { get; set; }
        public string ReciprocalCurrencyCode { get; set; }
        public decimal? ReciprocalExchangeRate { get; set; }
        public string ReciprocalPartnerCode { get; set; }
        public string ReciprocalContractCode { get; set; }
        public string ReciprocalFProductWorkCode { get; set; }
        public string ReciprocalSectionCode { get; set; }
        public string ReciprocalWorkPlaceCode { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        public decimal? CreditIncurred { get; set; }
        public string Note { get; set; }
        public string NoteE { get; set; }
        public decimal? AmountCur0 { get; set; }
        public decimal? Amount0 { get; set; }
        public string ContractCode0 { get; set; }
        public string FProductWorkCode0 { get; set; }
        public string PartnerCode0 { get; set; }
        public string PartnerName0 { get; set; }
        public string SectionCode0 { get; set; }
        public string CaseCode0 { get; set; }
        public string WorkPlaceCode0 { get; set; }
        public string ClearingPartnerCode { get; set; }
        public string ClearingContractCode { get; set; }
        public string ClearingFProductWorkCode { get; set; }
        public string ClearingSectionCode { get; set; }
        public string WarehouseCode { get; set; }
        public string TransWarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProducOrginCode { get; set; }
        public string UnitCode { get; set; }
        public decimal? TrxQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TrxPromotionQuantity { get; set; }
        public decimal? PromotionQuantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public string TaxCategoryCode { get; set; }
        public decimal? VatPercenatge { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSymbol { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoicePartnerName { get; set; }
        public string InvoicePartnerAddress { get; set; }
        public string TaxCode { get; set; }
        public string DebitOrCredit { get; set; }
        public string IncurredType0 { get; set; }
        //CK_KT0
        public string CheckDuplicate0 { get; set; }
    }
}
