using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Accounts
{
    public class VoucherType : TenantAuditedEntity<string>
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ListVoucher { get; set; }
        public string ListGroup { get; set; }
    }
}
