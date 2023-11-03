using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class ReportResponseDto<T> where T: class
    {
        public OrgUnitDto OrgUnit { get; set; }
        public List<T> Data { get; set; }
        public object RequestParameter { get; set; }
        public dynamic TenantSetting { get; set; }
        public CircularsDto Circulars { get; set; }
    }
}
