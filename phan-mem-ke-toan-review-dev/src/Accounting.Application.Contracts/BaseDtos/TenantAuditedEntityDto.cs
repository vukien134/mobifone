using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Accounting.BaseDtos
{
    public abstract class TenantAuditedEntityDto<TKey> : AuditedEntityDto<TKey>
    {
        public Guid? TenantId { get; set; }
    }
}
