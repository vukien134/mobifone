using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Financials.StatementOfValueAddedTax
{
    public class DataTaxDetailDto
    {
        public string OrgCode { get; set; }
        public string InvoiceGroup { get; set; }
        public string TaxCategoryCode { get; set; }
        public string OutOrIn { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string DebitAcc0 { get; set; }
        public string CreditAcc0 { get; set; }
        public int Deduct { get; set; }
        public decimal? AmountWithoutVat { get; set; }
        public decimal? Amount { get; set; }

    }
}
