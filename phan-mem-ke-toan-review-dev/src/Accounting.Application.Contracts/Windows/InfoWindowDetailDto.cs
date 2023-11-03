using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class InfoWindowDetailDto : BaseDto
    {
        public int? Ord { get; set; }
        public string InfoWindowId { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string TypeEditor { get; set; }
        public string LabelPosition { get; set; }
        public string LabelWidth { get; set; }
        public string Format { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string ReferenceId { get; set; }
        public string DefaultValue { get; set; }
        public bool? Hidden { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public bool? Required { get; set; }
        public string ValueChange { get; set; }
    }
}
