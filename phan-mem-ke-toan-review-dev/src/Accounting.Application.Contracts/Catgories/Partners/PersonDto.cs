using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Partners
{
    public class PersonDto : TenantOrgDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
