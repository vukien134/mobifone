using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class AssignPermissionDto
    {
        
        public List<ValuePermissionDto> Items { get; set; }
        public Guid? RoleId { get; set; }
    }
}
