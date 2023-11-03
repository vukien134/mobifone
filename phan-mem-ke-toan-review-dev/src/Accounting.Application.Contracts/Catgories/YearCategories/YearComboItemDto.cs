using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.YearCategories
{
    public class YearComboItemDto : ComboItemDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginDate { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndDate { get; set; }
    }
}
