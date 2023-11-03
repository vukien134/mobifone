using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.CostProductions
{
    public class CrudInfoExportAutoDto : CruOrgBaseDto
    {
        public int Year { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime BeginDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime EndDate { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string FProductWork { get; set; }
        public string OrdGrp { get; set; }
        public string Type { get; set; }
        public string OrdRec { get; set; }
    }
}
