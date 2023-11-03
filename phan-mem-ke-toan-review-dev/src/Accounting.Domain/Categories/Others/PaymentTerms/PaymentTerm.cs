using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others.PaymentTerms
{
    public class PaymentTerm : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public ICollection<PaymentTermDetail> PaymentTermDetails { get; set; }
    }
}
