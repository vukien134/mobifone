﻿using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class WorkPlace : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
