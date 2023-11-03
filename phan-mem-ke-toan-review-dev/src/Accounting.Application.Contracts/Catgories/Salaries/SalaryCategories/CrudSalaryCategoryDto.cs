using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.Salaries.SalaryCategories
{
    public class CrudSalaryCategoryDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }
        public int? SalaryType { get; set; }
        public string Nature { get; set; }
        public string Formular { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
        public string TaxType { get; set; }
    }
}
