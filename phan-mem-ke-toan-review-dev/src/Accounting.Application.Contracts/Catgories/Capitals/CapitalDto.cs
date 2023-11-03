using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Capitals
{
    public class CapitalDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
