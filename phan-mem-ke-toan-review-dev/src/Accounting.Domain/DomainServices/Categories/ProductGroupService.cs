using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductGroupService : BaseDomainService<ProductGroup, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public ProductGroupService(IRepository<ProductGroup, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(ProductGroup entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(ProductGroup entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductGroup, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<ProductGroup> GetByCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<ProductGroup>> GetByProductGroupParnerAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ProductGroup>> GetByProductGroupsAsync(string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == ordCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ProductGroup>> GetByProductGroupAsync(string Id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ParentId == Id);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsParentGroup(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.ParentId == id);
        }
    }
}
