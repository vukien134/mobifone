using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.FProductWorks
{
    public class CrudFProductWorkDto : CruOrgBaseDto
    {
        //CT_SP
        public string FProductOrWork { get; set; }

        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
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
