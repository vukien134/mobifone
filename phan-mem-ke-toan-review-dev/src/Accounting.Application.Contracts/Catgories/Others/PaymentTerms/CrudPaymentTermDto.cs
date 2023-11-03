using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.PaymentTerms
{
    public class CrudPaymentTermDto : CruOrgBaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<CrudPaymentTermDetailDto> PaymentTermDetails { get; set; }
    }
}
