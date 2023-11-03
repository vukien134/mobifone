using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class AccVoucher : TenantOrgEntity
    {
        public int Year { get; set; }
        //Mã bộ phận, phòng ban
        public string DepartmentCode { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        //Mã hạch toán
        public string BusinessCode { get; set; }
        //Tài khoản hạch toán
        public string BusinessAcc { get; set; }
        public string VoucherNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime VoucherDate { get; set; }
        public string PaymentTermsCode { get; set; }
        public string PartnerCode0 { get; set; }
        public string PartnerName0 { get; set; }
        //Ông bà người đại diện
        public string Representative { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string OriginVoucher { get; set; }
        public string CurrencyCode { get; set; }
        //Tỷ giá
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmountWithoutVatCur { get; set; }
        public decimal TotalAmountWithoutVat { get; set; }
        public decimal TotalAmountVatCur { get; set; }
        public decimal TotalAmountVat { get; set; }
        public decimal TotalAmountCur { get; set; }
        public decimal TotalAmount { get; set; }
        //Dùng cho khử trùng
        public string DebitOrCredit { get; set; }
        public string AccType { get; set; }
        public string Status { get; set; }
        public bool? Locked { get; set; }
        public ICollection<AccVoucherDetail> AccVoucherDetails { get; set; }
        public ICollection<AccTaxDetail> AccTaxDetails { get; set; }
        public ICollection<VoucherExciseTax> VoucherExciseTaxes { get; set; }
    }
}
