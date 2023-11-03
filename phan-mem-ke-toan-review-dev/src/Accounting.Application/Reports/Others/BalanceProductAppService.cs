using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
using System.Linq;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Caching;

namespace Accounting.Reports.Others
{
    public class BalanceProductAppService : AccountingAppService
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
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public BalanceProductAppService(ReportDataService reportDataService,
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
                        ProductVoucherService productVoucherService,
                        ProductVoucherDetailService productVoucherDetailService,
                        VoucherCategoryService voucherCategoryService,
                        AccountingCacheManager accountingCacheManager)
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
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _voucherCategoryService = voucherCategoryService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        public async Task<ReportResponseDto<BalanceProductDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var ord0 = "";
            var attachPartner = "";
            //Theo hợp đồng
            var attachContract = "";
            string ordRec = null;
            //Theo Khoản mục
            var attachAccSection = "";
            //Mã khoản mục
            var attachWorkPlace = "";
            //Theo ngoại tệ
            var attachCurrency = "";
            //Theo phân xưởng
            var attachProductCost = "";
            var ord0Code = "";
            decimal debitIncurred = 0;//ps_no
            decimal debitIncurredCur = 0;
            DateTime? dateNew;
            int year = 0;
            decimal residual = 0;
            decimal residualCur = 0;
            DateTime ac = (DateTime)dto.Parameters.FromDate;
            year = ac.Year;
            dto.Parameters.Year = year;
            dto.Parameters.FromDate = DateTime.Now;
            var dic = GetWarehouseBookParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);
            var lstLedger = (from a in incurredData
                             where a.VoucherId == ordRec
                             select new BalanceProductDto
                             {
                                 CreationTime = a.CreationTime
                             }).Take(1).FirstOrDefault();
            dateNew = lstLedger != null ? lstLedger.CreationTime : null;
            if (dateNew == null)
            {
                dateNew = DateTime.Now;
            }


            //var lstVoucheCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            //var voucherCategory = lstVoucheCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.Parameters.VoucherCode).FirstOrDefault();
            //var voucherOrd = voucherCategory.VoucherOrd;

            var lstProduct = await _productService.GetQueryableAsync();
            var producuct = lstProduct.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.Parameters.ProductCode).FirstOrDefault();
            var attachProductLot = producuct.AttachProductLot;
            var attachProductOrigin = producuct.AttachProductOrigin;
            if (attachProductLot != "C" || string.IsNullOrEmpty(dto.Parameters.ProductLotCode))
            {
                dto.Parameters.ProductLotCode = null;
            }
            if (attachProductOrigin != null || string.IsNullOrEmpty(dto.Parameters.ProductOriginCode))
            {
                dto.Parameters.ProductOriginCode = null;
            }


            //var dic2 = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetProductOpeningBalance(dic);
            var excessQuantity = openingBalance.ImportQuantity;
            residual = (decimal)openingBalance.ImportAmount;
            residualCur = (decimal)openingBalance.ImportAmountCur;



            var resulIncurredData = from a in incurredData
                                    where int.Parse(a.Status) < 2
                                    && (a.VoucherDate < dto.Parameters.FromDate) || (a.VoucherDate == dto.Parameters.FromDate && (dateNew == null || a.CreationTime <= dateNew || (dateNew == a.CreationTime && a.VoucherId == ordRec)))
                                    select new
                                    {
                                        a.ExportQuantity,
                                        a.ExportAmount,
                                        a.ExportAmountCur,
                                        a.ImportQuantity,
                                        a.ImportAmount,
                                        a.ImportAmountCur
                                    };
            var importQuantity = resulIncurredData.Select(p => p.ImportQuantity).Sum() - resulIncurredData.Select(p => p.ExportQuantity).Sum();
            var importAmount = resulIncurredData.Select(p => p.ImportAmount).Sum() - resulIncurredData.Select(p => p.ExportAmount).Sum();
            var importAmountCur = resulIncurredData.Select(p => p.ImportAmountCur).Sum() - resulIncurredData.Select(p => p.ExportAmountCur).Sum();

            excessQuantity = excessQuantity + importQuantity;
            residual = (decimal)(residual + importAmount);
            residualCur = (decimal)(residualCur + importAmountCur);
            List<BalanceProductDto> productBalanceDtos = new List<BalanceProductDto>();
            BalanceProductDto productBalanceDto = new BalanceProductDto();
            productBalanceDto.ExcessQuantity = excessQuantity;
            productBalanceDto.PreExisting = residual;
            productBalanceDto.PreExistingCur = residualCur;
            productBalanceDto.PriceEx = excessQuantity != 0 ? residual / excessQuantity : 0;
            productBalanceDto.PreExistingCur = excessQuantity != 0 ? residualCur / excessQuantity : 0;

            productBalanceDtos.Add(productBalanceDto);
            string orgCode = _webHelper.GetCurrentOrgUnit();



            var reportResponse = new ReportResponseDto<BalanceProductDto>();
            reportResponse.Data = productBalanceDtos;
            //reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            //reportResponse.RequestParameter = dto.Parameters;
            //reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            //reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
            //                        _webHelper.GetCurrentYear());
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
        #region Private
        private async Task<ProductBalanceDto> GetProductOpeningBalance(Dictionary<string, object> dic)
        {


            var productOpeningBalances = await _reportDataService.GetProductOpeningBalancesAsync(dic);
            var balances = new ProductBalanceDto()
            {
                WarehouseCode = productOpeningBalances.Max(p => p.WarehouseCode),
                OrgCode = productOpeningBalances.Max(p => p.OrgCode),
                AccCode = productOpeningBalances.Max(p => p.AccCode),
                ProductCode = productOpeningBalances.Max(p => p.ProductCode),
                ProductLotCode = productOpeningBalances.Max(p => p.ProductLotCode),
                ProductOriginCode = productOpeningBalances.Max(p => p.ProductOriginCode),
                ImportQuantity = productOpeningBalances.Sum(s => s.ImportQuantity),
                ImportAmount = productOpeningBalances.Sum(s => s.ImportAmount),
                ImportAmountCur = productOpeningBalances.Sum(s => s.ImportAmountCur),
                ExportQuantity = productOpeningBalances.Sum(s => s.ExportQuantity),
                ExportAmount = productOpeningBalances.Sum(s => s.ExportAmount),
                ExportAmountCur = productOpeningBalances.Sum(s => s.ExportAmountCur)
            };

            return balances;
        }
        private async Task<List<BalanceProductDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.Select(p => new BalanceProductDto()
            {
                Sort = 0,
                Sort0 = 1,
                Bold = "K",
                OrgCode = p.OrgCode,
                Year = p.Year,
                VoucherNumber = p.VoucherNumber,
                VoucherDate = p.VoucherDate,
                Ord0 = p.Ord0,
                GroupCode = null,
                Description = p.Description,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                ExchangeRate = p.ExchangeRate,
                Quantity = p.Quantity,
                Amount = p.Amount,
                AmountCur = p.AmountCur,
                VoucherId = p.VoucherId,
                Id = p.Id,
                VoucherCode = p.VoucherCode,
                FProductWorkCode = p.FProductWorkCode,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                Price = p.Price,
                PriceCur = p.PriceCur,
                BusinessAcc = p.BusinessAcc,
                PartnerCode = p.PartnerCode,
                PartnerName = null,
                Unit = p.UnitCode,
                CaseName = null,
                WarehouseCode = p.WarehouseCode,
                WarehouseName = null,
                //ProductGroupCode = p.ProductGroupCode,
                ProductCode = p.ProductCode,
                //ProductName = p.pro
                TransWarehouseCode = p.TransWarehouseCode,
                Note = p.Note,
                VoucherGroup = p.VoucherGroup,
                AccName = null,
                DepartmentCode = p.DepartmentCode,
                DepartmentName = null,
                Status = p.Status,
                CreationTime = p.CreationTime,
                ContractCode = p.ContractCode,
                ExportQuantity = p.ExportQuantity,
                ImportQuantity = p.ImportQuantity,
                ExportAmount = p.ExportAmount,
                ExportAmountCur = p.ExportAmountCur,
                ImportAmount = p.ImportAmount,
                ImportAmountCur = p.ImportAmountCur



            }).ToList();


            return incurredData;
        }
        private async Task<List<WarehouseBookGeneralDto>> GetWarehouseBook(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetWarehouseBookData(dic);
            return data;
        }


        private Dictionary<string, object> GetWarehouseBookParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.OrgCode, _webHelper.GetCurrentOrgUnit());
            //dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");
            dic.Add(WarehouseBookParameterConst.ToDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.Year, dto.Year);
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }

            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }

            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductOriginCode, dto.ProductOriginCode);
            }
            return dic;
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

