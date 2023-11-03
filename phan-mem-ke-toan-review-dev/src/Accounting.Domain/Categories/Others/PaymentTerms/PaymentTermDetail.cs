using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others.PaymentTerms
{
    public class PaymentTermDetail : TenantOrgEntity
    {
        public string PaymentTermId { get; set; }
        public int? Ord { get; set; }
        public int? Days { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Amount { get; set; }
        public PaymentTerm PaymentTerm { get; set; }
    }
}
