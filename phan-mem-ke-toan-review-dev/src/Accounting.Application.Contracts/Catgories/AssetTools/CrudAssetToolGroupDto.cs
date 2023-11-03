using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.AssetTools
{
    public class CrudAssetToolGroupDto : CruOrgBaseDto
    {
        public string AssetOrTool { get; set; }

        [Required]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }

        public string ParentId { get; set; }
    }
}
