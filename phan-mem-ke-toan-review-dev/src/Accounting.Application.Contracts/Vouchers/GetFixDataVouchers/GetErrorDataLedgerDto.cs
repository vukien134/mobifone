using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.GetFixDataVouchers
{
    public class GetErrorDataLedgerDto
    {
        public string errorId { get; set; }
        public string errorName { get; set; }
        public string keyError { get; set; }
        public string voucherId { get; set; }
        public string ord0 { get; set; }
        public string id { get; set; }
        public int year { get; set; }
        //REC_HT0
        public string ord0Extra { get; set; }
        public string departmentCode { get; set; }
        public string voucherCode { get; set; }
        public int voucherGroup { get; set; }
        public string businessCode { get; set; }
        public string businessAcc { get; set; }
        public string checkDuplicate { get; set; }
        public string voucherNumber { get; set; }
        public string invoiceNbr { get; set; }
        //Số CTGS
        public string recordingVoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime voucherDate { get; set; }
        //Số ngày
        public int? days { get; set; }
        public string paymentTermsCode { get; set; }
        public string contractCode { get; set; }
        public string clearingContractCode { get; set; }
        public string currencyCode { get; set; }
        public decimal? exchangeRate { get; set; }
        public string partnerCode0 { get; set; }
        public string partnerName0 { get; set; }
        public string representative { get; set; }
        public string address { get; set; }
        public string description { get; set; }
        public string descriptionE { get; set; }
        public string originVoucher { get; set; }
        public string debitAcc { get; set; }
        public decimal? debitExchangeRate { get; set; }
        public string debitCurrencyCode { get; set; }
        public string debitPartnerCode { get; set; }
        public string debitContractCode { get; set; }
        public string debitFProductWorkCode { get; set; }
        public string debitSectionCode { get; set; }
        public string debitWorkPlaceCode { get; set; }
        public decimal? debitAmountCur { get; set; }
        public string creditAcc { get; set; }
        public string creditCurrencyCode { get; set; }
        public decimal? creditExchangeRate { get; set; }
        public string creditPartnerCode { get; set; }
        public string creditContractCode { get; set; }
        public string creditFProductWorkCode { get; set; }
        public string creditSectionCode { get; set; }
        public string creditWorkPlaceCode { get; set; }
        public decimal? creditAmountCur { get; set; }
        public decimal? creditAmount { get; set; }
        public decimal? amountCur { get; set; }
        public decimal? amount { get; set; }
        public string note { get; set; }
        public string noteE { get; set; }
        public string fProductWorkCode { get; set; }
        public string partnerCode { get; set; }
        public string sectionCode { get; set; }
        public string clearingFProductWorkCode { get; set; }
        public string clearingPartnerCode { get; set; }
        public string clearingSectionCode { get; set; }
        public string workPlaceCode { get; set; }
        public string caseCode { get; set; }
        public string warehouseCode { get; set; }
        public string transWarehouseCode { get; set; }
        public string productCode { get; set; }
        public string productLotCode { get; set; }
        public string productOriginCode { get; set; }
        public string unitCode { get; set; }
        //Số lượng giao dịch
        public decimal? trxQuantity { get; set; }
        public decimal? quantity { get; set; }
        public decimal? trxPromotionQuantity { get; set; }
        public decimal? promotionQuantity { get; set; }
        public decimal? priceCur { get; set; }
        public decimal? price { get; set; }
        public string taxCategoryCode { get; set; }
        public decimal? vatPercentage { get; set; }
        public string invoiceNumber { get; set; }
        public string invoiceSymbol { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? invoiceDate { get; set; }
        public string invoicePartnerName { get; set; }
        public string invoicePartnerAddress { get; set; }
        public string taxCode { get; set; }
        //N_C
        public string debitOrCredit { get; set; }
        //CK_KT0
        public string checkDuplicate0 { get; set; }
        public string salesChannelCode { get; set; }
        public string productName0 { get; set; }
        //Mã bảo mật
        public string securityNo { get; set; }
        public string status { get; set; }
    }
}
