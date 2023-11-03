using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.FProductWorkNorms
{
    public class FProductWorkNormDto : TenantOrgDto
    {
        public int Year { get; set; }
        public string FProductWorkCode { get; set; }
        public string FProductWorkName { get; set; }
        public decimal Quantity { get; set; }
        public string Note { get; set; }
    }
}
