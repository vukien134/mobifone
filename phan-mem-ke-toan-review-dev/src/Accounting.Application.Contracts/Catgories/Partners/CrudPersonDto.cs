using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.Partners
{
    public class CrudPersonDto : CruOrgBaseDto
    {
        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
