using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.FeeTypes
{
    public class FeeTypeDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
