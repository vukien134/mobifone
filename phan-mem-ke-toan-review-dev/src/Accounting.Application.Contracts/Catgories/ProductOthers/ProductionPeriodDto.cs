using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.ProductOthers
{
    public class ProductionPeriodDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
