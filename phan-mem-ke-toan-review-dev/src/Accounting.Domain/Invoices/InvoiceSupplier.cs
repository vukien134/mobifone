using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Invoices
{
    public class InvoiceSupplier : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Active { get; set; }
        public string Link { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PartnerGuid { get; set; }
        public string PartnerToken { get; set; }
        public string AppId { get; set; }
        public string Url { get; set; }
        public string UserSevice { get; set; }
        public string PassSevice { get; set; }
        public string CertificateSerial { get; set; }
        public string CheckCircular { get; set; }
        public string TaxCode { get; set; }
    }
}
