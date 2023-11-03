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
using Accounting.Vouchers;
using StackExchange.Redis;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using static NPOI.HSSF.Util.HSSFColor;
using Accounting.Caching;

namespace Accounting.Reports.DebitBooks
{
    public class CompareConfirmBebtAppService : AccountingAppService
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
        public CompareConfirmBebtAppService(ReportDataService reportDataService,
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
        public async Task<ReportResponseDto<CompareConfirmBebtDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstVoucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var voucherType = lstVoucherType.Where(p => p.Code == "PBH").FirstOrDefault();
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var parnerGroup = await _partnerGroupService.GetQueryableAsync();
            var lstPartnerGroup = parnerGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dic);
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            var yearCategory = await _yearCategoryService.GetQueryableAsync();
            string lstYearcategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault().UsingDecision.ToString();
            var circularsService = await _circularsService.GetQueryableAsync();
            var titleV = circularsService.Where(p => p.Code == lstYearcategory).FirstOrDefault().TitleV;
            var accName = "";
            if (!string.IsNullOrEmpty(dto.Parameters.AccCode))
            {
                accName = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == dto.Parameters.AccCode).FirstOrDefault().AccName;
            }

            var balanceAcc = (from a in openingBalance
                              join b in lstPartner on a.PartnerCode equals b.Code

                              select new
                              {
                                  a.AccCode,
                                  a.ContractCode,
                                  a.Credit,
                                  a.CreditCur,
                                  a.CurrencyCode,
                                  a.Debit,
                                  a.DebitCur,
                                  a.CreditIncurred,
                                  a.CreditIncurredCur,
                                  a.DebitIncurred,
                                  a.DebitIncurredCur,
                                  a.FProductCode,
                                  a.PartnerCode,
                                  a.SectionCode,
                                  a.WorkPlaceCode,
                                  PartnerGroupCode = b.PartnerGroup.Code,
                                  b.Name,
                                  b.TaxCode,
                                  b.Address,
                                  b.Tel
                              }).ToList();
            var resulBalanceAcc = (from a in balanceAcc
                                   join b in lstPartnerGroup on a.PartnerGroupCode equals b.Code into c
                                   from pg in c.DefaultIfEmpty()
                                   select new
                                   {
                                       a.AccCode,
                                       a.ContractCode,
                                       a.Credit,
                                       a.CreditCur,
                                       a.CurrencyCode,
                                       a.Debit,
                                       a.DebitCur,
                                       a.CreditIncurred,
                                       a.CreditIncurredCur,
                                       a.DebitIncurred,
                                       a.DebitIncurredCur,
                                       a.FProductCode,
                                       a.PartnerCode,
                                       a.SectionCode,
                                       a.WorkPlaceCode,
                                       PartnerName = a.Name,
                                       PartnerGroupCode = a.PartnerGroupCode,
                                       PartnerGroupName = pg != null ? pg.Name : null
                                   }).ToList();

            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var lstAccPartner = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroup);
                resulBalanceAcc = (from a in resulBalanceAcc
                                   join b in lstAccPartner on a.PartnerGroupCode equals b.Code
                                   select new
                                   {
                                       a.AccCode,
                                       a.ContractCode,
                                       a.Credit,
                                       a.CreditCur,
                                       a.CurrencyCode,
                                       a.Debit,
                                       a.DebitCur,
                                       a.CreditIncurred,
                                       a.CreditIncurredCur,
                                       a.DebitIncurred,
                                       a.DebitIncurredCur,
                                       a.FProductCode,
                                       a.PartnerCode,
                                       a.SectionCode,
                                       a.WorkPlaceCode,
                                       a.PartnerName,
                                       PartnerGroupCode = a.PartnerGroupCode,
                                       PartnerGroupName = a.PartnerGroupName
                                   }).ToList();
            }


            var incurredData = await GetIncurredData(dic);
            var resulIncurredData = (from a in incurredData
                                     join b in resulBalanceAcc on a.PartnerCode equals b.PartnerCode into d
                                     from c in d.DefaultIfEmpty()

                                     where a.CreditAcc == dto.Parameters.AccCode
                                     select new CompareConfirmBebtDto
                                     {
                                         OrgCode = a.Ord0,
                                         PartnerCode = a.PartnerCode,
                                         PartnerName = a.PartnerName,
                                         Year = a.Year,
                                         VoucherCode = a.VoucherCode,
                                         VoucherNumber = a.VoucherNumber,
                                         VoucherDate = a.VoucherDate,
                                         VoucherId = a.VoucherId,
                                         Id = a.Id,
                                         Ord0 = a.Ord0,
                                         DebitAcc = a.DebitAcc,
                                         CreditAcc = a.CreditAcc,
                                         DebitAmountCur = a.DebitAmountCur,
                                         CreditAmountCur = a.CreditAmountCur,
                                         Amount = a.Amount0,
                                         Note = a.Note,
                                         Description = a.Description,
                                         InvoiceDate = a.InvoiceDate,
                                         InvoiceNumber = a.InvoiceNumber,
                                         CurrencyCode = a.CurrencyCode,
                                         ExchangeRate = a.ExchangeRate,
                                         InvoicePartnerName = a.InvoicePartnerName,
                                         CheckDuplicate = a.CheckDuplicate,
                                         CreditPartnerCode = a.CreditPartnerCode,
                                         DebitPartnerCode = a.DebitPartnerCode,
                                         DebitIncurred = 0,
                                         DebitIncurredCur = 0,
                                         CreditIncurred = a.CreditIncurred,
                                         CreditIncurredCur = a.CreditIncurredCur,
                                         ReciprocalAcc = a.ReciprocalAcc,
                                         InventoryAmount = (decimal)(c != null ? c.Debit : 0),
                                         InventoryAmountCur = (decimal)(c != null ? c.DebitCur : 0),
                                         Credit = (decimal)(c != null ? c.Credit : 0),
                                         CreditCur = (decimal)(c != null ? c.CreditCur : 0),

                                     }).ToList();
            incurredData = (from a in incurredData
                            join b in resulBalanceAcc on a.PartnerCode equals b.PartnerCode into d
                            from c in d.DefaultIfEmpty()
                            join e in lstPartner on a.PartnerCode equals e.Code into g
                            from f in g.DefaultIfEmpty()
                            where a.DebitAcc == dto.Parameters.AccCode
                            select new CompareConfirmBebtDto
                            {
                                OrgCode = a.Ord0,
                                PartnerCode = a.PartnerCode,
                                PartnerName = a.PartnerName,
                                Year = a.Year,
                                VoucherCode = a.VoucherCode,
                                VoucherNumber = a.VoucherNumber,
                                VoucherDate = a.VoucherDate,
                                VoucherId = a.VoucherId,
                                Id = a.Id,
                                Ord0 = a.Ord0,
                                DebitAcc = a.DebitAcc,
                                CreditAcc = a.CreditAcc,
                                DebitAmountCur = a.DebitAmountCur,
                                CreditAmountCur = a.CreditAmountCur,
                                Amount = a.Amount0,
                                Note = a.Note,
                                Description = a.Description,
                                InvoiceDate = a.InvoiceDate,
                                InvoiceNumber = a.InvoiceNumber,
                                CurrencyCode = a.CurrencyCode,
                                ExchangeRate = a.ExchangeRate,
                                InvoicePartnerName = a.InvoicePartnerName,
                                CheckDuplicate = a.CheckDuplicate,
                                CreditPartnerCode = a.CreditPartnerCode,
                                DebitPartnerCode = a.DebitPartnerCode,
                                DebitIncurred = a.DebitIncurred,
                                DebitIncurredCur = a.DebitIncurredCur,
                                CreditIncurred = 0,
                                CreditIncurredCur = 0,
                                ReciprocalAcc = a.ReciprocalAcc,
                                InventoryAmount = (decimal)(c != null ? c.Debit : 0),
                                InventoryAmountCur = (decimal)(c != null ? c.DebitCur : 0),
                                Credit = (decimal)(c != null ? c.Credit : 0),
                                CreditCur = (decimal)(c != null ? c.CreditCur : 0),
                                TaxCode = f != null ? f.TaxCode : null,
                                Address = f != null ? f.Address : null,
                                Tel = f != null ? f.Tel : null
                            }).ToList();
            incurredData.AddRange(resulIncurredData);
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var lstAccPartner = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroup);
                incurredData = (from a in incurredData

                                join b in lstAccPartner on a.PartnerCode equals b.Code
                                join e in resulBalanceAcc on a.PartnerCode equals e.PartnerCode into d
                                from c in d.DefaultIfEmpty()
                                select new CompareConfirmBebtDto
                                {
                                    OrgCode = a.Ord0,
                                    PartnerCode = a.PartnerCode,
                                    PartnerName = a.PartnerName,
                                    Year = a.Year,
                                    VoucherCode = a.VoucherCode,
                                    VoucherNumber = a.VoucherNumber,
                                    VoucherDate = a.VoucherDate,
                                    VoucherId = a.VoucherId,
                                    Id = a.Id,
                                    Ord0 = a.Ord0,
                                    DebitAcc = a.DebitAcc,
                                    CreditAcc = a.CreditAcc,
                                    DebitAmountCur = a.DebitAmountCur,
                                    CreditAmountCur = a.CreditAmountCur,
                                    Amount = a.Amount0,
                                    Note = a.Note,
                                    Description = a.Description,
                                    InvoiceDate = a.InvoiceDate,
                                    InvoiceNumber = a.InvoiceNumber,
                                    CurrencyCode = a.CurrencyCode,
                                    ExchangeRate = a.ExchangeRate,
                                    InvoicePartnerName = a.InvoicePartnerName,
                                    CheckDuplicate = a.CheckDuplicate,
                                    CreditPartnerCode = a.CreditPartnerCode,
                                    DebitPartnerCode = a.DebitPartnerCode,
                                    DebitIncurred = a.Amount0,
                                    DebitIncurredCur = a.AmountCur0,
                                    CreditIncurred = a.CreditIncurred,
                                    CreditIncurredCur = a.CreditIncurredCur,
                                    ReciprocalAcc = a.ReciprocalAcc,
                                    InventoryAmount = (decimal)(c != null ? c.Debit : 0),
                                    InventoryAmountCur = (decimal)(c != null ? c.DebitCur : 0),
                                    Credit = (decimal)(c != null ? c.Credit : 0),
                                    CreditCur = (decimal)(c != null ? c.CreditCur : 0),
                                    TaxCode = b != null ? b.TaxCode : null,
                                    Address = b != null ? b.Address : null,
                                    Tel = b != null ? b.Tel : null
                                }).ToList();
            }


            decimal surplusAmount﻿﻿ = 0; // define a variable
            decimal surplusAmount﻿﻿Cur = 0;
            decimal idNature = -1;
            string product1 = null;
            string product2 = null;
            decimal inventoryAmount = 0;
            decimal inventoryAmountCur = 0;

            var lst = new List<CompareConfirmBebtDto>();
            var lstPartners = incurredData.GroupBy(p => new
            {
                PartnerCode = p.PartnerCode ?? "",
            }).Select(p => new AccountBalanceDto()
            {
                AccCode = "",
                PartnerCode = p.Key.PartnerCode,
                SectionCode = "",
                WorkPlaceCode = "",
                CurrencyCode = "",
                ContractCode = "",
                FProductCode = "",
                Debit = p.Sum(s => s.Debit),
                DebitCur = p.Sum(s => s.DebitCur),
                Credit = p.Sum(s => s.Credit),
                CreditCur = p.Sum(s => s.CreditCur)
            }).OrderBy(p => p.PartnerCode).ToList();

            foreach (var items in lstPartners)
            {
                decimal balance = 0;
                decimal balanceCur = 0;
                decimal credit = 0;
                decimal creditCur = 0;
                int i = 1;
                var incurredDataPartner = incurredData.Where(p => p.PartnerCode == items.PartnerCode)
                                     .OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                                     .ThenBy(p => p.VoucherId).ThenBy(p => p.Ord0).ToList();

                if (incurredDataPartner.Count > 0)
                {
                    var openingBalances = openingBalance.Where(p => p.PartnerCode == items.PartnerCode).ToList();
                    foreach (var itemOpeningBalancePartner in openingBalances)
                    {
                        balance = itemOpeningBalancePartner.Debit.GetValueOrDefault(0);
                        balanceCur = itemOpeningBalancePartner.DebitCur.GetValueOrDefault(0);
                        credit = itemOpeningBalancePartner.Credit.GetValueOrDefault(0);
                        creditCur = itemOpeningBalancePartner.CreditCur.GetValueOrDefault(0);
                    }

                    decimal totalDebitIncurredCur = 0,
                            totalDebitIncurred = 0,
                            totalCreditIncurred = 0,
                            totalCreditIncurredCur = 0;
                    var accPartner = await _accPartnerService.GetQueryableAsync();
                    var lstaccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == items.PartnerCode);

                    var accPartnerName = lstaccPartner.FirstOrDefault()?.Name ?? "";
                    lst.Add(new CompareConfirmBebtDto()
                    {
                        Bold = "C",
                        Note = accPartnerName,
                        //BalanceCur = balanceCur,
                        //Balance = balance,
                        //Credit = credit,
                        //CreditCur = creditCur
                    });
                    lst.Add(new CompareConfirmBebtDto()
                    {
                        Bold = "C",
                        Note = "Số dư đầu kỳ",
                        DebitCur = balanceCur,
                        Debit = balance,
                        Credit = credit,
                        CreditCur = creditCur,
                        AmountDky = balance
                    });
                    foreach (var item in incurredDataPartner)
                    {
                        if (i == 1)
                        {
                            balance = balance + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0);
                            balanceCur = balanceCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                            credit = balance != 0 ? 0 : credit + item.DebitIncurred.GetValueOrDefault(0) > 0 ? credit + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0) : item.CreditIncurred.GetValueOrDefault(0);
                            creditCur = balanceCur != 0 ? 0 : creditCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                            totalDebitIncurredCur = totalDebitIncurredCur + item.DebitIncurredCur.GetValueOrDefault(0);
                            totalDebitIncurred = totalDebitIncurred + item.DebitIncurred.GetValueOrDefault(0);
                            totalCreditIncurredCur = totalCreditIncurredCur + item.CreditIncurredCur.GetValueOrDefault(0);
                            totalCreditIncurred = totalCreditIncurred + item.CreditIncurred.GetValueOrDefault(0);
                        }
                        else
                        {
                            balance = balance + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0);
                            balanceCur = balanceCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                            credit = balance != 0 ? 0 : credit + item.CreditIncurred.GetValueOrDefault(0);
                            creditCur = balanceCur != 0 ? 0 : creditCur + balanceCur + item.CreditIncurredCur.GetValueOrDefault(0);
                            totalDebitIncurredCur = totalDebitIncurredCur + item.DebitIncurredCur.GetValueOrDefault(0);
                            totalDebitIncurred = totalDebitIncurred + item.DebitIncurred.GetValueOrDefault(0);
                            totalCreditIncurredCur = totalCreditIncurredCur + item.CreditIncurredCur.GetValueOrDefault(0);
                            totalCreditIncurred = totalCreditIncurred + item.CreditIncurred.GetValueOrDefault(0);
                        }
                        i += 1;

                        item.PartnerName = await GetPartnerName(orgCode, item.PartnerCode);
                        item.Debit = balance;
                        item.DebitCur = balanceCur;
                        item.Credit = credit;
                        item.CreditCur = creditCur;
                    }
                    lst.AddRange(incurredDataPartner);
                    lst.Add(new CompareConfirmBebtDto()
                    {
                        Sort = "B",
                        Bold = "C",
                        Note = "Phát sinh trong kỳ",
                        DebitIncurredCur = totalDebitIncurredCur,
                        DebitIncurred = totalDebitIncurred,
                        CreditIncurredCur = totalCreditIncurredCur,
                        CreditIncurred = totalCreditIncurred,
                        TotalCredit = totalCreditIncurred,
                        TotalDebit = totalDebitIncurred
                    });
                    lst.Add(new CompareConfirmBebtDto()
                    {
                        Bold = "C",
                        Note = "Số dư cuối kỳ",
                        DebitCur = balanceCur,
                        Debit = balance,
                        Credit = credit,
                        CreditCur = creditCur,
                        AmountCky = balance
                    });
                    //var reciprocalAccs = await GetSumByReciprocalAcc(incurredDataPartner);
                    //lst.AddRange(reciprocalAccs);
                    lst.Add(new CompareConfirmBebtDto()
                    {
                        Bold = "C",
                        Note = "-------------------------------"
                    });
                }
            }


            var reportResponse = new ReportResponseDto<CompareConfirmBebtDto>();
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
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
        private async Task<List<CompareConfirmBebtDto>> GetIncurredData(Dictionary<string, object> dic)
        {

            var warehouseBook = await GetDataledger(dic);
            var incurredData = warehouseBook.Where(p => p.Status == "1").GroupBy(p => new
            {
                p.OrgCode,
                p.Year,
                p.VoucherCode,
                p.VoucherNumber,
                p.VoucherDate,
                p.DebitAcc,
                p.CreditAcc,
                p.Description,
                p.InvoiceDate,
                p.InvoiceNumber,
                p.CurrencyCode,
                p.ExchangeRate,
                p.InvoicePartnerName,
                p.CheckDuplicate,
                p.CreditPartnerCode,
                p.DebitPartnerCode,
                p.PartnerCode,
                p.PartnerName0
            }).Select(p => new CompareConfirmBebtDto()
            {
                OrgCode = p.Max(p => p.Ord0),
                PartnerCode = p.Max(p => p.PartnerCode),
                PartnerName = p.Max(p => p.PartnerName0),
                Year = p.Max(p => p.Year),
                VoucherCode = p.Max(p => p.VoucherCode),
                VoucherNumber = p.Max(p => p.VoucherNumber),
                VoucherDate = p.Max(p => p.VoucherDate),
                VoucherId = p.Max(p => p.VoucherId),
                Id = p.Max(p => p.Id),
                Ord0 = p.Max(p => p.Ord0),
                DebitAcc = p.Max(p => p.DebitAcc),
                CreditAcc = p.Max(p => p.CreditAcc),
                DebitAmountCur = p.Sum(p => p.DebitAmountCur),
                CreditAmountCur = p.Sum(p => p.CreditAmountCur),
                Amount = p.Sum(p => p.Amount0),
                Note = p.Max(p => p.Note),
                Description = p.Max(p => p.Description),
                InvoiceDate = p.Max(p => p.InvoiceDate),
                InvoiceNumber = p.Max(p => p.InvoiceNumber),
                CurrencyCode = p.Max(p => p.CurrencyCode),
                ExchangeRate = p.Max(p => p.ExchangeRate),
                InvoicePartnerName = p.Max(p => p.InvoicePartnerName),
                CheckDuplicate = p.Max(p => p.CheckDuplicate),
                CreditPartnerCode = p.Max(p => p.CreditPartnerCode),
                DebitPartnerCode = p.Max(p => p.DebitPartnerCode),
                DebitIncurred = p.Sum(p => p.Amount0),
                DebitIncurredCur = p.Sum(p => p.AmountCur0),
                CreditIncurred = p.Sum(p => p.Amount0),
                CreditIncurredCur = p.Sum(p => p.CreditAmountCur),
                ReciprocalAcc = p.Max(p => p.ReciprocalAcc),

            }).ToList();


            return incurredData;
        }

        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string, object> dic)
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
            if (!string.IsNullOrEmpty(dto.PartnerGroup))
            {
                dic.Add(LedgerParameterConst.PartnerGroup, dto.PartnerGroup);
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

            return dic;
        }
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
                PartnerCode = openingBalances.Max(p => p.PartnerCode),
                SectionCode = openingBalances.Max(p => p.SectionCode),
                FProductCode = openingBalances.Max(p => p.FProductCode),
                WorkPlaceCode = openingBalances.Max(p => p.WorkPlaceCode),
                ContractCode = openingBalances.Max(p => p.ContractCode),
                CurrencyCode = openingBalances.Max(p => p.CurrencyCode),
                AccCode = openingBalances.Max(p => p.AccCode),
                Debit = openingBalances.Sum(p => p.Debit),
                Credit = openingBalances.Sum(p => p.Credit),
                DebitCur = openingBalances.Sum(p => p.DebitCur),
                CreditCur = openingBalances.Sum(p => p.CreditCur),
                DebitIncurred = openingBalances.Sum(p => p.DebitIncurred),
                DebitIncurredCur = openingBalances.Sum(p => p.DebitIncurredCur),
                CreditIncurred = openingBalances.Sum(p => p.CreditIncurred),
                CreditIncurredCur = openingBalances.Sum(p => p.CreditIncurredCur)
            };
            List<AccountBalanceDto> list = new List<AccountBalanceDto>();
            list.Add(balances);
            return list;
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

