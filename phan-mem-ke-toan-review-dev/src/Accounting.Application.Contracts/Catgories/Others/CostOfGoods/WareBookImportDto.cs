using System;
namespace Accounting.Catgories.Others.CostOfGoods
{
    public class WareBookImportDto
    {
        public string id { get; set; }
        public string OrgCode { get; set; }
        public string ProductVoucherId { get; set; }
        public string Ord0 { get; set; }
        public string Rec0_dc { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherGroup { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string BusinessCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string? WarehouseCode { get; set; }
        public string? TransWarehouseCode { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductLotCode { get; set; }
        public string? ProductOriginCode { get; set; }
        public string UnitCode { get; set; }
        public decimal? TrxQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuantityCb  { get; set; }
        public decimal? PriceCb { get; set; }
        public decimal? PriceCurCb { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price0 { get; set; }
        public decimal? PriceCur0 { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? ImportQuantity { get; set; }
        public decimal? ImportAmount { get; set; }
        public decimal? ImportAmountCur { get; set; }
        public decimal? ExportQuantity { get; set; }
        public decimal? ExportAmount { get; set; }
        public decimal? ExportAmountCur { get; set; }
        public string PriceCalculatingMethod { get; set; }
        public string IsTransfer { get; set; }
        public string IsAssembly { get; set; }
        public bool FixedPrice { get; set; }
        public string GetPrice { get; set; }
        public string Lrdc { get; set; }
        public string ImportAvg { get; set; }
        public DateTime? BookClosingDate { get; set; }
        public string CalculationStep { get; set; }
        public decimal? QuantityCruCb { get; set; }

    }
}

