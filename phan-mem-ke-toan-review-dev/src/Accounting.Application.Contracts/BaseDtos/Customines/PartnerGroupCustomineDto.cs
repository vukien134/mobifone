using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class PartnerGroupCustomineDto
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int Rank { get; set; }
        public string OrdGroup { get; set; }
    }
}
