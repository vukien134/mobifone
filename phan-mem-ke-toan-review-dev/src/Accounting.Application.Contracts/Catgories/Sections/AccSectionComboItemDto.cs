using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Sections
{
    public class AccSectionComboItemDto : ComboItemDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string AttachProductCost { get; set; }
        public string SectionType { get; set; }
    }
}
