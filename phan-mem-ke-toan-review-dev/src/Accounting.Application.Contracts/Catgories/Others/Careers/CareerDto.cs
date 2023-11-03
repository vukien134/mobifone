using Accounting.BaseDtos;
using System;

namespace Accounting.Catgories.Others.Careers
{
    public class CareerDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
