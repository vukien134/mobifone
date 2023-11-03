using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Catgories.Others.Currencies;
using Accounting.Catgories.ProductVouchers;
using Accounting.JsonConverters;
using Accounting.Vouchers.AccVouchers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.Printings
{
    public class PrintingProductVoucherDto
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
        public DateTime DeliveryDate { get; set; }
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
        public string VoteMaker { get; set; }
        public Guid? CreatorId { get; set; }
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
        public string AmountByWord { get; set; }
        public OrgUnitDto OrgUnitDto { get; set; }
        public CurrencyDto Currency { get; set; }
        public dynamic TenantSetting { get; set; }
        public List<PrintingVoucherAccount> PrintingVoucherAccounts { get; set; }
        public List<AccTaxDetailDto> AccTaxDetails { get; set; }
        public List<ProductVoucherDetailDto> ProductVoucherDetails { get; set; }
        public CircularsDto Circulars { get; set; }
    }
}
