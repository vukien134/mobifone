using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class PurchaseOrderTrackinDto
    {
        public int Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? DeliveryDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? QuantityTh { get; set; }
        public decimal? PriceCurTh { get; set; }
        public decimal? PriceTh { get; set; }
        public decimal? AmountCurTh { get; set; }
        public decimal? AmountTh { get; set; }
        public decimal? QuantityCl { get; set; }
        public decimal? AmountCurCl { get; set; }
        public decimal? AmountCl { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string UnitCode { get; set; }

    }
}

