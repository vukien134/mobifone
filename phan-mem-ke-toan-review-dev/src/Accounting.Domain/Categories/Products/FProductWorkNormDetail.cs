using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    // Định mức ct, sp
    public class FProductWorkNormDetail : TenantOrgEntity
    {
        public string FProductWorkNormId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Ord0 { get; set; }
        public string AccCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string SectionCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductLotCode { get; set; }
        public string ProductOrigin { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuantityLoss { get; set; }
        public decimal? PercentLoss { get; set; }
        public decimal? PriceCur { get; set; }
        public decimal? Price { get; set; }
        public decimal? AmountCur { get; set; }
        public decimal? Amount { get; set; }
        public DateTime ApplicableDate1 { get; set; }
        public DateTime ApplicableDate2 { get; set; }
        public FProductWorkNorm FProductWorkNorm { get; set; }
    }
}
