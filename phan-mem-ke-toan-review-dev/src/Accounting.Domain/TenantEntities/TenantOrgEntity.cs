namespace Accounting.TenantEntities
{
    public abstract class TenantOrgEntity : TenantAuditedEntity<string>
    {
        public string OrgCode { get; set; }
    }
}
