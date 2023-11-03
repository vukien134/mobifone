using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Taxes.SalesTaxDirectLists
{
    public class SalesTaxDirectListDataTaxDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public int ViewType { get; set; }
        public string OrgCode { get; set; }
        public string CheckDuplicate { get; set; }
        public string Id { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        public string ContractCode { get; set; }
        public string DepartmentCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? InvoiceDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string TaxCategoryCode { get; set; }
        public string InvoiceGroup { get; set; }
        public string InvoiceBookCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string InvoiceSymbol { get; set; }
        public string InvoiceNumber { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string ProductName { get; set; }
        public decimal? AmountWithoutVatCur { get; set; }
        public decimal? AmountWithoutVat { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public string Note { get; set; }
        public int Deduct { get; set; }
        public string OutOrIn { get; set; }
        public string DebitAcc0 { get; set; }
        public string CreditAcc0 { get; set; }
    }
}
