using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.Employees
{
    public class EmployeeDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BirthDay { get; set; }

        public string Gender { get; set; }
        public string DepartmentCode { get; set; }
        public string PositionCode { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? InsuranceSalary { get; set; }
        public string PartnerCode { get; set; }
    }
}
