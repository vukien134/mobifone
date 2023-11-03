using Accounting.BaseDtos;
using Accounting.JsonConverters;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.RefVouchers;
using Accounting.Vouchers.VoucherExciseTaxs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.ProductVouchers
{
    public class CheckDebtCeilingDto
    {
        public string OrgCode { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public decimal DebtCeiling { get; set; }
        public decimal Balance { get; set; }
    }
}
