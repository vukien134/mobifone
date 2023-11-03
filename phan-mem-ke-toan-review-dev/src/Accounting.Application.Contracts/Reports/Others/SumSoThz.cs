using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Others
{
    public class SumSoThz
    {
        public string SectionCode { get; set; }
        public string OrgCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string FieldName { get; set; }
        public decimal? Amount { get; set; }
    }
}
