using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others.InvoiceBooks
{
    public class InvoiceBook : TenantOrgEntity
    {
        public string Code { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string InvoiceTemplate { get; set; }
        public string InvoiceSerial { get; set; }
        public int? Quantity { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string Note { get; set; }
    }
}
