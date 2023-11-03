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
    public class ConfigFStatement133L05AppService : AccountingAppService
    {
        #region Fields
        private readonly DefaultFStatement133L05Service _defaultFStatement133L05Service;
        private readonly FStatement133L05Service _fStatement133L05Service;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public ConfigFStatement133L05AppService(
                DefaultFStatement133L05Service defaultFStatement133L05Service,
                FStatement133L05Service fStatement133L05Service,
                YearCategoryService yearCategoryService,
                WebHelper webHelper,
                IUnitOfWorkManager unitOfWorkManager,
                ICurrentTenant currentTenant
            )
        {
            _defaultFStatement133L05Service = defaultFStatement133L05Service;
            _fStatement133L05Service = fStatement133L05Service;
            _yearCategoryService = yearCategoryService;
            _webHelper = webHelper;
            _unitOfWorkManager = unitOfWorkManager;
            _currentTenant = currentTenant;
        }
        #endregion
        #region Methods
        public async Task<List<FStatement133L05Dto>> ListAsync()
        {
            var yearCategory = await this.GetYearCategoryAsync();
            var lstTenant = (await _fStatement133L05Service.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode && 
                            p.Year == yearCategory.Year && p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
            if (lstTenant.Count > 0)
            {
                return lstTenant.Select(p => ObjectMapper.Map<TenantFStatement133L05, FStatement133L05Dto>(p))
                        .OrderBy(p => p.Ord).ToList();
            }
            var lstDefault = (await _defaultFStatement133L05Service.GetQueryableAsync()).Where(p => p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
            if (lstDefault.Count > 0)
            {
                return lstDefault.Select(p => ObjectMapper.Map<DefaultFStatement133L05, FStatement133L05Dto>(p))
                            .OrderBy(p => p.Ord).ToList();
            }
            return new List<FStatement133L05Dto>();
        }
        public async Task Save(ListRequestDto<CrudFStatement133L05Dto> dtos)
        {
            var yearCategory = await this.GetYearCategoryAsync();
            try
            {
                var lstTenant = (await _fStatement133L05Service.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode &&
                            p.Year == yearCategory.Year && p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _fStatement133L05Service.DeleteManyAsync(lstTenant);
                await CreateTenantFStatement133L05Async(dtos.Data,yearCategory);
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
        private async Task CreateTenantFStatement133L05Async(List<CrudFStatement133L05Dto> dtos,YearCategory yearCategory)
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
            var entities = dtos.Select(p => ObjectMapper.Map<CrudFStatement133L05Dto, TenantFStatement133L05>(p))
                            .ToList();
            await _fStatement133L05Service.CreateManyAsync(entities);
        }
        #endregion
    }
}
