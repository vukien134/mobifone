using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.CostReports
{
    public class CostReportDto
    {
        public string Bold { get; set; }
        public string OrgCode { get; set; }
        public string FProductWorkCode { get; set; }
        public decimal? Opening154 { get; set; }
        public decimal? Ps621 { get; set; }
        public decimal? Ps622 { get; set; }
        public decimal? Ps623 { get; set; }
        public decimal? Ps627 { get; set; }        
        public decimal? Ending154 { get; set; }
        public decimal? TotalZ { get; set; }
        public decimal? Ps511 { get; set; }        
        public string FProductWorkName { get; set; }
        public string SectionCode { get; set; }
        public decimal? TotalIncurred
        {
            get
            {
                decimal _ps621 = this.Ps621 == null ? 0 : this.Ps621.Value;
                decimal _ps622 = this.Ps622 == null ? 0 : this.Ps622.Value;
                decimal _ps623 = this.Ps623 == null ? 0 : this.Ps623.Value;
                decimal _ps627 = this.Ps627 == null ? 0 : this.Ps627.Value;

                return _ps621 + _ps622 + _ps623 + _ps627;
            }
        }
        public decimal? Profit
        {
            get
            {
                decimal _ps511 = this.Ps511 == null ? 0 : this.Ps511.Value;
                decimal _totalZ = this.TotalZ == null ? 0 : this.TotalZ.Value;
                return _ps511 - _totalZ;
            }
        }
    }
}
