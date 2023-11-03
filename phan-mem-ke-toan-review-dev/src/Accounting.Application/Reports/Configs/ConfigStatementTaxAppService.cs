using Accounting.BaseDtos;
using Accounting.Categories.Accounts;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Reports.TT133;
using Accounting.Helpers;
using Accounting.Reports.Financials.Tenant;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Tenants;
using Accounting.Reports.Tenants.TenantStatementTaxs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Reports.Configs
{
    public class ConfigStatementTaxAppService : AccountingAppService
    {
        #region Fields
        private readonly DefaultStatementTaxService _defaultStatementTaxService;
        private readonly TenantStatementTaxService _tenantStatementTaxService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public ConfigStatementTaxAppService(
                DefaultStatementTaxService defaultStatementTaxService,
                TenantStatementTaxService tenantStatementTaxService,
                YearCategoryService yearCategoryService,
                WebHelper webHelper,
                IUnitOfWorkManager unitOfWorkManager,
                ICurrentTenant currentTenant
            )
        {
            _defaultStatementTaxService = defaultStatementTaxService;
            _tenantStatementTaxService = tenantStatementTaxService;
            _yearCategoryService = yearCategoryService;
            _webHelper = webHelper;
            _unitOfWorkManager = unitOfWorkManager;
            _currentTenant = currentTenant;
        }
        #endregion
        #region Methods
        public async Task<List<TenantStatementTaxDto>> ListAsync()
        {
            var yearCategory = await this.GetYearCategoryAsync();
            var lstTenant = (await _tenantStatementTaxService.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode && 
                            p.Year == yearCategory.Year).ToList();
            if (lstTenant.Count > 0)
            {
                return lstTenant.Select(p => ObjectMapper.Map<TenantStatementTax, TenantStatementTaxDto>(p))
                        .OrderBy(p => p.Ord).ToList();
            }
            var lstDefault = (await _defaultStatementTaxService.GetQueryableAsync()).ToList();
            if (lstDefault.Count > 0)
            {
                return lstDefault.Select(p => ObjectMapper.Map<DefaultStatementTax, TenantStatementTaxDto>(p))
                            .OrderBy(p => p.Ord).ToList();
            }
            return new List<TenantStatementTaxDto>();
        }
        public async Task Save(ListRequestDto<CrudTenantStatementTaxDto> dtos)
        {
            var yearCategory = await this.GetYearCategoryAsync();
            try
            {
                var lstTenant = (await _tenantStatementTaxService.GetQueryableAsync()).Where(p => p.OrgCode == yearCategory.OrgCode &&
                            p.Year == yearCategory.Year).ToList();
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _tenantStatementTaxService.DeleteManyAsync(lstTenant);
                await CreateTenantStatementTaxAsync(dtos.Data,yearCategory);
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
        private async Task CreateTenantStatementTaxAsync(List<CrudTenantStatementTaxDto> dtos,YearCategory yearCategory)
        {
            var tenantId = _currentTenant.Id;
            var orgCode = _webHelper.GetCurrentOrgUnit();
            foreach(var item in dtos)
            {
                item.Id = this.GetNewObjectId();
                item.Year = yearCategory.Year;
                item.OrgCode = orgCode;
            }
            var entities = dtos.Select(p => ObjectMapper.Map<CrudTenantStatementTaxDto, TenantStatementTax>(p))
                            .ToList();
            await _tenantStatementTaxService.CreateManyAsync(entities);
        }
        #endregion
    }
}
