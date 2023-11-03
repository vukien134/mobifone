using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class SurvivingVoucherDto
    {
        public string Id { get; set; }
        public string OrgCode { get; set; }
        public string Ord { get; set; }
        public string Ord0 { get; set; }
        public string VoucherOrd { get; set; }
        public int VoucherGroup { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime DateNew { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductOriginCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal AmountCur { get; set; }
        public decimal Amount { get; set; }
    }
}
