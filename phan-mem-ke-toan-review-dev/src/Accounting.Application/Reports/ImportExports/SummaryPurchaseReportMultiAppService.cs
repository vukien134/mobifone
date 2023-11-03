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
using Accounting.Catgories.Others.Warehouses;
using NPOI.SS.Formula.Functions;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class SummaryPurchaseReportMultiAppService : AccountingAppService
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
        private readonly CurrencyService _currencyService;
        private readonly SaleChannelService _saleChannelService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public SummaryPurchaseReportMultiAppService(ReportDataService reportDataService,
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
                        CurrencyService currencyService,
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
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _productGroupAppService = productGroupAppService;
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _workPlaceSevice = workPlaceSevice;
            _businessCategoryService = businessCategoryService;
            _currencyService = currencyService;
            _saleChannelService = saleChannelService;
            _accountingCacheManager = accountingCacheManager;
        }
        [Authorize(ReportPermissions.SummaryPurchaseMultiReportView)]
        public async Task<ReportResponseDto<SummaryPurchaseReportMultiDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {

            var lstVoucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            if (dto.Parameters.Type == "NH")
            {
                dto.Parameters.LstVoucherCode = lstVoucherType.Where(p => p.Code == "PNH").FirstOrDefault().ListVoucher;
            }
            if (dto.Parameters.Type == "BH")
            {
                dto.Parameters.LstVoucherCode = lstVoucherType.Where(p => p.Code == "PBH").FirstOrDefault().ListVoucher;
            }
            if (dto.Parameters.Type == "XH")
            {
                dto.Parameters.LstVoucherCode = lstVoucherType.Where(p => p.Code == "PXH").FirstOrDefault().ListVoucher;
            }
            var dic = GetWarehouseBookParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);

            var products = await _productService.GetQueryableAsync();
            var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var caseCodes = await _accCaseService.GetQueryableAsync();
            var lstCaseCode = caseCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var sessionCodes = await _accSectionService.GetQueryableAsync();
            var lstSessionCode = sessionCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var partnerCode = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partnerCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var departments = await _departmentService.GetQueryableAsync();
            var lstDepartmen = departments.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstFProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var currencys = await _currencyService.GetQueryableAsync();
            var lstCurrency = currencys.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accCodes = await _accountSystemService.GetQueryableAsync();
            var lstAccCode = accCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();


            var salesChannels = await _saleChannelService.GetQueryableAsync();
            var lstsalesChannels = salesChannels.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();


            var businessCodes = await _businessCategoryService.GetQueryableAsync();
            var lstBusinessCodes = businessCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            incurredData = (from p in incurredData
                            join c in lstProducts on p.ProductCode equals c.Code into m
                            from pr in m.DefaultIfEmpty()
                            join b in lstCaseCode on p.CaseCode equals b.Code into i
                            from ca in i.DefaultIfEmpty()
                            join d in lstSessionCode on p.SectionCode equals d.Code into u
                            from se in u.DefaultIfEmpty()
                            join y in lstDepartmen on p.DepartmentCode equals y.Code into q
                            from de in q.DefaultIfEmpty()
                            join e in lstFProductWork on p.FProductWorkCode equals e.Code into r
                            from fp in r.DefaultIfEmpty()
                            join t in lstCurrency on p.CurrencyCode equals t.Code into g
                            from cu in g.DefaultIfEmpty()
                            join n in lstAccCode on p.ReciprocalAcc equals n.AccCode into k
                            from acc in k.DefaultIfEmpty()
                            join a in lstsalesChannels on p.SalesChannelCode equals a.Code into h
                            from sa in h.DefaultIfEmpty()
                            join mn in lstPartner on p.PartnerCode equals mn.Code into ps
                            from pa in ps.DefaultIfEmpty()
                            join ty in lstBusinessCodes on p.BusinessCode equals ty.Code into pe
                            from bu in pe.DefaultIfEmpty()

                            select new SummaryPurchaseReportMultiDto
                            {
                                Tag = 0,
                                Sort0 = 2,
                                Bold = "K",
                                Rank = 3,
                                OrgCode = p.OrgCode,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                PartnerCode = p.PartnerCode,
                                ProductCode = p.ProductCode,
                                Unit = p.Unit,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                Note = p.Note,
                                ImportQuantity = p.ImportQuantity,
                                Price = p.Price,
                                PriceCur = p.PriceCur,
                                ImportAmount = p.ImportAmount,
                                ImportAmountCur = p.ImportAmountCur,
                                PartnerName = pa != null ? pa.Name : null,
                                DepartmentCode = p.DepartmentCode,
                                DepartmentName = de != null ? de.Name : null,
                                ProductLotCode = p.ProductLotCode,
                                ProductOriginCode = p.ProductOriginCode,
                                BusinessCode = p.BusinessCode,
                                ExportQuantity = p.Quantity,
                                ExportAmount = p.Amount,
                                ExportAmountCur = p.AmountCur,
                                QuantitySale = 0,
                                Price2 = p.Price2,
                                PriceCur2 = p.PriceCur2,
                                AmountSale = p.AmountSale ?? 0,
                                Amount = p.Amount ?? 0,
                                AmountCur = p.AmountCur ?? 0,
                                DiscountAmount = p.DiscountAmount ?? 0,
                                DiscountAmountCur = p.DiscountAmountCur ?? 0,
                                VatAmount = p.VatAmount ?? 0,
                                VatAmountCur = p.VatAmountCur ?? 0,
                                TotalAmountCur = p.TotalAmountCur,
                                TotalAmount = p.TotalAmount,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                InvoiceDate = p.InvoiceDate,
                                InvoiceSymbol = p.InvoiceSymbol,
                                InvoiceNumber = p.InvoiceNumber,
                                ReciprocalAcc = p.ReciprocalAcc,
                                GroupCode = dto.Parameters.DetailBy == "1" ? p.PartnerCode : p.ProductCode,
                                CaseCode = p.CaseCode,
                                FProductWorkCode = p.FProductWorkCode,
                                SectionCode = p.SectionCode,
                                CaseName = ca != null ? ca.Name : null,
                                FProductWorkName = fp != null ? fp.Name : null,
                                SectionName = se != null ? se.Name : null,
                                GroupCode1 = null,
                                GroupCode2 = null,
                                GroupName1 = null,
                                GroupName2 = null,
                                NoteHtml = null,
                                ProductName = pr != null ? pr.Name : null,
                                Quantity = p.Quantity,
                            }).ToList();

            var groupData1 = await this.GroupData1(incurredData, dto.Parameters.Sort1.ToString());
            var result = new List<SummaryPurchaseReportMultiDto>();
            foreach (var group1 in groupData1)
            {
                result.Add(group1);
                var groupData2 = await this.GroupData2(incurredData, dto.Parameters.Sort1.ToString(),
                                            group1.GroupCode, dto.Parameters.Sort2.ToString());
                foreach (var group2 in groupData2)
                {
                    result.Add(group2);
                    var queryable = this.FilterByGroupCode(incurredData.AsQueryable(), dto.Parameters.Sort1.ToString(), group1.GroupCode);
                    //var productVoucher = incurredData.Where(p => p.GroupCode == group1.GroupCode).ToList();
                    var details = await this.GetDetailData(queryable.ToList(), dto.Parameters.Sort2.ToString(), group2.GroupCode, dto.Parameters.DetailBy);
                    result.AddRange(details);
                }
            }

            var totalQuantity = incurredData.Select(p => p.Quantity).Sum();
            var totalAmount = incurredData.Select(p => p.Amount).Sum();
            var totalAmountCur = incurredData.Select(p => p.AmountCur).Sum();
            var totalAmountFunds = incurredData.Select(p => p.AmountFunds).Sum();
            var totalAmountFundsCur = incurredData.Select(p => p.AmountFundsCur).Sum();
            var totalDevaluationAmount = incurredData.Select(p => p.DevaluationAmount).Sum();
            var totalDevaluationAmountCur = incurredData.Select(p => p.DevaluationAmountCur).Sum();
            var totalDiscountAmount = incurredData.Select(p => p.DiscountAmount).Sum();
            var totalDiscountAmountCur = incurredData.Select(p => p.DiscountAmountCur).Sum();
            var totalExpenseAmount = incurredData.Select(p => p.ExpenseAmount).Sum();
            var toatlExpenseAmountCur = incurredData.Select(p => p.ExpenseAmountCur).Sum();
            var totalVatAmount = incurredData.Select(p => p.VatAmount).Sum();
            var totalVatAmountCur = incurredData.Select(p => p.VatAmountCur).Sum();
            var TurnoverAmount = totalAmount + totalDiscountAmount + totalVatAmount;
            var TurnoverAmountCur = totalAmountCur + totalDiscountAmountCur + totalVatAmountCur;
            var totalAmountPay = incurredData.Select(p => p.AmountPay).Sum();
            var totalAmountPayCur = incurredData.Select(p => p.AmountPayCur).Sum();
            var totalAmounts = incurredData.Select(p => p.TotalAmount).Sum();
            var totalAmountCurs = incurredData.Select(p => p.TotalAmountCur).Sum();



            SummaryPurchaseReportMultiDto crud = new SummaryPurchaseReportMultiDto();

            crud.Tag = 3;
            crud.Rank = 9;
            crud.Bold = "C";
            crud.GroupCode = "zzzzzz";
            crud.GroupCode1 = "zzzzzzz";
            crud.GroupCode2 = "zzzzzzz";
            crud.Note = "Tổng cộng";
            crud.Quantity = totalQuantity;
            crud.Amount = totalAmount;
            crud.AmountCur = totalAmountCur;
            crud.VatAmount = totalVatAmount;
            crud.VatAmountCur = totalVatAmountCur;
            crud.AmountPay = totalAmountPay;
            crud.AmountPayCur = totalAmountPayCur;
            crud.DevaluationAmount = totalDevaluationAmount;
            crud.DevaluationAmountCur = totalDevaluationAmountCur;
            crud.DiscountAmount = totalDiscountAmount;
            crud.DiscountAmountCur = totalDiscountAmountCur;
            crud.ExpenseAmountCur = toatlExpenseAmountCur;
            crud.ExpenseAmount = totalExpenseAmount;
            crud.AmountFunds = totalAmountFunds;
            crud.AmountFundsCur = totalAmountFundsCur;
            crud.TotalAmount = totalAmounts;
            crud.TotalAmountCur = totalAmountCurs;
            result.Add(crud);

            var reportResponse = new ReportResponseDto<SummaryPurchaseReportMultiDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupData1(List<SummaryPurchaseReportMultiDto> warehouseData, string group)
        {
            var queryable = warehouseData.AsQueryable();
            List<SummaryPurchaseReportMultiDto> result = group switch
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
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupData2(List<SummaryPurchaseReportMultiDto> warehouseData, string group1, string code, string group2)
        {
            var queryable = warehouseData.AsQueryable();
            queryable = this.FilterByGroupCode(queryable, group1, code);
            List<SummaryPurchaseReportMultiDto> result = group2 switch
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
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupByPartner(IQueryable<SummaryPurchaseReportMultiDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.PartnerCode
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                GroupCode = p.Key.PartnerCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
            }).OrderBy(p => p.PartnerCode).ToList();
            foreach (var item in result)
            {
                item.Note = await this.GetPartnerName(item.GroupCode, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupByProduct(IQueryable<SummaryPurchaseReportMultiDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.ProductCode
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                GroupCode = p.Key.ProductCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
            }).OrderBy(p => p.ProductCode).ToList();
            foreach (var item in result)
            {
                item.Note = await this.GetProductName(item.GroupCode, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupByDepartment(IQueryable<SummaryPurchaseReportMultiDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.DepartmentCode
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                GroupCode = p.Key.DepartmentCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
            }).OrderBy(p => p.DepartmentCode).ToList();
            foreach (var item in result)
            {
                item.Note = await this.GetDepartmentName(item.GroupCode, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupBySection(IQueryable<SummaryPurchaseReportMultiDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.SectionCode
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                GroupCode = p.Key.SectionCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
            }).OrderBy(p => p.GroupCode).ToList();
            foreach (var item in result)
            {
                item.Note = await this.GetSectionName(item.GroupCode, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupByCase(IQueryable<SummaryPurchaseReportMultiDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.CaseCode
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                ProductGroupCode = p.Key.CaseCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
            }).OrderBy(p => p.CaseCode).ToList();
            foreach (var item in result)
            {
                item.Note = await this.GetCaseName(item.GroupCode, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupByBusiness(IQueryable<SummaryPurchaseReportMultiDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.BusinessCode
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                GroupCode = p.Key.BusinessCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
            }).OrderBy(p => p.BusinessCode).ToList();
            foreach (var item in result)
            {
                item.Note = await this.GetBusinessName(item.GroupCode, item.OrgCode);
            }

            return result;
        }
        private async Task<List<SummaryPurchaseReportMultiDto>> GroupByFProductWork(IQueryable<SummaryPurchaseReportMultiDto> queryable)
        {
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.FProductWorkCode
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                Bold = "C",
                OrgCode = p.Key.OrgCode,
                GroupCode = p.Key.FProductWorkCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
            }).OrderBy(p => p.FProductWorkCode).ToList();
            foreach (var item in result)
            {
                item.Note = await this.GetFProductWorkName(item.GroupCode, item.OrgCode);
            }

            return result;
        }
        private async Task<string> GetPartnerName(string partnerCode, string orgCode)
        {
            var partner = await _accPartnerService.GetAccPartnerByCodeAsync(partnerCode, orgCode);
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
        private async Task<List<SummaryPurchaseReportMultiDto>> GetDetailData(List<SummaryPurchaseReportMultiDto> warehouseData, string group, string code, string detailBy)
        {
            var queryable = warehouseData.AsQueryable();
            queryable = this.FilterByGroupCode(queryable, group, code);

            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.GroupCode,
                g.VoucherDate,
                g.VoucherNumber
            }).Select(p => new SummaryPurchaseReportMultiDto()
            {
                OrgCode = p.Key.OrgCode,
                GroupCode = p.Key.GroupCode,
                Amount = p.Sum(p => p.Amount),
                AmountCur = p.Sum(p => p.AmountCur),
                Quantity = p.Sum(p => p.Quantity),
                DiscountAmount = p.Sum(p => p.DiscountAmount),
                DiscountAmountCur = p.Sum(p => p.DiscountAmountCur),
                VatAmount = p.Sum(p => p.VatAmount),
                VatAmountCur = p.Sum(p => p.VatAmountCur),
                AmountPay = p.Sum(p => p.AmountPay),
                AmountPayCur = p.Sum(p => p.AmountPayCur),
                DevaluationAmount = p.Sum(p => p.DevaluationAmount),
                DevaluationAmountCur = p.Sum(p => p.DevaluationAmountCur),
                ExpenseAmountCur = p.Sum(p => p.ExpenseAmountCur),
                ExpenseAmount = p.Sum(p => p.ExpenseAmount),
                AmountFunds = p.Sum(p => p.AmountFunds),
                AmountFundsCur = p.Sum(p => p.AmountFundsCur),
                VoucherCode = p.Max(p => p.VoucherCode),
                VoucherDate = p.Key.VoucherDate,
                Unit = p.Max(p => p.Unit),
                AccCode = p.Max(p => p.AccCode),
                Price = p.Max(p => p.Price),
                VoucherNumber = p.Key.VoucherNumber,
                ProductCode = detailBy == "1" ? p.Max(p => p.PartnerCode) : p.Max(p => p.ProductCode),
                ProductName = p.Max(p => p.ProductName),
                InvoiceDate = p.Max(p => p.InvoiceDate),
                InvoiceNumber = p.Max(p => p.InvoiceNumber),
                InvoiceSymbol = p.Max(p => p.InvoiceSymbol),
                PartnerCode = p.Max(p => p.PartnerCode),
                PartnerName = p.Max(p => p.PartnerName),
                WarehouseCode = p.Max(p => p.WarehouseCode),
                Price0 = p.Max(p => p.Price0),
                VoucherId = p.Max(p => p.VoucherId),
            }).OrderBy(p => p.ProductCode).ToList();
            foreach (var item in result)
            {
                item.Note = detailBy == "1" ? await this.GetPartnerName(item.PartnerCode, item.OrgCode) : await this.GetProductName(item.GroupCode, item.OrgCode);
            }
            return result;
        }

        private IQueryable<SummaryPurchaseReportMultiDto> FilterByGroupCode(IQueryable<SummaryPurchaseReportMultiDto> queryable,
                                        string group, string code)
        {
            var query = group switch
            {
                "1" => queryable.Where(p => p.PartnerCode == code),
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
        #region Private
        [Authorize(ReportPermissions.SummaryPurchaseMultiReportPrint)]
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
        private async Task<List<SummaryPurchaseReportMultiDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);

            var incurredData = warehouseBook.Select(p => new SummaryPurchaseReportMultiDto()
            {
                Tag = 0,
                Sort0 = 2,
                Bold = "K",
                Rank = 2,
                OrgCode = p.OrgCode,
                VoucherId = p.VoucherId,
                Id = p.Id,
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                ProductCode = p.ProductCode,
                Unit = p.UnitCode,
                CurrencyCode = p.CurrencyCode,
                ExchangeRate = p.ExchangeRate,
                Note = p.Note,
                ImportQuantity = p.ImportQuantity ?? 0,
                Price = p.Price ?? 0,
                PriceCur = p.PriceCur ?? 0,
                ImportAmount = p.ImportAmount ?? 0,
                ImportAmountCur = p.ImportAmountCur ?? 0,
                PartnerName = null,
                DepartmentCode = p.DepartmentCode,
                ProductLotCode = p.ProductLotCode,
                ProductOriginCode = p.ProductOriginCode,
                BusinessCode = p.BusinessCode,
                ExportQuantity = p.Quantity ?? 0,
                ExportAmount = p.Amount ?? 0,
                ExportAmountCur = p.AmountCur ?? 0,
                QuantitySale = 0,
                Price2 = p.Price2,
                PriceCur2 = p.PriceCur2,
                AmountSale = p.Amount2 ?? 0,
                Amount = p.Amount ?? 0,
                AmountCur = p.AmountCur ?? 0,
                DiscountAmount = p.DiscountAmount ?? 0,
                DiscountAmountCur = p.DiscountAmountCur ?? 0,
                VatAmount = p.VatAmount ?? 0,
                VatAmountCur = p.VatAmountCur ?? 0,
                TotalAmountCur = p.AmountCur2 ?? 0 - p.DiscountAmountCur ?? 0 + p.VatAmountCur ?? 0,
                TotalAmount = p.Amount2 ?? 0 - p.DiscountAmount ?? 0 + p.VatAmount ?? 0,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                ReciprocalAcc = p.VoucherGroup == 1 ? p.CreditAcc : p.DebitAcc,
                GroupCode = p.PartnerCode,
                CaseCode = p.CaseCode,
                FProductWorkCode = p.FProductWorkCode,
                SectionCode = p.SectionCode,
                Quantity = p.Quantity,
                GroupCode1 = null,
                GroupCode2 = null,
                GroupName1 = null,
                GroupName2 = null,
                NoteHtml = null

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
            dic.Add(WarehouseBookParameterConst.DebitOrCredit, "0");

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

