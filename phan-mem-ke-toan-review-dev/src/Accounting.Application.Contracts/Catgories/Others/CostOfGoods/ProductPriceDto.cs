using System;
namespace Accounting.Catgories.Others.CostOfGoods
{
    public class ProductPriceDto
    {
        public string ProductCode { get; set; }
        public 	string WarehouseCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOriginCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityCur { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountCur { get; set; }
        public decimal Pricre { get; set; }
        public decimal PriceCur { get; set; }
    }
}

