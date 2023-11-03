using Accounting.Categories.CostProductions;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories.CostProduction;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Accounting.DomainServices.Categories
{
    public class AllotmentForwardCategoryService : BaseDomainService<AllotmentForwardCategory, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly DefaultAllotmentForwardCategoryService _defaultAllotmentForwardCategoryService;
        private readonly IObjectMapper _objectMapper;
        private readonly WebHelper _webHelper;
        #endregion
        public AllotmentForwardCategoryService(IRepository<AllotmentForwardCategory, string> repository,
                    IStringLocalizer<AccountingResource> localizer,
                    IObjectMapper objectMapper,
                    WebHelper webHelper,
                    DefaultAllotmentForwardCategoryService defaultAllotmentForwardCategoryService
                ) : base(repository)
        {
            _localizer = localizer;
            _objectMapper = objectMapper;
            _webHelper = webHelper;
            _defaultAllotmentForwardCategoryService = defaultAllotmentForwardCategoryService;
        }
        public async Task<bool> IsExistCode(AllotmentForwardCategory entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Year == entity.Year
                                && p.Id != entity.Id
                                );
        }
        public override async Task CheckDuplicate(AllotmentForwardCategory entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AllotmentForwardCategory, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<bool> IsExistListAsync(string type, string productOrWork, int usingDecision,
                                                        string ordGrp, string orgCode, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Type == type
                                    && p.FProductWork == productOrWork
                                    && p.DecideApply == usingDecision
                                    && p.Year == year
                                    );
            if (!string.IsNullOrEmpty(ordGrp))
            {
                queryable = queryable.Where(p => p.OrdGrp == ordGrp);
            }
            return await AsyncExecuter.AnyAsync(queryable);
        }

        public async Task<List<AllotmentForwardCategory>> GetData(string orgCode)
        {
            var allotmentForwardCategory = await this.GetQueryableAsync();
            var lstAllotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == orgCode).ToList();
            if (lstAllotmentForwardCategory.Count() == 0)
            {
                var defaultAllotmentForwardCategory = await _defaultAllotmentForwardCategoryService.GetQueryableAsync();
                lstAllotmentForwardCategory = defaultAllotmentForwardCategory
                                           .Select(p => _objectMapper.Map<DefaultAllotmentForwardCategory, AllotmentForwardCategory>(p)).ToList();
                foreach (var item in lstAllotmentForwardCategory)
                {
                    item.OrgCode = orgCode;
                    item.Year = _webHelper.GetCurrentYear();
                }
            }
            return lstAllotmentForwardCategory;
        }
    }
}
