using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.CostProductions
{
    public class GroupCoefficient : TenantOrgEntity
    {
        public string FProductWork { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? ApplicableDate1 { get; set; }
        public DateTime? ApplicableDate2 { get; set; }
        public string Description { get; set; }
        public ICollection<GroupCoefficientDetail> GroupCoefficientDetails { get; set; }
    }
}
