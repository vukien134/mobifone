using Accounting.BaseDtos;
using System;
using System.Collections.Generic;

namespace Accounting.Vouchers.ResetVoucherNumbers
{
    public class LockVoucherDto
    {
        public List<LockVoucherDataDto> Data { get; set; }
    }
}
