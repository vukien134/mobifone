using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Vouchers.Printings
{
    public class PrintingVoucherResponseDto
    {
        public string TemplateFile { get; set; }
        public object DataSource { get; set; }
        public string Type { get; set; }
    }
}
