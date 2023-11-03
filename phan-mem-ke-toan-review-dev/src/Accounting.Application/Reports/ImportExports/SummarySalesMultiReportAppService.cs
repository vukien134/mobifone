using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.BusinessCategories;
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
    public class SummarySalesMultiReportAppService : AccountingAppService
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
        private readonly AccPartnerService _partnerService;
        private readonly DepartmentService _departmentService;
        private readonly AccSectionService _accSectionService;
        private readonly AccCaseService _accCaseService;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public SummarySalesMultiReportAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ProductService productService,
                        ProductGroupService productGroupService,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccPartnerService partnerService,
                        DepartmentService departmentService,
                        AccSectionService accSectionService,
                        AccCaseService accCaseService,
                        BusinessCategoryService businessCategoryService,
                        FProductWorkService fProductWorkService,
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
            _partnerService = partnerService;
            _departmentService = departmentService;
            _accSectionService = accSectionService;
            _accCaseService = accCaseService;
            _businessCategoryService = businessCategoryService;
            _fProductWorkService = fProductWorkService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.SummarySalesMultiReportView)]
        public async Task<ReportResponseDto<SummarySalesReportDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var warehouseData = await this.GetWarehousesDataAsync(dto.Parameters);
            var groupData1 = await this.GroupData1(warehouseData, dto.Parameters.Group1);

            var result = new List<SummarySalesReportDto>();
            foreach (var group1 in groupData1)
            {
                result.Add(group1);
                var groupData2 = await this.GroupData2(warehouseData, dto.Parameters.Group1,
                                            group1.Code, dto.Parameters.Group2);
                foreach (var group2 in groupData2)
                {
                    result.Add(group2);
                    var details = await this.GetDetailData(warehouseData, dto.Parameters.Group2, group2.Code);
                    result.AddRange(details);
                }
            }
            var total = this.GetTotal(groupData1);
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

        [Authorize(ReportPermissions.SummarySalesMultiReportPrint)]
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
            dic.Add(WarehouseBookParameterConst.FProductWorkCode, dto.FProductWorkCode);
            dic.Add(WarehouseBookParameterConst.CaseCode, dto.CaseCode);
            dic.Add(WarehouseBookParameterConst.SectionCode, dto.SectionCode);
            dic.Add(WarehouseBookParameterConst.Acc1, dto.AccCode);
            dic.Add(WarehouseBookParameterConst.ReciprocalAcc, dto.ReciprocalAcc);
            return dic;
        }
        private async Task<List<WarehouseBookGeneralDto>> GetWarehousesDataAsync(ReportBaseParameterDto dto)
        {
            var dic = this.GetWarehouseParameter(dto);
            var data = await _reportDataService.GetWarehouseBookData(dic);
            return data;
        }
        private async Task<List<SummarySalesReportDto>> GetDetailData(List<WarehouseBookGeneralDto> warehouseData, string group, string code)
        {
            var queryable = warehouseData.AsQueryable();
            queryable = this.FilterByGroupCode(queryable, group, code);

            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.ProductCode,
                g.Price2
            }).Select(p => new SummarySalesReportDto()
            {
                OrgCode = p.Key.OrgCode,
                Code = p.Key.ProductCode,
                Price2 = p.Key.Price2,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetProductName(item.Code, item.OrgCode);
            }
            return result;
        }
        public SummarySalesReportDto GetTotal(List<SummarySalesReportDto> groupData)
        {
            var total = new SummarySalesReportDto()
            {
                Bold = "C",
                Description = "Tổng cộng",
                Quantity = groupData.Sum(s => s.Quantity),
                Amount = groupData.Sum(s => s.Amount),
                AmountCur = groupData.Sum(s => s.AmountCur),
                Amount2 = groupData.Sum(s => s.Amount2),
                Amount2Cur = groupData.Sum(s => s.Amount2Cur),
                DiscountAmount = groupData.Sum(s => s.DiscountAmount),
                DiscountAmountCur = groupData.Sum(s => s.DiscountAmountCur),
                VatAmount = groupData.Sum(s => s.VatAmount),
                VatAmountCur = groupData.Sum(s => s.VatAmountCur)
            };
            return total;
        }
        private async Task<Product> GetProduct(string productCode, string orgCode)
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
        private async Task<List<SummarySalesReportDto>> GroupData1(List<WarehouseBookGeneralDto> warehouseData, string group)
        {
            var queryable = warehouseData.AsQueryable();
            List<SummarySalesReportDto> result = group switch
            {
                "1" => await this.GroupByPartner(queryable),
                "2" => await this.GroupByProduct(queryable),
                "3" => await this.GroupByDepartment(queryable),
                "4" => await this.GroupBySection(queryable),
                "5" => await this.GroupByCase(queryable),
                "6" => await this.GroupByBusiness(queryable),
                "7" => await this.GroupByFProductWork(queryable),
                _ => null
            };
            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupData2(List<WarehouseBookGeneralDto> warehouseData, string group1, string code, string group2)
        {
            var queryable = warehouseData.AsQueryable();
            queryable = this.FilterByGroupCode(queryable, group1, code);
            List<SummarySalesReportDto> result = group2 switch
            {
                "1" => await this.GroupByPartner(queryable),
                "2" => await this.GroupByProduct(queryable),
                "3" => await this.GroupByDepartment(queryable),
                "4" => await this.GroupBySection(queryable),
                "5" => await this.GroupByCase(queryable),
                "6" => await this.GroupByBusiness(queryable),
                "7" => await this.GroupByFProductWork(queryable),
                _ => null
            };
            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupByPartner(IQueryable<WarehouseBookGeneralDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.PartnerCode0
            }).Select(p => new SummarySalesReportDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                Code = p.Key.PartnerCode0,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetPartnerName(item.Code, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupByProduct(IQueryable<WarehouseBookGeneralDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.ProductCode
            }).Select(p => new SummarySalesReportDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                Code = p.Key.ProductCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetProductName(item.Code, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupByDepartment(IQueryable<WarehouseBookGeneralDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.DepartmentCode
            }).Select(p => new SummarySalesReportDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                Code = p.Key.DepartmentCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetDepartmentName(item.Code, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupBySection(IQueryable<WarehouseBookGeneralDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.SectionCode
            }).Select(p => new SummarySalesReportDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                Code = p.Key.SectionCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetSectionName(item.Code, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupByCase(IQueryable<WarehouseBookGeneralDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.CaseCode
            }).Select(p => new SummarySalesReportDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                Code = p.Key.CaseCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetCaseName(item.Code, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupByBusiness(IQueryable<WarehouseBookGeneralDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.BusinessCode
            }).Select(p => new SummarySalesReportDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                Code = p.Key.BusinessCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetBusinessName(item.Code, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummarySalesReportDto>> GroupByFProductWork(IQueryable<WarehouseBookGeneralDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.FProductWorkCode
            }).Select(p => new SummarySalesReportDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                Code = p.Key.FProductWorkCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Amount2 = p.Sum(p => p.Amount2),
                Amount2Cur = p.Sum(p => p.AmountCur2),
                Quantity = p.Sum(p => p.ExportQuantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur)
            }).OrderBy(p => p.Code).ToList();
            foreach (var item in result)
            {
                item.Description = await this.GetFProductWorkName(item.Code, item.OrgCode);
            }

            return result;
        }
        private async Task<string> GetPartnerName(string partnerCode, string orgCode)
        {
            var partner = await _partnerService.GetAccPartnerByCodeAsync(partnerCode, orgCode);
            if (partner == null) return "{Không có tên đối tượng}";
            return partner.Name;
        }
        private async Task<string> GetProductName(string productCode, string orgCode)
        {
            var item = await _productService.GetByCodeAsync(productCode, orgCode);
            if (item == null) return "{Không có tên mặt hàng}";
            return item.Name;
        }
        private async Task<string> GetDepartmentName(string code, string orgCode)
        {
            var item = await _departmentService.GetDepartmentByCodeAsync(code, orgCode);
            if (item == null) return "{Không có tên bộ phận}";
            return item.Name;
        }
        private async Task<string> GetSectionName(string code, string orgCode)
        {
            var item = await _accSectionService.GetByCodeAsync(code, orgCode);
            if (item == null) return "{Không có tên khoản mục}";
            return item.Name;
        }
        private async Task<string> GetCaseName(string code, string orgCode)
        {
            var item = await _accCaseService.GetByCodeAsync(code, orgCode);
            if (item == null) return "{Không có tên vụ việc}";
            return item.Name;
        }
        private async Task<string> GetBusinessName(string code, string orgCode)
        {
            var item = await _businessCategoryService.GetBusinessByCodeAsync(code, orgCode);
            if (item == null) return "{Không có tên hạch toán}";
            return item.Name;
        }
        private async Task<string> GetFProductWorkName(string code, string orgCode)
        {
            var item = await _fProductWorkService.GetByFProductWorkAsync(code, orgCode);
            if (item == null) return "{Không có tên công trình, sản phẩm}";
            return item.Name;
        }
        private IQueryable<WarehouseBookGeneralDto> FilterByGroupCode(IQueryable<WarehouseBookGeneralDto> queryable,
                                        string group, string code)
        {
            var query = group switch
            {
                "1" => queryable.Where(p => p.PartnerCode0 == code),
                "2" => queryable.Where(p => p.ProductCode == code),
                "3" => queryable.Where(p => p.DepartmentCode == code),
                "4" => queryable.Where(p => p.SectionCode == code),
                "5" => queryable.Where(p => p.CaseCode == code),
                "6" => queryable.Where(p => p.BusinessCode == code),
                "7" => queryable.Where(p => p.FProductWorkCode == code),
                _ => queryable
            };
            return query;
        }
        #endregion
    }
}
