using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Reasons
{
    public class ReasonDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ReasonType { get; set; }
    }
}
