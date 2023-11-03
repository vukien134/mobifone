using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class ListPaymentFilterDto
    {
        public List<PaymentFilterDto> Data { get; set; }
    }
}
