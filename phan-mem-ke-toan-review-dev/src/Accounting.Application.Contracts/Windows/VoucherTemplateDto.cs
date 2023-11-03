using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class VoucherTemplateDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string FileTemplate { get; set; }
        public string WindowId { get; set; }
        public string UrlApi { get; set; }
    }
}
