using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.Employees
{
    public class SalaryEmployeeDto : TenantOrgDto
    {
        public string EmployeeCode { get; set; }
        public string SalaryCode { get; set; }
        public decimal? Amount { get; set; }
    }
}
