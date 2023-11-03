using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Vouchers
{
    public class DefaultVoucherType : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ListVoucher { get; set; }
        public string ListGroup { get; set; }
    }
}
