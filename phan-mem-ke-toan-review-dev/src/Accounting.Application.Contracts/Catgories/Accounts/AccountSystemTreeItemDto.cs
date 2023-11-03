using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Accounts
{
    public class AccountSystemTreeItemDto : BaseTreeItemDto<AccountSystemTreeItemDto>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
    }
}
