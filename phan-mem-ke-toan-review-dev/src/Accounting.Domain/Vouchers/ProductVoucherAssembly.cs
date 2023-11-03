using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers
{
    public class ProductVoucherAssembly : TenantOrgEntity
    {
        public string ProductVoucherId { get; set; }
        public int Year { get; set; }
        public string AssemblyWarehouseCode { get; set; }
        public string AssemblyProductCode { get; set; }
        public string AssemblyUnitCode { get; set; }
        public string AssemblyWorkPlaceCode { get; set; }
        public string AssemblyProductLotCode { get; set; }
        public decimal? TrxQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public ProductVoucher ProductVoucher { get; set; }
    }
}
