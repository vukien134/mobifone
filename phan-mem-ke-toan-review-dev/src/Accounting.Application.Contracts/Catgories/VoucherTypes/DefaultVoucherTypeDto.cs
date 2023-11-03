using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.VoucherTypes
{
    public class DefaultVoucherTypeDto : BaseDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ListVoucher { get; set; }
        public string ListGroup { get; set; }
    }
}
