using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.Others.Departments
{
    public class CrudDepartmentDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
        public string NameTemp { get; set; }
        public string ParentId { get; set; }
        public string DepartmentType { get; set; }
        public string ParentCode { get; set; }
    }
}
