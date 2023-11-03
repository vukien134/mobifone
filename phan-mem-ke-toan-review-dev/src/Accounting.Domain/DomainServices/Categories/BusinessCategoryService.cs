
﻿using Accounting.Categories.Accounts;
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


namespace Accounting.DomainServices.BusinessCategories
{
    public class BusinessCategoryService : BaseDomainService<BusinessCategory, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public BusinessCategoryService(IRepository<BusinessCategory, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) 

            : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(BusinessCategory entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code && p.OrgCode == entity.OrgCode
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(string orgCode,string code)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == code && p.OrgCode == orgCode);                               
        }
        public override async Task CheckDuplicate(BusinessCategory entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.BusinessCategory, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<List<BusinessCategory>> GetLstBusinessByCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<BusinessCategory> GetBusinessByCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<bool> IsExistListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode.Equals(orgCode));
        }
    }
}
