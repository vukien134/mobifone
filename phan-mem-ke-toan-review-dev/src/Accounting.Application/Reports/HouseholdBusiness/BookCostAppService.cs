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
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.HouseholdBusiness;
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
    public class BookCostAppService : AccountingAppService
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
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public BookCostAppService(ReportDataService reportDataService,
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
            _accSectionService = accSectionService;
            _productLotService = productLotService;
            _orgUnitService = orgUnitService;
            _ledgerService = ledgerService;
            _accPartnerAppService = accPartnerAppService;
            _productAppService = productAppService;
            _warehouseBookService = warehouseBookService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.BookCostHkdReportView)]
        public async Task<ReportResponseDto<BookCostDto>> CreateDataAsync(ReportRequestDto<BookCostParamaterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstSectitonCode = GetSplit(dto.Parameters.LstSection, '|');
            var accSection = await _accSectionService.GetQueryableAsync();
            var lstAccSection = accSection.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var ledger = await _ledgerService.GetQueryableAsync();
            string v154A = "";
            string v154B = "";
            string v154C = "";
            string v154D = "";
            string v154E = "";
            string v154F = "";
            string v154G = "";
            v154A = lstSectitonCode.Where(p => p.Id == 1).Select(p => p.Data).FirstOrDefault() ?? "";
            v154B = lstSectitonCode.Where(p => p.Id == 2).Select(p => p.Data).FirstOrDefault() ?? "";
            v154C = lstSectitonCode.Where(p => p.Id == 3).Select(p => p.Data).FirstOrDefault() ?? "";
            v154D = lstSectitonCode.Where(p => p.Id == 4).Select(p => p.Data).FirstOrDefault() ?? "";
            v154E = lstSectitonCode.Where(p => p.Id == 5).Select(p => p.Data).FirstOrDefault() ?? "";
            v154F = lstSectitonCode.Where(p => p.Id == 6).Select(p => p.Data).FirstOrDefault() ?? "";
            v154G = lstSectitonCode.Where(p => p.Id == 7).Select(p => p.Data).FirstOrDefault() ?? "";
            var data = ledger.Where(p => p.OrgCode == orgCode && String.Compare(p.Status, "2") < 0
                                           && p.VoucherDate >= dto.Parameters.FromDate && p.VoucherDate <= dto.Parameters.ToDate
                                           && (p.DebitAcc ?? "").StartsWith(dto.Parameters.AccCode)
                                        ).GroupBy(p => new
                                        {
                                            OrgCode = p.OrgCode,
                                            DocId = p.VoucherId,
                                            VoucherCode = p.VoucherCode,
                                            VoucherDate = p.VoucherDate,
                                            VoucherNumber = p.VoucherNumber,
                                            Description = p.Description,
                                            Note = p.Note,
                                            SectionCode = p.SectionCode
                                        }).Select(p => new BookCostDto
                                        {
                                            Sort = "B",
                                            Bold = "K",
                                            OrgCode = p.Key.OrgCode,
                                            SectionCode = p.Key.SectionCode,
                                            VoucherDate = p.Key.VoucherDate,
                                            VoucherNumber = p.Key.VoucherNumber,
                                            Description = p.Key.Description,
                                            Note = p.Key.Note,
                                            DocId = p.Key.DocId,
                                            VoucherId = p.Key.DocId,
                                            VoucherCode = p.Key.VoucherCode, 
                                            Ord0 = p.Max(p => p.Ord0),
                                            AmountCur = p.Sum(p => p.AmountCur ?? 0),
                                            Amount = p.Sum(p => p.Amount ?? 0),
                                            Amount154A = p.Sum(p => p.SectionCode == v154A ? p.Amount ?? 0 : 0),
                                            Amount154B = p.Sum(p => p.SectionCode == v154B ? p.Amount ?? 0 : 0),
                                            Amount154C = p.Sum(p => p.SectionCode == v154C ? p.Amount ?? 0 : 0),
                                            Amount154D = p.Sum(p => p.SectionCode == v154D ? p.Amount ?? 0 : 0),
                                            Amount154E = p.Sum(p => p.SectionCode == v154E ? p.Amount ?? 0 : 0),
                                            Amount154F = p.Sum(p => p.SectionCode == v154F ? p.Amount ?? 0 : 0),
                                            Amount154G = p.Sum(p => p.SectionCode == v154G ? p.Amount ?? 0 : 0),
                                        }).ToList();
            data.Add(new BookCostDto
            {
                Sort = "C",
                Bold = "C",
                OrgCode = _webHelper.GetCurrentOrgUnit(),
                Note = "Tổng cộng",
                AmountCur = data.Sum(p => p.AmountCur ?? 0),
                Amount = data.Sum(p => p.Amount ?? 0),
                Amount154A = data.Sum(p => p.SectionCode == v154A ? p.Amount ?? 0 : 0),
                Amount154B = data.Sum(p => p.SectionCode == v154B ? p.Amount ?? 0 : 0),
                Amount154C = data.Sum(p => p.SectionCode == v154C ? p.Amount ?? 0 : 0),
                Amount154D = data.Sum(p => p.SectionCode == v154D ? p.Amount ?? 0 : 0),
                Amount154E = data.Sum(p => p.SectionCode == v154E ? p.Amount ?? 0 : 0),
                Amount154F = data.Sum(p => p.SectionCode == v154F ? p.Amount ?? 0 : 0),
                Amount154G = data.Sum(p => p.SectionCode == v154G ? p.Amount ?? 0 : 0),
            });
            data.Add(new BookCostDto
            {
                Sort = "A",
                Bold = "C",
                OrgCode = _webHelper.GetCurrentOrgUnit(),
                Note = "Số phát sinh trong kỳ",
            });
            foreach (var item in data)
            {
                item.Amount154A = item.Amount154A == 0 ? null : item.Amount154A;
                item.Amount154B = item.Amount154B == 0 ? null : item.Amount154B;
                item.Amount154C = item.Amount154C == 0 ? null : item.Amount154C;
                item.Amount154D = item.Amount154D == 0 ? null : item.Amount154D;
                item.Amount154E = item.Amount154E == 0 ? null : item.Amount154E;
                item.Amount154F = item.Amount154F == 0 ? null : item.Amount154F;
                item.Amount154G = item.Amount154G == 0 ? null : item.Amount154G;
            }
            var reportResponse = new ReportResponseDto<BookCostDto>();
            reportResponse.Data = data.OrderBy(p => p.Sort).ThenBy(p => p.VoucherDate)
                                      .ThenBy(p => p.VoucherNumber).ThenBy(p => p.DocId)
                                      .ThenBy(p => p.Ord0).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.BookCostsHkdReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<BookCostParamaterDto> dto)
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
        private List<LstAccDto> GetSplit(string str, char spt) // lấy list
        {
            var lst = new List<LstAccDto>();
            var lstAcc = str.Split(spt).ToList();
            for (var i = 0; i < lstAcc.Count; i++)
            {
                lst.Add(new LstAccDto
                {
                    Id = i + 1,
                    Data = lstAcc[i],
                });
            }
            return lst;
        }
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
