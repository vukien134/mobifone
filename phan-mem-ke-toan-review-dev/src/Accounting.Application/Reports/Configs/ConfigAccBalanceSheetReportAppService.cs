using Accounting.BaseDtos;
using Accounting.Categories.Accounts;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.Financials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Reports.Configs
{
    public class ConfigAccBalanceSheetReportAppService : AccountingAppService
    {
        #region Fields
        private readonly DefaultAccBalanceSheetService _defaultAccBalanceSheetService;
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public ConfigAccBalanceSheetReportAppService(
                DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                TenantAccBalanceSheetService tenantAccBalanceSheetService,
                YearCategoryService yearCategoryService,
                WebHelper webHelper,
                IUnitOfWorkManager unitOfWorkManager,
                ICurrentTenant currentTenant
            )
        {
            _defaultAccBalanceSheetService = defaultAccBalanceSheetService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _yearCategoryService = yearCategoryService;
            _webHelper = webHelper;
            _unitOfWorkManager = unitOfWorkManager;
            _currentTenant = currentTenant;
        }
        #endregion
        #region Methods
        public async Task<List<TenantAccBalanceSheetDto>> ListAsync()
        {
            var yearCategory = await this.GetYearCategoryAsync();
            var lstTenant = await _tenantAccBalanceSheetService.GetAllAsync(yearCategory.OrgCode, 
                            yearCategory.Year, yearCategory.UsingDecision.Value);
            if (lstTenant.Count > 0)
            {
                return lstTenant.Select(p => ObjectMapper.Map<TenantAccBalanceSheet, TenantAccBalanceSheetDto>(p))
                        .OrderBy(p => p.Ord).ToList();
            }
            var lstDefault = await _defaultAccBalanceSheetService.GetAllAsync(yearCategory.UsingDecision.Value);
            if (lstDefault.Count > 0)
            {
                return lstDefault.Select(p => ObjectMapper.Map<DefaultAccBalanceSheet, TenantAccBalanceSheetDto>(p))
                            .OrderBy(p => p.Ord).ToList();
            }
            return new List<TenantAccBalanceSheetDto>();
        }
        public async Task Save(ListRequestDto<CrudTenantAccBalanceSheetDto> dtos)
        {
            var yearCategory = await this.GetYearCategoryAsync();
            try
            {
                var lstTenant = await _tenantAccBalanceSheetService.GetAllAsync(yearCategory.OrgCode,
                            yearCategory.Year, yearCategory.UsingDecision.Value);
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _tenantAccBalanceSheetService.DeleteManyAsync(lstTenant);
                await CreateTenantAccBalanceSheetAsync(dtos.Data,yearCategory);
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
        private async Task CreateTenantAccBalanceSheetAsync(List<CrudTenantAccBalanceSheetDto> dtos,YearCategory yearCategory)
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
            var entities = dtos.Select(p => ObjectMapper.Map<CrudTenantAccBalanceSheetDto, TenantAccBalanceSheet>(p))
                            .ToList();
            await _tenantAccBalanceSheetService.CreateManyAsync(entities);
        }
        #endregion
    }
}
