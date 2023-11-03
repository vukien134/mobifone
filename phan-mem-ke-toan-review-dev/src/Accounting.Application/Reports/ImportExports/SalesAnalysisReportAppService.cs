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
    public class SalesAnalysisReportAppService : AccountingAppService
    {
        #region Private
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ProductService _productService;
        private readonly ProductGroupService _productGroupService;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public SalesAnalysisReportAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ProductService productService,
                        ProductGroupService productGroupService,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccountingCacheManager accountingCacheManager
                    )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _productService = productService;
            _productGroupService = productGroupService;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.SalesAnalysisReportView)]
        public async Task<ReportResponseDto<SummarySalesReportDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var warehouseData = await this.GetWarehousesDataAsync(dto.Parameters);
            var details = await this.GetDetailData(warehouseData);
            var groupData = await this.GetGroupData(details);
            var total = this.GetTotal(groupData);

            var result = new List<SummarySalesReportDto>();
            foreach(var group in groupData)
            {
                result.Add(group);
                var items = details.Where(p => p.ProductGroupCode == group.ProductGroupCode)
                                .OrderBy(p => p.ProductCode).ToList();
                result.AddRange(items);
            }
            result.Add(total);

            var reportResponse = new ReportResponseDto<SummarySalesReportDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.SalesAnalysisReportPrint)]
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
        private Dictionary<string, object> GetWarehouseParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.OrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            dic.Add(WarehouseBookParameterConst.BusinessCode, dto.BusinessCode);
            dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.ProductGroupCode);
            dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
            dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
            dic.Add(WarehouseBookParameterConst.PartnerGroup, dto.PartnerGroup);
            dic.Add(WarehouseBookParameterConst.LstVoucherCode, dto.LstVoucherCode);
            dic.Add(WarehouseBookParameterConst.CreditAcc2, dto.CreditAcc2);
            dic.Add(WarehouseBookParameterConst.DebitAcc, dto.DebitAcc);
            return dic;
        }
        private async Task<List<WarehouseBookGeneralDto>> GetWarehousesDataAsync(ReportBaseParameterDto dto)
        {
            var dic = this.GetWarehouseParameter(dto);
            var data = await _reportDataService.GetWarehouseBookData(dic);
            
            return data;
        }
        private async Task<List<SummarySalesReportDto>> GetDetailData(List<WarehouseBookGeneralDto> warehouseData)
        {
            var details = warehouseData.GroupBy(g => new
            {
                g.OrgCode,
                g.ProductCode,                
                g.UnitCode
            }).Select(p => new SummarySalesReportDto(){
                OrgCode = p.Key.OrgCode,
                ProductCode = p.Key.ProductCode,                
                UnitCode = p.Key.UnitCode,
                Quantity = p.Sum(s => s.ExportQuantity),
                Capital = p.Sum(s => s.ExportAmount),
                Revenue = p.Sum(s => s.Amount2),
                DiscountAmount = p.Where(s => s.VoucherGroup == 2).Sum(s => s.DiscountAmount),
                ReturnQuantity = p.Sum(s => s.ImportQuantity),
                ReturnAmount = p.Where(s => s.VoucherGroup == 1).Sum(s => s.Amount2 - s.DiscountAmount ?? 0),
                ReturnCapital = p.Where(s => s.VoucherGroup == 1).Sum(s => s.Amount),
                ImportAmount = p.Sum(s => s.ImportAmount),
                ExportAmount = p.Sum(s => s.ExportAmount)
            }).ToList();
            foreach(var item in details)
            {
                var product = await this.GetProduct(item.ProductCode, item.OrgCode);
                if (product == null) continue;
                item.ProductName = product.Name;
                item.ProductGroupCode = product.ProductGroupCode;
            }
            return details;
        }
        private async Task<List<SummarySalesReportDto>> GetGroupData(List<SummarySalesReportDto> dtos)
        {
            var groupData = dtos.GroupBy(g => new
            {
                g.OrgCode,
                g.ProductGroupCode                
            }).Select(p => new SummarySalesReportDto()
            {
                OrgCode = p.Key.OrgCode,
                ProductGroupCode = p.Key.ProductGroupCode,
                ProductCode = p.Key.ProductGroupCode,
                Quantity = p.Sum(s => s.Quantity),
                Capital = p.Sum(s => s.Capital),
                Revenue = p.Sum(s => s.Revenue),
                DiscountAmount = p.Sum(s => s.DiscountAmount),
                ReturnQuantity = p.Sum(s => s.ReturnQuantity),
                ReturnAmount = p.Sum(s => s.ReturnAmount),
                Bold = "C"
            }).OrderBy(p => p.ProductCode).ToList();
            foreach (var item in groupData)
            {
                var product = await this.GetProductGroup(item.ProductCode, item.OrgCode);
                if (product == null) continue;
                item.ProductName = product.Name;                
            }
            return groupData;
        }
        public SummarySalesReportDto GetTotal(List<SummarySalesReportDto> groupData)
        {
            var total = new SummarySalesReportDto()
            {
                Bold = "C",
                ProductName = "Tổng cộng",
                Quantity = groupData.Sum(s => s.Quantity),
                Capital = groupData.Sum(s => s.Capital),
                Revenue = groupData.Sum(s => s.Revenue),
                DiscountAmount = groupData.Sum(s => s.DiscountAmount),
                ReturnQuantity = groupData.Sum(s => s.ReturnQuantity),
                ReturnAmount = groupData.Sum(s => s.ReturnAmount),
            };
            return total;
        }
        private async Task<Product> GetProduct(string productCode,string orgCode)
        {
            var product = await _productService.GetByCodeAsync(productCode, orgCode);
            return product;
        }
        private async Task<ProductGroup> GetProductGroup(string productCode, string orgCode)
        {
            var group = await _productGroupService.GetByCodeAsync(productCode, orgCode);
            return group;
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
        #endregion
    }
}
