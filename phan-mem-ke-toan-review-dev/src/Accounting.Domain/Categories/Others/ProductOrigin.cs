using Accounting.TenantEntities;

namespace Accounting.Categories.Others
{
    public class ProductOrigin : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
