using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    // Định mức ct, sp
    public class FProductWorkNorm : TenantOrgEntity
    {
        public int Year { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal Quantity { get; set; }
        public string Note { get; set; }
        public ICollection<FProductWorkNormDetail> FProductWorkNormDetails { get; set; }
    }
}
