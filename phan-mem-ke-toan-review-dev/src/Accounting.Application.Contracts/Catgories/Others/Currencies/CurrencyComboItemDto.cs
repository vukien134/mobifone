using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.Currencies
{
    public class CurrencyComboItemDto : BaseComboItemDto
    {
        public decimal? ExchangeRate { get; set; }
        public string Default { get; set; }
        public bool ExchangeMethod { get; set; }
    }
}
