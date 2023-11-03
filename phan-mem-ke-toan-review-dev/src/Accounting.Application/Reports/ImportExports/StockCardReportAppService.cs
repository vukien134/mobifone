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
using Accounting.Reports.Cores;
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
    public class StockCardReportAppService : AccountingAppService
    {
        #region Privates
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly ProductService _productService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly CircularsService _circularsService;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly ProductGroupService _productGroupService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public StockCardReportAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        YearCategoryService yearCategoryService,
                        ProductService productService,
                        FProductWorkService fProductWorkService,
                        IWebHostEnvironment hostingEnvironment,
                        OrgUnitService orgUnitService,
                        TenantSettingService tenantSettingService,
                        CircularsService circularsService,
                        ReportTemplateService reportTemplateService,
                        ProductGroupService productGroupService,
                        AccountingCacheManager accountingCacheManager
                )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _yearCategoryService = yearCategoryService;
            _productService = productService;
            _fProductWorkService = fProductWorkService;
            _hostingEnvironment = hostingEnvironment;
            _orgUnitService = orgUnitService;
            _tenantSettingService = tenantSettingService;
            _circularsService = circularsService;
            _reportTemplateService = reportTemplateService;
            _productGroupService = productGroupService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.StockCardReportView)]
        public async Task<ReportResponseDto<StockCardDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var openingBalances = await this.GetOpeningBalance(dto.Parameters);
            var warehouseData = await this.GetWarehousesDataAsync(dto.Parameters);
            var incurredData = this.GetIncurredData(warehouseData);
            var balances = this.SummaryBalance(openingBalances, incurredData);
            var products = await this.GetProductsAsync(dto.Parameters);

            var result = new List<StockCardDto>();
            foreach (var item in balances)
            {
                string productName = this.GetProductName(products, item.ProductCode);
                var openGroup = new StockCardDto()
                {
                    Bold = "C",
                    ProductCode = item.ProductCode,
                    Description = $"{item.ProductCode} - {productName}",
                    ProductName = productName,
                    RemainingQuantity = item.OpeningQuantity
                };

                decimal remainingQuantity = openGroup.RemainingQuantity == null ? 0 : openGroup.RemainingQuantity.Value;
                var details = this.GetDetail(item.ProductCode, warehouseData);
                foreach (var detail in details)
                {
                    detail.ProductName = productName;
                    openGroup.UnitCode = detail.UnitCode;
                    decimal importQuantity = detail.ImportQuantity == null ? 0 : detail.ImportQuantity.Value;
                    decimal exportQuantity = detail.ExportQuantity == null ? 0 : detail.ExportQuantity.Value;
                    remainingQuantity = remainingQuantity + importQuantity - exportQuantity;
                    detail.RemainingQuantity = remainingQuantity;
                }

                result.Add(openGroup);
                result.AddRange(details);

                var endGroup = new StockCardDto()
                {
                    Bold = "C",
                    ProductCode = item.ProductCode,
                    Description = $"Tổng cộng: {item.ProductCode} - {productName}",
                    RemainingQuantity = remainingQuantity,
                    ImportQuantity = details.Sum(p => p.ImportQuantity),
                    ExportQuantity = details.Sum(p => p.ExportQuantity)
                };
                result.Add(endGroup);
                endGroup = new StockCardDto()
                {
                    Bold = "C",
                    ProductCode = item.ProductCode,
                    Description = $"---------------------"
                };
                result.Add(endGroup);
            }

            var reportResponse = new ReportResponseDto<StockCardDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.StockCardReportPrint)]
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
        private List<ProductBalanceDto> SummaryBalance(List<ProductBalanceDto> openingBalance, List<ProductBalanceDto> incurredData)
        {
            var balances = openingBalance.Select(p => new ProductBalanceDto()
            {
                OrgCode = p.OrgCode,
                ProductCode = p.ProductCode,
                ProductLotCode = p.ProductLotCode,
                ProductOriginCode = p.ProductOriginCode,
                WarehouseCode = p.WarehouseCode,
                OpeningQuantity = p.BalanceQuantity,
                OpeningAmount = p.BalanceAmount,
                OpeningAmountCur = p.OpeningAmountCur
            }).ToList();
            balances.AddRange(incurredData);

            return balances.GroupBy(g => new
            {
                g.OrgCode,
                g.WarehouseCode,                
                g.ProductCode,
                g.ProductLotCode,
                g.ProductOriginCode
            }).Select(p => new ProductBalanceDto()
            {
                WarehouseCode = p.Key.WarehouseCode,
                OrgCode = p.Key.OrgCode,                
                ProductCode = p.Key.ProductCode,
                ProductLotCode = p.Key.ProductLotCode,
                ProductOriginCode = p.Key.ProductOriginCode,
                OpeningQuantity = p.Sum(s => s.OpeningQuantity),
                OpeningAmount = p.Sum(s => s.OpeningAmount),
                OpeningAmountCur = p.Sum(s => s.OpeningAmountCur),
                ImportQuantity = p.Sum(s => s.ImportQuantity),
                ImportAmount = p.Sum(s => s.ImportAmount),
                ImportAmountCur = p.Sum(s => s.ImportAmountCur),
                ExportQuantity = p.Sum(s => s.ExportQuantity),
                ExportAmount = p.Sum(s => s.ExportAmount),
                ExportAmountCur = p.Sum(s => s.ExportAmountCur)
            }).OrderBy(p => p.ProductCode).ToList();
        }
        private List<StockCardDto> GetDetail(string productCode, List<WarehouseBookGeneralDto> warehouseData)
        {
            var queryable = warehouseData.AsQueryable();
            var query = queryable.Where(p => p.ProductCode == productCode)
                            .Select(p => new StockCardDto()
                            {

                                VoucherDate = p.VoucherDate,
                                VoucherId = p.VoucherId,
                                VoucherCode = p.VoucherCode,
                                VoucherGroup = p.VoucherGroup,
                                ImportVoucherNumber = p.VoucherGroup == 1 ? p.VoucherNumber : null,
                                ExportVoucherNumber = p.VoucherGroup == 2 ? p.VoucherNumber : null,
                                VoucherNumber = p.VoucherNumber,
                                Description = p.Description,
                                ImportQuantity = p.ImportQuantity,
                                ExportQuantity = p.ExportQuantity,
                                FProductWorkCode = p.FProductWorkCode,
                                ProductCode = p.ProductCode,
                                UnitCode = p.UnitCode,
                                OrgCode = p.OrgCode
                            }).OrderBy(p => p.VoucherDate)
                                .ThenBy(p => p.VoucherGroup)
                                .ThenBy(p => p.VoucherNumber);
            return query.ToList();
        }
        private async Task<List<WarehouseBookGeneralDto>> GetWarehousesDataAsync(ReportBaseParameterDto dto)
        {
            var dic = this.GetParameterIncurred(dto);
            var data = await _reportDataService.GetWarehouseBookData(dic);
            return data;
        }
        private async Task<List<ProductBalanceDto>> GetOpeningBalance(ReportBaseParameterDto dto)
        {
            var dict = await this.GetParameterOpeningBalance(dto);
            var result = await _reportDataService.GetProductBalancesAsync(dict);
            return result;
        }
        private List<ProductBalanceDto> GetIncurredData(List<WarehouseBookGeneralDto> warehouseData)
        {
            return warehouseData.GroupBy(g => new
            {
                g.OrgCode,
                g.WarehouseCode,
                g.AccCode,
                g.ProductCode,
                g.ProductLotCode,
                g.ProductOriginCode
            }).Select(p => new ProductBalanceDto()
            {
                WarehouseCode = p.Key.WarehouseCode,
                OrgCode = p.Key.OrgCode,
                AccCode = p.Key.AccCode,
                ProductCode = p.Key.ProductCode,
                ProductLotCode = p.Key.ProductLotCode,
                ProductOriginCode = p.Key.ProductOriginCode,
                ImportQuantity = p.Sum(s => s.ImportQuantity),
                ImportAmount = p.Sum(s => s.ImportAmount),
                ImportAmountCur = p.Sum(s => s.ImportAmountCur),
                ExportQuantity = p.Sum(s => s.ExportQuantity),
                ExportAmount = p.Sum(s => s.ExportAmount),
                ExportAmountCur = p.Sum(s => s.ExportAmountCur)
            }).ToList();
        }
        private async Task<Dictionary<string, object>> GetParameterOpeningBalance(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();

            string orgCode = _webHelper.GetCurrentOrgUnit();
            dic.Add(WarehouseBookParameterConst.OrgCode, orgCode);

            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, dto.FromDate.Value);
            if (yearCategory != null)
            {
                dic.Add(WarehouseBookParameterConst.OpenYear, yearCategory.Year);
            }

            dic.Add(WarehouseBookParameterConst.FromDate, yearCategory.BeginDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.FromDate.Value.AddDays(-1));
            dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            dic.Add(WarehouseBookParameterConst.Acc1, dto.AccCode);
            dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.ProductGroupCode);
            dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.ProductLotCode);
            dic.Add(WarehouseBookParameterConst.ProductOriginCode, dto.ProductOriginCode);

            return dic;
        }
        private Dictionary<string, object> GetParameterIncurred(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = new Dictionary<string, object>();

            dic.Add(WarehouseBookParameterConst.OrgCode, orgCode);
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            dic.Add(WarehouseBookParameterConst.Acc1, dto.AccCode);
            dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.ProductGroupCode);
            dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.ProductLotCode);
            dic.Add(WarehouseBookParameterConst.ProductOriginCode, dto.ProductOriginCode);

            return dic;
        }
        private string GetProductName(List<Product> products, string code)
        {
            if (products.Count == 0) return null;
            var product = products.Where(p => p.Code.Equals(code)).FirstOrDefault();
            if (product == null) return null;
            return product.Name;
        }
        private async Task<string> GetFProducWorkName(string code, string orgCode)
        {
            var product = await _fProductWorkService.GetByFProductWorkAsync(code, orgCode);
            if (product == null) return "";
            return product.Name;
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
        private async Task<List<Product>> GetProductsAsync(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var queryableProduct = await _productService.GetQueryableAsync();
            queryableProduct = queryableProduct.Where(p => p.OrgCode.Equals(orgCode));
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                queryableProduct = queryableProduct.Where(p => p.Code.Equals(dto.ProductCode));
            }            
            var queryableProductGroup = await _productGroupService.GetQueryableAsync();
            queryableProductGroup = queryableProductGroup.Where(p => p.OrgCode.Equals(orgCode));
            if (!string.IsNullOrEmpty(dto.ProductGroupId))
            {
                queryableProductGroup = queryableProductGroup.Where(p => p.ParentId.Equals(dto.ProductGroupId));
            }

            var query = from p in queryableProduct
                        join c in queryableProductGroup on p.ProductGroupId equals c.Id
                        select new Product()
                        {
                            Code = p.Code,
                            Name = p.Name
                        };
            return await AsyncExecuter.ToListAsync(query);
        }
        #endregion
    }
}
