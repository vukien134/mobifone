using Accounting.BaseDtos;
using Accounting.Catgories.VoucherCategoryDeletes;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.CategoryDatas
{
    public class DeleteDataDto
    {
        public List<VoucherCategoryDeleteDto> LstVoucherCategories { get; set; }
        public List<CategoryDataDto> LstCategories { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd")]
        public DateTime ToDate { get; set; }
    }
}
