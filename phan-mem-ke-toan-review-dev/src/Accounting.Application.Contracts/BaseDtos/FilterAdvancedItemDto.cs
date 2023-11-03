using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos
{
    public class FilterAdvancedItemDto
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public object Value { get; set; }
    }
}
