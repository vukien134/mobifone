using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    public class DiscountPricePartner : TenantOrgEntity
    {
        public string DiscountPriceId { get; set; }
        public string PartnerCode { get; set; }
        public string Note { get; set; }
        public DiscountPrice DiscountPrice { get; set; }
    }
}
