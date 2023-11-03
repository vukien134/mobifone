using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.Salaries.Employees
{
    public class CrudEmployeeDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
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
