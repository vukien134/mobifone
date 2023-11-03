using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class UserRolePermissionDto
    {
        public List<PermissionTreeItemDto> PermissionTrees { get; set; }
        public List<string> GrantNames { get; set; }
    }
}
