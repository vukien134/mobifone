using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class PaymentFilterDto
    {
        public string DocumentId { get; set; }
        public string AccVoucherId { get; set; }
        public int Times { get; set; }
        public decimal Amount { get; set; }
    }
}
