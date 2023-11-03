using System;
using Accounting.BaseDtos;

namespace Accounting.Invoices
{
    public class UpdateInvoiceNumberDto : TenantOrgDto
    {
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public string ReservationCode { get; set; }
        public string TransactionID { get; set; }
        public string InvoiceTemplate { get; set; }
        public string HDonID { get; set; }
        public string cttb_id  { get; set; }
        public string Id { get; set; }
    }
}
