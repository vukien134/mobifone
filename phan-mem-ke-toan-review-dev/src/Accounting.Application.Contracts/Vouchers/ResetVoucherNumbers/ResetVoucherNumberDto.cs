using Accounting.BaseDtos;
using System;
using System.Collections.Generic;

namespace Accounting.Vouchers.ResetVoucherNumbers
{
    public class ResetVoucherNumberDto : TenantOrgDto
    {
        public List<ResetVoucherNumberDataDto> Data { get; set; }
    }
}
