using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.ExciseTaxes
{
    public class ExciseTaxComboItemDto : BaseComboItemDto
    {
        public decimal? ExciseTaxPercentage { get; set; }
    }
}
