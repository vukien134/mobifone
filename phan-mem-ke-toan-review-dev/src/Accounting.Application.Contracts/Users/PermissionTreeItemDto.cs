using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class PermissionTreeItemDto : BaseTreeItemDto<PermissionTreeItemDto>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
