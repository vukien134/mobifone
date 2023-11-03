using System;
using Accounting.BaseDtos;

namespace Accounting.Vouchers.RefVouchers
{
    public class CrudRefVoucherDto : CruOrgBaseDto
    {
        public string SrcId { get; set; }
        public string DestId { get; set; }
    }
}

