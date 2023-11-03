using System;
using Accounting.JsonConverters;

namespace Accounting.Reports.CostReports
{
    public class ProductionCostReportDataDto
    {
        public string Sort { get; set; }
        public string Status { get; set; }
        public string Bold { get; set; }
        public int Rank { get; set; }
        public string OrgCode { get; set; }
        public string Code { get; set; }
        public string ProductCode0 { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string ProductionPeriodCode { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal? BeginQuantity { get; set; }
        public decimal? BeginPercentage { get; set; }
        public decimal? BeginAmount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? IncurredTotalCost { get; set; }
        public decimal? TotalZ { get; set; }
        public decimal? EndQuantity { get; set; }
        public decimal? EndPercentage { get; set; }
        public decimal? EndAmount { get; set; }

    }
}

