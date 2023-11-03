using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Salaries
{
    public class Employee : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Gender { get; set; }
        public string DepartmentCode { get; set; }
        public string PositionCode { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? InsuranceSalary { get; set; }
        public string PartnerCode { get; set; }
    }
}
