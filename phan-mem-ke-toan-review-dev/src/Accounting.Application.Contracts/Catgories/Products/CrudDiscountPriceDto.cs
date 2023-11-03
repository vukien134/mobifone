using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.Products
{
    public class CrudDiscountPriceDto : CruOrgBaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        public string Note { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public string Price { get; set; }
        public string DiscountPercentage { get; set; }
        public string NoteDetail { get; set; }
        public string PartnerCode { get; set; }
        public string NotePartner { get; set; }
        public List<CrudDiscountPriceDetailDto> DiscountPriceDetails { get; set; }
        public List<CrudDiscountPricePartnerDto> DiscountPricePartners { get; set; }
    }
}
