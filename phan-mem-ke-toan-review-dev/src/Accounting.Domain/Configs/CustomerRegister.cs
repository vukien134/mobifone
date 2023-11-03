using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Configs
{
    public class CustomerRegister : AuditedEntity<string>
    {
        public string AccessCode { get; set; }
        public string Email { get; set; }
        public int? Type { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string CompanyType { get; set; }
        public int? RegNumUser { get; set; }
        public int? RegNumMonth { get; set; }
        public int? RegNumCompany { get; set; }
        public string TaxCode { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public bool? IsDemo { get; set; }
        public int? UsingDecision { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
