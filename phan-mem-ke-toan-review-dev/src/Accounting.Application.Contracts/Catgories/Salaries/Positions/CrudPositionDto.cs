﻿using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.Positions
{
    public class CrudPositionDto : CruOrgBaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
