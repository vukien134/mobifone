using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Invoices
{
    public class InvoiceAuthDetail : TenantOrgEntity
    {
        public string InvoiceAuthId { get; set; }
        public string Ord0 { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? Promotion { get; set; }
        public string TaxCategoryCode { get; set; }
        public string WarehouseCode { get; set; }
        public decimal? DecreaseAmount { get; set; }
        public decimal? DecreasePercentage { get; set; }
        public InvoiceAuth InvoiceAuth { get; set; }
    }
}
