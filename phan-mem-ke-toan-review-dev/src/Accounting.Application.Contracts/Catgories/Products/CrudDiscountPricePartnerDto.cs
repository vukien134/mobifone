using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Products
{
    public class CrudDiscountPricePartnerDto : CruOrgBaseDto
    {
        public string DiscountPriceId { get; set; }
        public string PartnerCode { get; set; }
        public string Note { get; set; }
    }
}
