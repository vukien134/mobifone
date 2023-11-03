using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Products
{
    public class ProductLotDto : TenantOrgDto
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
