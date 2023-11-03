using Accounting.BaseDtos;
using Accounting.Categories.Accounts;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Reports.TT200;
using Accounting.Helpers;
using Accounting.Reports.Financials;
using Accounting.Reports.Financials.Tenant;
using Accounting.Reports.Statements.T200.Defaults;
using Accounting.Reports.Statements.T200.Tenants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Reports.Configs
{
    public class ConfigFStatement200L10AppService : AccountingAppService
    {
        #region Fields
        private readonly DefaultFStatement200L10Service _defaultFStatement200L10Service;
        private readonly FStatement200L10Service _fStatement200L10Service;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public ConfigFStatement200L10AppService(
                DefaultFStatement200L10Service defaultFStatement200L10Service,
                FStatement200L10Service fStatement200L10Service,
                YearCategoryService yearCategoryService,
                WebHelper webHelper,
                IUnitOfWorkManager unitOfWorkManager,
                ICurrentTenant currentTenant
            )
        {
            _defaultFStatement200L10Service = defaultFStatement200L10Service;
            _fStatement200L10Service = fStatement200L10Service;
            _yearCategoryService = yearCategoryService;
            _webHelper = webHelper;
            _unitOfWorkManager = unitOfWorkManager;
            _currentTenant = currentTenant;
        }
        #endregion
        #region Methods
        public async Task<List<FStatement200L10Dto>> ListAsync()
        {
            var yearCategory = await this.GetYearCategoryAsync();
            var lstTenant = (await _fStatement200L10Service.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode && 
                            p.Year == yearCategory.Year && p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
            if (lstTenant.Count > 0)
            {
                return lstTenant.Select(p => ObjectMapper.Map<TenantFStatement200L10, FStatement200L10Dto>(p))
                        .OrderBy(p => p.Ord).ToList();
            }
            var lstDefault = (await _defaultFStatement200L10Service.GetQueryableAsync()).Where(p => p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
            if (lstDefault.Count > 0)
            {
                return lstDefault.Select(p => ObjectMapper.Map<DefaultFStatement200L10, FStatement200L10Dto>(p))
                            .OrderBy(p => p.Ord).ToList();
            }
            return new List<FStatement200L10Dto>();
        }
        public async Task Save(ListRequestDto<CrudFStatement200L10Dto> dtos)
        {
            var yearCategory = await this.GetYearCategoryAsync();
            try
            {
                var lstTenant = (await _fStatement200L10Service.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode &&
                            p.Year == yearCategory.Year && p.UsingDecision == yearCategory.UsingDecision.Value).ToList();
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _fStatement200L10Service.DeleteManyAsync(lstTenant);
                await CreateTenantFStatement200L10Async(dtos.Data,yearCategory);
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
        private async Task CreateTenantFStatement200L10Async(List<CrudFStatement200L10Dto> dtos,YearCategory yearCategory)
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
            var entities = dtos.Select(p => ObjectMapper.Map<CrudFStatement200L10Dto, TenantFStatement200L10>(p))
                            .ToList();
            await _fStatement200L10Service.CreateManyAsync(entities);
        }
        #endregion
    }
}
