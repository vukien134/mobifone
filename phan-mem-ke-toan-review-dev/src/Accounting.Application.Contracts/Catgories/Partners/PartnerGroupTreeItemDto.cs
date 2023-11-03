using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Partners
{
    public class PartnerGroupTreeItemDto : BaseTreeItemDto<PartnerGroupTreeItemDto>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
    }
}
