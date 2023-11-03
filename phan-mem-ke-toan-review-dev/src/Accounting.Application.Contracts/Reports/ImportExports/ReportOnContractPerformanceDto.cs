using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.ImportExports
{
    public class ReportOnContractPerformanceDto
    {
        public int Sort { get; set; }
        public string Bold { get; set; }
        [JsonDateTimeFormat("dd/MM/yyyy")]
        public DateTime? BeginDate { get; set; }
        public string ContractType { get; set; }
        public string PartnerCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TrxQuantity { get; set; }
        public decimal? TrxPrice { get; set; }
        public decimal? TrxAmount { get; set; }
        public decimal? QuantityCl { get; set; }
        public decimal? PriceCl { get; set; }
        public decimal? AmountCl { get; set; }
        public decimal? Amount { get; set; }
        public string ContractCode { get; set; }
        public decimal? QuantityTh { get; set; }
        public decimal? AmountCurTh { get; set; }
        public decimal? AmountTh { get; set; }
    }
}

