using Accounting.Categories.OrgUnits;
using Accounting.TenantEntities;
using System;

namespace Accounting.Permissions
{
    public class OrgUnitPermission : TenantOrgEntity
    {
        public string OrgUnitId { get; set; }
        public Guid UserId { get; set; }
        public OrgUnit OrgUnit { get; set; }
    }
}
