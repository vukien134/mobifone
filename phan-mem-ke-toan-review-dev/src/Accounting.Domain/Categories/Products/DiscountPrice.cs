using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    public class DiscountPrice : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Note { get; set; }
        public ICollection<DiscountPriceDetail> DiscountPriceDetails { get; set; }
        public ICollection<DiscountPricePartner> DiscountPricePartners { get; set; }
    }
}
