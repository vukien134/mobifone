using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.CostProductions
{
    public class CrudGroupCoefficientDto : CruOrgBaseDto
    {
        public string FProductWork { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ApplicableDate1 { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ApplicableDate2 { get; set; }
        public string Description { get; set; }
        public List<CrudGroupCoefficientDetailDto> GroupCoefficientDetails { get; set; }
    }
}
