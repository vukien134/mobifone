using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    public class DiscountPriceDetail : TenantOrgEntity
    {
        public string DiscountPriceId { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price2 { get; set; }
        public decimal? PriceCur2 { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmountPrice { get; set; }
        public decimal? DiscountAmountPriceCur { get; set; }
        public string Note { get; set; }
        public DiscountPrice DiscountPrice { get; set; }
    }
}
