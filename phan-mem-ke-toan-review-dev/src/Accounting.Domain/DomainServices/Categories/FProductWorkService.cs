using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class FProductWorkService : BaseDomainService<FProductWork, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public FProductWorkService(IRepository<FProductWork, string> repository,
                IStringLocalizer<AccountingResource> localizer
            )
            : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(FProductWork entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(string orgCode,string code)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode
                                && p.Code == code);
        }
        public override async Task CheckDuplicate(FProductWork entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<FProductWork> GetByFProductWorkAsync(string code, string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && p.Code == code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<FProductWork>> GetDataReference(string orgCode, string filterValue)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue)))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
        public async Task<List<FProductWork>> GetListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
