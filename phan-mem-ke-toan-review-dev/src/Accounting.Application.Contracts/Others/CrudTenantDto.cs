using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Others
{
    public class CrudTenantDto
    {
        public string Id { get; set; }
        public string AccessCode { get; set; }
        public int? Type { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string CompanyType { get; set; }
        public int? RegNumUser { get; set; }
        public int? RegNumMonth { get; set; }
        public int? RegNumCompany { get; set; }
        public string TaxCode { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public int? UsingDecision { get; set; }
        public bool? IsDemo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
