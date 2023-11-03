using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class GroupPermissionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsSelect { get; set; }
        public string DisplayName { get; set; }
    }
}
