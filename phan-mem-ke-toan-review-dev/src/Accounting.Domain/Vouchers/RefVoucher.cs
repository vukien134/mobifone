using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class RefVoucher : TenantOrgEntity
    {
        public string SrcId { get; set; }
        public string DestId { get; set; }
    }
}
