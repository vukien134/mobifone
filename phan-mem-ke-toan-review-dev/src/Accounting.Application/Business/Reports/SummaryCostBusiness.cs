using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Licenses;
using Accounting.Localization;
using Accounting.Report;
using Accounting.Reports;
using Accounting.Reports.Cores;
using Accounting.Reports.Financials;
using Accounting.Reports.Financials.StatementOfValueAddedTax;
using Accounting.Reports.HouseholdBusiness;
using Accounting.Reports.Tenants;
using Accounting.Reports.Tenants.TenantStatementTaxs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.Business
{
    public class SummaryCostBusiness : BaseBusiness, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly ProductGroupAppService _productGroupAppService;
        private readonly WarehouseService _warehouseService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly CareerService _careerService;
        private readonly ProductService _productService;
        private readonly AccSectionService _accSectionService;
        private readonly ProductLotService _productLotService;
        private readonly OrgUnitService _orgUnitService;
        private readonly LedgerService _ledgerService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly ProductAppService _productAppService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly IObjectMapper _objectMapper;

        #endregion
        public SummaryCostBusiness(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        ProductGroupAppService productGroupAppService,
                        WarehouseService warehouseService,
                        VoucherTypeService voucherTypeService,
                        CareerService careerService,
                        ProductService productService,
                        AccSectionService accSectionService,
                        ProductLotService productLotService,
                        OrgUnitService orgUnitService,
                        LedgerService ledgerService,
                        AccPartnerAppService accPartnerAppService,
                        ProductAppService productAppService,
                        WarehouseBookService warehouseBookService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        IObjectMapper objectMapper,
                        IStringLocalizer<AccountingResource> localizer) : base(localizer)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _productGroupAppService = productGroupAppService;
            _warehouseService = warehouseService;
            _voucherTypeService = voucherTypeService;
            _careerService = careerService;
            _productService = productService;
            _accSectionService = accSectionService;
            _productLotService = productLotService;
            _orgUnitService = orgUnitService;
            _ledgerService = ledgerService;
            _accPartnerAppService = accPartnerAppService;
            _productAppService = productAppService;
            _warehouseBookService = warehouseBookService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _objectMapper = objectMapper;
        }
        #region Methods
        public async Task<ReportResponseDto<SummaryCostDto>> CreateDataAsync(ReportRequestDto<SummaryCostParamaterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var accSection = await _accSectionService.GetQueryableAsync();
            var lstAccSection = accSection.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var ledger = await _ledgerService.GetQueryableAsync();
            var lstLedger = ledger.Where(p => p.OrgCode == orgCode && String.Compare(p.Status, "2") < 0
                                           && p.VoucherDate >= dto.Parameters.FromDate && p.VoucherDate <= dto.Parameters.ToDate
                                           && (p.DebitAcc ?? "").StartsWith(dto.Parameters.AccCode)
                                        ).GroupBy(p => new { OrgCode = p.OrgCode, SectionCode = p.SectionCode }).Select(p => new SummaryCostDto
                                        {
                                            OrgCode = p.Key.OrgCode,
                                            SectionCode = p.Key.SectionCode,
                                            AmountCur = p.Sum(p => p.AmountCur ?? 0),
                                            Amount = p.Sum(p => p.Amount ?? 0),
                                        }).ToList();
            var data = (from a in lstAccSection
                        join b in lstLedger on a.Code equals b.SectionCode into ajb
                        from b in ajb.DefaultIfEmpty()
                        where a.Code.StartsWith("154")
                        group new { a, b } by new
                        {
                            a.OrgCode,
                            a.Code,
                            a.Name,
                        } into gr
                        orderby gr.Key.Code
                        select new SummaryCostDto
                        {
                            Sort = "A",
                            Bold = "K",
                            OrgCode = gr.Key.OrgCode,
                            SectionCode = gr.Key.Code,
                            SectionName = gr.Key.Name,
                            AmountCur = gr.Sum(p => p.b?.AmountCur ?? 0),
                            Amount = gr.Sum(p => p.b?.Amount ?? 0),
                        }).ToList();
            data.Add(new SummaryCostDto
            {
                Sort = "B",
                Bold = "C",
                OrgCode = _webHelper.GetCurrentOrgUnit(),
                SectionName = "Tổng cộng",
                AmountCur = data.Sum(p => p.AmountCur ?? 0),
                Amount = data.Sum(p => p.Amount ?? 0),
            });
            int i = 24;
            foreach (var item in data)
            {
                item.NumberCode = "[" + i + "]";
                item.Amount = item.Amount == 0 ? null : item.Amount;
                item.AmountCur = item.AmountCur == 0 ? null : item.AmountCur;
                i++;
            }
            var reportResponse = new ReportResponseDto<SummaryCostDto>();
            reportResponse.Data = data.OrderBy(p => p.Sort).ThenBy(p => p.SectionCode).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return _objectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
        }
        private async Task<dynamic> GetTenantSetting(string orgCode)
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            var tenantSettings = await _tenantSettingService.GetBySettingTypeAsync(orgCode, TenantSettingType.Report);
            foreach (var setting in tenantSettings)
            {
                ((IDictionary<String, Object>)exo).Add(setting.Key, setting.Value);
            }
            return exo;
        }
        private async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return _objectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        #endregion
    }
}
