using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.DebitBooks
{
    public class CutomerBalanceDetailProductAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly ProductService _productService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly PartnerGroupAppService _partnerGroupAppService;
        private readonly OrgUnitService _orgUnitService;
        private readonly UnitService _unitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public CutomerBalanceDetailProductAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        ProductService productService,
                        AccPartnerAppService accPartnerAppService,
                        PartnerGroupAppService partnerGroupAppService,
                        OrgUnitService orgUnitService,
                        UnitService unitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager
                        )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _productService = productService;
            _unitService = unitService;
            _accPartnerAppService = accPartnerAppService;
            _partnerGroupAppService = partnerGroupAppService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        public async Task<ReportResponseDto<CutomerBalanceDetailProductDto>> CreateDataAsync(ReportRequestDto<CutomerBalanceDetailBookParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalances = await GetOpeningBalance(dic);
            var incurredData = await GetIncurredData(dic);
            openingBalances.AddRange(incurredData.Select(p => new AccountBalanceDto
            {
                AccCode = p.Acc,
                PartnerCode = p.PartnerCode,
                FProductCode = "",
                WorkPlaceCode = "",
                SectionCode = "",
                CurrencyCode = p.CurrencyCode,
                ContractCode = p.ContractCode,
            }));
            var lst = new List<CutomerBalanceDetailProductDto>();
            var accSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccSystem = accSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            // chỉ lấy đối tượng trong nhóm đối tượng ở dto
            if (dto.Parameters.PartnerGroup != "")
            {
                var partnerGroup = await _partnerGroupAppService.GetByIdAsync(dto.Parameters.PartnerGroup);
                dto.Parameters.PartnerGroup = partnerGroup?.Code ?? "";
                var accPartner = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroup);
                incurredData = incurredData.Where(p => accPartner.Select(a => a.Code).Contains(p.PartnerCode))
                                                                 .OrderBy(p => p.PartnerCode).ThenBy(p => p.VoucherDate)
                                                                 .ThenBy(p => p.VoucherNumber).ThenBy(p => p.ReciprocalAcc)
                                                                 .ThenBy(p => p.VoucherId).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                incurredData = incurredData.Where(p => p.PartnerCode == dto.Parameters.PartnerCode).ToList();
            }

            var dataGroupAcc = (from a in incurredData
                                join b in lstAccSystem on new { AccCode = a.ReciprocalAcc, Year = a.Year } equals new { AccCode = b.AccCode, Year = b.Year } into ajb
                                from b in ajb.DefaultIfEmpty()
                                group new { a, b } by new
                                {
                                    a.PartnerCode,
                                    a.ReciprocalAcc,
                                    AccName = b?.AccName ?? ""
                                } into gr
                                select new
                                {
                                    PartnerCode = gr.Key.PartnerCode,
                                    ReciprocalAcc = gr.Key.ReciprocalAcc,
                                    AccName = gr.Key.AccName,
                                    DebitIncurredCur = gr.Sum(p => p.a.DebitIncurredCur ?? 0),
                                    DebitIncurred = gr.Sum(p => p.a.DebitIncurred ?? 0),
                                    CreditIncurredCur = gr.Sum(p => p.a.CreditIncurredCur ?? 0),
                                    CreditIncurred = gr.Sum(p => p.a.CreditIncurred ?? 0),
                                }).ToList();
            var lstOpeningBalancePartner = openingBalances.GroupBy(p => new
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
                Debit = p.Sum(s => s.Debit ?? 0),
                DebitCur = p.Sum(s => s.DebitCur ?? 0),
                Credit = p.Sum(s => s.Credit ?? 0),
                CreditCur = p.Sum(s => s.CreditCur ?? 0)
            }).OrderBy(p => p.PartnerCode).ToList();
            var accPartnerss = await _accPartnerService.GetQueryableAsync();
            foreach (var itemOpeningBalancePartner in lstOpeningBalancePartner)
            {
                var incurredDataPartner = incurredData.Where(p => p.PartnerCode == itemOpeningBalancePartner.PartnerCode)
                                      .OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                                      .ThenBy(p => p.VoucherId).ThenBy(p => p.Ord0).ToList();
                if (incurredDataPartner.Count > 0)
                {
                    decimal balance = itemOpeningBalancePartner.Debit.GetValueOrDefault(0) - itemOpeningBalancePartner.Credit.GetValueOrDefault(0);
                    decimal balanceCur = itemOpeningBalancePartner.DebitCur.GetValueOrDefault(0) - itemOpeningBalancePartner.CreditCur.GetValueOrDefault(0);
                    decimal totalDebitIncurredCur = 0,
                            totalDebitIncurred = 0,
                            totalCreditIncurred = 0,
                            totalCreditIncurredCur = 0;

                    var accPartners = accPartnerss.Where(p => p.Code == itemOpeningBalancePartner.PartnerCode);
                    var accPartnerName = accPartners.FirstOrDefault()?.Name ?? "";
                    lst.Add(new CutomerBalanceDetailProductDto()
                    {
                        Bold = "C",
                        Note = accPartnerName,
                        BalanceCur = balanceCur,
                        Balance = balance
                    });
                    lst.Add(new CutomerBalanceDetailProductDto()
                    {
                        Bold = "C",
                        Note = "Số dư đầu kỳ",
                        BalanceCur = balanceCur,
                        Balance = balance
                    });
                    foreach (var item in incurredDataPartner)
                    {
                        balance = balance + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0);
                        balanceCur = balanceCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                        totalDebitIncurredCur = totalDebitIncurredCur + item.DebitIncurredCur.GetValueOrDefault(0);
                        totalDebitIncurred = totalDebitIncurred + item.DebitIncurred.GetValueOrDefault(0);
                        totalCreditIncurredCur = totalCreditIncurredCur + item.CreditIncurredCur.GetValueOrDefault(0);
                        totalCreditIncurred = totalCreditIncurred + item.CreditIncurred.GetValueOrDefault(0);
                        var product = await GetProduct(orgCode, item.ProductCode);
                        item.PartnerName = await GetPartnerName(orgCode, item.PartnerCode);
                        item.ProductName = product?.Name ?? null;
                        item.UnitCode = product?.UnitCode ?? null;
                        item.Balance = balance;
                        item.BalanceCur = balanceCur;
                    }
                    lst.AddRange(incurredDataPartner);
                    lst.Add(new CutomerBalanceDetailProductDto()
                    {
                        Sort = "B",
                        Bold = "C",
                        Note = "Phát sinh trong kỳ",
                        DebitIncurredCur = totalDebitIncurredCur,
                        DebitIncurred = totalDebitIncurred,
                        CreditIncurredCur = totalCreditIncurredCur,
                        CreditIncurred = totalCreditIncurred
                    });
                    lst.Add(new CutomerBalanceDetailProductDto()
                    {
                        Bold = "C",
                        Note = "Số dư cuối kỳ",
                        BalanceCur = balanceCur,
                        Balance = balance
                    });
                    var reciprocalAccs = await GetSumByReciprocalAcc(incurredDataPartner);
                    lst.AddRange(reciprocalAccs);
                    lst.Add(new CutomerBalanceDetailProductDto()
                    {
                        Bold = "C",
                        Note = "-------------------------------"
                    });
                }
            }
            var reportResponse = new ReportResponseDto<CutomerBalanceDetailProductDto>();
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<CutomerBalanceDetailBookParameterDto> dto)
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
        private async Task<List<CutomerBalanceDetailProductDto>> GetSumByReciprocalAcc(List<CutomerBalanceDetailProductDto> accountDetailBooks)
        {
            if (accountDetailBooks.Count == 0) return new List<CutomerBalanceDetailProductDto>();
            var accounts = await _accountSystemService.GetAccounts(_webHelper.GetCurrentOrgUnit());

            var groupData = accountDetailBooks.GroupBy(g => new { g.ReciprocalAcc, g.Year })
                            .Select(p => new CutomerBalanceDetailProductDto()
                            {
                                Year = p.Key.Year,
                                ReciprocalAcc = p.Key.ReciprocalAcc,
                                DebitIncurredCur = p.Sum(i => i.DebitIncurredCur),
                                DebitIncurred = p.Sum(i => i.DebitIncurred),
                                CreditIncurredCur = p.Sum(i => i.CreditIncurredCur),
                                CreditIncurred = p.Sum(i => i.CreditIncurred)
                            }).ToList();

            var query = from d in groupData
                        join a in accounts on new { AccCode = d.ReciprocalAcc, d.Year } equals new { a.AccCode, a.Year } into ac
                        from n in ac.DefaultIfEmpty()
                        orderby d.ReciprocalAcc
                        select new CutomerBalanceDetailProductDto()
                        {
                            Bold = "C",
                            Year = d.Year,
                            ReciprocalAcc = d.ReciprocalAcc,
                            DebitIncurredCur = d.DebitIncurredCur,
                            DebitIncurred = d.DebitIncurred,
                            CreditIncurredCur = d.CreditIncurredCur,
                            CreditIncurred = d.CreditIncurred,
                            Note = n?.AccName ?? String.Empty
                        };
            return query.ToList();
        }
        private async Task<List<CutomerBalanceDetailProductDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var ledgerData = await GetDataledger(dic);
            var incurredData = ledgerData.GroupBy(g => new
            {
                g.OrgCode,
                g.Year,
                g.VoucherCode,
                g.VoucherDate,
                g.VoucherNumber,
                g.VoucherGroup,
                g.VoucherId,
                g.Note,
                g.PartnerCode,
                g.ProductCode,
                g.FProductWorkCode,
                g.Representative,
                g.ReciprocalAcc,
                g.Acc,
                g.CurrencyCode,
                g.ExchangeRate,
                g.InvoicePartnerName,
                g.InvoiceNumber,
                g.InvoiceDate,
                g.ContractCode,
            }).Select(p => new CutomerBalanceDetailProductDto()
            {
                OrgCode = p.Key.OrgCode,
                Year = p.Key.Year,
                VoucherCode = p.Key.VoucherCode,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                VoucherId = p.Key.VoucherId,
                Note = p.Key.Note,
                PartnerCode = p.Key.PartnerCode,
                ProductCode = p.Key.ProductCode,
                FProductWorkCode = p.Key.FProductWorkCode,
                Representative = p.Key.Representative,
                ReciprocalAcc = p.Key.ReciprocalAcc,
                Ord0 = p.Max(g => g.Ord0),
                Acc = p.Key.Acc,
                CurrencyCode = p.Key.CurrencyCode,
                ExchangeRate = p.Key.ExchangeRate,
                InvoicePartnerName = p.Key.InvoicePartnerName,
                InvoiceNumber = p.Key.InvoiceNumber,
                InvoiceDate = p.Key.InvoiceDate,
                ContractCode = p.Key.ContractCode,
                DebitIncurredCur = p.Sum(g => g.DebitIncurredCur),
                DebitIncurred = p.Sum(g => g.DebitIncurred),
                CreditIncurredCur = p.Sum(g => g.CreditIncurredCur),
                CreditIncurred = p.Sum(g => g.CreditIncurred),
                Quantity = p.Sum(g => g.Quantity),
                Price = p.Max(g => g.Price)
            }).OrderBy(p => p.PartnerCode)
                .ThenBy(p => p.VoucherDate)
                .ThenBy(p => p.VoucherNumber)
                .ThenBy(p => p.ReciprocalAcc)
                .ThenBy(p => p.VoucherId).ToList();
            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private Dictionary<string, object> GetLedgerParameter(CutomerBalanceDetailBookParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
            //dic.Add(LedgerParameterConst.PartnerCode, dto.PartnerCode);
            dic.Add(LedgerParameterConst.PartnerGroup, dto.PartnerGroup);
            dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            dic.Add(LedgerParameterConst.CurrencyCode, dto.CurrencyCode);
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
            return openingBalances;
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
        private async Task<string> GetProductName(string orgCode, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var product = await _productService.GetByCodeAsync(code, orgCode);
            if (product == null) return null;
            return product.Name;
        }

        private async Task<Product> GetProduct(string orgCode, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var product = await _productService.GetByCodeAsync(code, orgCode);
            if (product == null) return null;
            return product;
        }
        #endregion
    }
}
