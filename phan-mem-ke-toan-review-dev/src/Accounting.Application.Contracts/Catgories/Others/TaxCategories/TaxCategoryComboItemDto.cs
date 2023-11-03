using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.TaxCategories
{
    public class TaxCategoryComboItemDto : BaseComboItemDto
    {
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public decimal? Percentage { get; set; }
    }
}
