using Accounting.BaseDtos;

namespace Accounting.Catgories.WorkPlace
{
    public class WorkPlaceDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
