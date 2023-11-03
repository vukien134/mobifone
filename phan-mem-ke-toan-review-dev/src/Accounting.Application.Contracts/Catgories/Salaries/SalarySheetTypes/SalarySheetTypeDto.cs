using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Salaries.SalaryTypes
{
    public class SalarySheetTypeDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string VoucherCode { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
    }
}
