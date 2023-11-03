using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.VoucherExciseTaxs
{
    public class CrudVoucherExciseTaxDto : CruOrgBaseDto
    {
        public string AccVoucherId { get; set; }
        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        public string DepartmentCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
        //Mã thuế tiêu thụ đặc biệt
        public string ExciseTaxCode { get; set; }
        public string InvoiceGroup { get; set; }
        public string InvoiceBookCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceSerial { get; set; }
        public string InvoiceNumber { get; set; }
        public string DebitAcc { get; set; }
        public decimal? DebitExchange { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string ContractCode { get; set; }
        public string SectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string CreditAcc { get; set; }
        public decimal? CreditExchange { get; set; }
        public string CleaningPartnerCode { get; set; }
        public string CleaningFProducWorkCode { get; set; }
        public string CleaningContractCode { get; set; }
        public string CleaningSectionCode { get; set; }
        public string CleaningWorkPlaceCode { get; set; }
        public string PartnerName { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string ProductName { get; set; }
        public decimal? AmountWithoutTaxCur { get; set; }
        public decimal? AmountWithoutTax { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string NoteE { get; set; }
        public string CaseCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public string ProductCode0 { get; set; }
        public string UnitCode0 { get; set; }
        public string ProductName0 { get; set; }
        public string Type { get; set; }
    }
}
