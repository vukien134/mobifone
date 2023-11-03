﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Categories.Others
{
    public class DefaultCurrency : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameE { get; set; }
        public decimal? ExchangeRate { get; set; }
        public DateTime? ExchangeRateDate { get; set; }
        public bool ExchangeMethod { get; set; }
        public string Default { get; set; }
        public string OddCurrencyVN { get; set; }
        public string OddCurrencyEN { get; set; }
    }
}
