using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.Others.ExciseTaxes
{
    public class CrudExciseTaxDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
        public string Htkk { get; set; }
        public string Htkk0 { get; set; }
        public string HtkkName { get; set; }
        public decimal? ExciseTaxPercentage { get; set; }
    }
}
