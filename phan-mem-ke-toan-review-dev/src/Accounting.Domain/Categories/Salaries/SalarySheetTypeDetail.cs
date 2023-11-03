using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Salaries
{
    public class SalarySheetTypeDetail : TenantOrgEntity
    {
        public string SalarySheetTypeId { get; set; }
        public int? Ord { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string Formular { get; set; }
        public int? Width { get; set; }
        public string DataType { get; set; }
        public string Format { get; set; }
        public SalarySheetType SalarySheetType { get; set; }
    }
}
