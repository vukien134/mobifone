using Accounting.TenantEntities;
using System;

namespace Accounting.Categories.Accounts
{
    public class VoucherCategory : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameE { get; set; }
        public int VoucherGroup { get; set; }
        public string VoucherType { get; set; }
        public string VoucherOrd { get; set; }
        public string CurrencyCode { get; set; }
        //Theo hạch toán
        public string AttachBusiness { get; set; }
        //DANH_SO
        public string IncreaseNumberMethod { get; set; }
        public string ProductType { get; set; }
        //TRUNG_SO : có check trùng số chứng từ hay không
        public string ChkDuplicateVoucherNumber { get; set; }
        //DC_NB
        public string IsTransfer { get; set; }
        //LR_NB
        public string IsAssembly { get; set; }
        //TINH_GIA : Phương pháp tính giá
        public string PriceCalculatingMethod { get; set; }
        //IS_KT: Có lưu sổ cái không
        public string IsSavingLedger { get; set; }
        //IS_KHO: Có lưu sổ kho không
        public string IsSavingWarehouseBook { get; set; }
        //Xem số dư tức thời tài khoản
        public string IsCalculateBalanceAcc { get; set; }
        //Xem số dư tức thời sản phẩm
        public string IsCalculateBalanceProduct { get; set; }
        public string Prefix { get; set; }
        public string SeparatorCharacter { get; set; }
        public string Suffix { get; set; }
        public DateTime? BookClosingDate { get; set; }
        public DateTime? BusinessBeginningDate { get; set; }
        public DateTime? BusinessEndingDate { get; set; }
        public string TaxType { get; set; }
        //DP: Loại chứng từ
        public string VoucherKind { get; set; }
        //GIA_DT
        public string AttachPartnerPrice { get; set; }
        public int? TenantType { get; set; }
    }
}
