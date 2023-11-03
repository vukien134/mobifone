using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accounting.Categories.Partners;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using ContractService = Accounting.DomainServices.Categories.ContractService;
using System.Linq;
using ContractDetailService = Accounting.DomainServices.Categories.ContractDetailService;
using NPOI.SS.Formula.Functions;
using Accounting.Report;
using Accounting.Reports.ImportExports.Parameters;
using Microsoft.AspNetCore.Mvc;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class ReportOnContractPerformanceAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly AccountSystemService _accountSystemService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccSectionService _accSectionService;
        private readonly DepartmentService _departmentService;
        private readonly AccCaseService _accCaseService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly ProductGroupService _productGroupService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductAppService _productAppService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly SaleChannelService _saleChannelService;
        private readonly ContractService _contractService;
        private readonly ContractDetailService _contractDetailService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public ReportOnContractPerformanceAppService(ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccSectionService accSectionService,
                        DepartmentService departmentService,
                        AccCaseService accCaseService,
                        FProductWorkService fProductWorkService,
                        AccPartnerService accPartnerService,
                        PartnerGroupService partnerGroupService,
                        AccPartnerAppService accPartnerAppService,
                        ProductGroupService productGroupService,
                        ProductService productService,
                        WarehouseService warehouseService,
                        ProductAppService productAppService,
                        VoucherCategoryService voucherCategoryService,
                        VoucherTypeService voucherTypeService,
                        SaleChannelService saleChannelService,
                        ContractService contractService,
                        ContractDetailService contractDetailService,
                        ProductVoucherService productVoucherService,
                        ProductVoucherDetailService productVoucherDetailService,
                        AccountingCacheManager accountingCacheManager
            )
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accSectionService = accSectionService;
            _departmentService = departmentService;
            _accCaseService = accCaseService;
            _fProductWorkService = fProductWorkService;
            _accPartnerService = accPartnerService;
            _partnerGroupService = partnerGroupService;
            _accPartnerAppService = accPartnerAppService;
            _productGroupService = productGroupService;
            _productService = productService;
            _warehouseService = warehouseService;
            _productAppService = productAppService;
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _saleChannelService = saleChannelService;
            _contractService = contractService;
            _contractDetailService = contractDetailService;
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _accountingCacheManager = accountingCacheManager;
        }

        #region Methods
        public async Task<ReportResponseDto<ReportOnContractPerformanceDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();

            var contracts = await _contractService.GetQueryableAsync();
            var lstContracts = contracts.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var contracDetail = await _contractDetailService.GetQueryableAsync();
            var lstcontractDetail = contracDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var resul = from a in lstContracts
                        join b in lstcontractDetail on a.Id equals b.ContractId
                        where a.BeginDate >= dto.Parameters.FromDate && a.BeginDate <= dto.Parameters.ToDate
                        group new
                        {
                            a.BeginDate,
                            a.ContractType,
                            a.PartnerCode,
                            a.Code,
                            a.Name,
                            b.Quantity,
                            b.Price,
                            b.Amount,
                            b.TrxQuantity,
                            b.TrxPrice,
                            b.TrxPriceCur,
                            b.TrxAmount,
                            b.TrxAmountCur
                        } by new
                        {
                            a.Code
                        } into gr
                        select new ReportOnContractPerformanceDto
                        {
                            Sort = 2,
                            Bold = "K",
                            BeginDate = gr.Max(p => p.BeginDate),
                            ContractType = gr.Max(p => p.ContractType),
                            PartnerCode = gr.Max(p => p.PartnerCode),
                            Code = gr.Key.Code,
                            Name = gr.Max(p => p.Name),
                            Quantity = gr.Sum(p => p.Quantity),
                            Price = gr.Max(p => p.Price),
                            Amount = gr.Sum(p => p.Amount),
                            TrxQuantity = gr.Sum(p => p.TrxQuantity),
                            TrxPrice = gr.Max(p => p.TrxPrice),
                            TrxAmount = gr.Sum(p => p.TrxAmount),
                            QuantityCl = 0,
                            PriceCl = 0,
                            AmountCl = 0,
                            ContractCode = gr.Key.Code
                        };
            var productvoucher = await _productVoucherService.GetQueryableAsync();
            productvoucher = productvoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.VoucherDate >= dto.Parameters.FromDate && p.VoucherDate <= dto.Parameters.ToDate);
            var productvoucherdetail = await _productVoucherDetailService.GetQueryableAsync();
            productvoucherdetail = productvoucherdetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var resulproductVoucher = from a in productvoucher.ToList()
                                      join b in productvoucherdetail.ToList() on a.Id equals b.ProductVoucherId
                                      join c in resul.ToList() on b.ContractCode equals c.Code
                                      group new
                                      {
                                          b.ProductCode,
                                          b.ContractCode,
                                          b.Amount,
                                          b.Amount2,
                                          b.AmountCur,
                                          b.AmountCur2,
                                          a.VoucherCode,
                                          b.Quantity
                                      } by new
                                      {

                                          b.ContractCode
                                      } into gr
                                      select new
                                      {

                                          ContractCode = gr.Key.ContractCode,
                                          Amount = gr.Max(p => p.VoucherCode) == "PBH" ? gr.Sum(p => p.Amount2) : gr.Sum(p => p.Amount),
                                          AmountCur = gr.Max(p => p.VoucherCode) == "PBH" ? gr.Sum(p => p.AmountCur2) : gr.Sum(p => p.AmountCur),
                                          Quantity = gr.Sum(p => p.Quantity)
                                      };
            resul = from a in resul
                    join b in resulproductVoucher on a.ContractCode equals b.ContractCode into c
                    from d in c.DefaultIfEmpty()
                    select new ReportOnContractPerformanceDto
                    {
                        Sort = 2,
                        Bold = "K",
                        BeginDate = a.BeginDate,
                        ContractType = a.ContractType,
                        PartnerCode = a.PartnerCode,
                        Code = a.Code,
                        Name = a.Name,
                        Quantity = a.Quantity,
                        Price = a.Price,
                        Amount = a.Amount,
                        TrxQuantity = a.TrxQuantity,
                        TrxPrice = a.TrxPrice,
                        TrxAmount = a.TrxAmount,
                        QuantityCl = 0,
                        PriceCl = 0,
                        AmountCl = 0,
                        ContractCode = a.ContractCode,
                        QuantityTh = d != null ? d.Quantity : 0,
                        AmountTh = d != null ? d.Amount : 0,
                        AmountCurTh = d != null ? d.AmountCur : 0
                    };

            if (!string.IsNullOrEmpty(dto.Parameters.ContractCode))
            {

                resul = resul.Where(p => p.ContractCode == dto.Parameters.ContractCode);
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {

                resul = resul.Where(p => p.Code == dto.Parameters.PartnerCode);

            }
            if (!string.IsNullOrEmpty(dto.Parameters.ContractType))
            {
                resul = resul.Where(p => p.ContractType == dto.Parameters.ContractType);
            }
            var sumResul = SumData(resul.ToList());
            List<ReportOnContractPerformanceDto> reportOnContractPerformanceDtos = new List<ReportOnContractPerformanceDto>();
            foreach (var item in sumResul)
            {
                ReportOnContractPerformanceDto reportOnContractPerformanceDto = new ReportOnContractPerformanceDto();
                reportOnContractPerformanceDto.Sort = 1;
                reportOnContractPerformanceDto.Bold = "C";
                reportOnContractPerformanceDto.BeginDate = item.BeginDate;
                reportOnContractPerformanceDto.Code = item.Code;
                reportOnContractPerformanceDto.Quantity = item.Quantity;
                reportOnContractPerformanceDto.Price = item.Price;
                reportOnContractPerformanceDto.Amount = item.Amount;
                reportOnContractPerformanceDto.TrxAmount = item.TrxAmount;
                reportOnContractPerformanceDto.TrxPrice = item.TrxPrice;
                reportOnContractPerformanceDto.TrxAmount = item.TrxAmount;
                reportOnContractPerformanceDto.AmountTh = item.AmountTh;
                reportOnContractPerformanceDto.AmountCurTh = item.AmountCurTh;
                reportOnContractPerformanceDtos.Add(reportOnContractPerformanceDto);
            }
            resul = (from a in resul
                     select new ReportOnContractPerformanceDto
                     {
                         Sort = 2,
                         Bold = "K",
                         BeginDate = a.BeginDate,
                         ContractType = a.ContractType,
                         PartnerCode = a.PartnerCode,
                         Code = a.Code,
                         Name = a.Name,
                         Quantity = a.Quantity,
                         Price = a.Price,
                         Amount = a.Amount,
                         TrxQuantity = a.TrxQuantity,
                         TrxPrice = a.TrxPrice,
                         TrxAmount = a.TrxAmount,
                         QuantityCl = a.Quantity - a.TrxQuantity - a.QuantityTh,
                         PriceCl = a.Price,
                         AmountCl = a.Amount - a.TrxAmount - a.AmountTh,
                         QuantityTh = a.QuantityTh,
                         AmountTh = a.AmountTh,
                         AmountCurTh = a.AmountCurTh
                     }).ToList();
            reportOnContractPerformanceDtos.AddRange(resul);
            reportOnContractPerformanceDtos = reportOnContractPerformanceDtos.OrderBy(p => p.Code)
                                                                             .ThenBy(p => p.Sort)
                                                                             .ToList();
            var reportResponse = new ReportResponseDto<ReportOnContractPerformanceDto>();
            reportResponse.Data = reportOnContractPerformanceDtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public List<ReportOnContractPerformanceDto> SumData(List<ReportOnContractPerformanceDto> reportOnContractPerformanceDtos)
        {
            var sumResul = (from a in reportOnContractPerformanceDtos
                            group new
                            {
                                a.Code,
                                a.BeginDate,
                                a.Quantity,
                                a.Price,
                                a.Amount,
                                a.TrxQuantity,
                                a.TrxPrice,
                                a.TrxAmount,
                                a.QuantityCl,
                                a.PriceCl,
                                a.AmountCl,
                                a.AmountTh,
                                a.AmountCurTh
                            } by new
                            {

                                a.Code,
                                a.BeginDate
                            } into gr
                            select new ReportOnContractPerformanceDto
                            {
                                Code = gr.Key.Code,
                                BeginDate = gr.Key.BeginDate,
                                Quantity = gr.Sum(p => p.Quantity),
                                Price = gr.Sum(p => p.Price),
                                Amount = gr.Sum(p => p.Amount),
                                TrxQuantity = gr.Sum(p => p.TrxQuantity),
                                TrxPrice = gr.Sum(p => p.TrxPrice),
                                TrxAmount = gr.Sum(p => p.TrxAmount),
                                AmountTh = gr.Sum(p => p.AmountTh),
                                AmountCurTh = gr.Sum(p => p.AmountCurTh)
                            }).ToList();
            return sumResul;
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
    }
}

