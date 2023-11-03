using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class FieldDto : BaseDto
    {
        public int Ord { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public int? FieldWidth { get; set; }
        public int? FieldHeight { get; set; }
        public int? ColumnWidth { get; set; }
        public int? LabelWidth { get; set; }
        public string LabelPosition { get; set; }
        public string Format { get; set; }
        public bool? ReadOnly { get; set; }
        public bool? ColumnHidden { get; set; }
        public bool? FormHidden { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }
        public string TabId { get; set; }
        public string FieldType { get; set; }
        public string Template { get; set; }
        public string TypeEditor { get; set; }
        public string TypeFilter { get; set; }
        public string ReferenceId { get; set; }
        public bool? Required { get; set; }
        public string FieldExpression { get; set; }
        public string DefaultValue { get; set; }
        public string Css { get; set; }
    }
}
