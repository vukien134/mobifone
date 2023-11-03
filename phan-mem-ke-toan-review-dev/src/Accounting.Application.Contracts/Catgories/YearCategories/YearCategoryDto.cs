using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Text.Json.Serialization;

namespace Accounting.Catgories.YearCategories
{
    public class YearCategoryDto : TenantOrgDto
    {
        public int Year { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
        public int? UsingDecision { get; set; }
    }
}
