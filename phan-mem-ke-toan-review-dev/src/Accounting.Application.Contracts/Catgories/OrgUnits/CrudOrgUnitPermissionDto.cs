using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.OrgUnits
{
    public class CrudOrgUnitPermissionDto : CruBaseDto
    {
        public string OrgUnitId { get; set; }
        public Guid UserId { get; set; }        
    }
}
