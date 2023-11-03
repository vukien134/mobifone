using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Salaries
{
    public class SalaryEmployee : TenantOrgEntity
    {
        public string EmployeeCode { get; set; }
        public string SalaryCode { get; set; }
        public decimal? Amount { get; set; }
    }
}
