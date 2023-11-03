using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Contracts
{
    public class Contract : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PartnerCode { get; set; }
        public string ContractType { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string Note { get; set; }
        public ICollection<ContractDetail> ContractDetails { get; set; }
    }
}
