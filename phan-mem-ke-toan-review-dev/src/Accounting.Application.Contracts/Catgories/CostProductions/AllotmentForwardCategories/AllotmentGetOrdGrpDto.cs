using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Catgories.CostProductions
{
    public class AllotmentGetOrdGrpDto : TenantOrgDto
    {
        public string Id { get; set; }
        public bool SelectRow { get; set; }
        public string FProductWork { get; set; }
        public string Type { get; set; }
        public string OrdGrp { get; set; }
        public int Ord { get; set; }
    }
}
