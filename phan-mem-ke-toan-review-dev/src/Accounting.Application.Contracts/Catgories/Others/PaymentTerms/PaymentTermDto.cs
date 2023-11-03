using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.PaymentTerms
{
    public class PaymentTermDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
