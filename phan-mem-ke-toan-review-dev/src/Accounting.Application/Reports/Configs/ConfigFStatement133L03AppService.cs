using Accounting.BaseDtos;
using Accounting.Categories.Accounts;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Reports.TT133;
using Accounting.Helpers;
using Accounting.Reports.Financials.Tenant;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Tenants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Reports.Configs
{
    public class ConfigFStatement133L03AppService : AccountingAppService
    {
        #region Fields
        private readonly DefaultFStatement133L03Service _defaultFStatement133L03Service;
        private readonly FStatement133L03Service _fStatement133L03Service;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public ConfigFStatement133L03AppService(
                DefaultFStatement133L03Service defaultFStatement133L03Service,
                FStatement133L03Service fStatement133L03Service,
                YearCategoryService yearCategoryService,
                WebHelper webHelper,
                IUnitOfWorkManager unitOfWorkManager,
                ICurrentTenant currentTenant
            )
        {
            _defaultFStatement133L03Service = defaultFStatement133L03Service;
            _fStatement133L03Service = fStatement133L03Service;
            _yearCategoryService = yearCategoryService;
            _webHelper = webHelper;
            _unitOfWorkManager = unitOfWorkManager;
            _currentTenant = currentTenant;
        }
        #endregion
        #region Methods
        public async Task<List<FStatement133L03Dto>> ListAsync()
        {
            var yearCategory = await this.GetYearCategoryAsync();
            var lstTenant = (await _fStatement133L03Service.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode && 
                            p.Year == yearCategory.Year && p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
            if (lstTenant.Count > 0)
            {
                return lstTenant.Select(p => ObjectMapper.Map<TenantFStatement133L03, FStatement133L03Dto>(p))
                        .OrderBy(p => p.Ord).ToList();
            }
            var lstDefault = (await _defaultFStatement133L03Service.GetQueryableAsync()).Where(p => p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
            if (lstDefault.Count > 0)
            {
                return lstDefault.Select(p => ObjectMapper.Map<DefaultFStatement133L03, FStatement133L03Dto>(p))
                            .OrderBy(p => p.Ord).ToList();
            }
            return new List<FStatement133L03Dto>();
        }
        public async Task Save(ListRequestDto<CrudFStatement133L03Dto> dtos)
        {
            var yearCategory = await this.GetYearCategoryAsync();
            try
            {
                var lstTenant = (await _fStatement133L03Service.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode &&
                            p.Year == yearCategory.Year && p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _fStatement133L03Service.DeleteManyAsync(lstTenant);
                await CreateTenantFStatement133L03Async(dtos.Data,yearCategory);
                await unitOfWork.CompleteAsync();
            }
            catch(Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }
        #endregion
        #region Privates
        private async Task<YearCategory> GetYearCategoryAsync()
        {
            int year = _webHelper.GetCurrentYear();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            return yearCategory;
        }
        private async Task CreateTenantFStatement133L03Async(List<CrudFStatement133L03Dto> dtos,YearCategory yearCategory)
        {
            var tenantId = _currentTenant.Id;
            var orgCode = _webHelper.GetCurrentOrgUnit();
            foreach(var item in dtos)
            {
                item.Id = this.GetNewObjectId();
                item.UsingDecision = yearCategory.UsingDecision;
                item.Year = yearCategory.Year;
                item.OrgCode = orgCode;
            }
            var entities = dtos.Select(p => ObjectMapper.Map<CrudFStatement133L03Dto, TenantFStatement133L03>(p))
                            .ToList();
            await _fStatement133L03Service.CreateManyAsync(entities);
        }
        #endregion
    }
}
