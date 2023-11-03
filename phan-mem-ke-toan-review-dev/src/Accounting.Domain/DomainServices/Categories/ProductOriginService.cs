using Accounting.Categories.Others;
using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductOriginService : BaseDomainService<ProductOrigin, string>
    {
        private readonly WebHelper _webHelper;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        public ProductOriginService(IRepository<ProductOrigin, string> repository,
                    WebHelper webHelper,
                    IStringLocalizer<AccountingResource> localizer
            ): base(repository)
        {
            _webHelper = webHelper;
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(ProductOrigin entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(ProductOrigin entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductOrigin, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<List<ProductOrigin>> GetByAccCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code &&
                                             p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return await AsyncExecuter.ToListAsync(queryable);
        }

        public async Task<List<ProductOrigin>> GetDataReference(string orgCode, string filterValue)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue)))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
        public IQueryable<ProductOrigin> GetQueryableQuickSearch(IQueryable<ProductOrigin> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue));
            return queryable;
        }
    }
}
