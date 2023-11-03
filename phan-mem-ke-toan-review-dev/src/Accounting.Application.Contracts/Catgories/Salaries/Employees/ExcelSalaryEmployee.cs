using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.Employees
{
    public class ExcelSalaryEmployee : CruOrgBaseDto
    {
        public string EmployeeCode { get; set; }
        public string SalaryCode { get; set; }
        public double? Amount { get; set; }
    }
}
