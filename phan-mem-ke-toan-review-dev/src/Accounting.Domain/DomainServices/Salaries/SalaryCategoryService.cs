using Accounting.Categories.Salaries;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Salaries
{
    public class SalaryCategoryService : BaseDomainService<SalaryCategory, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public SalaryCategoryService(IRepository<SalaryCategory, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(SalaryCategory entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(SalaryCategory entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SalaryCategory, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<SalaryCategory> GetByCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<List<SalaryCategory>> GetDataReference(string orgCode, string filterValue)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue)))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
        public IQueryable<SalaryCategory> GetQueryableQuickSearch(IQueryable<SalaryCategory> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue));
            return queryable;
        }
        public async Task<List<SalaryCategory>> GetList()
        {
            return await this.GetRepository().ToListAsync();
        }
    }
}
