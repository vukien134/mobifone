using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class SelectRoleDto
    {
        public bool IsSelect { get; set; }
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }
}
