using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;

namespace Accounting.Catgories.VoucherCategoryDeletes
{
    public class VoucherCategoryDeleteDto
    {
        public string Id { get; set; }
        public int SelectRow { get; set; } = 0;
        public string Code { get; set; }
        public string Name { get; set; }
        public string VoucherKind { get; set; }
    }
}
