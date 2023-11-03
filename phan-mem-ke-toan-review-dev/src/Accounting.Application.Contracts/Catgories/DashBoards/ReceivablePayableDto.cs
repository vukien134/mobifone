using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.DashBoards
{
    public class ReceivablePayableDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Receivable { get; set; }
        public decimal? Payable { get; set; }
    }
}
