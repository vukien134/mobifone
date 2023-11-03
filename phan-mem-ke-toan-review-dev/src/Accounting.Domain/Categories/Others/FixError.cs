using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class FixError : TenantOrgEntity
    {
        public string ErrorId { get; set; }
        public string ErrorName { get; set; }
        public int Tag { get; set; }
        public string KeyError { get; set; }
        public int Classify { get; set; }
    }
}
