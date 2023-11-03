using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.GeneralDiaries
{
    public class ListOfVouchersBookdto
    {
        public int Year { get; set; }
        public string OrgCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string Note { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Amount { get; set; }
        public string VoucherId { get; set; }
        public string Id { get; set; }
        public string VoucherCode { get; set; }

        public string CaseCode { get; set; }
        public string PartnerCode { get; set; }
        public string ClearingPartnerCode { get; set; }
        //ma_nh
        public string GroupCode { get; set; }
        public string SectionCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string CurrencyCode { get; set; }
        public string DepartmentCode { get; set; }
        public decimal? DebitIncurred { get; set; }
        public decimal? CreditIncurred { get; set; }
        public decimal? DebitIncurredCur { get; set; }
        public decimal? CreditIncurredCur { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceSymbol { get; set; }
        public decimal? DebitAmountCur { get; set; }
        public decimal? CreditAmountCur { get; set; }
        public string CaseName { get; set; }//vv
        public string PartnerName { get; set; }
        public string SectionName { get; set; }
        public string DepartmentName { get; set; }
        public string FProductWorkName { get; set; }
        public string CurrencyName { get; set; }
        public string AccName { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy HH:mm:ss")]
        public DateTime? CreationTime { get; set; }
        public string CreatorName { get; set; }
        public int Sort { get; set; }
        public string Bold { get; set; }
        public string ReciprocalAcc { get; set; }
        public string GroupCode1 { get; set; }
        public string GroupName1 { get; set; }
        public string GroupCode2 { get; set; }
        public string GroupName2 { get; set; }
        public string WorkPlaceCode { get; set; }
        public string ContractCode { get; set; }
        public string Acc { get; set; }
        public string PartnerGroupId { get; set; }
    }
}

