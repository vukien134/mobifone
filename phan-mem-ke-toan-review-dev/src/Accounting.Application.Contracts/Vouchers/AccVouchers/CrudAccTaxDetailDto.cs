using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.AccVouchers
{
    public class CrudAccTaxDetailDto : CruOrgBaseDto
    {
        public string AccVoucherId { get; set; }
        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        //CK_KT
        public string CheckDuplicate { get; set; }
        public string VoucherCode { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        public string TaxCategoryCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime InvoiceDate { get; set; }
        public string InvoiceGroup { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceNumber { get; set; }
        public string DebitAcc { get; set; }
        public decimal? DebitExchangeRate { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string ContractCode { get; set; }
        public string SectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string CreditAcc { get; set; }
        public decimal? CreditExchangeRate { get; set; }
        public string ClearingPartnerCode { get; set; }
        public string ClearingFProductWorkCode { get; set; }
        public string ClearingContractCode { get; set; }
        public string ClearingSectionCode { get; set; }
        public string ClearingWorkPlaceCode { get; set; }
        public string PartnerName { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string ProductName { get; set; }
        public decimal? AmountWithoutVatCur { get; set; }
        public decimal? AmountWithoutVat { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string NoteE { get; set; }
        public string CaseCode { get; set; }
        //Số CTGS
        public string RecordingVoucherNumber { get; set; }
        public decimal? TotalAmountCur { get; set; }
        public decimal? TotalAmount { get; set; }
        public string InvoiceLink { get; set; }
        public string SecurityNo { get; set; }
        public string InvoiceBookCode { get; set; }
    }
}
