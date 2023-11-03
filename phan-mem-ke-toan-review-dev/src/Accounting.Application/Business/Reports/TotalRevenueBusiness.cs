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
using Accounting.DomainServices.Configs;
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
    public class TotalRevenueBusiness : BaseBusiness, IUnitOfWorkEnabled
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
        private readonly ICurrentTenant _currentTenant;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly DefaultVoucherTypeService _defaultVoucherTypeService;
        #endregion
        public TotalRevenueBusiness(ReportDataService reportDataService,
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
                        ICurrentTenant currentTenant,
                        TenantExtendInfoService tenantExtendInfoService,
                        DefaultVoucherTypeService defaultVoucherTypeService,
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
            _currentTenant = currentTenant;
            _tenantExtendInfoService = tenantExtendInfoService;
            _defaultVoucherTypeService = defaultVoucherTypeService;
        }
        #region Methods
        public async Task<ReportResponseDto<TotalRevenueDto>> CreateDataAsync(ReportRequestDto<TotalRevenueParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstPartner = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroupCode ?? "");
            var lstProduct = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode ?? "");
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherType = voucherType.Where(p => p.Code == "PBH").FirstOrDefault();
            string ListVoucher = "";
            var lstDeffaultVoucherType = await _defaultVoucherTypeService.GetQueryableAsync();
            if (lstVoucherType != null)
            {
                ListVoucher = lstVoucherType.ListVoucher;
            }
            else
            {
                ListVoucher = lstDeffaultVoucherType.Where(p => p.Code == "PBH").FirstOrDefault().ListVoucher;
            }
            var lst = new List<TotalRevenueDto>();
            var career = await _careerService.GetQueryableAsync();
            var lstCareer = career.ToList();
            var tenantType = await this.GetTenantType();
            if (lstCareer.Count() == 0)
            {
                var defaultCareers = await _careerService.GetByTenantTypeAsync(tenantType);
                lstCareer = defaultCareers.Select(p => _objectMapper.Map<DefaultCareer, Career>(p))
                               .ToList();
            }
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            var dataWarehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                           && p.Status == "1"
                                                           && (ListVoucher + ",PTL").Contains(p.VoucherCode)
                                                           && p.VoucherDate >= dto.Parameters.FromDate
                                                           && p.VoucherDate <= dto.Parameters.ToDate
                                                           && ((dto.Parameters.PartnerCode ?? "") == "" || p.PartnerCode == dto.Parameters.PartnerCode)
                                                           && ((dto.Parameters.PartnerGroupCode ?? "") == "" || lstPartner.Select(p => p.Code).Contains(p.PartnerCode))
                                                           && ((dto.Parameters.ProductCode ?? "") == "" || p.ProductCode == dto.Parameters.ProductCode)
                                                           && ((dto.Parameters.ProductGroupCode ?? "") == "" || lstProduct.Select(p => p.Code).Contains(p.ProductCode))
                                                        )
                                                  .OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                                                  .Select(p => new TotalRevenueDataWarehouseBookDto
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
                                                      Quantity = (p.VoucherGroup == 1 ? -1 : 1) * p.Quantity,
                                                      Price = (p.VoucherGroup == 4 ? p.Price : p.Price2),
                                                      PriceCur = (p.VoucherGroup == 4 ? p.PriceCur : p.PriceCur2),
                                                      Amount = (p.VoucherGroup == 4 ? p.Amount : (p.VoucherGroup == 1 ? -1 : 1) * p.TotalAmount2),
                                                      AmountCur = (p.VoucherGroup == 4 ? p.AmountCur : (p.VoucherGroup == 1 ? -1 : 1) * p.AmountCur2),
                                                      DebitAcc = (p.VoucherGroup == 4 ? p.DebitAcc : p.DebitAcc2),
                                                      CreditAcc = (p.VoucherGroup == 4 ? p.CreditAcc : p.CreditAcc2),
                                                      ProductName0 = p.ProductName0,
                                                      AmountDecrease = p.TotalAmount2 - p.Amount2,
                                                  }).ToList();
            var data = (from a in dataWarehouseBook
                        join b in lstProduct on a.ProductCode equals b.Code
                        group new { a, b } by new
                        {
                            a.OrgCode,
                            b.CareerCode
                        } into gr
                        select new TotalRevenueDataWarehouseBookDto
                        {
                            OrgCode = gr.Key.OrgCode,
                            CareerCode = gr.Key.CareerCode,
                            AmountDecrease = gr.Sum(p => p.a.AmountDecrease),
                            AmountValueAdded = gr.Sum(p => (p.b.VatPercentage ?? 0) != 0 ? p.a.Amount : 0),
                            AmountPersonal = gr.Sum(p => (p.b.PITPercentage ?? 0) != 0 ? p.a.Amount : 0),
                        }).ToList();
            int i = 1;
            foreach (var item in lstCareer)
            {
                if ((item.Code ?? "") != "")
                {
                    var itemData = data.Where(p => p.CareerCode == item.Code).FirstOrDefault();
                    var itemLst = new TotalRevenueDto();
                    itemLst.Sort = "A";
                    itemLst.Bold = "K";
                    itemLst.OrgCode = _webHelper.GetCurrentOrgUnit();
                    itemLst.CareerCode = item.Code;
                    itemLst.Ord = i;
                    itemLst.NumberCode = "[" + (i + 27) + "]";
                    itemLst.Turnover = itemData?.AmountValueAdded ?? 0;
                    itemLst.TurnoverPersonal = itemData?.AmountPersonal ?? 0;
                    switch (item.Code)
                    {
                        case "01":
                            itemLst.CareerName = item.Name + " <i>(Tỷ lệ thuế GTGT 1%, thuế TNCN 0,5%)</i>";
                            itemLst.CareerName0 = item.Name + " (Tỷ lệ thuế GTGT 1%, thuế TNCN 0,5%)";
                            itemLst.Vat = Math.Round(1 * (itemData?.AmountValueAdded ?? 0) / 100, 0) - itemData?.AmountDecrease ?? 0;
                            itemLst.VatPersonal = Math.Round((decimal)0.5 * (itemData?.AmountPersonal ?? 0) / 100, 0);
                            break;
                        case "02":
                            itemLst.CareerName = item.Name + " <i>(Tỷ lệ thuế GTGT 5%, thuế TNCN 2%)</i>";
                            itemLst.CareerName0 = item.Name + " (Tỷ lệ thuế GTGT 5%, thuế TNCN 2%)";
                            itemLst.Vat = Math.Round(5 * (itemData?.AmountValueAdded ?? 0) / 100, 0) - itemData?.AmountDecrease ?? 0;
                            itemLst.VatPersonal = Math.Round(2 * (itemData?.AmountPersonal ?? 0) / 100, 0);
                            break;
                        case "03":
                            itemLst.CareerName = item.Name + " <i>(Tỷ lệ thuế GTGT 3%, thuế TNCN 1,5%)</i>";
                            itemLst.CareerName0 = item.Name + " (Tỷ lệ thuế GTGT 3%, thuế TNCN 1,5%)";
                            itemLst.Vat = Math.Round(3 * (itemData?.AmountValueAdded ?? 0) / 100, 0) - itemData?.AmountDecrease ?? 0;
                            itemLst.VatPersonal = Math.Round((decimal)1.5 * (itemData?.AmountPersonal ?? 0) / 100, 0);
                            break;
                        case "04":
                            itemLst.CareerName = item.Name + " <i>(Tỷ lệ thuế GTGT 2%, thuế TNCN 1%)</i>";
                            itemLst.CareerName0 = item.Name + " (Tỷ lệ thuế GTGT 2%, thuế TNCN 1%)";
                            itemLst.Vat = Math.Round(2 * (itemData?.AmountValueAdded ?? 0) / 100, 0) - itemData?.AmountDecrease ?? 0;
                            itemLst.VatPersonal = Math.Round(1 * (itemData?.AmountPersonal ?? 0) / 100, 0);
                            break;
                        default:
                            itemLst.CareerName = "";
                            itemLst.CareerName0 = "";
                            itemLst.Vat = 0;
                            itemLst.VatPersonal = 0;
                            break;
                    }
                    lst.Add(itemLst);
                    i++;
                }
            }
            lst.Add(new TotalRevenueDto
            {
                OrgCode = _webHelper.GetCurrentOrgUnit(),
                Sort = "B",
                Bold = "K",
                NumberCode = "[" + (i + 27) + "]",
                CareerName = "Tổng Cộng",
                Turnover = lst.Sum(p => p.Turnover),
                Vat = lst.Sum(p => p.Vat),
                TurnoverPersonal = lst.Sum(p => p.TurnoverPersonal),
                VatPersonal = lst.Sum(p => p.VatPersonal),
            });
            foreach (var item in lst)
            {
                item.Turnover = item.Turnover == 0 ? null : item.Turnover;
                item.Vat = item.Vat == 0 ? null : item.Vat;
                item.TurnoverPersonal = item.TurnoverPersonal == 0 ? null : item.TurnoverPersonal;
                item.VatPersonal = item.VatPersonal == 0 ? null : item.VatPersonal;
                item.Ord = item.Ord == 0 ? null : item.Ord;
            }
            var reportResponse = new ReportResponseDto<TotalRevenueDto>();
            reportResponse.Data = lst.OrderBy(p => p.Sort).ThenBy(p => p.CareerCode).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
        private async Task<int?> GetTenantType()
        {
            var tenantExtendInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantExtendInfo == null) return null;
            return tenantExtendInfo.TenantType;
        }
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
