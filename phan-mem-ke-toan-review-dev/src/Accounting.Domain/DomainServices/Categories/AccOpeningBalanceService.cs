using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class AccOpeningBalanceService : BaseDomainService<AccOpeningBalance, string>
    {
        private readonly WebHelper _webHelper;
        public AccOpeningBalanceService(IRepository<AccOpeningBalance, string> repository, WebHelper webHelper)
            : base(repository)
        {
            _webHelper = webHelper;
        }
        public async Task<bool> IsExistCode(AccOpeningBalance entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.AccCode == entity.AccCode
                                && p.CurrencyCode == entity.CurrencyCode
                                && p.PartnerCode == entity.PartnerCode
                                && p.ContractCode == entity.ContractCode
                                && p.FProductWorkCode == entity.FProductWorkCode
                                && p.AccSectionCode == entity.AccSectionCode
                                && p.WorkPlaceCode == entity.WorkPlaceCode
                                && p.Year == entity.Year
                                && p.Id != entity.Id);
        }
        public async Task<List<AccOpeningBalance>> GetAccOpeningBalances()
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() &&
                                             p.Year == _webHelper.GetCurrentYear());
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<AccOpeningBalance>> GetByAccCodeAsync(string accCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AccCode == accCode &&
                                             p.OrgCode == _webHelper.GetCurrentOrgUnit() &&
                                             p.Year == _webHelper.GetCurrentYear());
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}