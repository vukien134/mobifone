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
using Microsoft.AspNetCore.Authorization;
using Accounting.Permissions;
using Accounting.Reports.GeneralDiaries;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class SalesTransactionListAppService : AccountingAppService
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
        public SalesTransactionListAppService(ReportDataService reportDataService,
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
        [Authorize(ReportPermissions.SalesTransactionListReportView)]
        public async Task<ReportResponseDto<SalesTransactionListDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstVoucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var voucherType = lstVoucherType.Where(p => p.Code == "PBH").FirstOrDefault();
            if (dto.Parameters.VoucherCode == "" || string.IsNullOrEmpty(dto.Parameters.VoucherCode))
            {
                dto.Parameters.LstVoucherCode = voucherType.ListVoucher;
            }
            var dic = GetWarehouseBookParameter(dto.Parameters);
            var voucherCode2 = lstVoucherType.Where(p => p.Code == "000").FirstOrDefault();
            var incurredData = await GetIncurredData(dic);
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productGroup = await _productGroupService.GetQueryableAsync();
            var lstProductGroup = productGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var caseCode = await _accCaseService.GetQueryableAsync();
            var lstCase = caseCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var warehouse = await _warehouseService.GetQueryableAsync();
            var lstWarehouse = warehouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());

            var department = await _departmentService.GetQueryableAsync();
            var lstDepartment = department.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstfProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var saleChannels = await _saleChannelService.GetQueryableAsync();
            var lstSaleChannel = saleChannels.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            incurredData = (from p in incurredData
                            join b in lstProduct on p.ProductCode equals b.Code into c
                            from pro in c.DefaultIfEmpty()
                            select new SalesTransactionListDto
                            {
                                Tag = 0,
                                Bold = "K",
                                Sort = 2,
                                OrgCode = p.OrgCode,
                                SortProductGroup = null,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                Ord0 = p.Ord0,
                                VoucherCode = p.VoucherCode,
                                Quantity = voucherCode2.ListVoucher.Contains(p.VoucherCode) == true ? p.ExportQuantity : p.Quantity,
                                Price = voucherCode2.ListVoucher.Contains(p.VoucherCode) == true ? p.Price2 : p.Price,
                                PriceCur = voucherCode2.ListVoucher.Contains(p.VoucherCode) == true ? p.PriceCur2 : p.PriceCur,
                                VoucherDate = p.VoucherDate,
                                VoucherDate01 = p.VoucherDate,
                                ProductCode = p.ProductCode,
                                ProductName = pro != null ? pro.Name : p.ProductName,
                                PartnerCode = p.PartnerCode,
                                ProductGroupCode = p.ProductGroupCode,
                                Unit = p.Unit,
                                SectionCode = p.SectionCode,
                                FProductWorkCode = p.FProductWorkCode,
                                CaseCode = p.CaseCode,
                                DepartmentCode = p.DepartmentCode,
                                WarehouseCode = p.WarehouseCode,
                                SalesChannelCode = p.SalesChannelCode,
                                ProductOriginCode = p.ProductOriginCode,
                                WorkPlaceCode = p.WorkPlaceCode,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                VoucherNumber = p.VoucherNumber,
                                VoucherNumber1 = p.VoucherNumber,
                                Note = p.Note,
                                Description = p.Description,
                                DescriptionE = p.DescriptionE,
                                AccCode = p.AccCode,
                                ImportQuantity = p.ImportQuantity,
                                ExportQuantity = p.ExportQuantity,
                                Price2 = p.Price2,
                                PriceCur2 = p.PriceCur2,
                                Amount = voucherCode2.ListVoucher.Contains(p.VoucherCode) == true ? p.Amount2 : p.Amount,
                                AmountCur = voucherCode2.ListVoucher.Contains(p.VoucherCode) == true ? p.AmountCur2 : p.AmountCur,
                                Amount2 = p.Amount2,
                                AmountCur2 = p.AmountCur2,
                                Price0 = p.Price0,
                                PriceCur0 = p.PriceCur0,
                                ExportAmountCur = p.ExportAmountCur,
                                DevaluationAmountCur = 0,
                                DevaluationAmount = 0,
                                DiscountAmount = p.DiscountAmount,
                                DiscountAmountCur = p.DiscountAmountCur,
                                ExpenseAmountCur0 = p.ExpenseAmountCur0,
                                ExpenseAmount0 = p.ExpenseAmount0,
                                ExpenseAmount1 = p.ExpenseAmount1,
                                ExpenseAmountCur1 = p.ExpenseAmountCur1,
                                ExpenseAmount = p.ExpenseAmount,
                                ExprenseAmountCur = p.ExprenseAmountCur,
                                AmountVat = p.AmountVat,
                                AmountVatCur = p.AmountVatCur,
                                AmountPay = voucherCode2.ListVoucher.Contains(p.VoucherCode) == true ? p.Amount + p.Amount2 + p.AmountVat : 0,
                                AmountPayCur = voucherCode2.ListVoucher.Contains(p.VoucherCode) == true ? p.AmountCur + p.AmountCur2 + p.AmountVatCur : 0,
                                DebitAcc2 = p.DebitAcc2,
                                CreditAcc2 = p.CreditAcc2,
                                InvoiceNumber = p.InvoiceNumber,
                                InvoiceSymbol = p.InvoiceSymbol,
                                ReciprocalAcc = p.ReciprocalAcc,
                                DebitAcc111 = p.DebitAcc111,
                                DebitAcc131 = p.DebitAcc131,
                                CaseName = null,
                                PartnerName = null,
                                SectionName = null,
                                DepartmentName = null,
                                FProductWorkName = null,
                                CurrencyName = null,
                                AccName = null,
                                SalesChannelName = null,
                                Representative = p.Representative,
                                RankProduct = 0,
                                Address = p.Address,
                                TotalAmount2 = p.TotalAmount2,
                                AmountFundsCur = p.AmountFundsCur,
                                AmountFunds = p.AmountFunds,
                                Payments = p.Amount2 + p.AmountVat ?? 0,
                                PaymentsCur = p.AmountCur2 + p.AmountVatCur ?? 0,
                                Profit = p.Amount2 - p.AmountFunds ?? 0
                            }).ToList();

            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = incurredData.Where(p => GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber)
                                 && GetVoucherNumber(dto.Parameters.EndVoucherNumber) <= GetVoucherNumber(p.VoucherNumber)).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var lstPartners = await _accPartnerService.GetByIdAsync(dto.Parameters.PartnerGroup);
                if (lstPartners.Count > 0)
                {
                    var partners = await _accPartnerAppService.GetListByPartnerGroupCode(lstPartners.FirstOrDefault().Code);
                    incurredData = incurredData.Where(p => partners.Select(p => p.Code).Contains(p.PartnerCode)).ToList();
                }

            }
            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var lstProducts = await _productService.GetByProductIdAsync(dto.Parameters.ProductGroupCode);
                if (lstProducts.Count > 0)
                {
                    var products = await _productAppService.GetListByProductGroupCode(lstProducts.FirstOrDefault().Code);
                    incurredData = incurredData.Where(p => products.Select(p => p.Code).Contains(p.ProductCode)).ToList();
                }

            }

            incurredData = (from p in incurredData
                            join b in lstPartner on p.PartnerCode equals b.Code into c
                            from pa in c.DefaultIfEmpty()
                            join i in lstProduct on p.ProductCode equals i.Code into l
                            from pr in l.DefaultIfEmpty()
                            join y in lstDepartment on p.DepartmentCode equals y.DepartmentType into j
                            from de in j.DefaultIfEmpty()
                            join w in lstSaleChannel on p.SalesChannelCode equals w.Code into t
                            from sa in t.DefaultIfEmpty()
                            select new SalesTransactionListDto
                            {
                                Tag = 0,
                                Bold = "K",
                                Sort = 2,
                                OrgCode = p.OrgCode,
                                SortProductGroup = null,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                Ord0 = p.Ord0,
                                VoucherCode = p.VoucherCode,
                                Quantity = p.Quantity,
                                Price = p.Price,
                                PriceCur = p.PriceCur,
                                VoucherDate = p.VoucherDate,
                                VoucherDate01 = p.VoucherDate,
                                ProductCode = p.ProductCode,
                                ProductName = p.ProductName,
                                PartnerCode = p.PartnerCode,
                                ProductGroupCode = p.ProductGroupCode,
                                Unit = p.Unit,
                                SectionCode = p.SectionCode,
                                FProductWorkCode = p.FProductWorkCode,
                                CaseCode = p.CaseCode,
                                DepartmentCode = p.DepartmentCode,
                                WarehouseCode = p.WarehouseCode,
                                SalesChannelCode = p.SalesChannelCode,
                                ProductOriginCode = p.ProductOriginCode,
                                WorkPlaceCode = p.WorkPlaceCode,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                VoucherNumber = p.VoucherNumber,
                                VoucherNumber1 = p.VoucherNumber,
                                Note = p.ProductCode + " - " + p.ProductName,
                                Description = p.Description,
                                DescriptionE = p.DescriptionE,
                                AccCode = p.AccCode,
                                ImportQuantity = p.ImportQuantity,
                                ExportQuantity = p.ExportQuantity,
                                Price2 = p.VoucherCode == "PDV" ? p.Price0 : p.Price2,
                                PriceCur2 = p.PriceCur2,
                                Amount = p.Amount,
                                AmountCur = p.AmountCur,
                                Amount2 = p.VoucherCode == "PDV" ? p.Amount : p.Amount2,
                                AmountCur2 = p.AmountCur2,
                                Price0 = p.Price0,
                                PriceCur0 = p.PriceCur0,
                                ExportAmountCur = p.ExportAmountCur,
                                DevaluationAmountCur = 0,
                                DevaluationAmount = 0,
                                DiscountAmount = p.DiscountAmount,
                                DiscountAmountCur = p.DiscountAmountCur,
                                ExpenseAmountCur0 = p.ExpenseAmountCur0,
                                ExpenseAmount0 = p.ExpenseAmount0,
                                ExpenseAmount1 = p.ExpenseAmount1,
                                ExpenseAmountCur1 = p.ExpenseAmountCur1,
                                ExpenseAmount = p.ExpenseAmount,
                                ExprenseAmountCur = p.ExprenseAmountCur,
                                AmountVat = p.AmountVat,
                                AmountVatCur = p.AmountVatCur,
                                AmountPay = p.AmountPay,
                                AmountPayCur = p.AmountPayCur,
                                DebitAcc2 = p.DebitAcc2,
                                CreditAcc2 = p.CreditAcc2,
                                InvoiceNumber = p.InvoiceNumber,
                                InvoiceSymbol = p.InvoiceSymbol,
                                ReciprocalAcc = p.ReciprocalAcc,
                                DebitAcc111 = p.DebitAcc111,
                                DebitAcc131 = p.DebitAcc131,
                                CaseName = null,
                                PartnerName = pa != null ? pa.Name : null,
                                SectionName = null,
                                DepartmentName = de != null ? de.Name : null,
                                FProductWorkName = null,
                                CurrencyName = null,
                                AccName = null,
                                SalesChannelName = sa != null ? sa.Name : null,
                                Representative = p.Representative,
                                RankProduct = 0,
                                Address = p.Address,
                                TotalAmount2 = p.TotalAmount2,
                                AmountFundsCur = p.AmountFundsCur,
                                AmountFunds = p.AmountFunds,
                                Payments = p.Payments,
                                PaymentsCur = p.PaymentsCur,
                                Profit = p.Profit
                            }).ToList();
            var totalQuantity = incurredData.Select(p => p.Quantity).Sum();
            var totalAmount = incurredData.Select(p => p.Amount).Sum();
            var amountFunds = incurredData.Select(p => p.AmountFunds).Sum();// tien_von
            var amountFundsCur = incurredData.Select(p => p.AmountFundsCur).Sum();
            var totalDevaluationAmountCur = incurredData.Select(p => p.DevaluationAmountCur).Sum();
            var totalDevaluationAmount = incurredData.Select(p => p.DevaluationAmount).Sum();
            var totalDiscountAmount = incurredData.Select(p => p.DiscountAmount).Sum();
            var totalDiscountAmountCur = incurredData.Select(p => p.DiscountAmountCur).Sum();
            var totalExpenseAmount = incurredData.Select(p => p.ExpenseAmount).Sum();
            var totalExpenseAmountCur = incurredData.Select(p => p.ExprenseAmountCur).Sum();
            var totalAmountPay = incurredData.Select(p => p.AmountPay).Sum();
            var totalAmountPayCur = incurredData.Select(p => p.AmountPayCur).Sum();
            var totalAmount2 = incurredData.Select(p => p.Amount2).Sum();
            var totalAmountVat = incurredData.Select(p => p.AmountVat).Sum();
            var totalProfist = incurredData.Select(p => p.Profit).Sum();
            var result = dto.Parameters.Sort switch
            {
                "1" => this.DataSortByDate(incurredData),
                "2" => this.DataSortByProduct(incurredData),
                "3" => this.DataSortByPartner(incurredData),
                "4" => this.DataSortBySection(incurredData),
                "5" => this.DataSortByWareHouse(incurredData),
                "6" => this.DataSortByAcc(incurredData),
                "7" => this.DataSortByDepartmen(incurredData),
                "8" => this.DataSortByFproductWork(incurredData),
                _ => this.DataSortByProductGroup(incurredData)
            };


            List<SalesTransactionListDto> salesTransactionListDtos = new List<SalesTransactionListDto>();
            SalesTransactionListDto salesTransactionListDto = new SalesTransactionListDto();
            salesTransactionListDto.Tag = 3;
            salesTransactionListDto.Bold = "C";
            salesTransactionListDto.Sort = 3;
            salesTransactionListDto.ProductGroupCode = "zzzzzzz";
            salesTransactionListDto.Quantity = totalQuantity;
            salesTransactionListDto.DiscountAmount = totalDiscountAmount;
            salesTransactionListDto.DiscountAmountCur = totalDiscountAmountCur;
            salesTransactionListDto.ExpenseAmount = totalExpenseAmount;
            salesTransactionListDto.ExprenseAmountCur = totalExpenseAmountCur;
            salesTransactionListDto.AmountFunds = amountFunds;
            salesTransactionListDto.AmountFundsCur = amountFundsCur;
            salesTransactionListDto.DevaluationAmount = totalDevaluationAmount;
            salesTransactionListDto.DevaluationAmountCur = totalDevaluationAmountCur;
            salesTransactionListDto.Note = "Tổng cộng";
            salesTransactionListDto.Payments = totalAmountPay;
            salesTransactionListDto.Amount2 = totalAmount2;
            salesTransactionListDto.AmountVat = totalAmountVat;
            salesTransactionListDto.Profit = totalProfist;
            salesTransactionListDtos.Add(salesTransactionListDto);
            incurredData.AddRange(salesTransactionListDtos);
            var resul = incurredData.OrderBy(p => p.ProductGroupCode)
                                    .ThenBy(p => p.Sort)
                                    .ThenBy(p => p.Bold)
                                    .ToList();

            var reportResponse = new ReportResponseDto<SalesTransactionListDto>();
            reportResponse.Data = resul;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
        [Authorize(ReportPermissions.SalesTransactionListReportPrint)]
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
        private async Task<List<SalesTransactionListDto>> DataSortByDate(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.VoucherDate
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.VoucherDate.ToString(),
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.Description)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.VoucherDate.ToString();
            });
            incurredData.AddRange(sumAktGroup0);
            return incurredData;
        }
        private async Task<List<SalesTransactionListDto>> DataSortByProduct(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.ProductCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.ProductCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.Description)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.ProductCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return incurredData;
        }
        private async Task<List<SalesTransactionListDto>> DataSortByPartner(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.PartnerCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.PartnerCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.PartnerCode) == null ? "Không có đối tượng" : gr.Max(p => p.p.PartnerCode) + " - " + gr.Max(p => p.p.PartnerName)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.PartnerCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return incurredData;
        }

        private Task<List<SalesTransactionListDto>> DataSortBySection(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.SectionCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.SectionCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.SectionCode) == null ? "Không có vụ việc" : gr.Max(p => p.p.SectionCode) + " - " + gr.Max(p => p.p.SectionName)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.SectionCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return Task.FromResult(incurredData);
        }

        private Task<List<SalesTransactionListDto>> DataSortByWareHouse(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.WarehouseCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.WarehouseCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.WarehouseCode) == null ? "Không có kho" : gr.Max(p => p.p.WarehouseCode) + " - " + gr.Max(p => p.p.WarehouseName)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.WarehouseCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return Task.FromResult(incurredData);
        }
        private Task<List<SalesTransactionListDto>> DataSortByAcc(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.AccCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.AccCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.AccCode) == null ? "Không có tài khoản" : gr.Max(p => p.p.AccCode) + " - " + gr.Max(p => p.p.AccName)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.AccCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return Task.FromResult(incurredData);
        }
        private Task<List<SalesTransactionListDto>> DataSortByDepartmen(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.DepartmentCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.DepartmentCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = string.IsNullOrEmpty(gr.Max(p => p.p.DepartmentCode).ToString()) == true ? "Không có bộ phận" : gr.Max(p => p.p.DepartmentCode) + " - " + gr.Max(p => p.p.DepartmentName)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.DepartmentCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return Task.FromResult(incurredData);
        }
        private Task<List<SalesTransactionListDto>> DataSortByFproductWork(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.FProductWorkCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.FProductWorkCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.FProductWorkCode) == null ? "Không có chứng từ sản phẩm" : gr.Max(p => p.p.FProductWorkCode) + " - " + gr.Max(p => p.p.FProductWorkName)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.FProductWorkCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return Task.FromResult(incurredData);
        }
        private Task<List<SalesTransactionListDto>> DataSortByProductGroup(List<SalesTransactionListDto> incurredData)
        {
            var sumAktGroup0 = from p in incurredData
                               group new
                               {
                                   p
                               }
                               by new
                               {
                                   p.ProductGroupCode
                               } into gr
                               select new SalesTransactionListDto
                               {
                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   ProductCode = gr.Max(p => p.p.ProductCode),
                                   ProductGroupCode = gr.Key.ProductGroupCode,
                                   Quantity = gr.Sum(p => p.p.Quantity),
                                   Amount = gr.Sum(p => p.p.Amount),
                                   AmountCur = gr.Sum(p => p.p.AmountCur),
                                   ExpenseAmount = gr.Sum(p => p.p.ExpenseAmount),
                                   ExprenseAmountCur = gr.Sum(p => p.p.ExprenseAmountCur),
                                   DevaluationAmount = gr.Sum(p => p.p.DevaluationAmount),
                                   DevaluationAmountCur = gr.Sum(p => p.p.DevaluationAmountCur),
                                   DiscountAmount = gr.Sum(p => p.p.DiscountAmount),
                                   DiscountAmountCur = gr.Sum(p => p.p.DiscountAmountCur),
                                   AmountVat = gr.Sum(p => p.p.AmountVat),
                                   AmountVatCur = gr.Sum(p => p.p.AmountVatCur),
                                   AmountPay = gr.Sum(p => p.p.AmountPay),
                                   AmountPayCur = gr.Sum(p => p.p.AmountPayCur),
                                   AmountFunds = gr.Sum(p => p.p.AmountFunds),
                                   AmountFundsCur = gr.Sum(p => p.p.AmountFundsCur),
                                   Payments = gr.Sum(p => p.p.Payments),
                                   Profit = gr.Sum(p => p.p.Profit),
                                   Amount2 = gr.Sum(p => p.p.Amount2),
                                   Note = gr.Max(p => p.p.ProductGroupCode) == null ? "Không có nhóm hàng hoá" : gr.Max(p => p.p.ProductGroupCode) + " - " + gr.Max(p => p.p.ProductGroupName)
                               };


            Parallel.ForEach(incurredData, item =>
            {
                item.ProductGroupCode = item.ProductGroupCode;
            });
            incurredData.AddRange(sumAktGroup0);
            return Task.FromResult(incurredData);
        }
        private async Task<List<SalesTransactionListDto>> GetIncurredData(Dictionary<string, object> dic)
        {

            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.Select(p => new SalesTransactionListDto()
            {
                Tag = 0,
                Bold = "K",
                Sort = 2,
                OrgCode = p.OrgCode,
                SortProductGroup = null,
                VoucherId = p.VoucherId,
                Id = p.Id,
                Ord0 = p.Ord0,
                VoucherCode = p.VoucherCode,
                VoucherDate = p.VoucherDate,
                VoucherDate01 = p.VoucherDate,
                ProductCode = p.ProductCode,
                ProductName = null,
                PartnerCode = p.PartnerCode == "" || string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode0 : p.PartnerCode,
                ProductGroupCode = null,
                Unit = p.UnitCode,
                SectionCode = p.SectionCode,
                FProductWorkCode = p.FProductWorkCode,
                CaseCode = p.CaseCode,
                Quantity = p.Quantity,
                DepartmentCode = p.DepartmentCode,
                WarehouseCode = p.WarehouseCode,
                SalesChannelCode = p.SalesChannelCode,
                ProductOriginCode = p.ProductOriginCode,
                WorkPlaceCode = p.WorkPlaceCode,
                CurrencyCode = p.CurrencyCode,
                ExchangeRate = p.ExchangeRate,
                VoucherNumber = p.VoucherNumber,
                VoucherNumber1 = p.VoucherNumber,
                Note = p.ProductCode,
                Description = p.Description,
                DescriptionE = p.DescriptionE,
                AccCode = p.ExportAcc,
                ImportQuantity = p.ImportQuantity,
                ExportQuantity = p.ExportQuantity,
                Price2 = p.Price2,
                Price = p.Price,
                PriceCur2 = p.PriceCur2,
                Amount = p.Amount,
                AmountCur = p.AmountCur,
                Amount2 = p.Amount2,
                AmountCur2 = p.AmountCur2,
                Price0 = p.Price0,
                PriceCur0 = p.PriceCur0,
                ExportAmountCur = p.ExportAmountCur,
                DevaluationAmountCur = 0,
                DevaluationAmount = 0,
                DiscountAmount = p.DiscountAmount,
                DiscountAmountCur = p.DiscountAmountCur,
                ExpenseAmountCur0 = p.ExpenseAmountCur0,
                ExpenseAmount0 = p.ExpenseAmount0,
                ExpenseAmount1 = p.ExpenseAmount1,
                ExpenseAmountCur1 = p.ExpenseAmountCur1,
                ExpenseAmount = p.ExpenseAmount,
                ExprenseAmountCur = p.ExprenseAmountCur,
                AmountVat = p.VatAmount,
                AmountVatCur = p.VatAmount,
                AmountPay = 0,
                AmountPayCur = 0,
                DebitAcc2 = p.DebitAcc2,
                CreditAcc2 = p.CreditAcc2,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceSymbol = p.InvoiceSymbol,
                ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc2 : p.CreditAcc2,
                DebitAcc111 = !string.IsNullOrEmpty(p.DebitAcc2) ? p.DebitAcc2.Contains("111") == true ? p.Amount2 : 0 : 0,
                DebitAcc131 = !string.IsNullOrEmpty(p.DebitAcc2) ? p.DebitAcc2.Contains("131") == true ? p.Amount2 : 0 : 0,
                CaseName = null,
                PartnerName = null,
                SectionName = null,
                DepartmentName = null,
                FProductWorkName = null,
                CurrencyName = null,
                AccName = null,
                SalesChannelName = null,
                Representative = p.Representative,
                RankProduct = 0,
                Address = p.Address,
                TotalAmount2 = p.TotalAmount2,
                AmountFundsCur = p.ExportAmountCur,
                AmountFunds = p.ExportAmount
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
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
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
            if (!string.IsNullOrEmpty(dto.SaleChannel))
            {
                dic.Add(WarehouseBookParameterConst.SaleChannel, dto.SaleChannel);
            }

            if (!string.IsNullOrEmpty(dto.LstVoucherCode))
            {
                dic.Add(WarehouseBookParameterConst.LstVoucherCode, dto.LstVoucherCode);
            }
            if (!string.IsNullOrEmpty(dto.Sort))
            {
                dic.Add(WarehouseBookParameterConst.Sort, dto.Sort);
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

