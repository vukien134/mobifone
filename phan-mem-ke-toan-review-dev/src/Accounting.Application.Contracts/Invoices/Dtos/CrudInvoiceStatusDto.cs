﻿using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Invoices.Dtos
{
    public class CrudInvoiceStatusDto : CruOrgBaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
