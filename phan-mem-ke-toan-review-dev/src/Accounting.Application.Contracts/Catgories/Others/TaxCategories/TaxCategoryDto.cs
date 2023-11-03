using Accounting.BaseDtos;
using System;

namespace Accounting.Catgories.Others.TaxCategories
{
    public class TaxCategoryDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Percentage { get; set; }
        //Thuế VAT đầu vào hay ra
        public string OutOrIn { get; set; }
        public int Deduct { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        //Là loại thuế trực tiếp
        public bool IsDirect { get; set; }
        public decimal? Percetage0 { get; set; }
    }
}
