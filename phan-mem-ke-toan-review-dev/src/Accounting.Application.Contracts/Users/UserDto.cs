using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class UserDto 
    {
        public Guid? Id { get; set; }
        public Guid? TenantId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
