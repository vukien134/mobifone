using Accounting.Categories.AssetTools;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.AssetTools
{
    public class AdjustDepreciationService : BaseDomainService<AdjustDepreciation, string>
    {
        public AdjustDepreciationService(IRepository<AdjustDepreciation, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(AdjustDepreciation entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(AdjustDepreciation entity)
        {
            //bool isExist = await IsExistCode(entity);
            //if (isExist)
            //{
            //    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AdjustDepreciation, ErrorCode.Duplicate),
            //            $"AdjustDepreciation Code ['{entity.Code}'] already exist ");
            //}
        }
    }
}
