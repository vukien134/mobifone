using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class CrudUserDto
    {
        public Guid? Id { get; set; }
        public Guid? TenantId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
}
