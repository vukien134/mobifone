using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.GetFixDataVouchers
{
    public class GetErrorDataProductDto
    {
        public string errorId { get; set; }
        public string errorName { get; set; }
        public string keyError { get; set; }
        public string id { get; set; }
        public string ord0 { get; set; }
        public int year { get; set; }
        public string warehouseCode { get; set; }
        public string accCode { get; set; }
        public string productCode { get; set; }
        //Mã lô hàng
        public string productLotCode { get; set; }
        //Mã nguồn
        public string productOriginCode { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        //Giá ngoại tệ
        public decimal priceCur { get; set; }
        public decimal amount { get; set; }
        //Tiền ngoại tệ
        public decimal amountCur { get; set; }
    }
}
