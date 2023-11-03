using Accounting.Catgories.OrgUnits;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class SaveRoleDto
    {
        public Guid? UserId { get; set; }
        public List<SelectRoleDto> Roles { get; set; }
        public List<SelectOrgUnitDto> OrgUnits { get; set; }
    }
}
