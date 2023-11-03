using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;

using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.GeneralDiaries;
using Accounting.Vouchers.Ledgers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Products
{
    public class ProductDetailBookAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly CircularsService _circularsService;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ProductDetailBookAppService(ReportDataService reportDataService,
                            WebHelper webHelper,
                            YearCategoryService yearCategoryService,
                            OrgUnitService orgUnitService,
                            TenantSettingService tenantSettingService,
                            CircularsService circularsService,
                            ReportTemplateService reportTemplateService,
                            IWebHostEnvironment webHostEnvironment,
                            AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _yearCategoryService = yearCategoryService;
            _orgUnitService = orgUnitService;
            _tenantSettingService = tenantSettingService;
            _circularsService = circularsService;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = webHostEnvironment;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        public async Task<ReportResponseDto<ProductDetailBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var dic = GetParameters(dto);
            var productOpenings = await GetProductOpeningBalance(dic);
            var incurredData = await GetIncurredData(dic);
            var lst = new List<ProductDetailBookDto>();
            foreach (var item in incurredData)
            {
                item.Sort0 = 1;
            }
            var groupSumProduct = incurredData.GroupBy(g => new { g.OrgCode, g.WarehouseCode, g.ProductLotCode, g.ProductOrigin, g.ProductCode })
                                   .Select(p => new ProductDetailBookDto()
                                   {
                                       OrgCode = p.Key.OrgCode,
                                       AccCode = p.Max(p => p.AccCode),
                                       WarehouseCode = p.Key.WarehouseCode,
                                       ProductLotCode = p.Key.ProductLotCode,
                                       ProductOrigin = p.Key.ProductOrigin,
                                       ProductCode = p.Key.ProductCode,
                                       ImportQuantity1 = p.Max(p => p.VoucherGroup) == 1 ? p.Sum(x => x.ImportQuantity) : -1 * p.Sum(p => p.ExportQuantity),
                                       ImportAmount1 = p.Max(p => p.VoucherGroup) == 1 ? p.Sum(x => x.ImportAmount) : -1 * p.Sum(p => p.ExportAmount),
                                       ImportAmountCur1 = p.Sum(x => x.ImportAmountCur),
                                       ImportQuantity = p.Sum(p => p.ImportQuantity),
                                       ImportAmount = p.Sum(p => p.ImportAmount),
                                       ImportAmountCur = p.Sum(p => p.ImportAmountCur),
                                       ExportQuantity = p.Sum(p => p.ExportQuantity),
                                       ExportAmount = p.Sum(p => p.ExportAmount),
                                       ExportAmountCur = p.Sum(p => p.ExportAmountCur),
                                       ImportAmount2 = p.Max(p => p.VoucherGroup) == 1 ? p.Sum(x => x.ImportQuantity) : -1 * p.Sum(p => p.ExportQuantity),
                                       ImportAmountCur2 = p.Max(p => p.VoucherGroup) == 1 ? p.Sum(x => x.ImportAmount) : -1 * p.Sum(p => p.ExportAmount),
                                       ImportQuantity2 = p.Max(p => p.VoucherGroup) == 1 ? p.Sum(x => x.ImportQuantity) : -1 * p.Sum(p => p.ExportQuantity),
                                       Sort0 = 0,

                                   });
            //decimal? importQuantity1 = groupSumProduct.Sum(p => p.ImportQuantity1);
            //decimal? importAmountCur1 = groupSumProduct.Sum(p => p.ImportAmountCur1);
            //decimal? importAmount1 = groupSumProduct.Sum(p => p.ImportAmount1);
            //decimal? importQuantity2 = groupSumProduct.Sum(p => p.ImportQuantity2);
            //decimal? importAmount2 = groupSumProduct.Sum(p => p.ImportAmount2);
            //decimal? importAmountCur2 = groupSumProduct.Sum(p => p.ImportAmountCur2);
            //decimal? importQuantity = groupSumProduct.Sum(p => p.ImportQuantity);
            //decimal? importAmount = groupSumProduct.Sum(p => p.ImportAmount);
            //decimal? importAmountCur = groupSumProduct.Sum(p => p.ImportAmountCur);
            //decimal? exportQuantity = groupSumProduct.Sum(p => p.ExportQuantity);
            //decimal? exportAmountCur = groupSumProduct.Sum(p => p.ExportAmountCur);
            //decimal? exportAmount = groupSumProduct.Sum(p => p.ExportAmount);
            //string productCode = groupSumProduct.Max(p => p.ProductCode);
            //string warehouseCode = groupSumProduct.Max(p => p.WarehouseCode);
            var listSum = groupSumProduct.ToList();
            for (int i = 0; i < listSum.Count; i++)
            {
                incurredData.Add(new ProductDetailBookDto()
                {
                    Sort0 = 2,
                    Note = "Tổng cộng " + listSum[i].ProductCode,
                    ProductCode = listSum[i].ProductCode,
                    WarehouseCode = listSum[i].WarehouseCode,
                    ImportQuantity = listSum[i].ImportQuantity,
                    ImportAmount = listSum[i].ImportAmount,
                    ImportAmountCur = listSum[i].ImportAmountCur,
                    ExportQuantity = listSum[i].ExportQuantity,
                    ExportAmountCur = listSum[i].ExportAmountCur,
                    ExportAmount = listSum[i].ExportAmount,
                    RemainingAmount = listSum[i].ImportAmount2,
                    RemainingAmountCur = listSum[i].ImportAmountCur2,
                    RemainingQuantity = listSum[i].ImportQuantity2,
                    Bold = "C",

                });
                incurredData.Add(new ProductDetailBookDto()
                {
                    Sort0 = 3,
                    ProductCode = listSum[i].ProductCode,
                    Note = "-------------------------------",
                    Bold = "C"
                });
            }
            var unionData = incurredData.Union(groupSumProduct);

            var result = unionData.OrderBy(p => p.ProductCode)
                             .ThenBy(p => p.Sort0)
                             .ThenBy(p => p.WarehouseCode)
                             .ToList();


            int row = 0;
            //lst.Add(new ProductDetailBookDto()
            //{
            //    Bold = "C",
            //    Note = "",

            //});
            //foreach (var item in incurredData)
            //{
            //    item.ProductCode 
            //}
            lst.AddRange(result);
            var reportResponse = new ReportResponseDto<ProductDetailBookDto>();
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }

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
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, FolderConst.Report,
                                        templateFile);
            return filePath;
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
        private async Task<List<ProductDetailBookDto>> GetProductOpeningBalance(Dictionary<string, object> dic)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime fromDate = Convert.ToDateTime(dic[WarehouseBookParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            if (!dic.ContainsKey(WarehouseBookParameterConst.Year))
            {
                dic.Add(WarehouseBookParameterConst.Year, yearCategory.Year);
            }
            dic[WarehouseBookParameterConst.Year] = yearCategory.Year;
            dic[WarehouseBookParameterConst.ToDate] = fromDate.AddDays(-1);
            dic[WarehouseBookParameterConst.FromDate] = yearCategory.BeginDate;
            var productBalances = await _reportDataService.GetProductBalancesAsync(dic);
            return productBalances.Select(p => new ProductDetailBookDto()
            {
                ProductCode = p.ProductCode,
                Description = p.ProductCode,
                RemainingQuantity = p.BalanceQuantity,
                RemainingAmount = p.BalanceAmount,
                RemainingAmountCur = p.BalanceAmountCur
            }).ToList();
        }
        private async Task<List<ProductDetailBookDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBooks = await GetDataWarehouseBook(dic);
            var incurredDatas = warehouseBooks.Select(p => new ProductDetailBookDto()
            {
                ProductCode = p.ProductCode,
                VoucherDate = p.VoucherDate,
                VoucherNumber = p.VoucherNumber,
                Description = p.Description,
                ImportAmount = p.ImportAmount,
                ImportAmountCur = p.ImportAmountCur,
                ImportQuantity = p.ImportQuantity,
                ExportAmount = p.ExportAmount,
                ExportAmountCur = p.ExportAmountCur,
                ExportQuantity = p.ExportQuantity,
                AccCode = p.AccCode,
                ProductLotCode = p.ProductLotCode,
                ProductOrigin = p.ProductOriginCode,
                WarehouseCode = p.WarehouseCode,


            });
            return incurredDatas.ToList();
        }
        private async Task<List<WarehouseBookGeneralDto>> GetDataWarehouseBook(Dictionary<string, object> dic)
        {
            var result = await _reportDataService.GetWarehouseBookData(dic);
            return result;
        }
        private Dictionary<string, object> GetParameters(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.OrgCode, orgCode);
            dic.Add(WarehouseBookParameterConst.FromDate, dto.Parameters.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.Parameters.ToDate);

            if (!string.IsNullOrEmpty(dto.Parameters.AccCode))
                dic.Add(WarehouseBookParameterConst.Acc, dto.Parameters.AccCode);
            if (!string.IsNullOrEmpty(dto.Parameters.CurrencyCode))
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.Parameters.CurrencyCode);
            if (!string.IsNullOrEmpty(dto.Parameters.WarehouseCode))
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.Parameters.WarehouseCode);
            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
                dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.Parameters.ProductGroupCode);
            if (!string.IsNullOrEmpty(dto.Parameters.ProductCode))
                dic.Add(WarehouseBookParameterConst.ProductCode, dto.Parameters.ProductCode);
            if (!string.IsNullOrEmpty(dto.Parameters.ProductLotCode))
                dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.Parameters.ProductLotCode);
            if (!string.IsNullOrEmpty(dto.Parameters.ProductOriginCode))
                dic.Add(WarehouseBookParameterConst.ProductOrigin, dto.Parameters.ProductOriginCode);
            return dic;
        }
        #endregion
    }

}