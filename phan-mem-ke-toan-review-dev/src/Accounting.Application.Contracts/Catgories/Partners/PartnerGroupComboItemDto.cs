using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Partners
{
    public class PartnerGroupComboItemDto : ComboItemDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
    }
}
