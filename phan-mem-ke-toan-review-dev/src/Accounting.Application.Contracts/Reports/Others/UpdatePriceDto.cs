using System;
using Accounting.Vouchers.VoucherExciseTaxs;
using System.Collections.Generic;
using Accounting.JsonConverters;

namespace Accounting.Reports.Others
{
    public class UpdatePriceDto
    {
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime FromDate { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime ToDate { get; set; }
        public string VoucherCode { get; set; }
        public List<CostPriceUpdateDto> costPriceUpdateDtos { get; set; }
    }
}

