using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Vouchers.AccVouchers
{
    public class AccVoucherDto : TenantOrgDto
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
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
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
        public string FormatDateVoucher
        {
            get
            {
                if (this.VoucherDate == null) return null;
                return $"{this.VoucherDate:dd/MM/yyyy}";
            }
        }
        public bool? Locked { get; set; }      
    }
}
