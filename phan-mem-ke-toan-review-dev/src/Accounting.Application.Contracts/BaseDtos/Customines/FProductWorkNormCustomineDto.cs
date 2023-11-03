using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class FProductWorkNormCustomineDto
    {
        public string Id { get; set; }
        public int Year { get; set; }
        public string FProductWorkCode { get; set; }
        public string FProductWorkName { get; set; }
        public decimal Quantity { get; set; }
        public string Note { get; set; }
    }
}
