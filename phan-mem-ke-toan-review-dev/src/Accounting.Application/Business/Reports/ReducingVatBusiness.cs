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
    public class ReducingVatBusiness : BaseBusiness, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
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
        private readonly ProductLotService _productLotService;
        private readonly OrgUnitService _orgUnitService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly ProductAppService _productAppService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly IObjectMapper _objectMapper;

        #endregion
        public ReducingVatBusiness(ReportDataService reportDataService,
                        WebHelper webHelper,
                        AccountingCacheManager accountingCacheManager,
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
                        ProductLotService productLotService,
                        OrgUnitService orgUnitService,
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
            _accountingCacheManager = accountingCacheManager;
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
            _productLotService = productLotService;
            _orgUnitService = orgUnitService;
            _accPartnerAppService = accPartnerAppService;
            _productAppService = productAppService;
            _warehouseBookService = warehouseBookService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _objectMapper = objectMapper;
        }
        #region Methods
        public async Task<ReportResponseDto<ReducingVatDto>> CreateDataAsync(ReportRequestDto<TotalRevenueParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstPartner = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroupCode ?? "");
            var lstProduct = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode ?? "");
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherType = voucherType.Where(p => p.Code == "PBH").FirstOrDefault();
            var career = await _careerService.GetQueryableAsync();
            var lstCareer = career.ToList();
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            var dataWarehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                           && p.Status == "1"
                                                           && (lstVoucherType.ListVoucher + ",PTL").Contains(p.VoucherCode)
                                                           && p.VoucherDate >= dto.Parameters.FromDate
                                                           && p.VoucherDate <= dto.Parameters.ToDate
                                                           && ((dto.Parameters.PartnerCode ?? "") == "" || p.PartnerCode == dto.Parameters.PartnerCode)
                                                           && ((dto.Parameters.PartnerGroupCode ?? "") == "" || lstPartner.Select(p => p.Code).Contains(p.PartnerCode))
                                                           && ((dto.Parameters.ProductCode ?? "") == "" || p.ProductCode == dto.Parameters.ProductCode)
                                                           && ((dto.Parameters.ProductGroupCode ?? "") == "" || lstProduct.Select(p => p.Code).Contains(p.ProductCode))
                                                        )
                                                  .OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                                                  .Select(p => new ReducingVatDataWarehouseBookDto
                                                  {
                                                      OrgCode = p.OrgCode,
                                                      DocId = p.ProductVoucherId,
                                                      OrdRec0 = p.Ord0,
                                                      VoucherCode = p.VoucherCode,
                                                      VoucherNumber = p.VoucherNumber,
                                                      VoucherDate = p.VoucherDate,
                                                      PartnerCode = p.PartnerCode0,
                                                      Description = p.Description,
                                                      CurrencyCode = p.CurrencyCode,
                                                      ExchangeRate = p.ExchangeRate,
                                                      WarehouseCode = p.WarehouseCode,
                                                      ProductCode = p.ProductCode,
                                                      UnitCode = p.UnitCode,
                                                      Quantity = p.Quantity,
                                                      Price = (p.VoucherGroup == 4 ? p.Price : p.Price2),
                                                      PriceCur = (p.VoucherGroup == 4 ? p.PriceCur : p.PriceCur2),
                                                      Amount = (p.VoucherGroup == 4 ? p.Amount : p.TotalAmount2),
                                                      AmountCur = (p.VoucherGroup == 4 ? p.AmountCur : p.AmountCur2),
                                                      DebitAcc = (p.VoucherGroup == 4 ? p.DebitAcc : p.DebitAcc2),
                                                      CreditAcc = (p.VoucherGroup == 4 ? p.CreditAcc : p.CreditAcc2),
                                                      ProductName = p.ProductName0,
                                                      AmountDecrease = p.TotalAmount2 - p.Amount2,
                                                  }).ToList();
            dataWarehouseBook = dataWarehouseBook.Where(p => p.AmountDecrease > 0).ToList();
            int i = 1;
            var data = (from a in dataWarehouseBook
                        join b in lstProduct on a.ProductCode equals b.Code
                        group new { a, b } by new
                        {
                            a.OrgCode,
                            a.ProductCode,
                            b.Name,
                            b.VatPercentage
                        } into gr
                        where gr.Sum(p => (p.a.Amount ?? 0)) != 0
                        orderby gr.Key.ProductCode
                        select new ReducingVatDto
                        {
                            Ord = i++,
                            Sort = "A",
                            Bold = "K",
                            OrgCode = gr.Key.OrgCode,
                            ProductCode = gr.Key.ProductCode,
                            ProductName = gr.Key.Name,
                            VatPercentage = gr.Key.VatPercentage,
                            ReductionPercent = gr.Key.VatPercentage * 80 / 100,
                            Turnover = gr.Sum(p => p.a.Amount ?? 0),
                            VatReduction = gr.Sum(p => p.a.AmountDecrease ?? 0),
                        }).ToList();
            data.Add(new ReducingVatDto
            {
                Sort = "B",
                Bold = "C",
                ProductName = "Tổng cộng",
                Turnover = data.Sum(p => p.Turnover ?? 0),
                VatReduction = data.Sum(p => p.VatReduction ?? 0),
            });
            foreach (var item in data)
            {
                var itemProduct = lstProduct.Where(p => p.Code == item.ProductCode).FirstOrDefault();
                var itemCareer = lstCareer.Where(p => p.Code == (itemProduct?.CareerCode ?? "")).FirstOrDefault();
                item.CareerCode = itemCareer?.Code ?? "";
                item.CareerName = itemCareer?.Name ?? "";
            }
            var reportResponse = new ReportResponseDto<ReducingVatDto>();
            reportResponse.Data = data.OrderBy(p => p.Sort).ThenBy(p => p.ProductCode).ToList();
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
