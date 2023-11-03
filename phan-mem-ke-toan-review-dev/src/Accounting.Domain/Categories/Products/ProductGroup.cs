using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    public class ProductGroup : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
