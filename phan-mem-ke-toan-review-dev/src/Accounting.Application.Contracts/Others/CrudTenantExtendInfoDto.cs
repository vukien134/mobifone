using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Others
{
    public class CrudTenantExtendInfoDto : CruBaseDto
    {
        public Guid? TenantId { get; set; }
        public int? TenantType { get; set; }
        public string LicenseXml { get; set; }
        public string CompanyType { get; set; }
        public int? RegNumUser { get; set; }
        public int? RegNumMonth { get; set; }
        public int? RegNumCompany { get; set; }
    }
}
