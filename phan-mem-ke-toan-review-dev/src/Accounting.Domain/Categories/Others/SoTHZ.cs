using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class SoTHZ : TenantOrgEntity
    {
        public int? Year { get; set; }
        public int? UsingDecision { get; set; }
        public int? Ord { get; set; }
        //QD_TC
        public string FinanceDecision { get; set; }
        //CT_SP
        public string FProductOrWork { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string DebitAcc { get; set; }
        public string DebitSection { get; set; }
        public string DebitFProductWork { get; set; }
        public string CreditAcc { get; set; }
        public string CreditSection { get; set; }
        public string CreditFProductWork { get; set; }
        public string TSum { get; set; }
        public string TGet { get; set; }
    }
}
