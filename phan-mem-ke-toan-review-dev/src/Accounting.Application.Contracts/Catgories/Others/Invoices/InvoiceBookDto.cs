using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.Invoices
{
    public class InvoiceBookDto : TenantOrgDto
    {
        public string Code { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? PurchaseDate { get; set; }

        public string InvoiceTemplate { get; set; }
        public string InvoiceSerial { get; set; }
        public int? Quantity { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string Note { get; set; }
    }
}
