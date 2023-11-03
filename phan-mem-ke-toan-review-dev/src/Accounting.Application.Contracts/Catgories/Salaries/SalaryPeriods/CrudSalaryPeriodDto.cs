using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounting.Catgories.Salaries.SalaryPeriods
{
    public class CrudSalaryPeriodDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? FromDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ToDate { get; set; }
        public int? Days { get; set; }
        public string Note { get; set; }
    }
}
