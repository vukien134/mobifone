using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.FProductWorks
{
    public class FProductWorkDto : TenantOrgDto
    {
        //CT_SP
        public string FProductOrWork { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameTemp { get; set; }
        public string FPWType { get; set; }
        public int? Rank { get; set; }
        public string ParentCode { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginningDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndingDate { get; set; }
        //CHU_TC
        public string WorkOwner { get; set; }
        public string Note { get; set; }
        public string ParentId { get; set; }
    }
}
