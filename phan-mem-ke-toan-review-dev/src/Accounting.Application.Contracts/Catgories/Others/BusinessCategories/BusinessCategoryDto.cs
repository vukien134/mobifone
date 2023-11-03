using Accounting.BaseDtos;
using System;

using System.Collections.Generic;
using System.Text;


namespace Accounting.Catgories.Others.BusinessCategories
{
    public class BusinessCategoryDto : TenantOrgDto
    {
        public string VoucherCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public bool IsAccVoucher { get; set; }
        public bool IsProductVoucher { get; set; }
        public string Prefix { get; set; }
        public string Separator { get; set; }
        public string Suffix { get; set; }
    }
}
