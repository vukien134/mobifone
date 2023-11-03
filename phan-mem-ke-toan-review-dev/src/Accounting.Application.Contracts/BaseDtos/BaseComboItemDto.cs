using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos
{
    public class BaseComboItemDto : ComboItemDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string DataId { get; set; }
    }
}
