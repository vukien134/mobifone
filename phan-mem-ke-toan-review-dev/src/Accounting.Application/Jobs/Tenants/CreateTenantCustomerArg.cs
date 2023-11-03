using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace Accounting.Jobs.Tenants
{
    [BackgroundJobName("createTenantCustomer")]
    public class CreateTenantCustomerArg
    {
        public string CustomerRegisterId { get; set; }
    }
}
