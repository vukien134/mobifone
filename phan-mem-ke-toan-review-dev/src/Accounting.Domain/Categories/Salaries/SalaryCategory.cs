using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Salaries
{
    public class SalaryCategory : TenantOrgEntity
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
