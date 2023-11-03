using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Licenses
{
    public class CrudTenantLicenseDto : CruBaseDto
    {
        public string LicXml { get; set; }
        public string Key { get; set; }
    }
}
