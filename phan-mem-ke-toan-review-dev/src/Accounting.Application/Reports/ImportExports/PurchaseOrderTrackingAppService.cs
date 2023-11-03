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
using System.Linq;
using Accounting.Vouchers;
using Org.BouncyCastle.Math;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.DomainServices.Vouchers;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class PurchaseOrderTrackingAppService : AccountingAppService
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
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly RefVoucherService _refVoucherService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public PurchaseOrderTrackingAppService(ReportDataService reportDataService,
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
                        ProductVoucherService productVoucherService,
                        ProductVoucherDetailService productVoucherDetailService,
                        RefVoucherService refVoucherService,
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
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _saleChannelService = saleChannelService;
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _refVoucherService = refVoucherService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.PurchaseOrderTrackingReportView)]
        public async Task<ReportResponseDto<PurchaseOrderTrackinDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();

            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                        && p.VoucherDate >= dto.Parameters.FromDate
                                                        && p.VoucherDate <= dto.Parameters.ToDate
                                                        && p.VoucherCode == dto.Parameters.VoucherCode
                                                        ).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                lstProductVoucher = lstProductVoucher.Where(p => p.PartnerCode0 == dto.Parameters.PartnerCode).ToList();
            }
            var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
            var lstProductVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var ht2 = dto.Parameters.Ht2;
            var refVoucher = await _refVoucherService.GetQueryableAsync();
            var lstrefVoucher = refVoucher.ToList();
            // kế hoạch
            var resulProductVoucher = (from a in lstProductVoucher
                                       join b in lstProductVoucherDetail on a.Id equals b.ProductVoucherId
                                       group new
                                       {
                                           a.OrgCode,
                                           a.VoucherNumber,
                                           a.Id,
                                           a.VoucherCode,
                                           a.VoucherDate,
                                           b.Ord0,
                                           a.DeliveryDate,
                                           b.ProductCode,
                                           b.ProductName,
                                           b.ProductLotCode,
                                           b.ProductOriginCode,
                                           b.Quantity,
                                           b.PriceCur,
                                           b.Price,
                                           b.AmountCur,
                                           b.Amount,
                                           b.Price2,
                                           b.PriceCur2,
                                           b.Amount2,
                                           b.AmountCur2,
                                           a.PartnerCode0,
                                           a.PartnerName0,
                                           b.UnitCode

                                       } by new
                                       {
                                           a.OrgCode,
                                           a.VoucherNumber,
                                           a.Id,
                                           a.VoucherCode,
                                           a.VoucherDate,
                                           b.Ord0,
                                           a.DeliveryDate,
                                           b.ProductCode,
                                           b.ProductName,
                                           b.ProductLotCode,
                                           b.ProductOriginCode,
                                           b.Quantity,
                                           b.PriceCur,
                                           b.Price,
                                           b.AmountCur,
                                           b.Amount,
                                           b.Price2,
                                           b.PriceCur2,
                                           b.Amount2,
                                           b.AmountCur2,
                                           a.PartnerCode0,
                                           a.PartnerName0
                                       } into gr
                                       select new PurchaseOrderTrackinDto
                                       {

                                           Sort = 1,
                                           Bold = "K",
                                           OrgCode = gr.Key.OrgCode,
                                           VoucherNumber = gr.Key.VoucherNumber,
                                           VoucherId = gr.Key.Id,
                                           VoucherCode = gr.Key.VoucherCode,
                                           VoucherDate = gr.Key.VoucherDate,
                                           DeliveryDate = gr.Key.DeliveryDate,
                                           ProductCode = gr.Key.ProductCode,
                                           ProductName = gr.Key.ProductName,
                                           ProductLotCode = gr.Key.ProductLotCode,
                                           ProductOriginCode = gr.Key.ProductOriginCode,
                                           Quantity = gr.Key.Quantity,
                                           PriceCur = ht2 == "K" ? gr.Key.PriceCur2 : gr.Key.PriceCur,
                                           Price = ht2 == "K" ? gr.Key.Price2 : gr.Key.Price,
                                           Amount = ht2 == "K" ? gr.Key.Amount2 : gr.Key.Amount,
                                           AmountCur = ht2 == "K" ? gr.Key.AmountCur2 : gr.Key.AmountCur,
                                           QuantityTh = (decimal?)0,
                                           PriceCurTh = (decimal?)0,
                                           PriceTh = (decimal?)0,
                                           AmountCurTh = (decimal?)0,
                                           AmountTh = (decimal?)0,
                                           QuantityCl = gr.Key.Quantity,
                                           AmountCurCl = gr.Key.AmountCur,
                                           AmountCl = gr.Key.Amount,
                                           PartnerCode = gr.Key.PartnerCode0,
                                           PartnerName = gr.Key.PartnerName0,
                                           UnitCode = gr.Max(p => p.UnitCode)
                                       }).ToList();


            var productvoucherTh = (from a in productVoucher
                                    join b in refVoucher on a.Id equals b.DestId
                                    join c in productVoucherDetail on a.Id equals c.ProductVoucherId
                                    group new
                                    {
                                        a
                                    } by new
                                    {
                                        a.OrgCode,
                                        a.VoucherNumber,
                                        a.Id,
                                        a.VoucherCode,
                                        a.VoucherDate,
                                        a.DeliveryDate,
                                        c.ProductCode,
                                        c.ProductName,
                                        c.ProductLotCode,
                                        c.ProductOriginCode,
                                        c.Price,
                                        c.Price2,
                                        c.PriceCur2,
                                        c.PriceCur,
                                        c.Amount,
                                        c.Amount2,
                                        c.AmountCur,
                                        c.AmountCur2,
                                        b.SrcId
                                    } into gr
                                    select new PurchaseOrderTrackinDto
                                    {
                                        Sort = 1,
                                        Bold = "K",
                                        OrgCode = gr.Key.OrgCode,
                                        VoucherNumber = gr.Key.VoucherNumber,
                                        VoucherId = gr.Key.SrcId,
                                        VoucherCode = gr.Key.VoucherCode,
                                        VoucherDate = gr.Key.VoucherDate,
                                        DeliveryDate = gr.Key.DeliveryDate,
                                        ProductCode = gr.Key.ProductCode,
                                        ProductName = gr.Key.ProductName,
                                        ProductLotCode = gr.Key.ProductLotCode,
                                        ProductOriginCode = gr.Key.ProductOriginCode,
                                        Quantity = gr.Sum(p => p.a.TotalQuantity),
                                        PriceCur = ht2 == "K" ? gr.Key.PriceCur2 : gr.Key.PriceCur,
                                        Price = ht2 == "K" ? gr.Key.Price2 : gr.Key.Price,
                                        Amount = ht2 == "K" ? gr.Key.Amount2 : gr.Key.Amount,
                                        AmountCur = ht2 == "K" ? gr.Key.AmountCur2 : gr.Key.AmountCur,
                                        QuantityTh = (decimal?)0,
                                        PriceCurTh = (decimal?)0,
                                        PriceTh = (decimal?)0,
                                        AmountCurTh = (decimal?)0,
                                        AmountTh = (decimal?)0,
                                        QuantityCl = gr.Sum(p => p.a.TotalQuantity),
                                        AmountCurCl = gr.Key.AmountCur,
                                        AmountCl = gr.Key.Amount,
                                        //PartnerCode = gr.Key.PartnerCode0,
                                        //PartnerName = gr.Key.PartnerName0,
                                        //UnitCode = gr.Max(p => p.UnitCode)
                                    }).ToList();

            productvoucherTh = (from a in productvoucherTh
                                join b in resulProductVoucher on a.VoucherId equals b.VoucherId //into c
                                                                                                // from d in c.DefaultIfEmpty()
                                where dto.Parameters.VoucherCode == "DBH" ? a.VoucherCode == "PBH" : a.VoucherCode == "PNH"
                                select new PurchaseOrderTrackinDto
                                {
                                    Sort = 1,
                                    Bold = "K",
                                    OrgCode = a.OrgCode,
                                    VoucherNumber = a.VoucherNumber,
                                    VoucherId = a.VoucherId,
                                    VoucherCode = a.VoucherCode,
                                    VoucherDate = a.VoucherDate,
                                    DeliveryDate = a.DeliveryDate,
                                    ProductCode = a.ProductCode,
                                    ProductName = a.ProductName,
                                    ProductLotCode = a.ProductLotCode,
                                    ProductOriginCode = a.ProductOriginCode,
                                    Quantity = (decimal)0,
                                    PriceCur = (decimal)0,
                                    Price = (decimal)0,
                                    Amount = (decimal)0,
                                    AmountCur = (decimal)0,
                                    QuantityTh = a.Quantity,
                                    PriceCurTh = a.PriceCur,
                                    PriceTh = a.Price,
                                    AmountCurTh = a.AmountCur,
                                    AmountTh = a.Amount,
                                    QuantityCl = (decimal)0,
                                    AmountCurCl = (decimal)0,
                                    AmountCl = (decimal)0
                                }).ToList();

            resulProductVoucher = (from a in resulProductVoucher
                                   join b in productvoucherTh on a.VoucherId equals b.VoucherId into c
                                   from d in c.DefaultIfEmpty()
                                   group new
                                   {
                                       a,
                                       QuantityTh = d != null ? d.QuantityTh : 0,
                                       PriceCurTh = d != null ? d.PriceCurTh : 0,
                                       AmountCurTh = d != null ? d.AmountCurTh : 0,
                                       AmountTh = d != null ? d.AmountTh : 0,
                                       PriceTh = d != null ? d.PriceTh : 0

                                   } by new
                                   {
                                       a.VoucherNumber,
                                       a.VoucherId,
                                       a.ProductCode,
                                       a.ProductName,
                                       a.VoucherDate,

                                   } into gr
                                   select new PurchaseOrderTrackinDto
                                   {
                                       Sort = 1,
                                       Bold = "K",
                                       OrgCode = gr.Max(p => p.a.OrgCode),
                                       VoucherNumber = gr.Key.VoucherNumber,
                                       VoucherId = gr.Key.VoucherId,
                                       VoucherCode = gr.Max(p => p.a.VoucherCode),
                                       VoucherDate = gr.Key.VoucherDate,
                                       DeliveryDate = gr.Max(p => p.a.DeliveryDate),
                                       ProductCode = gr.Key.ProductCode,
                                       ProductName = gr.Key.ProductName,
                                       ProductLotCode = gr.Max(p => p.a.ProductLotCode),
                                       ProductOriginCode = gr.Max(p => p.a.ProductOriginCode),
                                       Quantity = (decimal?)gr.Max(p => p.a.Quantity),
                                       PriceCur = (decimal?)gr.Max(p => p.a.PriceCur),
                                       Price = (decimal?)gr.Max(p => p.a.Price),
                                       Amount = (decimal?)gr.Max(p => p.a.Amount),
                                       AmountCur = (decimal?)gr.Max(p => p.a.AmountCur),
                                       QuantityTh = (decimal?)gr.Max(p => p.QuantityTh),
                                       PriceCurTh = (decimal?)gr.Max(p => p.PriceCurTh),
                                       PriceTh = (decimal?)gr.Max(p => p.PriceTh),
                                       AmountCurTh = (decimal?)gr.Max(p => p.AmountCurTh),
                                       AmountTh = (decimal?)gr.Max(p => p.AmountTh),
                                       QuantityCl = (decimal?)gr.Max(p => p.a.Quantity) - gr.Max(p => p.QuantityTh),
                                       AmountCurCl = (decimal?)gr.Max(p => p.a.AmountCur) - gr.Max(p => p.AmountCurTh),
                                       AmountCl = (decimal?)gr.Max(p => p.a.Amount) - gr.Max(p => p.AmountTh),
                                       PartnerCode = gr.Max(p => p.a.PartnerCode),
                                       PartnerName = gr.Max(p => p.a.PartnerName),
                                       UnitCode = gr.Max(p => p.a.UnitCode)
                                   }).ToList();
            var totalQuantity = resulProductVoucher.Select(p => p.Quantity).Sum();
            var totalAmount = resulProductVoucher.Select(p => p.Amount).Sum();
            var totalAmountCur = resulProductVoucher.Select(p => p.AmountCur).Sum();
            var totaQuantityTh = resulProductVoucher.Select(p => p.QuantityTh).Sum();
            var totalAmountCurTh = resulProductVoucher.Select(p => p.AmountCurTh).Sum();
            var totalAmountTh = resulProductVoucher.Select(p => p.AmountTh).Sum();
            var totalQuantityCl = resulProductVoucher.Select(p => p.QuantityCl).Sum();
            var totalAmountCurCl = resulProductVoucher.Select(p => p.AmountCurCl).Sum();
            var totalAmountCl = resulProductVoucher.Select(p => p.AmountCl).Sum();
            PurchaseOrderTrackinDto purchaseOrderTrackinDto = new PurchaseOrderTrackinDto();
            purchaseOrderTrackinDto.Sort = 2;
            purchaseOrderTrackinDto.Bold = "C";
            purchaseOrderTrackinDto.ProductName = "Tổng cộng";
            purchaseOrderTrackinDto.Quantity = totalQuantity;
            purchaseOrderTrackinDto.Amount = totalAmount;
            purchaseOrderTrackinDto.AmountCur = totalAmountCur;
            purchaseOrderTrackinDto.AmountTh = totalAmountTh;
            purchaseOrderTrackinDto.AmountCurTh = totalAmountCurTh;
            purchaseOrderTrackinDto.AmountCl = totalAmountCl;
            purchaseOrderTrackinDto.AmountCurCl = totalAmountCurCl;



            List<PurchaseOrderTrackinDto> purchaseOrderTrackinDtos = new List<PurchaseOrderTrackinDto>();
            purchaseOrderTrackinDtos.AddRange(resulProductVoucher);
            purchaseOrderTrackinDtos.Add(purchaseOrderTrackinDto);
            purchaseOrderTrackinDtos = purchaseOrderTrackinDtos.OrderBy(p => p.Sort)
                                                                .OrderBy(p => p.VoucherDate)
                                                                .OrderBy(p => p.VoucherNumber)
                                                                .ToList();
            var reportResponse = new ReportResponseDto<PurchaseOrderTrackinDto>();
            reportResponse.Data = purchaseOrderTrackinDtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        [Authorize(ReportPermissions.PurchaseOrderTrackingReportPrint)]
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

        private List<FormularDto> GetFormular(string formular)
        {
            var lst = new List<FormularDto>();
            formular = formular.Replace(" ", "");
            formular = formular.Replace("+", ",+,");
            formular = formular.Replace("-", ",-,");
            formular = "+," + formular;
            var lstData = formular.Split(',').ToList();
            for (var i = 0; i < lstData.Count; i += 2)
            {
                lst.Add(new FormularDto
                {
                    Code = lstData[i + 1],
                    AccCode = lstData[i + 1],
                    Math = lstData[i],
                });
            }
            return lst;
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private async Task<List<SummaryPurchaseReportDto>> GetIncurredData(Dictionary<string, object> dic)
        {

            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.GroupBy(p => new { p.OrgCode, p.ProductCode }).Select(p => new SummaryPurchaseReportDto()
            {
                Tag = 0,
                Bold = "K",
                Sort = 1,
                OrgCode = p.Max(p => p.OrgCode),
                ProductCode = p.Key.ProductCode,
                ProductName = null,
                ProductGroupCode = null,
                UnitCode = null,
                ImportQuantity = p.Sum(p => p.ImportQuantity),
                ImportAmount = p.Sum(p => p.ImportAmount),
                ImportAmountCur = p.Sum(p => p.ImportAmountCur),
                ExportQuantity = p.Sum(p => p.ExportQuantity),
                ExportAmount = p.Sum(p => p.ExportAmount),
                ExportAmountCur = p.Sum(p => p.ExportAmountCur),
                Price = 0
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
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);

            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
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

