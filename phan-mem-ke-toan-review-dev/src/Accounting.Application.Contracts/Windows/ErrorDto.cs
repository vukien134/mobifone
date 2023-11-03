using System;
using System.Collections.Generic;

namespace Accounting.Windows
{
    public class ErrorDto
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string Line { get; set; }
        public string WareHouse { get; set; }
        public List<InforDto> inforDtos { get; set; }
    }
}

