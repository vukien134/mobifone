using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.DashBoards
{
    public class ProductAlmostOutDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Value { get; set; }
    }
}
