using Accounting.TenantEntities;

namespace Accounting.Categories.Others
{
    public class Career : TenantAuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
