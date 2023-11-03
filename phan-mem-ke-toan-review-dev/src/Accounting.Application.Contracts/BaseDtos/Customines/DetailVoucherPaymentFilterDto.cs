using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class DetailVoucherPaymentFilterDto
    {
        public string OrgCode { get; set; }
        public string AccType { get; set; }
        public string PartnerCode { get; set; }
        public string AccCode { get; set; }
    }
}
