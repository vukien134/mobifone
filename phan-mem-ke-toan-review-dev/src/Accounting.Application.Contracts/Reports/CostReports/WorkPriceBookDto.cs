using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.CostReports
{
    public class WorkPriceBookDto
    {
        public int? Ord { get; set; }
        public string Note { get; set; }        
        public decimal? Ps621 { get; set; }
        public decimal? Ps622 { get; set; }
        public decimal? Ps623 { get; set; }
        public decimal? Ps627 { get; set; }
        public decimal? Dk154 { get; set; }
        public decimal? Ck154 { get; set; }
        public decimal? TotalZ { get; set; }
        public decimal? Amount
        {
            get
            {
                if (this.Dk154 != null && this.Dk154 != 0) return this.Dk154;
                if (this.Ck154 != null && this.Ck154 != 0) return this.Ck154;
                if (this.TotalZ != null && this.TotalZ != 0) return this.TotalZ;

                decimal _ps621 = this.Ps621 == null ? 0 : this.Ps621.Value;
                decimal _ps622 = this.Ps622 == null ? 0 : this.Ps622.Value;
                decimal _ps623 = this.Ps623 == null ? 0 : this.Ps623.Value;
                decimal _ps627 = this.Ps627 == null ? 0 : this.Ps627.Value;

                return _ps621 + _ps622 + _ps623 + _ps627;
            }
        }
    }
}
