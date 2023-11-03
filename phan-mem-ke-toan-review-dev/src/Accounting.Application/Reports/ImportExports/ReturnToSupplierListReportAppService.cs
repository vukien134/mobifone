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
using Accounting.Reports.DebitBooks;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Linq;
using Accounting.JsonConverters;
using static NPOI.HSSF.Util.HSSFColor;
using System.Security.Cryptography;
using Accounting.Vouchers;
using Accounting.Categories.Products;
using Accounting.Reports.GeneralDiaries;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Functions;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class ReturnToSupplierListReportAppService : AccountingAppService
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
        private readonly AccountingCacheManager _accountingCacheManager;

        #endregion

        public ReturnToSupplierListReportAppService(ReportDataService reportDataService,
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
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.ReturnToSupplierListReportView)]
        public async Task<ReportResponseDto<ReturnToSupplierListDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var dic = GetWarehouseBookParameter(dto.Parameters);
            var incurredData = await GetIncurredData(dic);
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = (incurredData.Where(p =>
                             GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber)
                                 && GetVoucherNumber(dto.Parameters.EndVoucherNumber) >= GetVoucherNumber(p.VoucherNumber)
                                )).ToList();
            }
            decimal? totalAmount = incurredData.Select(p => p.Amount).Sum();
            decimal? totalAmountCur = incurredData.Select(p => p.AmountCur).Sum();

            var aktGroup = DataAktGroup(incurredData, lstPartner);
            var resuls = ResulData(incurredData, lstPartner);
            resuls = ResulProductData(resuls, lstProduct);
            List<ReturnToSupplierListDto> returnToSupplierListDtos = new List<ReturnToSupplierListDto>();
            foreach (var item in aktGroup)
            {

                returnToSupplierListDtos.Add(Akgroup1(item));
                returnToSupplierListDtos.Add(Akgroup2(item));
                returnToSupplierListDtos.Add(Akgroup5(item));
            }

            foreach (var item in resuls)
            {
                returnToSupplierListDtos.Add(Akgroup3(item));
            }
            ReturnToSupplierListDto cru6 = new ReturnToSupplierListDto();
            cru6.Tag = 0;
            cru6.Sort = 6;
            cru6.Bold = "C";
            cru6.GroupCode = "zzzzzzzzzz";
            cru6.Description = "Tổng cộng ";
            cru6.TurnoverAmount = totalAmount;
            cru6.TurnoverAmountCur = totalAmountCur;
            returnToSupplierListDtos.Add(cru6);
            returnToSupplierListDtos = returnToSupplierListDtos.OrderBy(p => p.GroupCode)
                                                              .ThenBy(p => p.Sort)
                                                              .ToList();
            var reportResponse = new ReportResponseDto<ReturnToSupplierListDto>();
            reportResponse.Data = returnToSupplierListDtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        private decimal GetVoucherNumber(string VoucherNumber)
        {
            string[] numbers = Regex.Split(VoucherNumber, @"\D+");
            if (numbers.Length > 0)
            {
                return decimal.Parse(string.Join("", numbers));
            }
            else
            {
                return 0;
            }
        }
        public List<ReturnToSupplierListDto> DataAktGroup(List<ReturnToSupplierListDto> returnToSupplierListDtos, List<AccPartner> accPartners)
        {
            var aktGroup = (from a in returnToSupplierListDtos
                            join b in accPartners on a.PartnerCode equals b.Code into c
                            from pn in c.DefaultIfEmpty()
                            group new
                            {
                                a.VoucherNumber,
                                a.VoucherDate,
                                a.PartnerCode,
                                a.BusinessAcc,
                                a.Description,
                                a.DepartmentCode,
                                pn.Name,
                                a.Amount,
                                a.AmountCur,
                                a.DiscountAmount,
                                a.DiscountAmountCur,
                                a.VatAmount,
                                a.VatAmountCur,
                                a.ExpenseAmount,
                                a.ExpenseAmountCur
                            } by new
                            {
                                a.VoucherNumber,
                                a.VoucherDate,
                                a.PartnerCode,
                                a.BusinessAcc,
                                a.Description,
                                a.DepartmentCode,
                                pn.Name
                            } into gr
                            select new ReturnToSupplierListDto
                            {
                                VoucherNumber = gr.Key.VoucherNumber,
                                VoucherDate = gr.Key.VoucherDate,
                                PartnerCode = "",
                                BusinessAcc = "",
                                Description = gr.Key.Name,
                                DepartmentCode = "",
                                PartnerName = "",
                                GroupCode = gr.Key.VoucherDate.ToString() + gr.Key.VoucherNumber,
                                WarehouseCode = "",
                                Amount = gr.Sum(p => p.Amount),
                                AmountCur = gr.Sum(p => p.AmountCur),
                                DiscountAmount = gr.Sum(p => p.DiscountAmount),
                                DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                                VatAmount = gr.Sum(p => p.VatAmount),
                                VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                TurnoverAmount = gr.Sum(p => p.Amount) + gr.Sum(p => p.DiscountAmount) + gr.Sum(p => p.VatAmount),//doanh_thu
                                TurnoverAmountCur = gr.Sum(p => p.AmountCur) + gr.Sum(p => p.DiscountAmountCur) + gr.Sum(p => p.VatAmountCur)
                            }).ToList();
            return aktGroup;
        }
        public List<ReturnToSupplierListDto> ResulData(List<ReturnToSupplierListDto> returnToSupplierListDtos, List<AccPartner> accPartners)
        {

            var resuls = (from p in returnToSupplierListDtos
                          join b in accPartners on p.PartnerCode equals b.Code into c
                          from pn in c.DefaultIfEmpty()
                          select new ReturnToSupplierListDto
                          {
                              Tag = 0,
                              Sort = 3,
                              Bold = "K",
                              OrgCode = p.OrgCode,
                              VoucherId = p.VoucherId,
                              VoucherCode = p.VoucherCode,
                              VoucherDate = p.VoucherDate,
                              VoucherNumber = p.VoucherNumber,
                              GroupCode = p.VoucherDate.ToString() + p.VoucherNumber,
                              Description = p.Description,
                              PartnerCode = p.PartnerCode,
                              Note = p.Note,
                              DepartmentCode = p.DepartmentCode,
                              Unit = p.Unit,
                              WarehouseCode = p.WarehouseCode,
                              BusinessAcc = p.BusinessAcc,
                              Quantity = p.Quantity,
                              Price = p.Price,
                              Amount = p.Amount,
                              AmountCur = p.AmountCur,
                              PartnerName = pn != null ? pn.Name : null,
                              DiscountAmount = p.DiscountAmount,
                              DiscountAmountCur = p.DiscountAmountCur,
                              VatAmount = p.VatAmount,
                              VatAmountCur = p.VatAmountCur,
                              ExpenseAmount = p.ExpenseAmount,
                              ExpenseAmountCur = p.ExpenseAmountCur,
                              ProductCode = p.ProductCode
                          }).ToList();
            return resuls;
        }
        public List<ReturnToSupplierListDto> ResulProductData(List<ReturnToSupplierListDto> returnToSupplierListDtos, List<Product> products)
        {

            var resuls = (from p in returnToSupplierListDtos
                          join d in products on p.ProductCode equals d.Code
                          select new ReturnToSupplierListDto
                          {
                              Tag = 0,
                              Sort = 3,
                              Bold = "K",
                              OrgCode = p.OrgCode,
                              VoucherId = p.VoucherId,
                              VoucherCode = p.VoucherCode,
                              VoucherDate = p.VoucherDate,
                              VoucherNumber = p.VoucherNumber,
                              GroupCode = p.VoucherDate.ToString() + p.VoucherNumber,
                              Description = d.Code + "-" + d.Name,
                              PartnerCode = p.PartnerCode,
                              Note = p.Description,
                              DepartmentCode = p.DepartmentCode,
                              Unit = p.Unit,
                              WarehouseCode = p.WarehouseCode,
                              BusinessAcc = p.BusinessAcc,
                              Quantity = p.Quantity,
                              Price = p.Price,
                              Amount = p.Amount,
                              AmountCur = p.AmountCur,
                              PartnerName = p.PartnerName,
                              DiscountAmount = p.DiscountAmount,
                              DiscountAmountCur = p.DiscountAmountCur,
                              VatAmount = p.VatAmount,
                              VatAmountCur = p.VatAmountCur,
                              ExpenseAmount = p.ExpenseAmount,
                              ExpenseAmountCur = p.ExpenseAmountCur,
                              ProductCode = p.ProductCode
                          }).ToList();
            return resuls;
        }
        public ReturnToSupplierListDto Akgroup1(ReturnToSupplierListDto returnToSupplierListDto)
        {
            ReturnToSupplierListDto cru1 = new ReturnToSupplierListDto();
            cru1.Tag = 0;
            cru1.Sort = 1;
            cru1.Bold = "C";
            cru1.GroupCode = returnToSupplierListDto.GroupCode;
            cru1.Description = returnToSupplierListDto.Description;
            cru1.WarehouseCode = returnToSupplierListDto.WarehouseCode;
            return cru1;
        }
        public ReturnToSupplierListDto Akgroup2(ReturnToSupplierListDto returnToSupplierListDto)
        {
            ReturnToSupplierListDto cru2 = new ReturnToSupplierListDto();
            cru2.Tag = 0;
            cru2.Sort = 2;
            cru2.Bold = "K";
            cru2.GroupCode = returnToSupplierListDto.GroupCode;
            cru2.Description = "Xuất hàng trả lại ";
            cru2.BusinessAcc = returnToSupplierListDto.BusinessAcc;
            return cru2;
        }
        public ReturnToSupplierListDto Akgroup5(ReturnToSupplierListDto returnToSupplierListDto)
        {
            ReturnToSupplierListDto cru5 = new ReturnToSupplierListDto();
            cru5.Tag = 0;
            cru5.Sort = 5;
            cru5.Bold = "K";
            cru5.GroupCode = returnToSupplierListDto.GroupCode;
            cru5.Description = "------------------------- ";
            cru5.BusinessAcc = returnToSupplierListDto.BusinessAcc;
            return cru5;
        }
        public ReturnToSupplierListDto Akgroup3(ReturnToSupplierListDto returnToSupplierListDto)
        {
            ReturnToSupplierListDto cru3 = new ReturnToSupplierListDto();
            cru3.Tag = returnToSupplierListDto.Tag;
            cru3.Sort = returnToSupplierListDto.Sort;
            cru3.Bold = returnToSupplierListDto.Bold;
            cru3.OrgCode = returnToSupplierListDto.OrgCode;
            cru3.VoucherId = returnToSupplierListDto.VoucherId;
            cru3.VoucherCode = returnToSupplierListDto.VoucherCode;
            cru3.VoucherDate = returnToSupplierListDto.VoucherDate;
            cru3.VoucherNumber = returnToSupplierListDto.VoucherNumber;
            cru3.Description = returnToSupplierListDto.Description;
            cru3.Unit = returnToSupplierListDto.Unit;
            cru3.Quantity = returnToSupplierListDto.Quantity;
            cru3.Price = returnToSupplierListDto.Price;
            cru3.TurnoverAmount = returnToSupplierListDto.Amount;
            cru3.BusinessAcc = returnToSupplierListDto.BusinessAcc;
            cru3.Note = returnToSupplierListDto.Note;
            cru3.GroupCode = returnToSupplierListDto.VoucherDate.ToString() + returnToSupplierListDto.VoucherNumber;
            return cru3;
        }
        [Authorize(ReportPermissions.ReturnToSupplierListReportPrint)]
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

        private async Task<List<ReturnToSupplierListDto>> GetIncurredData(Dictionary<string, object> dic)
        {

            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.Where(p => p.Status == "1").Select(p => new ReturnToSupplierListDto()
            {
                Tag = 0,
                Sort = 3,
                Bold = "K",
                OrgCode = p.OrgCode,
                VoucherId = p.VoucherId,
                VoucherCode = p.VoucherCode,
                VoucherDate = p.VoucherDate,
                VoucherNumber = p.VoucherNumber,
                GroupCode = null,
                Description = p.Description,
                PartnerCode = p.PartnerCode0,
                Note = p.Note,
                DepartmentCode = p.DepartmentCode,
                Unit = p.UnitCode,
                WarehouseCode = p.WarehouseCode,
                BusinessAcc = p.BusinessAcc,
                Quantity = p.Quantity,
                Price = p.Price,
                Amount = p.Amount,
                AmountCur = p.AmountCur,
                PartnerName = null,
                DiscountAmount = p.DiscountAmount,
                DiscountAmountCur = p.DiscountAmountCur,
                VatAmount = p.VatAmount,
                VatAmountCur = p.VatAmountCur,
                ExpenseAmount = p.ExpenseAmount,
                ExpenseAmountCur = p.ExpenseAmountCur,
                ProductCode = p.ProductCode
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
            dic.Add(WarehouseBookParameterConst.DebitOrCredit, dto.DebitCredit);
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }

            if (!string.IsNullOrEmpty(dto.EndVoucherNumber))
            {
                dic.Add(WarehouseBookParameterConst.EndVoucherNumber, dto.EndVoucherNumber);
            }
            if (!string.IsNullOrEmpty(dto.BeginVoucherNumber))
            {
                dic.Add(WarehouseBookParameterConst.BeginVoucherNumber, dto.BeginVoucherNumber);
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
            }
            if (!string.IsNullOrEmpty(dto.ReciprocalAcc))
            {
                dic.Add(WarehouseBookParameterConst.ReciprocalAcc, dto.ReciprocalAcc);
            }

            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }
            if (!string.IsNullOrEmpty(dto.PartnerGroup))
            {
                dic.Add(WarehouseBookParameterConst.PartnerGroup, dto.PartnerGroup);
            }
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductGroupCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.ProductGroupCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(WarehouseBookParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.CaseCode))
            {
                dic.Add(WarehouseBookParameterConst.CaseCode, dto.CaseCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(WarehouseBookParameterConst.SectionCode, dto.SectionCode);
            }
            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                dic.Add(WarehouseBookParameterConst.BusinessCode, dto.BusinessCode);
            }
            if (!string.IsNullOrEmpty(dto.LstVoucherCode))
            {
                dic.Add(WarehouseBookParameterConst.LstVoucherCode, dto.LstVoucherCode);
            }
            //if (!string.IsNullOrEmpty(dto.Sort))
            //{
            //    dic.Add(WarehouseBookParameterConst.Sort, dto.Sort);
            //}



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

