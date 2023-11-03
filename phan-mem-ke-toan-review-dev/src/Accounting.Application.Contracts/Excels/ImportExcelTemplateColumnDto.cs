using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Excels
{
    public class ImportExcelTemplateColumnDto : BaseDto
    {
        public int Ord { get; set; }
        public string ImportExcelTemplateId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string ExcelCol { get; set; }
        public string DefaultValue { get; set; }
        public string Caption { get; set; }
    }
}
