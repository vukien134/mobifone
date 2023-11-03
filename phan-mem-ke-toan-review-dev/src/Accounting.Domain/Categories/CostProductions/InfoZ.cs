using Accounting.JsonConverters;
using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.CostProductions
{
    public class InfoZ : TenantOrgEntity
    {
        public string FProductWork { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime BeginM { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime EndM { get; set; }
        public string OrdGrp { get; set; }
        public string Type { get; set; }
        public string AllotmentForwardCode { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string DebitAcc { get; set; }
        public string DebitSectionCode { get; set; }
        public string DebitFProductWorkCode { get; set; }
        public string CreditAcc { get; set; }
        public string CreditSectionCode { get; set; }
        public string CreditFProductWorkCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string ProductCode { get; set; }
        public string PartnerCode { get; set; }
        public string ContractCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public string RecordBook { get; set; }
        public decimal? Ratio { get; set; }
        public decimal? BeginQuantity { get; set; }
        public decimal? BeginAmount { get; set; }
        public decimal? EndQuantity { get; set; }
        public decimal? EndAmount { get; set; }
    }
}
