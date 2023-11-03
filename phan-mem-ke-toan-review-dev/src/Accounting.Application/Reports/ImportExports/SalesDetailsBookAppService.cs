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
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class SalesDetailsBookAppService : AccountingAppService
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
        private readonly VoucherTypeService _voucherTypeService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public SalesDetailsBookAppService(ReportDataService reportDataService,
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
                        VoucherTypeService voucherTypeService,
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
            _voucherTypeService = voucherTypeService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.SaleDetailBookReportView)]
        public async Task<ReportResponseDto<SalesDetailsBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            voucherType = voucherType.Where(p => p.Code == "PBH").ToList();
            dto.Parameters.LstVoucherCode = voucherType.ToList().FirstOrDefault().ListVoucher;
            var dic = GetWarehouseBookParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productGroup = await _productGroupService.GetQueryableAsync();
            var lstProductGroup = productGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            incurredData = (from p in incurredData
                            join b in lstPartner on p.PartnerCode equals b.Code into c
                            from pa in c.DefaultIfEmpty()
                            join d in lstProduct on p.ProductCode equals d.Code into m
                            from pro in m.DefaultIfEmpty()
                            select new SalesDetailsBookDto
                            {
                                Sort = p.Sort,
                                Sort0 = p.Sort0,
                                Bold = "K",
                                OrgCode = p.OrgCode,
                                Year = p.Year,
                                VoucherNumber = p.VoucherNumber,
                                VoucherDate = p.VoucherDate,
                                Ord0 = p.Ord0,
                                GroupCode = p.VoucherDate + p.VoucherNumber + p.VoucherId,
                                Description = p.ProductCode + " - " + pro != null ? pro.Name : null,
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
                                DiscountAmount = p.DiscountAmount,
                                DiscountAmountCur = p.DiscountAmountCur,
                                VatAmount = p.VatAmount,
                                VatAmountCur = p.VatAmountCur,
                                Price0 = p.Price0,
                                ExportAmount = p.ExportAmount,
                                ExportAmountCur = p.ExportAmountCur,
                                PriceCur0 = p.PriceCur0,
                                PartnerCode = p.PartnerCode,
                                PartnerName = pa != null ? pa.Name : null,
                                UnitCode = p.UnitCode,
                                CaseCode = p.CaseCode,
                                CaseName = null,
                                WarehouseCode = p.WarehouseCode,
                                WarehouseName = null,
                                ProductCode = p.ProductCode,
                                ProductName = pro != null ? pro.Name : null,
                                TransWarehouseCode = p.TransWarehouseCode,
                                CreditAcc2 = p.CreditAcc2,
                                Note = p.Description,
                                VoucherGroup = p.VoucherGroup,
                                AccName = null,
                                DepartmentCode = p.DepartmentCode,
                                DepartmentName = null,

                            }).ToList();

            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                incurredData = incurredData.Where(p => p.PartnerCode == dto.Parameters.PartnerCode).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroupCode))
            {
                var accPartner = await _partnerGroupService.GetQueryableAsync();
                var lstaccPartner = accPartner.Where(p => p.Id == dto.Parameters.PartnerGroupCode).ToList();
                if (lstaccPartner.Count > 0)
                {
                    incurredData = incurredData.Where(p => lstaccPartner.Select(p => p.Code).Contains(p.PartnerCode)).ToList();
                }

            }
            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var lstProductGroups = lstProductGroup.Where(p => p.Id == dto.Parameters.ProductGroupCode).ToList();
                if (lstProductGroups.Count > 0)
                {
                    var code = lstProductGroups.FirstOrDefault().Code;
                    var products = await _productAppService.GetListByProductGroupCode(code);
                    incurredData = incurredData.Where(p => products.Select(p => p.Code).Contains(p.ProductCode)).ToList();
                }

            }
            var sumIncurredData = SumData(incurredData);
            var sumAktGroup = SumAktGroup(incurredData);
            incurredData.AddRange(IncurredData1(sumAktGroup));
            incurredData.AddRange(IncurredData2(sumAktGroup));
            incurredData.AddRange(IncurredData3(sumAktGroup));
            incurredData.AddRange(IncurredData4(sumAktGroup));
            incurredData.AddRange(IncurredData5(sumAktGroup));
            incurredData.AddRange(IncurredData6(sumAktGroup));
            incurredData.AddRange(IncurredData8(sumIncurredData));
            incurredData.AddRange(IncurredData9(sumIncurredData));
            incurredData.AddRange(IncurredData10(sumIncurredData));
            incurredData.AddRange(IncurredData11(sumIncurredData));

            var resul = incurredData.OrderBy(p => p.GroupCode)
                                    .ThenBy(p => p.Sort)
                                    .ThenBy(p => p.Sort0)
                                    .ToList();


            var reportResponse = new ReportResponseDto<SalesDetailsBookDto>();
            reportResponse.Data = resul;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public List<SalesDetailsBookDto> SumData(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    group new
                                    {
                                        p.OrgCode,
                                        p.Year,
                                        p.Amount,
                                        p.AmountCur,
                                        p.DiscountAmount,
                                        p.DiscountAmountCur,
                                        p.ExportAmount,
                                        p.ExportAmountCur,
                                        p.VatAmount,
                                        p.VatAmountCur,
                                        p.GroupCode,
                                        p.Note
                                    } by new
                                    {
                                        p.OrgCode,
                                        p.Year
                                    } into gr
                                    select new SalesDetailsBookDto
                                    {
                                        GroupCode = gr.Max(p => p.GroupCode),
                                        Amount = gr.Sum(p => p.Amount),
                                        AmountCur = gr.Sum(p => p.AmountCur),
                                        DiscountAmount = gr.Sum(p => p.DiscountAmount),
                                        VatAmount = gr.Sum(p => p.VatAmount),
                                        VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                        DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                                        TotalAmount = gr.Sum(p => p.Amount) - gr.Sum(p => p.DiscountAmount) + gr.Sum(p => p.VatAmount),
                                        TotalAmountCur = gr.Sum(p => p.AmountCur) - gr.Sum(p => p.DiscountAmountCur) + gr.Sum(p => p.VatAmountCur),
                                        Note = gr.Max(p => p.Note)
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> SumAktGroup(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from a in incurredData
                                    group new
                                    {
                                        a.VoucherId,
                                        a.VoucherDate,
                                        a.VoucherNumber,
                                        a.Description,
                                        a.PartnerName,
                                        a.Amount,
                                        a.AmountCur,
                                        a.DiscountAmount,
                                        a.DiscountAmountCur,
                                        a.VatAmount,
                                        a.VatAmountCur,
                                        a.PartnerCode,
                                        a.GroupCode,
                                        a.Note
                                    } by new
                                    {
                                        //a.VoucherId,
                                        a.VoucherDate,
                                        a.VoucherNumber,
                                        //a.Description,
                                        a.PartnerName,
                                        a.PartnerCode,

                                    } into gr
                                    select new SalesDetailsBookDto
                                    {
                                        Note = gr.Max(p => p.Note),
                                        GroupCode = gr.Max(p => p.GroupCode),
                                        VoucherId = gr.Max(p => p.VoucherId),
                                        VoucherDate = gr.Key.VoucherDate,
                                        VoucherNumber = gr.Key.VoucherNumber,
                                        Description = gr.Max(p => p.Description),
                                        PartnerName = gr.Key.PartnerName,
                                        PartnerCode = gr.Key.PartnerCode,
                                        Amount = gr.Sum(p => p.Amount),
                                        AmountCur = gr.Sum(p => p.AmountCur),
                                        DiscountAmount = gr.Sum(p => p.DiscountAmount),
                                        DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                                        VatAmount = gr.Sum(p => p.VatAmount),
                                        VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                        TotalAmount = gr.Sum(p => p.Amount) - gr.Sum(p => p.DiscountAmount) + gr.Sum(p => p.VatAmount),
                                        TotalAmountCur = gr.Sum(p => p.AmountCur) - gr.Sum(p => p.DiscountAmountCur) + gr.Sum(p => p.VatAmountCur)
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData1(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 0,
                                        Sort0 = 0,
                                        Bold = "C",
                                        GroupCode = p.VoucherDate + p.VoucherNumber + p.VoucherId,
                                        VoucherDate = p.VoucherDate,
                                        VoucherNumber = p.VoucherNumber,
                                        Description = p.Note,
                                        PartnerCode = p.PartnerCode,
                                        PartnerName = p.PartnerName
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData2(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 0,
                                        Sort0 = 2,
                                        Bold = "K",

                                        Description = "1.Tổng tiền hàng ",
                                        Amount = p.Amount,
                                        AmountCur = p.AmountCur,
                                        GroupCode = p.VoucherDate + p.VoucherNumber + p.VoucherId,
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData3(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 0,
                                        Sort0 = 3,
                                        Bold = "K",
                                        GroupCode = p.VoucherDate + p.VoucherNumber + p.VoucherId,
                                        Description = "2.Tổng tiền chiết khấu ",
                                        Amount = p.DiscountAmount,
                                        DiscountAmountCur = p.DiscountAmountCur
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData4(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {

                                        Sort = 0,
                                        Sort0 = 4,
                                        Bold = "K",
                                        GroupCode = p.VoucherDate + p.VoucherNumber + p.VoucherId,
                                        Description = "3.Tổng tiền thuế giá trị gia tăng",
                                        Amount = p.VatAmount,
                                        AmountCur = p.VatAmountCur
                                    }).ToList();
            return salesDetailsBookDtos;

        }

        public List<SalesDetailsBookDto> IncurredData5(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 0,
                                        Sort0 = 5,
                                        Bold = "K",
                                        GroupCode = p.VoucherDate + p.VoucherNumber + p.VoucherId,
                                        Description = "4.Tổng tiền thanh toán",
                                        Amount = p.TotalAmount,
                                        AmountCur = p.TotalAmountCur
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData6(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {

                                        Sort = 0,
                                        Sort0 = 6,
                                        Bold = "K",
                                        GroupCode = p.VoucherDate + p.VoucherNumber + p.VoucherId,
                                        Description = "----------------------------",
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData8(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 1,
                                        Sort0 = 1,
                                        Bold = "C",
                                        GroupCode = "zzzzzzzzz",
                                        Amount = p.Amount,
                                        AmountCur = p.AmountCur,
                                        Description = "A.Tổng cộng tiền hàng"
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData9(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 1,
                                        Sort0 = 2,
                                        Bold = "C",
                                        GroupCode = "zzzzzzzzz",
                                        Amount = p.DiscountAmount,
                                        AmountCur = p.DiscountAmountCur,
                                        Description = "B.Tổng cộng tiền chiết khấu",
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData10(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 1,
                                        Sort0 = 3,
                                        Bold = "C",
                                        GroupCode = "zzzzzzzzz",
                                        Amount = p.VatAmount,
                                        AmountCur = p.VatAmountCur,
                                        Description = "C.Tổng cộng tiền thuế GTGT",
                                    }).ToList();
            return salesDetailsBookDtos;

        }
        public List<SalesDetailsBookDto> IncurredData11(List<SalesDetailsBookDto> incurredData)
        {
            List<SalesDetailsBookDto> salesDetailsBookDtos = new List<SalesDetailsBookDto>();
            salesDetailsBookDtos = (from p in incurredData
                                    select new SalesDetailsBookDto
                                    {
                                        Sort = 1,
                                        Sort0 = 4,
                                        Bold = "C",
                                        GroupCode = "zzzzzzzzz",
                                        Amount = p.TotalAmount,
                                        AmountCur = p.TotalAmountCur,
                                        Description = "D.Tổng cộng tiền thanh toán",
                                    }).ToList();
            return salesDetailsBookDtos;

        }

        #endregion
        #region Private
        [Authorize(ReportPermissions.SaleDetailBookReportPrint)]
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
        private async Task<List<SalesDetailsBookDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstvoucherType = voucherType.Where(p => p.Code == "PBH");
            var listVoucher = lstvoucherType.Select(p => p.ListVoucher).FirstOrDefault();
            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.Select(p => new SalesDetailsBookDto()
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
                Amount = p.VoucherGroup == 2 ? p.Amount2 : p.Amount,
                AmountCur = p.VoucherGroup == 2 ? p.AmountCur2 : p.AmountCur,
                VoucherId = p.VoucherId,
                Id = p.Id,
                VoucherCode = p.VoucherCode,
                FProductWorkCode = p.FProductWorkCode,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                Price = p.VoucherGroup == 2 ? p.Price2 : p.Price,
                PriceCur = p.VoucherGroup == 2 ? p.PriceCur2 : p.PriceCur,
                BusinessAcc = p.BusinessAcc,
                DiscountAmount = p.DiscountAmount,
                DiscountAmountCur = p.DiscountAmountCur,
                VatAmount = p.VatAmount,
                VatAmountCur = p.VatAmountCur,
                Price0 = p.Price0,
                ExportAmount = p.ExportAmount,
                ExportAmountCur = p.ExportAmountCur,
                PriceCur0 = p.PriceCur0,
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                PartnerName = null,
                UnitCode = p.UnitCode,
                CaseCode = p.CaseCode,
                CaseName = null,
                WarehouseCode = p.WarehouseCode,
                WarehouseName = null,
                ProductCode = p.ProductCode,
                TransWarehouseCode = p.TransWarehouseCode,
                CreditAcc2 = p.CreditAcc2,
                Note = p.Note,
                VoucherGroup = p.VoucherGroup,
                AccName = null,
                DepartmentCode = p.DepartmentCode,
                DepartmentName = null,


            }).Where(p => listVoucher.Contains(p.VoucherCode) == true).ToList();


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
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);

            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
            }
            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }
            //if (!string.IsNullOrEmpty(dto.PartnerGroup))
            //{
            //    dic.Add(WarehouseBookParameterConst.PartnerGroup, dto.PartnerGroup);
            //}
            //if (!string.IsNullOrEmpty(dto.PartnerCode))
            //{
            //    dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
            //}
            //if (!string.IsNullOrEmpty(dto.ProductGroupCode))
            //{
            //    dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.ProductGroupCode);
            //}
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                dic.Add(WarehouseBookParameterConst.BusinessCode, dto.BusinessCode);
            }
            if (!string.IsNullOrEmpty(dto.LstVoucherCode))
            {
                dic.Add(WarehouseBookParameterConst.LstVoucherCode, dto.LstVoucherCode);
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

