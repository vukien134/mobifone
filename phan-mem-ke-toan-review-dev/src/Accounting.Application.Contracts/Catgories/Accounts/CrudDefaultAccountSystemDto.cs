using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Accounts
{
    public class CrudDefaultAccountSystemDto : CruBaseDto
    {
        public string AccCode { get; set; }
        public int AccPattern { get; set; }
        public string AssetOrEquity { get; set; }
        public string AccName { get; set; }
        public string AccNameEn { get; set; }
        //Bac TK
        public int AccRank { get; set; }
        public string AccType { get; set; }
        public string ParentCode { get; set; }
        public string AccNameTemp { get; set; }
        public string AccNameTempE { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string Province { get; set; }
        //Theo đối tượng, đối tác
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
        //Theo chứng từ
        public string AttachVoucher { get; set; }
        public string ParentAccId { get; set; }
    }
}
