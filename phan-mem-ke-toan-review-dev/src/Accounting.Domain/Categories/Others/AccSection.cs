using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    //Danh mục khoản mục
    public class AccSection : TenantOrgEntity
    {        
        public string Code { get; set; }
        public string Name { get; set; }
        public string AttachProductCost { get; set; }
        public string SectionType { get; set; }
    }
}
