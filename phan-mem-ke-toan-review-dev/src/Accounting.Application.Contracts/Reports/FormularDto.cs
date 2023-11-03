using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class FormularDto
    {
        public string Code { get; set; }
        public string AccCode { get; set; }
        public string Math { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
    }
}
