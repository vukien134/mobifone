using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.OrgUnits
{
    public class SelectOrgUnitDto
    {
        public bool IsSelect { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid? UserId { get; set; }
        public string OrgUnitId { get; set; }
    }
}
