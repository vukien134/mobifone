using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Products
{
    public class ProductComboItemDto : BaseComboItemDto
    {
        public string UnitCode { get; set; }
        //Tài khoản hàng hóa, vật tư
        public string ProductAcc { get; set; }
        //Tài khoản giá vốn
        public string ProductCostAcc { get; set; }
        public string RevenueAcc { get; set; }
        public string DiscountAcc { get; set; }
        //Tài khoản hàng bán bị trả lại
        public string SaleReturnsAcc { get; set; }
        public string AttachProductLot { get; set; }
        public string AttachProductOrigin { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? SalePriceCur { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? PurchasePriceCur { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? PITPercentage { get; set; }
        
    }
}
