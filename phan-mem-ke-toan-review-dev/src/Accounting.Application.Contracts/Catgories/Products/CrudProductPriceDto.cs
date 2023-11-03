﻿using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Products
{
    public class CrudProductPriceDto : CruOrgBaseDto
    {
        public string Ord0 { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        //Giá mua , giá nhập
        public decimal PurchasePrice { get; set; }
        //Giá mua , giá nhập NT
        public decimal PurchasePriceCur { get; set; }
        //Giá bán
        public decimal SalePrice { get; set; }
        //Giá bán NT
        public decimal SalePriceCur { get; set; }
        public string Note { get; set; }
    }
}
