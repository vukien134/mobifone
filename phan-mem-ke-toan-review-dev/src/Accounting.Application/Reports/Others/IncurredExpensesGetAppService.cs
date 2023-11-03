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
using Accounting.Reports.ImportExports;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Linq;
using NPOI.Util;
using Accounting.Categories.Accounts;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.Others
{
    public class IncurredExpensesGetAppService : AccountingAppService
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
        private readonly AccountingCacheManager _accountingCacheManager;

        #endregion
        public IncurredExpensesGetAppService(ReportDataService reportDataService,
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
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods        
        public async Task<ReportResponseDto<IncurredExpensesGetDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dic);
            var incurredData = await GetIncurredData(dic);
            var debitCredit = 0;
            var productionPeriodCode = ""; // ma_sx
            if (string.IsNullOrEmpty(dto.Parameters.DebitCredit))
            {
                debitCredit = 2;
            }
            var round = "K";
            var nC = "";
            var ctSp = "";
            // check khutrung
            var sterilizations = await _tenantSettingService.GetTenantSettingByKeyAsync("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            var sterilization = sterilizations.Value;
            var Acc = await _accountSystemService.GetQueryableAsync();
            var lstAcc = Acc.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();

            var accSesion = await _accSectionService.GetQueryableAsync();
            var lstAccSesion = accSesion.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.AttachProductCost != "C").ToList();

            var generatedNumber0 = (from a in lstAcc
                                    where 1 == 0
                                    select new
                                    {
                                        OrgCode = "",
                                        Year = 0,
                                        AccCode = "",
                                        PartnerCode = "",
                                        PartnerName0 = "",
                                        ContractCode = "",
                                        WorkPlaceCode = "",
                                        FProductWorkCode = "",
                                        FProductWorkCode0 = "",
                                        SectionCode = "",
                                        ProductCode = "",
                                        Quantity = (decimal)0,
                                        Amount = (decimal)0,
                                        AmountCur = (decimal)0,
                                        QuanltityDky = (decimal)0,
                                        AmountDky = (decimal)0,
                                        QuanltityCky = (decimal)0,
                                        AmountCky = (decimal)0,


                                    }).ToList();
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
            var lstProductVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();



            if (dto.Parameters.DebitCredit == "2")
            {

                if (ctSp.Contains("C,S,") != false)
                {
                    var productVouchers = (from a in productVoucher
                                           join b in productVoucherDetail on a.Id equals b.ProductVoucherId
                                           where a.VoucherCode == "KKD"
                                           && a.VoucherDate == dto.Parameters.FromDate
                                           && b.CreditAcc.Contains(dto.Parameters.AccCode) == true
                                           && a.Year == dto.Parameters.Year
                                           && string.IsNullOrEmpty(dto.Parameters.WorkPlaceCode) ? b.WorkPlaceCode.Contains(dto.Parameters.WorkPlaceCode) : b.WorkPlaceCode == dto.Parameters.WorkPlaceCode
                                           group new
                                           {
                                               a.OrgCode,
                                               a.Year,
                                               b.CreditAcc,
                                               a.PartnerCode0,
                                               b.ContractCode,
                                               b.WorkPlaceCode,
                                               b.FProductWorkCode,
                                               b.SectionCode,
                                               b.ProductCode,
                                               b.Amount,
                                               b.Quantity,
                                               a.PartnerName0
                                           } by new
                                           {
                                               a.OrgCode,
                                               a.Year,
                                               b.CreditAcc,
                                               a.PartnerCode0,
                                               b.ContractCode,
                                               b.WorkPlaceCode,
                                               b.FProductWorkCode,
                                               b.SectionCode,
                                               b.ProductCode,
                                               a.PartnerName0
                                           } into gr
                                           select new
                                           {
                                               OrgCode = gr.Key.OrgCode,
                                               Year = gr.Key.Year,
                                               AccCode = gr.Key.CreditAcc,
                                               PartnerCode = gr.Key.PartnerCode0,
                                               PartnerName0 = gr.Key.PartnerName0,
                                               ContractCode = gr.Key.ContractCode,
                                               WorkPlaceCode = gr.Key.WorkPlaceCode,
                                               FProductWorkCode = gr.Key.FProductWorkCode,
                                               FProductWorkCode0 = gr.Key.FProductWorkCode,
                                               SectionCode = gr.Key.SectionCode,
                                               ProductCode = gr.Key.ProductCode,
                                               Quantity = (decimal)0,
                                               Amount = (decimal)0,
                                               AmountCur = (decimal)0,
                                               QuanltityDky = (decimal)gr.Sum(p => p.Quantity),
                                               AmountDky = (decimal)gr.Sum(p => p.Amount),
                                               QuanltityCky = (decimal)0,
                                               AmountCky = (decimal)0,
                                           }).ToList();

                    generatedNumber0.AddRange(productVouchers);

                    // cuoiky
                    var productVoucher2 = (from a in productVoucher
                                           join b in productVoucherDetail on a.Id equals b.ProductVoucherId
                                           where a.VoucherCode == "KKD"
                                           && a.VoucherDate == dto.Parameters.FromDate
                                           && b.CreditAcc.Contains(dto.Parameters.AccCode) == true
                                           && a.Year == dto.Parameters.Year
                                           && string.IsNullOrEmpty(dto.Parameters.WorkPlaceCode) ? b.WorkPlaceCode.Contains(dto.Parameters.WorkPlaceCode) : b.WorkPlaceCode == dto.Parameters.WorkPlaceCode
                                           group new
                                           {
                                               a.OrgCode,
                                               a.Year,
                                               b.CreditAcc,
                                               a.PartnerCode0,
                                               b.ContractCode,
                                               b.WorkPlaceCode,
                                               b.FProductWorkCode,
                                               b.SectionCode,
                                               b.ProductCode,
                                               b.Amount,
                                               b.Quantity,
                                               a.PartnerName0
                                           } by new
                                           {
                                               a.OrgCode,
                                               a.Year,
                                               b.CreditAcc,
                                               a.PartnerCode0,
                                               b.ContractCode,
                                               b.WorkPlaceCode,
                                               b.FProductWorkCode,
                                               b.SectionCode,
                                               b.ProductCode,
                                               a.PartnerName0
                                           } into gr
                                           select new
                                           {
                                               OrgCode = gr.Key.OrgCode,
                                               Year = gr.Key.Year,
                                               AccCode = gr.Key.CreditAcc,
                                               PartnerCode = gr.Key.PartnerCode0,
                                               PartnerName0 = gr.Key.PartnerName0,
                                               ContractCode = gr.Key.ContractCode,
                                               WorkPlaceCode = gr.Key.WorkPlaceCode,
                                               FProductWorkCode = gr.Key.FProductWorkCode,
                                               FProductWorkCode0 = gr.Key.FProductWorkCode,
                                               SectionCode = gr.Key.SectionCode,
                                               ProductCode = gr.Key.ProductCode,
                                               Quantity = (decimal)0,
                                               Amount = (decimal)0,
                                               AmountCur = (decimal)0,
                                               QuanltityDky = (decimal)gr.Sum(p => p.Quantity),
                                               AmountDky = (decimal)gr.Sum(p => p.Amount),
                                               QuanltityCky = (decimal)0,
                                               AmountCky = (decimal)0,
                                           }).ToList();

                    generatedNumber0.AddRange(productVouchers);
                }
                if (dto.Parameters.TypeForward == "2")
                {
                    var generatedNumber0s = (from a in openingBalance
                                             select new
                                             {
                                                 OrgCode = _webHelper.GetCurrentOrgUnit(),
                                                 Year = _webHelper.GetCurrentYear(),
                                                 AccCode = dto.Parameters.AccCode,
                                                 PartnerCode = a.PartnerCode,
                                                 PartnerName0 = "",
                                                 ContractCode = a.ContractCode,
                                                 WorkPlaceCode = a.WorkPlaceCode,
                                                 FProductWorkCode = a.FProductCode,
                                                 FProductWorkCode0 = "",
                                                 SectionCode = "",
                                                 ProductCode = "",
                                                 Quantity = (decimal)0,
                                                 Amount = (decimal)(a.Debit - a.Credit),
                                                 AmountCur = (decimal)(a.DebitCur - a.CreditCur),
                                                 QuanltityDky = (decimal)0,
                                                 AmountDky = (decimal)0,
                                                 QuanltityCky = (decimal)0,
                                                 AmountCky = (decimal)0,
                                             }).ToList();
                    generatedNumber0.AddRange(generatedNumber0s);
                }
                if (!string.IsNullOrEmpty(nC) || nC == "N")
                {

                    var products = await _productService.GetQueryableAsync();
                    var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType == "T").ToList();

                    var lstProduct2s = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType != "T").ToList();
                    var incurredData2 = (from p in incurredData
                                         join b in lstProducts on new { p.OrgCode, Code = p.ProductCode } equals new { b.OrgCode, b.Code } into c
                                         from pr in c.DefaultIfEmpty()
                                         join m in lstProduct2s on new { p.OrgCode, Code = p.ProductCode } equals new { m.OrgCode, m.Code } into n
                                         from p2r in c.DefaultIfEmpty()
                                         where p.CheckDuplicate != "C"
                                         && !string.IsNullOrEmpty(dto.Parameters.SectionCode) ? p.SectionCode == dto.Parameters.SectionCode : 1 == 1
                                         && ctSp.Contains("C,S,") == false || round == "K"
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.Ord0.Contains("A") == false) || p.ProductCode.Contains(pr.Code) == false)
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.ProductCode.Contains(p2r.Code) == true))
                                         group new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.DebitAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.FProductWorkCode,
                                             p.ProductCode,
                                             p.Quantity,
                                             p.Amount0,
                                             p.DebitAmountCur,
                                             p.PartnerName
                                         } by new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.DebitAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.ProductCode,
                                             p.FProductWorkCode,
                                             p.PartnerName
                                         } into gr
                                         select new
                                         {
                                             OrgCode = gr.Key.OrgCode,
                                             Year = gr.Key.Year,
                                             AccCode = gr.Key.DebitAcc,
                                             PartnerCode = gr.Key.PartnerCode,
                                             PartnerName0 = gr.Key.PartnerName,
                                             ContractCode = gr.Key.DebitContractCode,
                                             WorkPlaceCode = gr.Key.DebitWorkPlaceCode,
                                             FProductWorkCode = gr.Key.DebitFProductWorkCode,
                                             FProductWorkCode0 = gr.Key.FProductWorkCode,
                                             SectionCode = gr.Key.DebitSectionCode,
                                             ProductCode = gr.Key.ProductCode,
                                             Quantity = (decimal)gr.Sum(p => p.Quantity),
                                             Amount = (decimal)gr.Sum(p => p.Amount0),
                                             AmountCur = (decimal)gr.Sum(p => p.DebitAmountCur),
                                             QuanltityDky = (decimal)0,
                                             AmountDky = (decimal)0,
                                             QuanltityCky = (decimal)0,
                                             AmountCky = (decimal)0,
                                         }).ToList();

                    generatedNumber0.AddRange(incurredData2);
                }

                if (!string.IsNullOrEmpty(nC) || nC == "C")
                {

                    var products = await _productService.GetQueryableAsync();
                    var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType == "T").ToList();

                    var lstProduct2s = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType != "T").ToList();
                    var incurredData2 = (from p in incurredData
                                         join b in lstProducts on new { p.OrgCode, Code = p.ProductCode } equals new { b.OrgCode, b.Code } into c
                                         from pr in c.DefaultIfEmpty()
                                         join m in lstProduct2s on new { p.OrgCode, Code = p.ProductCode } equals new { m.OrgCode, m.Code } into n
                                         from p2r in c.DefaultIfEmpty()
                                         where p.CheckDuplicate != "C"
                                         && !string.IsNullOrEmpty(dto.Parameters.SectionCode) ? p.SectionCode == dto.Parameters.SectionCode : 1 == 1
                                         && ctSp.Contains("C,S,") == false || round == "K"
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.Ord0.Contains("A") == false) || p.ProductCode.Contains(pr.Code) == false)
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.ProductCode.Contains(p2r.Code) == true))
                                         group new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.CreditAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.FProductWorkCode,
                                             p.ProductCode,
                                             p.Quantity,
                                             p.Amount0,
                                             p.CreditAmountCur,
                                             p.PartnerName
                                         } by new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.CreditAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.ProductCode,
                                             p.FProductWorkCode,
                                             p.PartnerName
                                         } into gr
                                         select new
                                         {
                                             OrgCode = gr.Key.OrgCode,
                                             Year = gr.Key.Year,
                                             AccCode = gr.Key.CreditAcc,
                                             PartnerCode = gr.Key.PartnerCode,
                                             PartnerName0 = gr.Key.PartnerName,
                                             ContractCode = gr.Key.DebitContractCode,
                                             WorkPlaceCode = gr.Key.DebitWorkPlaceCode,
                                             FProductWorkCode = gr.Key.DebitFProductWorkCode,
                                             FProductWorkCode0 = gr.Key.FProductWorkCode,
                                             SectionCode = gr.Key.DebitSectionCode,
                                             ProductCode = gr.Key.ProductCode,
                                             Quantity = (decimal)gr.Sum(p => p.Quantity),
                                             Amount = (decimal)gr.Sum(p => p.Amount0),
                                             AmountCur = (decimal)gr.Sum(p => p.CreditAmountCur),
                                             QuanltityDky = (decimal)0,
                                             AmountDky = (decimal)0,
                                             QuanltityCky = (decimal)0,
                                             AmountCky = (decimal)0,
                                         }).ToList();

                    generatedNumber0.AddRange(incurredData2);

                }
            }
            else
            {


                if (dto.Parameters.TypeForward == "2")
                {
                    var generatedNumber0s = (from a in openingBalance
                                             select new
                                             {
                                                 OrgCode = _webHelper.GetCurrentOrgUnit(),
                                                 Year = _webHelper.GetCurrentYear(),
                                                 AccCode = dto.Parameters.AccCode,
                                                 PartnerCode = a.PartnerCode,
                                                 PartnerName0 = "",
                                                 ContractCode = a.ContractCode,
                                                 WorkPlaceCode = a.WorkPlaceCode,
                                                 FProductWorkCode = a.FProductCode,
                                                 FProductWorkCode0 = "",
                                                 SectionCode = "",
                                                 ProductCode = "",
                                                 Quantity = (decimal)0,
                                                 Amount = (decimal)(a.Debit - a.Credit),
                                                 AmountCur = (decimal)(a.DebitCur - a.CreditCur),
                                                 QuanltityDky = (decimal)0,
                                                 AmountDky = (decimal)0,
                                                 QuanltityCky = (decimal)0,
                                                 AmountCky = (decimal)0,
                                             }).ToList();
                    generatedNumber0.AddRange(generatedNumber0s);

                }
                if (nC == "" || nC == "N")
                {

                    var products = await _productService.GetQueryableAsync();
                    var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType == "T").ToList();

                    var lstProduct2s = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType != "T").ToList();
                    var incurredData2 = (from p in incurredData
                                         join b in lstProducts on new { p.OrgCode, Code = p.ProductCode } equals new { b.OrgCode, b.Code } into c
                                         from pr in c.DefaultIfEmpty()
                                         join m in lstProduct2s on new { p.OrgCode, Code = p.ProductCode } equals new { m.OrgCode, m.Code } into n
                                         from p2r in c.DefaultIfEmpty()
                                         where p.CheckDuplicate != "C"
                                         && !string.IsNullOrEmpty(dto.Parameters.SectionCode) ? p.SectionCode == dto.Parameters.SectionCode : 1 == 1
                                         && ctSp.Contains("C,S,") == false || round == "K"
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.Ord0.Contains("A") == false) || p.ProductCode.Contains(pr.Code) == false)
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.ProductCode.Contains(p2r.Code) == true))

                                         group new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.DebitAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.FProductWorkCode,
                                             p.ProductCode,
                                             p.Quantity,
                                             p.Amount0,
                                             p.DebitAmountCur,
                                             p.PartnerName
                                         } by new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.DebitAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.ProductCode,
                                             p.FProductWorkCode,
                                             p.PartnerName
                                         } into gr
                                         select new
                                         {
                                             OrgCode = gr.Key.OrgCode,
                                             Year = gr.Key.Year,
                                             AccCode = gr.Key.DebitAcc,
                                             PartnerCode = gr.Key.PartnerCode,
                                             PartnerName0 = gr.Key.PartnerName,
                                             ContractCode = gr.Key.DebitContractCode,
                                             WorkPlaceCode = gr.Key.DebitWorkPlaceCode,
                                             FProductWorkCode = gr.Key.DebitFProductWorkCode,
                                             FProductWorkCode0 = gr.Key.FProductWorkCode,
                                             SectionCode = gr.Key.DebitSectionCode,
                                             ProductCode = gr.Key.ProductCode,
                                             Quantity = (decimal)gr.Sum(p => p.Quantity),
                                             Amount = (decimal)gr.Sum(p => p.Amount0),
                                             AmountCur = (decimal)gr.Sum(p => p.DebitAmountCur),
                                             QuanltityDky = (decimal)0,
                                             AmountDky = (decimal)0,
                                             QuanltityCky = (decimal)0,
                                             AmountCky = (decimal)0,
                                         }).ToList();
                    incurredData2 = incurredData2.Where(p => dto.Parameters.AccCode.Contains(p.AccCode)).ToList();
                    generatedNumber0.AddRange(incurredData2);
                }

                if (nC == "" || nC == "C")
                {

                    var products = await _productService.GetQueryableAsync();
                    var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType == "T").ToList();

                    var lstProduct2s = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductType != "T").ToList();
                    var incurredData2 = (from p in incurredData
                                         join b in lstProducts on new { p.OrgCode, Code = p.ProductCode } equals new { b.OrgCode, b.Code } into c
                                         from pr in c.DefaultIfEmpty()
                                         join m in lstProduct2s on new { p.OrgCode, Code = p.ProductCode } equals new { m.OrgCode, m.Code } into n
                                         from p2r in c.DefaultIfEmpty()
                                         where p.CheckDuplicate != "C"
                                         && !string.IsNullOrEmpty(dto.Parameters.SectionCode) ? p.SectionCode == dto.Parameters.SectionCode : 1 == 1
                                         && ctSp.Contains("C,S,") == false || round == "K"
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.Ord0.Contains("A") == false) || p.ProductCode.Contains(pr.Code) == false)
                                         && (productionPeriodCode == "" || (p.ProductCode == "" || p.ProductCode.Contains(p2r.Code) == true))
                                         group new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.CreditAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.FProductWorkCode,
                                             p.ProductCode,
                                             p.Quantity,
                                             p.Amount0,
                                             p.CreditAmountCur,
                                             p.PartnerName
                                         } by new
                                         {
                                             p.OrgCode,
                                             p.Year,
                                             p.CreditAcc,
                                             p.PartnerCode,
                                             p.DebitContractCode,
                                             p.DebitFProductWorkCode,
                                             p.DebitSectionCode,
                                             p.SectionCode,
                                             p.DebitWorkPlaceCode,
                                             p.ProductCode,
                                             p.FProductWorkCode,
                                             p.PartnerName
                                         } into gr
                                         select new
                                         {
                                             OrgCode = gr.Key.OrgCode,
                                             Year = gr.Key.Year,
                                             AccCode = gr.Key.CreditAcc,
                                             PartnerCode = gr.Key.PartnerCode,
                                             PartnerName0 = gr.Key.PartnerName,
                                             ContractCode = gr.Key.DebitContractCode,
                                             WorkPlaceCode = gr.Key.DebitWorkPlaceCode,
                                             FProductWorkCode = gr.Key.DebitFProductWorkCode,
                                             FProductWorkCode0 = gr.Key.FProductWorkCode,
                                             SectionCode = gr.Key.DebitSectionCode,
                                             ProductCode = gr.Key.ProductCode,
                                             Quantity = (decimal)gr.Sum(p => p.Quantity),
                                             Amount = (decimal)gr.Sum(p => p.Amount0),
                                             AmountCur = (decimal)gr.Sum(p => p.CreditAmountCur),
                                             QuanltityDky = (decimal)0,
                                             AmountDky = (decimal)0,
                                             QuanltityCky = (decimal)0,
                                             AmountCky = (decimal)0,
                                         }).ToList();
                    incurredData2 = incurredData2.Where(p => p.AccCode != null).ToList();
                    incurredData2 = incurredData2.Where(p => dto.Parameters.AccCode.Contains(p.AccCode)).ToList();
                    generatedNumber0.AddRange(incurredData2);
                }

            }

            //lstAccSesion
            generatedNumber0 = (from a in generatedNumber0
                                join b in lstAccSesion on a.SectionCode equals b.Code into c
                                from se in c.DefaultIfEmpty()
                                where a.SectionCode != null
                                select new
                                {
                                    OrgCode = a.OrgCode,
                                    Year = a.Year,
                                    AccCode = a.AccCode,
                                    PartnerCode = a.PartnerCode,
                                    PartnerName0 = a.PartnerName0,
                                    ContractCode = a.ContractCode,
                                    WorkPlaceCode = a.WorkPlaceCode,
                                    FProductWorkCode = a.FProductWorkCode,
                                    FProductWorkCode0 = a.FProductWorkCode,
                                    SectionCode = a.SectionCode,
                                    ProductCode = a.ProductCode,
                                    Quantity = a.Quantity,
                                    Amount = a.Amount,
                                    AmountCur = a.AmountCur,
                                    QuanltityDky = a.QuanltityDky,
                                    AmountDky = a.AmountDky,
                                    QuanltityCky = a.QuanltityCky,
                                    AmountCky = a.AmountCky,
                                }).ToList();

            generatedNumber0 = (from a in generatedNumber0
                                join b in lstAcc on new { a.AccCode, a.Year } equals new { b.AccCode, b.Year }
                                where b.AttachProductCost != "C" && b.Year == _webHelper.GetCurrentYear()
                                select new
                                {
                                    OrgCode = a.OrgCode,
                                    Year = a.Year,
                                    AccCode = a.AccCode,
                                    PartnerCode = b.AttachPartner != "C" ? "" : a.PartnerCode,
                                    PartnerName0 = a.PartnerName0,
                                    ContractCode = b.AttachContract != "C" ? "" : a.ContractCode,
                                    WorkPlaceCode = b.AttachWorkPlace != "C" ? "" : a.WorkPlaceCode,
                                    FProductWorkCode = "",
                                    FProductWorkCode0 = a.FProductWorkCode,
                                    SectionCode = b.AttachAccSection != "C" ? "" : a.FProductWorkCode,
                                    ProductCode = a.ProductCode,
                                    Quantity = a.Quantity,
                                    Amount = a.Amount,
                                    AmountCur = a.AmountCur,
                                    QuanltityDky = a.QuanltityDky,
                                    AmountDky = a.AmountDky,
                                    QuanltityCky = a.QuanltityCky,
                                    AmountCky = a.AmountCky,
                                }).ToList();
            if (dto.Parameters.AttachProduct != "C")
            {
                generatedNumber0 = (from a in generatedNumber0

                                    select new
                                    {
                                        OrgCode = a.OrgCode,
                                        Year = a.Year,
                                        AccCode = a.AccCode,
                                        PartnerCode = a.PartnerCode,
                                        PartnerName0 = a.PartnerName0,
                                        ContractCode = a.ContractCode,
                                        WorkPlaceCode = a.WorkPlaceCode,
                                        FProductWorkCode = a.FProductWorkCode,
                                        FProductWorkCode0 = a.FProductWorkCode0,
                                        SectionCode = a.SectionCode,
                                        ProductCode = "",
                                        Quantity = a.Quantity,
                                        Amount = a.Amount,
                                        AmountCur = a.AmountCur,
                                        QuanltityDky = a.QuanltityDky,
                                        AmountDky = a.AmountDky,
                                        QuanltityCky = a.QuanltityCky,
                                        AmountCky = a.AmountCky,
                                    }).ToList();
            }
            if (ctSp.Contains("C,S") == false)
            {
                generatedNumber0 = (from a in generatedNumber0
                                    group new
                                    {
                                        a.OrgCode,
                                        a.Year,
                                        a.AccCode,
                                        a.PartnerCode,
                                        a.ContractCode,
                                        a.FProductWorkCode,
                                        a.FProductWorkCode0,
                                        a.SectionCode,
                                        a.WorkPlaceCode,
                                        a.ProductCode,
                                        a.Quantity,
                                        a.Amount,
                                        a.AmountCur,
                                        a.QuanltityDky,
                                        a.AmountDky,
                                        a.QuanltityCky,
                                        a.AmountCky,
                                        a.PartnerName0
                                    } by new
                                    {
                                        a.OrgCode,
                                        a.Year,
                                        a.AccCode,
                                        a.PartnerCode,
                                        a.ContractCode,
                                        a.FProductWorkCode,
                                        a.FProductWorkCode0,
                                        a.SectionCode,
                                        a.WorkPlaceCode,
                                        a.ProductCode,
                                        a.PartnerName0
                                    } into gr
                                    where gr.Sum(p => p.Amount) > 0
                                    select new
                                    {
                                        OrgCode = gr.Key.OrgCode,
                                        Year = gr.Key.Year,
                                        AccCode = gr.Key.AccCode,
                                        PartnerCode = gr.Key.PartnerCode,
                                        PartnerName0 = gr.Key.PartnerName0,
                                        ContractCode = gr.Key.ContractCode,
                                        WorkPlaceCode = gr.Key.WorkPlaceCode,
                                        FProductWorkCode = gr.Key.FProductWorkCode,
                                        FProductWorkCode0 = gr.Key.FProductWorkCode0,
                                        SectionCode = gr.Key.SectionCode,
                                        ProductCode = gr.Key.ProductCode,
                                        Quantity = (decimal)0,
                                        Amount = gr.Sum(p => p.Amount),
                                        AmountCur = gr.Sum(p => p.AmountCur),
                                        QuanltityDky = gr.Sum(p => p.QuanltityDky),
                                        AmountDky = gr.Sum(p => p.AmountDky),
                                        QuanltityCky = gr.Sum(p => p.QuanltityCky),
                                        AmountCky = gr.Sum(p => p.AmountCky),
                                    }).ToList();
            }
            else
            {
                if (dto.Parameters.AttachProduct != "C")
                {
                    generatedNumber0 = (from a in generatedNumber0
                                        group new
                                        {
                                            a.OrgCode,
                                            a.Year,
                                            a.AccCode,
                                            a.PartnerCode,
                                            a.ContractCode,
                                            a.FProductWorkCode,
                                            a.FProductWorkCode0,
                                            a.SectionCode,
                                            a.WorkPlaceCode,
                                            a.ProductCode,
                                            a.Quantity,
                                            a.Amount,
                                            a.AmountCur,
                                            a.QuanltityDky,
                                            a.AmountDky,
                                            a.QuanltityCky,
                                            a.AmountCky,
                                            a.PartnerName0
                                        } by new
                                        {
                                            a.OrgCode,
                                            a.Year,
                                            a.AccCode,
                                            a.PartnerCode,
                                            a.ContractCode,
                                            a.FProductWorkCode,
                                            a.FProductWorkCode0,
                                            a.SectionCode,
                                            a.WorkPlaceCode,
                                            a.ProductCode,
                                            a.PartnerName0
                                        } into gr
                                        where gr.Sum(p => p.Amount) + gr.Sum(p => p.Quantity) + gr.Sum(p => p.QuanltityCky) + gr.Sum(p => p.QuanltityDky) + gr.Sum(p => p.AmountCky) + gr.Sum(p => p.AmountDky) != 0
                                        select new
                                        {
                                            OrgCode = gr.Key.OrgCode,
                                            Year = gr.Key.Year,
                                            AccCode = gr.Key.AccCode,
                                            PartnerCode = gr.Key.PartnerCode,
                                            PartnerName0 = gr.Key.PartnerName0,
                                            ContractCode = gr.Key.ContractCode,
                                            WorkPlaceCode = gr.Key.WorkPlaceCode,
                                            FProductWorkCode = gr.Key.FProductWorkCode,
                                            FProductWorkCode0 = gr.Key.FProductWorkCode0,
                                            SectionCode = gr.Key.SectionCode,
                                            ProductCode = gr.Key.ProductCode,
                                            Quantity = (decimal)0,
                                            Amount = gr.Sum(p => p.Amount),
                                            AmountCur = gr.Sum(p => p.AmountCur),
                                            QuanltityDky = gr.Sum(p => p.QuanltityDky),
                                            AmountDky = gr.Sum(p => p.AmountDky),
                                            QuanltityCky = gr.Sum(p => p.QuanltityCky),
                                            AmountCky = gr.Sum(p => p.AmountCky),
                                        }).ToList();
                }
                else
                {
                    generatedNumber0 = (from a in generatedNumber0
                                        group new
                                        {
                                            a.OrgCode,
                                            a.Year,
                                            a.AccCode,
                                            a.PartnerCode,
                                            a.ContractCode,
                                            a.FProductWorkCode,
                                            a.FProductWorkCode0,
                                            a.SectionCode,
                                            a.WorkPlaceCode,
                                            a.ProductCode,
                                            a.Quantity,
                                            a.Amount,
                                            a.AmountCur,
                                            a.QuanltityDky,
                                            a.AmountDky,
                                            a.QuanltityCky,
                                            a.AmountCky,
                                            a.PartnerName0
                                        } by new
                                        {
                                            a.OrgCode,
                                            a.Year,
                                            a.AccCode,
                                            a.PartnerCode,
                                            a.ContractCode,
                                            a.FProductWorkCode,
                                            a.FProductWorkCode0,
                                            a.SectionCode,
                                            a.WorkPlaceCode,
                                            a.ProductCode,
                                            a.PartnerName0
                                        } into gr
                                        where gr.Sum(p => p.Amount) + gr.Sum(p => p.Quantity) + gr.Sum(p => p.QuanltityCky) + gr.Sum(p => p.QuanltityDky) + gr.Sum(p => p.AmountCky) + gr.Sum(p => p.AmountDky) != 0
                                        select new
                                        {
                                            OrgCode = gr.Key.OrgCode,
                                            Year = gr.Key.Year,
                                            AccCode = gr.Key.AccCode,
                                            PartnerCode = gr.Key.PartnerCode,
                                            PartnerName0 = gr.Key.PartnerName0,
                                            ContractCode = gr.Key.ContractCode,
                                            WorkPlaceCode = gr.Key.WorkPlaceCode,
                                            FProductWorkCode = gr.Key.FProductWorkCode,
                                            FProductWorkCode0 = gr.Key.FProductWorkCode0,
                                            SectionCode = gr.Key.SectionCode,
                                            ProductCode = gr.Key.ProductCode,
                                            Quantity = gr.Sum(p => p.Quantity),
                                            Amount = gr.Sum(p => p.Amount),
                                            AmountCur = gr.Sum(p => p.AmountCur),
                                            QuanltityDky = gr.Sum(p => p.QuanltityDky),
                                            AmountDky = gr.Sum(p => p.AmountDky),
                                            QuanltityCky = gr.Sum(p => p.QuanltityCky),
                                            AmountCky = gr.Sum(p => p.AmountCky),
                                        }).ToList();
                }
            }
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var resul = from a in generatedNumber0
                        join b in lstPartner on a.PartnerCode equals b.Code into c
                        from pa in c.DefaultIfEmpty()
                        select new IncurredExpensesGetDto
                        {
                            OrgCode = a.OrgCode,
                            Year = a.Year,
                            AccCode = a.AccCode,
                            PartnerCode = a.PartnerCode,
                            PartnerName = pa != null ? pa.Name : null,
                            ContractCode = a.ContractCode,
                            WorkPlaceCode = a.WorkPlaceCode,
                            FProductWorkCode = a.FProductWorkCode,
                            FProductWorkCode0 = a.FProductWorkCode0,
                            SectionCode = a.SectionCode,
                            ProductCode = a.ProductCode,
                            Quantity = a.Quantity,
                            Amount = a.Amount,
                            AmountCur = a.AmountCur,
                            QuanltityDky = a.QuanltityDky,
                            AmountDky = a.AmountDky,
                            QuanltityCky = a.QuanltityCky,
                            AmountCky = a.AmountCky,
                        };





            var reportResponse = new ReportResponseDto<IncurredExpensesGetDto>();
            reportResponse.Data = resul.ToList();
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
        private async Task<List<AccountBalanceDto>> GetOpeningBalance(Dictionary<string, object> dic)
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

            return openingBalances;
        }
        private async Task<List<IncurredExpensesGetDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetLedger(dic);
            var incurredData = warehouseBook.Select(p => new IncurredExpensesGetDto()
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
                Amount0 = p.Amount0,
                AmountCur0 = p.AmountCur0,
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
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                PartnerName = p.PartnerName0,
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
                CheckDuplicate = p.CheckDuplicate,
                SectionCode = p.SectionCode,
                DebitContractCode = p.DebitContractCode,
                DebitWorkPlaceCode = p.DebitWorkPlaceCode,
                DebitSectionCode = p.DebitSectionCode,
                DebitFProductWorkCode = p.DebitFProductWorkCode,
                DebitAmountCur = p.DebitAmountCur,
                CreditAmountCur = p.CreditAmountCur
            }).Where(p => int.Parse(p.Status) < 2).ToList();


            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetLedger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }

        private Dictionary<string, object> GetLedgerParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(LedgerParameterConst.PartnerCode, dto.PartnerCode);
            }
            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(LedgerParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            }
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(LedgerParameterConst.CurrencyCode, dto.CurrencyCode);
            }
            if (!string.IsNullOrEmpty(dto.ContractCode))
            {
                dic.Add(LedgerParameterConst.ContractCode, dto.ContractCode);
            }
            return dic;
        }
        private Dictionary<string, object> GetWarehouseBookParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");
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
            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                dic.Add(WarehouseBookParameterConst.BusinessCode, dto.BusinessCode);
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

