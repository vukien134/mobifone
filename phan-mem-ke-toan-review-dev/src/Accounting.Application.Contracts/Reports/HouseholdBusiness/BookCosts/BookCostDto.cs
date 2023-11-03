using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.HouseholdBusiness
{
    public class BookCostDto
    {
        public string Sort { get; set; }
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string DocId { get; set; }
        public string Ord0 { get; set; }
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? VoucherDate { get; set; }
        public string VoucherNumber { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string SectionCode { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount154A { get; set; }
        public decimal? Amount154B { get; set; }
        public decimal? Amount154C { get; set; }
        public decimal? Amount154D { get; set; }
        public decimal? Amount154E { get; set; }
        public decimal? Amount154F { get; set; }
        public decimal? Amount154G { get; set; }
    }
}
