using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Partners
{
    public class Person : TenantOrgEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
