using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.CostProductions.SoTHZs
{
    public class SoTHZRequestDto : PageRequestDto
    {
        public string ProductOrWork { get; set; }
    }
}
