﻿using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.ProductOpeningBalances
{
    public class CrudProductOpeningBalanceDto : CruOrgBaseDto
    {
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public string WarehouseCode { get; set; }
        public string AccCode { get; set; }
        public string ProductCode { get; set; }
        //Mã lô hàng
        public string ProductLotCode { get; set; }
        //Mã nguồn
        public string ProductOriginCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        //Giá ngoại tệ
        public decimal PriceCur { get; set; }
        public decimal Amount { get; set; }
        //Tiền ngoại tệ
        public decimal AmountCur { get; set; }
    }
}
