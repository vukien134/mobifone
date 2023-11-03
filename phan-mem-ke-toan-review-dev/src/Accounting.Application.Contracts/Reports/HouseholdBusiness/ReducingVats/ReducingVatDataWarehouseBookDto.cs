using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.HouseholdBusiness
{
    public class ReducingVatDataWarehouseBookDto
    {
        public string OrgCode { get; set; }
        public int Ord { get; set; }
        public string DocId { get; set; }
        public string OrdRec0 { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime VoucherDate { get; set; }
        public string PartnerCode { get; set; }
        public string Description { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string ProductName { get; set; }
        public decimal? AmountDecrease { get; set; }
        public string CareerCode { get; set; }
        public decimal? AmountValueAdded { get; set; }
        public decimal? AmountPersonal { get; set; }
    }
}
