using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Products
{
    public class GetSalePriceDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? VoucherDate { get; set; }
        public string PartnerCode { get; set; }
        public string ProductCode { get; set; }
    }
}
