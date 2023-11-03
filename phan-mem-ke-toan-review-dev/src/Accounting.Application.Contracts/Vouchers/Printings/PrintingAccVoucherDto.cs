using Accounting.Catgories.Accounts;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Catgories.Others.Currencies;
using Accounting.JsonConverters;
using Accounting.Vouchers.AccVouchers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.Printings
{
    public class PrintingAccVoucherDto
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
        public decimal TotalAmountWithouVatCur { get; set; }
        public decimal TotalAmountWithouVat { get; set; }
        public decimal TotalAmountVatCur { get; set; }
        public decimal TotalAmountVat { get; set; }
        public decimal TotalAmountCur { get; set; }
        public decimal TotalAmount { get; set; }
        //Dùng cho khử trùng
        public string DebitOrCredit { get; set; }
        public string AccType { get; set; }
        public string Status { get; set; }
        public string AmountByWord { get; set; }
        public string VoteMaker { get; set; }
        public Guid? CreatorId { get; set; }
        public OrgUnitDto OrgUnitDto { get; set; }
        public CircularsDto Circulars { get; set; }
        public CurrencyDto Currency { get; set; }
        public dynamic TenantSetting { get; set; }

        public List<AccVoucherDetailDto> AccVoucherDetails { get; set; }
        public List<AccTaxDetailDto> AccTaxDetails { get; set; }
        public List<PrintingVoucherAccount> PrintingVoucherAccounts { get; set; }
        public List<PrintingAccVoucherDetail> PrintingAccVoucherDetails { get; set; }
        public List<AccountSystemDto> accountSystemDtos { get; set; }
    }
}
