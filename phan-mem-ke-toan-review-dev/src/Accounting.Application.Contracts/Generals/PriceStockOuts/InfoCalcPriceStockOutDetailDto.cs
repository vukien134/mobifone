using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Generals.PriceStockOuts
{
    public class InfoCalcPriceStockOutDetailDto : TenantOrgDto
    {
        public string Status { get; set; }
        public string InfoCalcPriceStockOutId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsError { get; set; }
        public string ErrorMsg { get; set; }
    }
}
