using Accounting.BaseDtos;

namespace Accounting.Catgories.Sections
{
    public class AccSectionDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string AttachProductCost { get; set; }
        public string SectionType { get; set; }
    }
}
