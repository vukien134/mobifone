using Accounting.BaseDtos;
using System;

namespace Accounting.Catgories.Others.Careers
{
    public class CrudCareerDto : CruOrgBaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
