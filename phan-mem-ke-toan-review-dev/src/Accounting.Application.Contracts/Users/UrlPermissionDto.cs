using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Users
{
    public class UrlPermissionDto
    {
        public string Url { get; set; }
        public bool? IsGranted { get; set; }
    }
}
