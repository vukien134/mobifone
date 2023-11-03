using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class IncurredExpensesDto
    {
        public string Id { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public string AccCode { get; set; }
        public string PartnerCode { get; set; }
        public string ContractCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string FProductWorkCode0 { get; set; }
        public string SectionCode { get; set; }
        public string ProductCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? BeginQuantity { get; set; }
        public decimal? BeginAmount { get; set; }
        public decimal? EndQuantity { get; set; }
        public decimal? EndAmount { get; set; }
    }
}
