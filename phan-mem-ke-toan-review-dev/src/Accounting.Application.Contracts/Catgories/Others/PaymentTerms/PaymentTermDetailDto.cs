using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.PaymentTerms
{
    public class PaymentTermDetailDto : TenantOrgDto
    {
        public string PaymentTermId { get; set; }
        public int? Ord { get; set; }
        public int? Days { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Amount { get; set; }
    }
}
