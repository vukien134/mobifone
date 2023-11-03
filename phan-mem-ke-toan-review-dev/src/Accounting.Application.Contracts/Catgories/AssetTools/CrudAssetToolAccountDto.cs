using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.AssetTools
{
    public class CrudAssetToolAccountDto : CruOrgBaseDto
    {
        public string AssetOrTool { get; set; }
        public string AssetToolId { get; set; }
        public string Ord0 { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? DepreciationDate { get; set; }
        public string DepartmentCode { get; set; }
        [Required]
        [DisplayName("DebitAcc")]
        [MinLength(3)]
        public string DebitAcc { get; set; }
        [Required]
        [DisplayName("CreditAcc")]
        [MinLength(3)]
        public string CreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public string Note { get; set; }
    }
}
