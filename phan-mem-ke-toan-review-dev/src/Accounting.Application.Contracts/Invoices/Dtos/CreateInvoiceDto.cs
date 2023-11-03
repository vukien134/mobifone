using Accounting.BaseDtos;
using System.Collections.Generic;

namespace Accounting.Invoices
{
    public class CreateInvoiceDto
    {
        public string VoucherCode { get; set; }
        public int Type { get; set; } // 0: HẠCH TOÁN 1, 1: HẠCH TOÁN 2
        public string Status { get; set; } = "Chờ ký";
        public List<string> ListId { get; set; }
    }
}
