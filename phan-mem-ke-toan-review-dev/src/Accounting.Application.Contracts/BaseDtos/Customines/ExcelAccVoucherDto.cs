using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class ExcelAccVoucherDto
    {
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        public string VoucherCode { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime InvoiceDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public string PartnerCode { get; set; }
        public string ClearingPartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string TaxCode { get; set; }
        public string Representative { get; set; }
        public string Address { get; set; }
        public string ProductName { get; set; }
        public decimal AmountCur { get; set; }
        public decimal Amount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal TotalTaxAmountCur { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public string Description { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string DebitAccTax { get; set; }
        public string CreditAccTax { get; set; }
        public string ContractCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public string NotTax { get; set; }
        public string DelExit { get; set; }
        public string OriginVoucher { get; set; }
        public string TaxCategoryCode { get; set; }
        public string Ord { get; set; }
        public string DepartmentCode { get; set; }
        public string BusinessCode { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
    }
}
