using System;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Accounting.Localization;
using Accounting.Categories.Partners;

namespace Accounting.DomainServices.Categories
{
    public class AccSectionService : BaseDomainService<AccSection, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public AccSectionService(IRepository<AccSection, string> repository,
                IStringLocalizer<AccountingResource> localizer
            )
            : base(repository)
        {
            _localizer = localizer;
        }

        public async Task<bool> IsExistCode(AccSection entity)
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
        public override async Task CheckDuplicate(AccSection entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccSection, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<AccSection> GetByCodeAsync(string code,string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Code == code);                                  
            
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<List<AccSection>> GetDataReference(string orgCode, string filterValue)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue)))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }

        public IQueryable<AccSection> GetQueryableQuickSearch(IQueryable<AccSection> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.OrgCode, filterValue) || EF.Functions.ILike(p.Name, filterValue));
            return queryable;
        }
        public async Task<List<AccSection>> GetAll(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode.Equals(orgCode));
        }
        public async Task<List<AccSection>> GetAllByOrgCode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode);
            return queryable.ToList();
        }
    }
}
