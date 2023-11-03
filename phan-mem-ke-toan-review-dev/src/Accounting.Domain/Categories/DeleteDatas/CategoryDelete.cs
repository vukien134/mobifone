using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Categories.CategoryDeletes
{
    public class CategoryDelete : AuditedEntity<string>
    {
        public string TabName { get; set; }
        public string Name { get; set; }
        public string FieldCode { get; set; }
        public string RefFieldCode { get; set; }
        public string ConditionField { get; set; }
        public string ConditionValue { get; set; }
        public int Type { get; set; }
        public string BusinessType { get; set; }
        public int Ord { get; set; }
    }
}
