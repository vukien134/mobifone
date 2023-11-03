using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.Currencies
{
    public class DefaultCurrencyDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameE { get; set; }
        public decimal? ExchangeRate { get; set; }
        public DateTime? ExchangeRateDate { get; set; }
        public bool ExchangeMethod { get; set; }
        public string Default { get; set; }
        public string OddCurrencyEN { get; set; }
        public string OddCurrencyVN { get; set; }
    }
}
