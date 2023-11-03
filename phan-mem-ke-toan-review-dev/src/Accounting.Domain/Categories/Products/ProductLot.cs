using Accounting.JsonConverters;
using Accounting.TenantEntities;
using System;

namespace Accounting.Categories.Products
{
    public class ProductLot : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ManufacturingDate { get; set; }
        public string ManufaturingCountry { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ImportDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ExpireDate { get; set; }
        public string Note { get; set; }
    }
}
