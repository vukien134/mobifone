using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.HouseholdBusiness
{
    public class DetailRevenueDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string DocId { get; set; }
        public string OrdRec0 { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string PartnerCode { get; set; }
        public string Description { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string ProductName { get; set; }
        public string ProductName0 { get; set; }
        public string CareerCode { get; set; }
        public string Note { get; set; }
        public decimal? Incurred01 { get; set; }
        public decimal? Incurred02 { get; set; }
        public decimal? Incurred03 { get; set; }
        public decimal? Incurred04 { get; set; }
        public decimal? Incurred05 { get; set; }
        public decimal? Incurred06 { get; set; }
        public decimal? Incurred07 { get; set; }
        public decimal? Incurred08 { get; set; }
        public decimal? Incurred09 { get; set; }
    }
}