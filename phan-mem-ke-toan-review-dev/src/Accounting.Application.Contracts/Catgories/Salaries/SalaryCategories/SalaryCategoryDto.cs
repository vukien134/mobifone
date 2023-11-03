using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.SalaryCategories
{
    public class SalaryCategoryDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? SalaryType { get; set; }
        public string Nature { get; set; }
        public string Formular { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
        public string TaxType { get; set; }
    }
}
