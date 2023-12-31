﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class UserRoleDto
    {
        public Guid? Id { get; set; }
        public Guid? TenantId { get; set; }
        public string Name { get; set; }
        public bool? IsStatic { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsDefault { get; set; }
    }
}
