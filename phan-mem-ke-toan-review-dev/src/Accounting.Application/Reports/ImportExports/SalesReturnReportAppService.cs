using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.BusinessCategories;
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
using Org.BouncyCastle.Bcpg;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class SalesReturnReportAppService : AccountingAppService
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
        private readonly ProductGroupAppService _productGroupAppService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public SalesReturnReportAppService(ReportDataService reportDataService,
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
                        ProductGroupAppService productGroupAppService,
                        VoucherCategoryService voucherCategoryService,
                        VoucherTypeService voucherTypeService,
                        WorkPlaceSevice workPlaceSevice,
                        BusinessCategoryService businessCategoryService,
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
            _productGroupAppService = productGroupAppService;
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _workPlaceSevice = workPlaceSevice;
            _businessCategoryService = businessCategoryService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.SalesReturnReportView)]
        public async Task<ReportResponseDto<SalesReturnReportDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {

            var dic = GetWarehouseBookParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);
            var products = await _productService.GetQueryableAsync();
            var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            incurredData = (from p in incurredData
                            join c in lstProducts on p.ProductCode equals c.Code into m
                            from pr in m.DefaultIfEmpty()
                            join n in lstPartner on p.PartnerCode equals n.Code into k
                            from pn in k.DefaultIfEmpty()
                            where p.VoucherCode == "PTL"
                            select new SalesReturnReportDto
                            {
                                Tag = 0,
                                Sort0 = 3,
                                Bold = "K",
                                OrgCode = p.OrgCode,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                VoucherCode = p.VoucherCode,
                                VoucherDate = p.VoucherDate,
                                VoucherNumber = p.VoucherNumber,
                                GroupCode = p.VoucherDate + p.VoucherNumber,
                                Note = p.Note,
                                PartnerCode = p.PartnerCode,
                                ProductCode = p.ProductCode,
                                WarehouseCode = p.WarehouseCode,
                                Unit = p.Unit,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                Description = p.Description,
                                Price = p.Price,
                                PriceCur = p.PriceCur,
                                DepartmentCode = p.DepartmentCode,
                                SectionCode = p.SectionCode,
                                CaseCode = p.CaseCode,
                                FProductWorkCode = p.FProductWorkCode,
                                WorkPlaceCode = p.WorkPlaceCode,
                                BusinessCode = p.BusinessCode,
                                TransWarehouseCode = p.TransWarehouseCode,
                                ExportQuantity = p.ExportQuantity,
                                ExportAmount = p.ExportAmount,
                                ExportAmountCur = p.ExportAmountCur,
                                Amount = p.Amount,
                                AmountCur = p.AmountCur,
                                DiscountAmount = p.DiscountAmount,
                                DiscountAmountCur = p.DiscountAmountCur,
                                ExpenseAmount = p.ExpenseAmount,
                                ExpenseAmountCur = p.ExpenseAmountCur,
                                AmountFunds = p.AmountFunds,
                                VatAmount = p.VatAmount,
                                VatAmountCur = p.VatAmountCur,
                                AccCode = null,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                InvoiceDate = p.InvoiceDate,
                                InvoiceSymbol = p.InvoiceSymbol,
                                InvoiceNumber = p.InvoiceNumber,
                                PartnerName = pn != null ? pn.Name : null,
                                Quantity = p.Quantity
                            }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var partners = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroup);
                incurredData = incurredData.Where(p => partners.Select(p => p.Code).Contains(p.PartnerCode)).ToList();
            }

            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var lstProduct = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode);
                incurredData = incurredData.Where(p => lstProduct.Select(p => p.Code).Contains(p.ProductCode)).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = incurredData.Where(p =>
                                 GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber)
                                 && GetVoucherNumber(dto.Parameters.EndVoucherNumber) <= GetVoucherNumber(p.VoucherNumber)
                                ).ToList();
            }
            var aktGroup = GetSalesReturnReportDtos(incurredData);
            List<SalesReturnReportDto> salesReturnReportDtos = new List<SalesReturnReportDto>();
            foreach (var item in aktGroup)
            {
                var crud1 = SalesReturnReportDtos1(item);
                salesReturnReportDtos.Add(crud1);

                var crud2 = SalesReturnReportDtos2(item);
                salesReturnReportDtos.Add(crud2);

                var crud3 = SalesReturnReportDtos3(item);
                salesReturnReportDtos.Add(crud3);

                if (item.DiscountAmount != 0 || item.DiscountAmountCur != 0)
                {
                    var crud4 = SalesReturnReportDtos4(item);
                    salesReturnReportDtos.Add(crud4);
                }
                if (item.VatAmount != 0 || item.VatAmountCur != 0)
                {
                    var crud5 = SalesReturnReportDtos5(item);
                    salesReturnReportDtos.Add(crud5);
                }
                if (item.ExpenseAmount != 0 || item.ExpenseAmountCur != 0)
                {
                    var crud6 = SalesReturnReportDtos6(item);
                    salesReturnReportDtos.Add(crud6);
                }
                if (item.TurnoverAmount != 0 || item.TurnoverAmountCur != 0)
                {
                    var crud7 = SalesReturnReportDtos7(item);
                    salesReturnReportDtos.Add(crud7);
                }
                var crud8 = SalesReturnReportDtos8(item);
                salesReturnReportDtos.Add(crud8);

            }

            salesReturnReportDtos.AddRange(incurredData);
            salesReturnReportDtos = salesReturnReportDtos.OrderBy(p => p.GroupCode)
                                                         .ThenBy(p => p.Sort0)
                                                         .ThenBy(p => p.VoucherDate)
                                                         .ThenBy(p => p.VoucherNumber)
                                                         .ToList();
            var reportResponse = new ReportResponseDto<SalesReturnReportDto>();
            reportResponse.Data = salesReturnReportDtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        private List<SalesReturnReportDto> GetSalesReturnReportDtos(List<SalesReturnReportDto> salesReturn)
        {
            var lst = new List<SalesReturnReportDto>();
            lst = (from p in salesReturn
                       //join b in lstPartner on p.PartnerCode equals b.Code into c
                       //from pn in c.DefaultIfEmpty()
                   group new
                   {
                       p.VoucherCode,
                       p.PartnerName,
                       p.DepartmentName,
                       p.Unit,
                       p.Amount,
                       p.AmountCur,
                       p.DiscountAmount,
                       p.DiscountAmountCur,
                       p.VatAmount,
                       p.VatAmountCur,
                       p.ExpenseAmount,
                       p.ExpenseAmountCur
                   } by new
                   {
                       p.VoucherNumber,
                       p.VoucherDate,
                       p.Description,
                       p.BusinessCode,
                       p.DepartmentName
                   } into gr
                   select new SalesReturnReportDto
                   {


                       VoucherDate = gr.Key.VoucherDate,
                       VoucherNumber = gr.Key.VoucherNumber,
                       GroupCode = gr.Key.VoucherDate + gr.Key.VoucherNumber,
                       Note = gr.Max(p => p.PartnerName) == null ? "Không có đối tượng " : gr.Max(p => p.PartnerName),
                       DepartmentName = gr.Key.DepartmentName,
                       Unit = gr.Max(p => p.Unit),
                       BusinessCode = gr.Key.BusinessCode,
                       Amount = gr.Sum(p => p.Amount),
                       AmountCur = gr.Sum(p => p.AmountCur),
                       DiscountAmount = gr.Sum(p => p.DiscountAmount),
                       DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                       VatAmount = gr.Sum(p => p.VatAmount),
                       VatAmountCur = gr.Sum(p => p.VatAmountCur),
                       ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                       ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                       TurnoverAmount = gr.Sum(p => p.Amount) + gr.Sum(p => p.DiscountAmount) + gr.Sum(p => p.VatAmount),
                       TurnoverAmountCur = gr.Sum(p => p.AmountCur) + gr.Sum(p => p.DiscountAmountCur) + gr.Sum(p => p.VatAmountCur)
                   }).ToList();
            return lst;
        }
        private SalesReturnReportDto SalesReturnReportDtos1(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();
            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 1;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.VoucherDate = salesReturn.VoucherDate;
            salesReturnReportDto.VoucherNumber = salesReturn.VoucherNumber;
            salesReturnReportDto.DepartmentName = salesReturn.DepartmentName;
            salesReturnReportDto.Description = salesReturn.Description;
            salesReturnReportDto.BusinessCode = salesReturn.BusinessCode;
            return salesReturnReportDto;
        }
        private SalesReturnReportDto SalesReturnReportDtos2(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();
            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 1;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.VoucherDate = salesReturn.VoucherDate;
            salesReturnReportDto.VoucherNumber = salesReturn.VoucherNumber;
            salesReturnReportDto.BusinessCode = salesReturn.BusinessCode;
            salesReturnReportDto.Description = "Hàng bán bị trả lại ";
            return salesReturnReportDto;
        }
        private SalesReturnReportDto SalesReturnReportDtos3(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();
            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 4;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.Amount = salesReturn.Amount;
            salesReturnReportDto.AmountCur = salesReturn.AmountCur;
            salesReturnReportDto.BusinessCode = salesReturn.BusinessCode;
            salesReturnReportDto.Description = 1 + ".Tổng tiền hàng ";
            return salesReturnReportDto;
        }
        private SalesReturnReportDto SalesReturnReportDtos4(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();
            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 5;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.Amount = salesReturn.Amount;
            salesReturnReportDto.AmountCur = salesReturn.AmountCur;
            salesReturnReportDto.BusinessCode = salesReturn.BusinessCode;
            salesReturnReportDto.Description = 2 + ".Tổng tiền chiết khấu";
            return salesReturnReportDto;
        }
        private SalesReturnReportDto SalesReturnReportDtos5(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();

            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 6;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.Amount = salesReturn.VatAmount;
            salesReturnReportDto.AmountCur = salesReturn.VatAmountCur;
            salesReturnReportDto.BusinessCode = salesReturn.BusinessCode;
            salesReturnReportDto.Description = 3 + ".Tổng tiền thuế";
            return salesReturnReportDto;
        }
        private SalesReturnReportDto SalesReturnReportDtos6(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();
            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 7;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.Amount = salesReturn.ExpenseAmount;
            salesReturnReportDto.AmountCur = salesReturn.ExpenseAmountCur;
            salesReturnReportDto.BusinessCode = salesReturn.BusinessCode;
            salesReturnReportDto.Description = 4 + ".Tổng tiền chi phí";
            return salesReturnReportDto;
        }
        private SalesReturnReportDto SalesReturnReportDtos7(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();
            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 8;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.Amount = salesReturn.ExpenseAmount;
            salesReturnReportDto.AmountCur = salesReturn.ExpenseAmountCur;
            salesReturnReportDto.BusinessCode = salesReturn.BusinessCode;
            salesReturnReportDto.Description = 5 + ".Tổng tiền thanh toán ";
            return salesReturnReportDto;
        }
        private SalesReturnReportDto SalesReturnReportDtos8(SalesReturnReportDto salesReturn)
        {
            SalesReturnReportDto salesReturnReportDto = new SalesReturnReportDto();
            salesReturnReportDto.Tag = 0;
            salesReturnReportDto.Bold = "K";
            salesReturnReportDto.Sort0 = 9;
            salesReturnReportDto.GroupCode = salesReturn.GroupCode;
            salesReturnReportDto.Description = "---------------------------";
            return salesReturnReportDto;
        }
        #endregion
        [Authorize(ReportPermissions.SalesReturnReportPrint)]
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



        #region Private
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
        private async Task<AccountBalanceDto> GetOpeningBalance(Dictionary<string, object> dic)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime fromDate = Convert.ToDateTime(dic[LedgerParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, yearCategory.Year);
            }
            dic[LedgerParameterConst.Year] = yearCategory.Year;

            var openingBalances = await _reportDataService.GetAccountBalancesAsync(dic);
            var balances = new AccountBalanceDto()
            {
                Debit = openingBalances.Sum(p => p.Debit),
                Credit = openingBalances.Sum(p => p.Credit),
                DebitCur = openingBalances.Sum(p => p.DebitCur),
                CreditCur = openingBalances.Sum(p => p.CreditCur)
            };

            return balances;
        }
        private async Task<List<SalesReturnReportDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);

            var incurredData = warehouseBook.Select(p => new SalesReturnReportDto()
            {
                Tag = 0,
                Sort0 = 3,
                Bold = "K",
                OrgCode = p.OrgCode,
                VoucherId = p.VoucherId,
                Id = p.Id,
                VoucherCode = p.VoucherCode,
                VoucherDate = p.VoucherDate,
                VoucherNumber = p.VoucherNumber,
                GroupCode = null,
                Note = p.Note,
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                ProductCode = p.ProductCode,
                WarehouseCode = p.WarehouseCode,
                Unit = p.UnitCode,
                CurrencyCode = p.CurrencyCode,
                ExchangeRate = p.ExchangeRate,
                Description = p.Description,
                Price = p.Price2,
                PriceCur = p.PriceCur2,
                DepartmentCode = p.DepartmentCode,
                SectionCode = p.SectionCode,
                CaseCode = p.CaseCode,
                FProductWorkCode = p.FProductWorkCode,
                WorkPlaceCode = p.WorkPlaceCode,
                BusinessCode = p.BusinessCode,
                TransWarehouseCode = p.TransWarehouseCode,
                ExportQuantity = p.ExportQuantity,
                ExportAmount = p.ExportAmount,
                ExportAmountCur = p.ExportAmountCur,
                Amount = p.Amount2,
                AmountCur = p.AmountCur2,
                DiscountAmount = p.DiscountAmount,
                DiscountAmountCur = p.DiscountAmountCur,
                ExpenseAmount = p.ExpenseAmount,
                ExpenseAmountCur = p.ExpenseAmountCur,
                AmountFunds = p.ImportAmount,
                VatAmount = p.VatAmount,
                VatAmountCur = p.VatAmountCur,
                AccCode = null,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                Quantity = p.Quantity,



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
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");

            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            }

            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(WarehouseBookParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.ReciprocalAcc))
            {
                dic.Add(WarehouseBookParameterConst.ReciprocalAcc, dto.ReciprocalAcc);
            }
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }
            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
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
            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(WarehouseBookParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
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

