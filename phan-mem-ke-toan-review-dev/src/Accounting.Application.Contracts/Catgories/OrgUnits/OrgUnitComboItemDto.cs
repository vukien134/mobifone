using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.OrgUnits
{
    public class OrgUnitComboItemDto : ComboItemDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
