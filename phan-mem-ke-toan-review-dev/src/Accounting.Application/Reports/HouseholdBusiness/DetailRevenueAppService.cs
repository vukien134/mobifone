using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.HouseholdBusiness;
using Accounting.Reports.ImportExports.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.ImportExports
{
    public class DetailRevenueAppService : AccountingAppService
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
        private readonly ProductLotService _productLotService;
        private readonly OrgUnitService _orgUnitService;        
        private readonly AccPartnerAppService _accPartnerAppService;        
        private readonly ProductAppService _productAppService;        
        private readonly WarehouseBookService _warehouseBookService;        
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public DetailRevenueAppService(ReportDataService reportDataService,
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
                        ProductLotService productLotService,
                        OrgUnitService orgUnitService,
                        AccPartnerAppService accPartnerAppService,
                        ProductAppService productAppService,
                        WarehouseBookService warehouseBookService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager
                        )
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
            _productLotService = productLotService;
            _orgUnitService = orgUnitService;
            _accPartnerAppService = accPartnerAppService;
            _productAppService = productAppService;
            _warehouseBookService = warehouseBookService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.DetailRevenueHkdReportView)]
        public async Task<ReportResponseDto<DetailRevenueDto>> CreateDataAsync(ReportRequestDto<TotalRevenueParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstPartner = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroupCode ?? "");
            var lstProduct = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode ?? "");
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherType = voucherType.Where(p => p.Code == "PBH").FirstOrDefault();
            var lst = new List<TotalRevenueDto>();
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
                                                  .Select(p => new TotalRevenueDataWarehouseBookDto
                                                  {
                                                      OrgCode = p.OrgCode,
                                                      OrdRec0 = p.Ord0,
                                                      VoucherCode = p.VoucherCode,
                                                      VoucherId = p.ProductVoucherId,
                                                      VoucherNumber = p.VoucherNumber,
                                                      VoucherDate = p.VoucherDate,
                                                      PartnerCode = p.PartnerCode0,
                                                      Description = p.Description,
                                                      CurrencyCode = p.CurrencyCode,
                                                      ExchangeRate = p.ExchangeRate,
                                                      WarehouseCode = p.WarehouseCode,
                                                      ProductCode = p.ProductCode,
                                                      UnitCode = p.UnitCode,
                                                      Quantity = (p.VoucherGroup == 1 ? -1 : 1)*p.Quantity,
                                                      Price = (p.VoucherGroup == 4 ? p.Price : p.Price2),
                                                      PriceCur = (p.VoucherGroup == 4 ? p.PriceCur : p.PriceCur2),
                                                      Amount = (p.VoucherGroup == 4 ? p.Amount : (p.VoucherGroup == 1 ? -1 : 1)*p.TotalAmount2),
                                                      AmountCur = (p.VoucherGroup == 4 ? p.AmountCur : (p.VoucherGroup == 1 ? -1 : 1)*p.AmountCur2),
                                                      DebitAcc = (p.VoucherGroup == 4 ? p.DebitAcc : p.DebitAcc2),
                                                      CreditAcc = (p.VoucherGroup == 4 ? p.CreditAcc : p.CreditAcc2),
                                                      ProductName0 = p.ProductName0,
                                                      AmountDecrease = p.TotalAmount2 - p.Amount2,
                                                  }).ToList();
            var data = (from a in dataWarehouseBook
                        join b in lstProduct on a.ProductCode equals b.Code
                        select new DetailRevenueDto
                        {
                            Sort = "A",
                            Bold = "K",
                            ProductName = (a.ProductName0 ?? "") == "" ? b.Name : a.ProductName0,
                            Note = b.Code + " - " + (a.ProductName0 ?? "") == "" ? b.Name : a.ProductName0,
                            CareerCode = a.CareerCode,
                            DocId = a.DocId,
                            OrdRec0 = a.OrdRec0,
                            VoucherCode = a.VoucherCode,
                            VoucherNumber = a.VoucherNumber,
                            VoucherDate = a.VoucherDate,
                            VoucherId = a.VoucherId,
                            PartnerCode = a.PartnerCode,
                            Description = a.Description,
                            CurrencyCode = a.CurrencyCode,
                            ExchangeRate = a.ExchangeRate,
                            WarehouseCode = a.WarehouseCode,
                            ProductCode = a.ProductCode,
                            UnitCode = a.UnitCode,
                            Quantity = a.Quantity,
                            Price = a.Price,
                            Amount = a.Amount,
                            AmountCur = a.AmountCur,
                            DebitAcc = a.DebitAcc,
                            CreditAcc = a.CreditAcc,
                            ProductName0 = a.ProductName0,
                            Incurred01 = (b.CareerCode == "01" && b.VatPercentage == 1) ? a.Amount : 0,
                            Incurred02 = (b.CareerCode == "01" && b.VatPercentage == 0) ? a.Amount : 0,
                            Incurred03 = (b.CareerCode == "02" && b.VatPercentage == 5 && b.PITPercentage == 2) ? a.Amount : 0,
                            Incurred04 = (b.CareerCode == "02" && b.VatPercentage == 0 && b.PITPercentage == 2) ? a.Amount : 0,
                            Incurred05 = (b.CareerCode == "02" && b.VatPercentage == 5 && b.PITPercentage == 5) ? a.Amount : 0,
                            Incurred06 = (b.CareerCode == "02" && b.VatPercentage == 0 && b.PITPercentage == 5) ? a.Amount : 0,
                            Incurred07 = (b.CareerCode == "03" && b.VatPercentage == 3) ? a.Amount : 0,
                            Incurred08 = (b.CareerCode == "03" && b.VatPercentage == 0) ? a.Amount : 0,
                            Incurred09 = (b.CareerCode == "04") ? a.Amount : 0,
                        }).ToList();
            var dataTotal = new DetailRevenueDto
                            {
                                Sort = "B",
                                Bold = "C",
                                Note = "Tổng Cộng",
                                VoucherDate = null,
                                AmountCur = data.Sum(p => p.AmountCur ?? 0),
                                Amount = data.Sum(p => p.Amount ?? 0),
                                Incurred01 = data.Sum(p => p.Incurred01 ?? 0),
                                Incurred02 = data.Sum(p => p.Incurred02 ?? 0),
                                Incurred03 = data.Sum(p => p.Incurred03 ?? 0),
                                Incurred04 = data.Sum(p => p.Incurred04 ?? 0),
                                Incurred05 = data.Sum(p => p.Incurred05 ?? 0),
                                Incurred06 = data.Sum(p => p.Incurred06 ?? 0),
                                Incurred07 = data.Sum(p => p.Incurred07 ?? 0),
                                Incurred08 = data.Sum(p => p.Incurred08 ?? 0),
                                Incurred09 = data.Sum(p => p.Incurred09 ?? 0)
                            };
            data.Add(dataTotal);
            data.Add(new DetailRevenueDto
            {
                Sort = "C",
                Bold = dataTotal.Bold,
                Note = "Thuế GTGT ước tính",
                AmountCur = 0,
                Amount = 0,
                VoucherDate = null,
                Incurred01 = Math.Round((dataTotal.Incurred01 * 1/100) ?? 0),
                Incurred02 = 0,
                Incurred03 = Math.Round((dataTotal.Incurred03 * 5/100) ?? 0),
                Incurred04 = 0,
                Incurred05 = Math.Round((dataTotal.Incurred05 * 5/100) ?? 0),
                Incurred06 = 0,
                Incurred07 = Math.Round((dataTotal.Incurred07 * 3/100) ?? 0),
                Incurred08 = 0,
                Incurred09 = Math.Round((dataTotal.Incurred09 * 2/100) ?? 0),
            });
            data.Add(new DetailRevenueDto
            {
                Sort = "D",
                Bold = dataTotal.Bold,
                Note = "Thuế TNCN ước tính",
                AmountCur = 0,
                Amount = 0,
                VoucherDate = null,
                Incurred01 = Math.Round((dataTotal.Incurred01 * (decimal)0.5 / 100) ?? 0),
                Incurred02 = Math.Round((dataTotal.Incurred02 * (decimal)0.5 / 100) ?? 0),
                Incurred03 = Math.Round((dataTotal.Incurred03 * 2 / 100) ?? 0),
                Incurred04 = Math.Round((dataTotal.Incurred04 * 2 / 100) ?? 0),
                Incurred05 = Math.Round((dataTotal.Incurred05 * 5 / 100) ?? 0),
                Incurred06 = Math.Round((dataTotal.Incurred06 * 5 / 100) ?? 0),
                Incurred07 = Math.Round((dataTotal.Incurred07 * (decimal)1.5 / 100) ?? 0),
                Incurred08 = Math.Round((dataTotal.Incurred08 * (decimal)1.5 / 100) ?? 0),
                Incurred09 = Math.Round((dataTotal.Incurred09 * 1 / 100) ?? 0),
            });
            foreach (var item in data)
            {
                if ("C,D".StartsWith(item.Sort))
                {
                    item.Amount = item.Incurred01 + item.Incurred02 + item.Incurred03 + item.Incurred04 + item.Incurred05
                                + item.Incurred06 + item.Incurred07 + item.Incurred08 + item.Incurred09;
                }
                item.Amount = item.Amount == 0 ? null : item.Amount;
                item.Incurred01 = item.Incurred01 == 0 ? null : item.Incurred01;
                item.Incurred02 = item.Incurred02 == 0 ? null : item.Incurred02;
                item.Incurred03 = item.Incurred03 == 0 ? null : item.Incurred03;
                item.Incurred04 = item.Incurred04 == 0 ? null : item.Incurred04;
                item.Incurred05 = item.Incurred05 == 0 ? null : item.Incurred05;
                item.Incurred06 = item.Incurred06 == 0 ? null : item.Incurred06;
                item.Incurred07 = item.Incurred07 == 0 ? null : item.Incurred07;
                item.Incurred08 = item.Incurred08 == 0 ? null : item.Incurred08;
                item.Incurred09 = item.Incurred09 == 0 ? null : item.Incurred09;
            }
            var reportResponse = new ReportResponseDto<DetailRevenueDto>();
            reportResponse.Data = data.OrderBy(p => p.Sort).ThenBy(p => p.VoucherDate)
                                      .ThenBy(p => p.VoucherNumber).ThenBy(p => p.DocId)
                                      .ThenBy(p => p.OrdRec0).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.DetailRevenueHkdReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<TotalRevenueParameterDto> dto)
        {
            var dataSource = await CreateDataAsync(dto);
            var currencyFormats = await _accountingCacheManager.GetCurrencyFormats(_webHelper.GetCurrentOrgUnit());
            var reportTemplate = await _reportTemplateService.GetByCodeAsync(dto.ReportTemplateCode);
            string fileTemplate = reportTemplate.FileTemplate.Replace(".xml", "");
            fileTemplate = fileTemplate + "_" + dto.VndNt;
            if (!File.Exists(GetFileTemplatePath(fileTemplate + ".xml")))
            {
                throw new Exception("Không tìm thấy mẫu in!");
            }
            var renderOption = new RenderOption()
            {
                DataSource = dataSource,
                TypePrint = dto.Type,
                TemplateFile = GetFileTemplatePath(fileTemplate + ".xml"),
                CurrencyFormats = currencyFormats
            };
            var render = new ReportRender(renderOption);
            var result = render.Execute();

            return new FileContentResult(result, MIMETYPE.GetContentType(dto.Type.ToLower()))
            {
                FileDownloadName = $"{fileTemplate}.{dto.Type}"
            };
        }
        #endregion
        #region Private
        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return ObjectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
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
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return ObjectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        private async Task<string> GetPartnerName(string orgCode, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var partner = await _accPartnerService.GetAccPartnerByCodeAsync(code, orgCode);
            if (partner == null) return null;

            return partner.Name;
        }
        #endregion
    }
}
