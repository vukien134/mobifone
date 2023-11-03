using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
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
using Accounting.Reports.ImportExports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Costs
{
    public class ProductQuotaReportAppService : AccountingAppService
    {
        #region Fields
        private readonly FProductWorkNormService _fProductWorkNormService;
        private readonly FProductWorkNormDetailService _fProductWorkNormDetailService;
        private readonly WebHelper _webHelper;
        private readonly FProductWorkService _fProductWorkService;
        private readonly ProductService _productService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ProductQuotaReportAppService(FProductWorkNormService fProductWorkNormService,
                        FProductWorkNormDetailService fProductWorkNormDetailService,
                        WebHelper webHelper,
                        FProductWorkService fProductWorkService,
                        ProductService productService,
                        YearCategoryService yearCategoryService,
                        CircularsService circularsService,
                        OrgUnitService orgUnitService,
                        TenantSettingService tenantSettingService,
                        IWebHostEnvironment hostingEnvironment,
                        ReportTemplateService reportTemplateService,
                        AccountingCacheManager accountingCacheManager
                )
        {
            _fProductWorkNormService = fProductWorkNormService;
            _fProductWorkNormDetailService = fProductWorkNormDetailService;
            _webHelper = webHelper;
            _fProductWorkService = fProductWorkService;
            _productService = productService;
            _yearCategoryService = yearCategoryService;
            _circularsService = circularsService;
            _orgUnitService = orgUnitService;
            _tenantSettingService = tenantSettingService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.ProductQuotaReportView)]
        public async Task<ReportResponseDto<ProductQuotaDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var productWorkNorms = await this.GetQueryableFProductWorkNormAsync(dto.Parameters);

            var result = new List<ProductQuotaDto>();
            foreach(var item in productWorkNorms)
            {
                var product = await this.GetProductAsync(item.FProductWorkCode);
                var productQuotaDto = new ProductQuotaDto()
                {
                    Bold = "C",
                    Code = item.FProductWorkCode,
                    Name = product.Name,
                    UnitCode = product.UnitCode,
                    Quantity = item.Quantity,
                    FProductWorkCode = item.FProductWorkCode
                };
                result.Add(productQuotaDto);

                var details = await this.GetFProductWorkNormDetais(dto.Parameters, item.Id);
                foreach(var detail in details)
                {
                    var productDetail = await this.GetProductAsync(detail.ProductCode);
                    var productDetailQuotaDto = new ProductQuotaDto()
                    {
                        Code = detail.ProductCode,
                        Name = productDetail?.Name ?? "",
                        UnitCode = productDetail?.UnitCode ?? "",
                        Quantity = detail.Quantity,
                        Price = detail.Price,
                        Amount = detail.Amount,
                        AccCode = detail.AccCode,
                        FromDate = detail.BeginDate,
                        ToDate = detail.EndDate,
                        Month = detail.Month,
                        FProductWorkCode = item.FProductWorkCode
                    };
                    result.Add(productDetailQuotaDto);
                }
            }

            var reportResponse = new ReportResponseDto<ProductQuotaDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.ProductQuotaReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
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
        #region Privates
        private async Task<IQueryable<FProductWorkNormDetail>> GetQueryableFProductWorkNormDetailAsync(ReportBaseParameterDto dto,string id)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            int fromMonth = dto.FromDate.Value.Month;
            int toMonth = dto.ToDate.Value.Month;
            var queryable = await _fProductWorkNormDetailService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Year == year
                                    && p.FProductWorkNormId == id
                                    && p.Month>= fromMonth && p.Month <= toMonth);
            queryable = queryable.OrderBy(p => p.ProductCode)
                                .ThenBy(p => p.Month);
            return queryable;
        }
        private async Task<IQueryable<FProductWorkNorm>> GetQueryableFProductWorkNormAsync(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var queryable = await _fProductWorkNormService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Year == year);
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                queryable = queryable.Where(p => p.FProductWorkCode == dto.FProductWorkCode);
            }
            queryable = queryable.OrderBy(p => p.FProductWorkCode);
            return queryable;
        }
        private async Task<List<FProductWorkNorm>> GetFProductWorkNorms(ReportBaseParameterDto dto)
        {
            var queryable = await this.GetQueryableFProductWorkNormAsync(dto);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private async Task<List<FProductWorkNormDetail>> GetFProductWorkNormDetais(ReportBaseParameterDto dto,string id)
        {
            var queryable = await this.GetQueryableFProductWorkNormDetailAsync(dto, id);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private async Task<Product> GetProductAsync(string code)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var queryable = await _productService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Code == code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
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
        private async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return ObjectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        #endregion
    }
}
