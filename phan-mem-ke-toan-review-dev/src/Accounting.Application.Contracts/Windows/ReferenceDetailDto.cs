using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class ReferenceDetailDto : BaseDto
    {
        public int? Ord { get; set; }
        public string ReferenceId { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public int? Width { get; set; }
        public string Format { get; set; }
        public string Template { get; set; }
    }
}
