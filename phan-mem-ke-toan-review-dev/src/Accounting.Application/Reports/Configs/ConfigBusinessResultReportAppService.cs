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
    public class ConfigBusinessResultReportAppService : AccountingAppService
    {
        #region Fields
        private readonly DefaultBusinessResultService _defaultBusinessResultService;
        private readonly TenantBusinessResultService _tenantBusinessResultService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public ConfigBusinessResultReportAppService(DefaultBusinessResultService defaultBusinessResultService,
                TenantBusinessResultService tenantBusinessResultService,
                WebHelper webHelper,
                YearCategoryService yearCategoryService,
                IUnitOfWorkManager unitOfWorkManager,
                ICurrentTenant currentTenant
            )
        {
            _tenantBusinessResultService = tenantBusinessResultService;
            _defaultBusinessResultService = defaultBusinessResultService;
            _webHelper = webHelper;
            _yearCategoryService = yearCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
            _currentTenant = currentTenant;
        }
        #endregion
        #region Methods
        public async Task<List<TenantBusinessResultDto>> ListAsync()
        {
            var yearCategory = await this.GetYearCategoryAsync();
            var lstTenant = await _tenantBusinessResultService.GetAllAsync(yearCategory.OrgCode,
                            yearCategory.Year, yearCategory.UsingDecision.Value);
            if (lstTenant.Count > 0)
            {
                return lstTenant.Select(p => ObjectMapper.Map<TenantBusinessResult, TenantBusinessResultDto>(p))
                        .OrderBy(p => p.Ord).ToList();
            }
            var lstDefault = await _defaultBusinessResultService.GetAllAsync(yearCategory.UsingDecision.Value);
            if (lstDefault.Count > 0)
            {
                return lstDefault.Select(p => ObjectMapper.Map<DefaultBusinessResult, TenantBusinessResultDto>(p))
                            .OrderBy(p => p.Ord).ToList();
            }
            return new List<TenantBusinessResultDto>();
        }
        public async Task Save(ListRequestDto<CrudTenantBusinessResultDto> dtos)
        {
            var yearCategory = await this.GetYearCategoryAsync();
            try
            {
                var lstTenant = await _tenantBusinessResultService.GetAllAsync(yearCategory.OrgCode,
                            yearCategory.Year, yearCategory.UsingDecision.Value);
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _tenantBusinessResultService.DeleteManyAsync(lstTenant);
                await CreateTenantConfigAsync(dtos.Data, yearCategory);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
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
        private async Task CreateTenantConfigAsync(List<CrudTenantBusinessResultDto> dtos, YearCategory yearCategory)
        {
            var tenantId = _currentTenant.Id;
            var orgCode = _webHelper.GetCurrentOrgUnit();
            foreach (var item in dtos)
            {
                item.Id = this.GetNewObjectId();
                item.UsingDecision = yearCategory.UsingDecision;
                item.Year = yearCategory.Year;
                item.OrgCode = orgCode;
            }
            var entities = dtos.Select(p => ObjectMapper.Map<CrudTenantBusinessResultDto, TenantBusinessResult>(p))
                            .ToList();
            await _tenantBusinessResultService.CreateManyAsync(entities);
        }
        #endregion
    }
}
