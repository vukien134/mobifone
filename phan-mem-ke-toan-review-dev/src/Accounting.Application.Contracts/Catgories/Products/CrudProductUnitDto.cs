using Accounting.BaseDtos;
using System;

namespace Accounting.Catgories.Products
{
    public class CrudProductUnitDto : CruOrgBaseDto
    {
        public string Ord0 { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public bool IsBasicUnit { get; set; }
        public decimal ExchangeRate { get; set; }
        //Giá mua , giá nhập
        public decimal PurchasePrice { get; set; }
        //Giá mua , giá nhập NT
        public decimal PurchasePriceCur { get; set; }
        //Giá bán
        public decimal SalePrice { get; set; }
        //Giá bán NT
        public decimal SalePriceCur { get; set; }
    }
}
