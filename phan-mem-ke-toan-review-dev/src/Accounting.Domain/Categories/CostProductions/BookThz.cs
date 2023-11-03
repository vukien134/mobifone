using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.CostProductions
{
    public class BookThz : TenantOrgEntity
    {
        public int? Year { get; set; }
        public int? DecideApply { get; set; }
        public int? Ord { get; set; }
        public string ProductOrWork { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string DebitAcc { get; set; }
        public string DebitSectionCode { get; set; }
        public string AttachDebitFProducWork { get; set; }
        public string CreditAcc { get; set; }
        public string CreditSectionCode { get; set; }
        public string AttachCreditFProductWork { get; set; }
        public string TSum { get; set; }
        public string TGet { get; set; }
    }
}
