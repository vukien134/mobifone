using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines.AssetTool
{
    public class DataAssetToolDto
    {
        public string Id { get; set; }
        public string OrgCode { get; set; }
        public string Code { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ReduceDate { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public string WorkPlaceCode { get; set; }
    }
}
