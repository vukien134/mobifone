using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.CostProductions
{
    public class AllotmentForwardCategoryDto : TenantOrgDto
    {
        public int Ord { get; set; }
        public int DecideApply { get; set; }
        public int Year { get; set; }
        public string FProductWork { get; set; }
        public string Type { get; set; }
        public string OrdGrp { get; set; }
        public string Code { get; set; }
        public int ForwardType { get; set; }
        public string LstCode { get; set; }
        public string GroupCoefficientCode { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string DebitAcc { get; set; }
        public string DebitSectionCode { get; set; }
        public int DebitCredit { get; set; }
        public string CreditAcc { get; set; }
        public string CreditSectionCode { get; set; }
        public string Note { get; set; }
        public int Active { get; set; }
        public string RecordBook { get; set; }
        public string AttachProduct { get; set; }
        public string QuantityType { get; set; }
        public string NormType { get; set; }
        public int ProductionPeriodType { get; set; }
    }
}
