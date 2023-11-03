using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
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
    public class YearCategoryService : BaseDomainService<YearCategory, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public YearCategoryService(IRepository<YearCategory, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ): base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(YearCategory entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Year == entity.Year
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(YearCategory entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.YearCategory, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Year]);
            }
        }
        public async Task<List<YearCategory>> GetByOrgCode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode)
                        .OrderBy(p => p.Year);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<YearCategory> GetByYearAsync(string orgCode,int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Year == year);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<YearCategory> GetLatestFromDateAsync(string orgCode, DateTime fromDate)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.BeginDate <= fromDate);
            queryable = queryable.OrderByDescending(p => p.BeginDate);

            var years = await AsyncExecuter.ToListAsync(queryable);
            if (years.Count > 0)
            {
                return years[0];
            }
            return null;
        }
    }
}
