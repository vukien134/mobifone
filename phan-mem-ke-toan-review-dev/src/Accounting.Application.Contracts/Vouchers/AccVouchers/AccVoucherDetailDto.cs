using Accounting.BaseDtos;
using System;

namespace Accounting.Vouchers.AccVouchers
{
    public class AccVoucherDetailDto : TenantOrgDto
    {
        public string AccVoucherId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string DebitAcc { get; set; }
        public decimal? DebitExchageRate { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        //Mã công trình, sản phẩm
        public string FProductWorkCode { get; set; }
        public string ContractCode { get; set; }
        //Mã khoản mục
        public string SectionCode { get; set; }
        //Mã phân xưởng
        public string WorkPlaceCode { get; set; }
        public string CreditAcc { get; set; }
        public decimal? CreditExchangeRate { get; set; }
        //Mã đối tượng bù trừ
        public string ClearingPartnerCode { get; set; }
        public string ClearingWorkPlaceCode { get; set; }
        public string ClearingContractCode { get; set; }
        public string ClearingSectionCode { get; set; }
        public string ClearingFProductWorkCode { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string CaseCode { get; set; }
        //Số CTGS
        public string RecordingVoucherNumber { get; set; }
    }
}
