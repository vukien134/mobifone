using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Salaries
{
    public class SalaryBook : TenantOrgEntity
    {
        public string SalarySheetTypeId { get; set; }
        public string SalaryPeriodId { get; set; }
        public string EmployeeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string SalaryCode { get; set; }
        public decimal? NumberValue { get; set; }
        public string StringValue { get; set; }
        public string Formular { get; set; }
        public string PositionCode { get; set; }
    }
}
