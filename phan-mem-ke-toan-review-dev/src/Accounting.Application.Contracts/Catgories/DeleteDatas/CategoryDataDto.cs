using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.CategoryDatas
{
    public class CategoryDataDto
    {
        public string TabName { get; set; }
        public int SelectRow { get; set; } = 0;
        public string Name { get; set; }
        public string FieldCode { get; set; }
        public string RefFieldCode { get; set; }
        public string ConditionField { get; set; }
        public string ConditionValue { get; set; }
        public int Type { get; set; } = 1; // (1. xét điều kiện không có phát sinh mới xóa)
                                           // (2. xóa theo năm không cần điều kiện)
                                           // (3. xóa theo điều kiện ConditionField = ConditionValue)
                                           // (4. dở dang đầu kỳ)
        public string BusinessType { get; set; }
        public int Ord { get; set; }
    }
}
