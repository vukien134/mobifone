using Accounting.BaseDtos;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.Others.Currencies
{
    public class CrudCurrencyDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
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
