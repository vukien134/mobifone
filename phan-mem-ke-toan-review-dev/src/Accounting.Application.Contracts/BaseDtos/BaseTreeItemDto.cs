using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos
{
    public class BaseTreeItemDto<T>
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public bool? Open { get; set; }
        public List<T> Data { get; set; }
        public int? Rank { get; set; }
        public string SortPath { get; set; }
        public string ParentId { get; set; }
        public bool? HasChild { get; set; }
    }
}
