using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.SalaryTypes
{
    public class SalarySheetTypeDetailDto : TenantOrgDto
    {
        public string SalarySheetTypeId { get; set; }
        public int? Ord { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string Formular { get; set; }
        public int? Width { get; set; }
        public string DataType { get; set; }
        public string Format { get; set; }
    }
}
