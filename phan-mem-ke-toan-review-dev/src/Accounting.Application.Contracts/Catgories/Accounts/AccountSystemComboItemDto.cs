using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Accounts
{
    public class AccountSystemComboItemDto : ComboItemDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string AttachPartner { get; set; }
        //Theo hợp đồng
        public string AttachContract { get; set; }
        //Theo Khoản mục
        public string AttachAccSection { get; set; }
        //Mã khoản mục
        public string AccSectionCode { get; set; }
        //Theo ngoại tệ
        public string AttachCurrency { get; set; }
        //Theo phân xưởng
        public string AttachWorkPlace { get; set; }
        //Theo giá thành, chi phí sản phẩm
        public string AttachProductCost { get; set; }
        //Là tài khoản ngoài bảng
        public string IsBalanceSheetAcc { get; set; }
    }
}
