using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.DashBoards
{
    public class CostDto
    {
        public string Color { get; set; }
        public decimal? Amount { get; set; }
        public string Month { get; set; }
    }
}
