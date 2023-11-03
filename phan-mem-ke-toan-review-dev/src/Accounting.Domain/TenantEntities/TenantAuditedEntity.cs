using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Accounting.TenantEntities
{
    [Serializable]
    public abstract class TenantAuditedEntity<TKey> : AuditedEntity<TKey>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }

        protected TenantAuditedEntity()
        {
        }

        protected TenantAuditedEntity(TKey id)
            : base(id)
        {
        }        
    }
}
