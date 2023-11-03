using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.ResetVoucherNumbers
{
    public class ResetVoucherNumberDataDto : CruOrgBaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameE { get; set; }
        public int VoucherGroup { get; set; }
        public string VoucherType { get; set; }
        public string VoucherOrd { get; set; }
        public string CurrencyCode { get; set; }
        public string AttachBusiness { get; set; }
        public string IncreaseNumberMethod { get; set; }
        public string ProductType { get; set; }
        public string ChkDuplicateVoucherNumber { get; set; }
        public string IsTransfer { get; set; }
        public string IsAssembly { get; set; }
        public string PriceCalculatingMethod { get; set; }
        public string IsSavingLedger { get; set; }
        public string IsSavingWarehouseBook { get; set; }
        public string IsCalculateBalanceAcc { get; set; }
        public string IsCalculateBalanceProduct { get; set; }
        public string Prefix { get; set; }
        public string SeparatorCharacter { get; set; }
        public string Suffix { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? BookClosingDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? BusinessBeginningDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime? BusinessEndingDate { get; set; }
        public string TaxType { get; set; }
        public string VoucherKind { get; set; }
        public string AttachPartnerPrice { get; set; }
        public string TenantId { get; set; }
    }
}
