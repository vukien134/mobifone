using Accounting.BaseDtos;
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace Accounting.Catgories.Others.BusinessCategories
{
    public class CrudBusinessCategoryDto : CruOrgBaseDto
    {
        public string VoucherCode { get; set; }

        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
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
