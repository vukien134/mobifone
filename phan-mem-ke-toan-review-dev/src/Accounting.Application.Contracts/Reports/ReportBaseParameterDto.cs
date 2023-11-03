using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class ReportBaseParameterDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public string SectionCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string CurrencyCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ContractCode { get; set; }
        public int Year { get; set; }
        public string ProductOriginCode { get; set; }
        public int? AccRank { get; set; }
        public bool? CleaningAcc { get; set; }
        public bool? CleaningFProductWork { get; set; }
        public string AccReciprocal { get; set; }
        public string BeginVoucherNumber { get; set; }
        public string EndVoucherNumber { get; set; }
        public string PartnerGroup { get; set; }
        public string PartnerGroupCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? CreationTime { get; set; }
        public string VoucherCode { get; set; }
        public string Sort { get; set; }
        public string CreationUser { get; set; }
        public string ReciprocalAcc { get; set; }
        public string DepartmentCode { get; set; }
        public string? DebitCredit { get; set; }
        public int? Sort1 { get; set; }
        public int? Sort2 { get; set; }
        public string CaseCode { get; set; }
        public string BusinessCode { get; set; }
        public string TypeForward { get; set; } //loai_kc
        public string AttachProduct { get; set; }
        public string SaleChannel { get; set; }
        public string LstVoucherCode { get; set; }
        public string Type { get; set; }
        public string DetailBy { get; set; }
        public bool? AttachProductLot { get; set; }
        public bool? AttachProductOrigin { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc2 { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public int? VoucherGroup { get; set; }
        public string Ht2 { get; set; }
        public string ContractType { get; set; }
        public int DebtType { get; set; }
        public string FProductOrWork { get; set; }
        public string Status { get; set; }
        public string SalarySheetTypeId { get; set; }
        public string SalaryPeriodId { get; set; }
        public string DepartmentId { get; set; }
        public string ProductGroupId { get; set; }
        public string InterestAcc { get; set; }
        public string HoleAcc { get; set; }
        public DateTime? FromDate0
        {
            get
            {
                return this.FromDate;
            }
        }

        public DateTime? ToDate0
        {
            get
            {
                return this.ToDate;
            }
        }
    }
}
