
﻿using Accounting.Categories.Accounts;
using Accounting.Categories.Others;

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
    public class TaxCategoryService : BaseDomainService<TaxCategory, string>
    {

        public TaxCategoryService(IRepository<TaxCategory, string> repository) 

            : base(repository)
        {
        }
        public async Task<bool> IsExistCode(TaxCategory entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code && p.OrgCode == entity.OrgCode
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(TaxCategory entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.TaxCategory, ErrorCode.Duplicate),
                        $"TaxCategory Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<TaxCategory> GetTaxByCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<TaxCategory>> GetByOutOrIn(string orgCode,string outOrIn)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OutOrIn == outOrIn && p.OrgCode == orgCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode.Equals(orgCode));
        }
    }

}
