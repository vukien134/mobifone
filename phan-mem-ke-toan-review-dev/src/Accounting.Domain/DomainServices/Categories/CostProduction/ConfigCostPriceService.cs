using Accounting.Categories.CostProductions;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ConfigCostPriceService : BaseDomainService<ConfigCostPrice, string>
    {
        public ConfigCostPriceService(IRepository<ConfigCostPrice, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(ConfigCostPrice entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(ConfigCostPrice entity)
        {
            //bool isExist = await IsExistCode(entity);
            //if (isExist)
            //{
            //    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ConfigCostPrice, ErrorCode.Duplicate),
            //            $"ConfigCostPrice Code ['{entity.Code}'] already exist ");
            //}
        }
    }
}
