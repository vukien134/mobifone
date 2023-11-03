using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Salaries
{
    public class Position : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
