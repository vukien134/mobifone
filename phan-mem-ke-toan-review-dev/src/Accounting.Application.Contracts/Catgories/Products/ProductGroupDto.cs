using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Products
{
    public class ProductGroupDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
    }
}
