using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.CostProductions.AllotmentForwardCategories
{
    public class AllotmentForwardRequestDto : PageRequestDto
    {
        public string Type { get; set; }
        public string FProductWork { get; set; }
        public string OrdGrp { get; set; }
    }
}
