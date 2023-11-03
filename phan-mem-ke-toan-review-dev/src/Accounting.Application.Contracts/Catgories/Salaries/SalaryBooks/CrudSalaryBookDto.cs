using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.SalaryBooks
{
    public class CrudSalaryBookDto : CruOrgBaseDto
    {
        public string SalarySheetTypeId { get; set; }
        public string SalaryPeriodId { get; set; }
        public string EmployeeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string SalaryCode { get; set; }
        public string PositionCode { get; set; }
        public decimal? NumberValue { get; set; }
        public string StringValue { get; set; }
        public string Formular { get; set; }
    }
}
