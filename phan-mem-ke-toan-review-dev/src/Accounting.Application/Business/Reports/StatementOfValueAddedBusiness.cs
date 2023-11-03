using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Licenses;
using Accounting.Localization;
using Accounting.Report;
using Accounting.Reports;
using Accounting.Reports.Cores;
using Accounting.Reports.Financials;
using Accounting.Reports.Financials.StatementOfValueAddedTax;
using Accounting.Reports.Tenants;
using Accounting.Reports.Tenants.TenantStatementTaxs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.Business
{
    public class StatementOfValueAddedBusiness : BaseBusiness, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly TenantStatementTaxDataService _tenantStatementTaxDataService;
        private readonly DefaultStatementTaxService _defaultStatementTaxService;
        private readonly TenantStatementTaxService _tenantStatementTaxService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly DefaultAccBalanceSheetService _defaultAccBalanceSheetService;
        private readonly DefaultCashFollowStatementService _defaultCashFollowStatementService;
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly TenantCashFollowStatementService _tenantCashFollowStatementService;
        private readonly OrgUnitService _orgUnitService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly IObjectMapper _objectMapper;
        private readonly AccountingCacheManager _accountingCacheManager;

        #endregion
        public StatementOfValueAddedBusiness(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        TenantStatementTaxDataService tenantStatementTaxDataService,
                        DefaultStatementTaxService defaultStatementTaxService,
                        TenantStatementTaxService tenantStatementTaxService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                        DefaultCashFollowStatementService defaultCashFollowStatementService,
                        TenantAccBalanceSheetService tenantAccBalanceSheetService,
                        TenantCashFollowStatementService tenantCashFollowStatementService,
                        OrgUnitService orgUnitService,
                        AccountingCacheManager accountingCacheManager,
                        AccTaxDetailService accTaxDetailService,
                        TaxCategoryService taxCategoryService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        IObjectMapper objectMapper,
                        IStringLocalizer<AccountingResource> localizer) : base(localizer)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _tenantStatementTaxDataService = tenantStatementTaxDataService;
            _defaultStatementTaxService = defaultStatementTaxService;
            _tenantStatementTaxService = tenantStatementTaxService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _defaultAccBalanceSheetService = defaultAccBalanceSheetService;
            _defaultCashFollowStatementService = defaultCashFollowStatementService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _tenantCashFollowStatementService = tenantCashFollowStatementService;
            _orgUnitService = orgUnitService;
            _accTaxDetailService = accTaxDetailService;
            _taxCategoryService = taxCategoryService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _objectMapper = objectMapper;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        public async Task<ReportResponseDto<DataStatementTaxDto>> CreateDataAsync(ReportRequestDto<StatementOfValueAddedTaxParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var fromMonth = dto.Parameters.FromDate.Month;
            var toMonth = dto.Parameters.ToDate.Month;
            var year = dto.Parameters.FromDate.Year;
            var taxPeriod = "";
            if (fromMonth == toMonth)
            {
                taxPeriod = "Tháng " + (fromMonth < 10 ? "0" + fromMonth : fromMonth) + " năm " + year;
            } 
            else if (fromMonth == 1 && toMonth == 3)
            {
                taxPeriod = "Qúy I năm " + year;
            }
            else if (fromMonth == 4 && toMonth == 6)
            {
                taxPeriod = "Qúy II năm " + year;
            }
            else if (fromMonth == 7 && toMonth == 9)
            {
                taxPeriod = "Qúy III năm " + year;
            }
            else if (fromMonth == 10 && toMonth == 12)
            {
                taxPeriod = "Qúy IV năm " + year;
            }
            else if (fromMonth == 1 && toMonth == 12)
            {
                taxPeriod = "Năm " + year;
            }
            else
            {
                taxPeriod = "Từ ngày " + dto.Parameters.FromDate.ToString("dd/MM/yyyy") + " đến ngày " + dto.Parameters.ToDate.ToString("dd/MM/yyyy");
            }
            var lst = new List<CashFlowDto>();
            var accTaxDetail = await _accTaxDetailService.GetQueryableAsync();
            var lstAccTaxDetail = accTaxDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var taxCategory = await _taxCategoryService.GetQueryableAsync();
            var lstTaxCategory = taxCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var tenantStatementTax = await _tenantStatementTaxService.GetAllAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            var defaultStatementTax = await _defaultStatementTaxService.GetAllAsync();
            var lstStatementTax = (tenantStatementTax.Count > 0) ?
                                      tenantStatementTax.OrderBy(p => p.Ord).Select(p => _objectMapper.Map<TenantStatementTax, TenantStatementTaxDto>(p)).ToList() :
                                      defaultStatementTax.OrderBy(p => p.Ord).Select(p => _objectMapper.Map<DefaultStatementTax, TenantStatementTaxDto>(p)).ToList();

            var statementTaxData = await _tenantStatementTaxDataService.GetQueryableAsync();
            var lstStatementTaxData = statementTaxData.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (!lstStatementTaxData.Any(p => p.BeginDate == dto.Parameters.FromDate && p.EndDate == dto.Parameters.ToDate))
            {
                var itemStatementTaxData = new TenantStatementTaxDataDto
                {
                    Id = GetNewObjectId(),
                    OrgCode = _webHelper.GetCurrentOrgUnit(),
                    BeginDate = dto.Parameters.FromDate,
                    EndDate = dto.Parameters.ToDate,
                    Extend = dto.Parameters.Extend,
                    DeductPre = dto.Parameters.DeductPre,
                    IncreasePre = dto.Parameters.IncreasePre,
                    ReducePre = dto.Parameters.ReducePre,
                    SuggestionReturn = dto.Parameters.SuggestionReturn,
                };
                var entity = _objectMapper.Map<TenantStatementTaxDataDto, TenantStatementTaxData>(itemStatementTaxData);
                await _tenantStatementTaxDataService.CreateAsync(entity, true);
            }
            else
            {
                var itemStatementTaxData = lstStatementTaxData.Where(p => p.BeginDate == dto.Parameters.FromDate && p.EndDate == dto.Parameters.ToDate).FirstOrDefault();
                if (itemStatementTaxData != null)
                {
                    itemStatementTaxData.Extend = dto.Parameters.Extend;
                    itemStatementTaxData.DeductPre = dto.Parameters.DeductPre;
                    itemStatementTaxData.IncreasePre = dto.Parameters.IncreasePre;
                    itemStatementTaxData.ReducePre = dto.Parameters.ReducePre;
                    itemStatementTaxData.SuggestionReturn = dto.Parameters.SuggestionReturn;
                    await _tenantStatementTaxDataService.UpdateAsync(itemStatementTaxData, true);
                }
            }
            // lấy số dư
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dic);
            // lấy dữ liệu phát sinh thuế
            var dataTaxDetail = (from a in lstAccTaxDetail
                                 join b in lstTaxCategory on a.TaxCategoryCode equals b.Code into ajb
                                 from b in ajb.DefaultIfEmpty()
                                 where a.VoucherDate >= dto.Parameters.FromDate
                                 && a.VoucherDate <= dto.Parameters.ToDate
                                 group new { a, b } by new
                                 {
                                     a.OrgCode,
                                     a.InvoiceGroup,
                                     TaxCategoryCode = a.TaxCategoryCode ?? "",
                                     OutOrIn = (b == null ? "" : b.OutOrIn ?? ""),
                                     DebitAcc = a.DebitAcc,
                                     CreditAcc = a.CreditAcc,
                                     Deduct = (b == null ? 0 : b.Deduct),
                                 } into gr
                                 select new DataTaxDetailDto
                                 {
                                     OrgCode = _webHelper.GetCurrentOrgUnit(),
                                     InvoiceGroup = gr.Key.InvoiceGroup,
                                     TaxCategoryCode = gr.Key.TaxCategoryCode,
                                     OutOrIn = gr.Key.OutOrIn,
                                     DebitAcc = gr.Key.DebitAcc,
                                     CreditAcc = gr.Key.CreditAcc,
                                     DebitAcc0 = gr.Key.DebitAcc,
                                     CreditAcc0 = gr.Key.CreditAcc,
                                     Deduct = gr.Key.Deduct,
                                     AmountWithoutVat = gr.Sum(p => p.a.AmountWithoutVat),
                                     Amount = gr.Sum(p => p.a.Amount),
                                 }).ToList();
            foreach (var itemTaxDetail in dataTaxDetail)
            {
                if ((itemTaxDetail.OutOrIn == "V" && itemTaxDetail.CreditAcc.StartsWith(dto.Parameters.AccBuy))
                    || (itemTaxDetail.OutOrIn == "R" && itemTaxDetail.DebitAcc.StartsWith(dto.Parameters.AccSell)))
                {
                    itemTaxDetail.DebitAcc = itemTaxDetail.CreditAcc0;
                    itemTaxDetail.CreditAcc = itemTaxDetail.DebitAcc0;
                    itemTaxDetail.AmountWithoutVat = itemTaxDetail.AmountWithoutVat * (-1);
                    itemTaxDetail.Amount = itemTaxDetail.Amount * (-1);
                }
            }
            var totalAmountWithoutVat0 = dataTaxDetail.Where(p => (p.CreditAcc ?? "").StartsWith(dto.Parameters.AccSell)).Sum(p => p.AmountWithoutVat ?? 0);
            var totalAmountWithoutVat = dataTaxDetail.Where(p => (p.CreditAcc ?? "").StartsWith(dto.Parameters.AccSell)).Sum(p => (p.Deduct == 1) ? p.AmountWithoutVat ?? 0 : 0);
            var pTDT = (totalAmountWithoutVat != 0) ? totalAmountWithoutVat * 100 / totalAmountWithoutVat0 : 10;
            var amountBuy = dataTaxDetail.Where(p => (p.CreditAcc ?? "").StartsWith(dto.Parameters.AccBuy)).Sum(p => (p.InvoiceGroup == "1") ? p.Amount ?? 0 : 0);
            var amount25b = dataTaxDetail.Where(p => (p.CreditAcc ?? "").StartsWith(dto.Parameters.AccBuy)).Sum(p => (p.InvoiceGroup == "2") ? p.Amount ?? 0 : 0);
            var amount25 = amountBuy;
            var amount25c = (amount25b * pTDT) / 100;
            amount25b = amount25b - amount25c;
            var deductPre = dto.Parameters.DeductPre ?? 0;
            var increasePre = dto.Parameters.IncreasePre ?? 0;
            var reducePre = dto.Parameters.ReducePre ?? 0;
            var suggestionReturn = dto.Parameters.SuggestionReturn ?? 0;
            decimal amountWithoutVat = 0;
            decimal amountVat = 0;
            var dataStatementTax = lstStatementTax.Select(p =>
                                new DataStatementTaxDto
                                {
                                    Year = p.Year,
                                    Ord = p.Ord,
                                    Printable = p.Printable,
                                    Bold = p.Bold,
                                    Ord0 = p.Ord0,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Rank = p.Rank,
                                    NumberCode = p.NumberCode,
                                    Formular = p.Formular,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    Condition = p.Condition,
                                    Sign = p.Sign,
                                    NumberCode1 = p.NumberCode1,
                                    Amount1 = p.Amount1,
                                    NumberCode2 = p.NumberCode2,
                                    Amount2 = p.Amount2,
                                    PrintWhen = p.PrintWhen,
                                    Id11 = p.Id11,
                                    Id12 = p.Id12,
                                    Id21 = p.Id21,
                                    Id22 = p.Id22,
                                    En1 = p.En1,
                                    En2 = p.En2,
                                    Re1 = p.Re1,
                                    Re2 = p.Re2,
                                    Va1 = p.Va1,
                                    Va2 = p.Va2,
                                    Mt1 = p.Mt1,
                                    Mt2 = p.Mt2,
                                    AssignValue = p.AssignValue,
                                    AmountWithoutVat = p.Amount1,
                                    AmountVat = p.Amount2,
                                }).ToList();
            var lstJobTaxDetail = new List<JsonObject>();
            foreach (var item in dataTaxDetail)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = (JsonObject)JsonObject.Parse(json);
                lstJobTaxDetail.Add((JsonObject)job);
            }
            var dataForeach = dataStatementTax.Where(p => p.Rank == 9).ToList();
            foreach (var item in dataForeach)
            {
                amountWithoutVat = 0;
                amountVat = 0;
                if (item.AssignValue != "")
                {
                    foreach (var itemStatementTax in dataStatementTax)
                    {
                        if (itemStatementTax.NumberCode == item.NumberCode)
                        {
                            switch (item.AssignValue)
                            {
                                case "deductPre":
                                    itemStatementTax.AmountVat = deductPre;
                                    break;
                                case "amount25":
                                    itemStatementTax.AmountVat = amount25;
                                    break;
                                case "amount25b":
                                    itemStatementTax.AmountVat = amount25b;
                                    break;
                                case "amount25c":
                                    itemStatementTax.AmountVat = amount25c;
                                    break;
                                case "increasePre":
                                    itemStatementTax.AmountVat = increasePre;
                                    break;
                                case "reducePre":
                                    itemStatementTax.AmountVat = reducePre;
                                    break;
                                case "suggestionReturn":
                                    itemStatementTax.AmountVat = suggestionReturn;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (item.Condition != "")
                    {
                        var lstJobFind = await AddCondition(lstJobTaxDetail, item.Condition);
                        if (item.DebitAcc != "") lstJobFind = lstJobFind.Where(p => p["DebitAcc"].ToString().StartsWith(item.DebitAcc)).ToList();
                        if (item.CreditAcc != "") lstJobFind = lstJobFind.Where(p => p["CreditAcc"].ToString().StartsWith(item.CreditAcc)).ToList();
                        amountWithoutVat = lstJobFind.Sum(p => decimal.Parse(p["AmountWithoutVat"].ToString() ?? "0", CultureInfo.InvariantCulture));
                        amountVat = lstJobFind.Sum(p => decimal.Parse(p["Amount"].ToString() ?? "0", CultureInfo.InvariantCulture));
                        foreach (var itemStatementTax in dataStatementTax)
                        {
                            if (itemStatementTax.NumberCode == item.NumberCode)
                            {
                                itemStatementTax.AmountWithoutVat = (item.Printable == "K" || item.NumberCode1 != "") ? Math.Round(amountWithoutVat, 0) : 0;
                                itemStatementTax.AmountVat = (item.Printable == "K" || item.NumberCode2 != "") ? Math.Round(amountVat, 0) : 0;
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemStatementTax in dataStatementTax)
                        {
                            if (itemStatementTax.NumberCode == item.NumberCode)
                            {
                                itemStatementTax.AmountWithoutVat = 0;
                                itemStatementTax.AmountVat = 0;
                            }
                        }
                    }
                }

            }
            // Tinh cac chi tieu con lai theo cong thuc
            var rankMax = (dataStatementTax.Count > 0) ? dataStatementTax.Max(p => p.Rank) : 0;
            while (rankMax > 0)
            {
                var dataStatementTaxByRank = dataStatementTax.Where(p => p.Rank == rankMax && p.Rank != 9).ToList();
                foreach (var item in dataStatementTaxByRank)
                {
                    amountWithoutVat = 0;
                    amountVat = 0;
                    if ((item.Formular ?? "") == "")
                    {
                        if (item.DebitAcc != "" || item.CreditAcc != "")
                        {
                            var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                            var lstCreditAcc = GetSplit(item.CreditAcc, ',');
                            var dataJoin = (from a in dataStatementTax
                                            join b in lstDebitAcc on 0 equals 0
                                            where (a.DebitAcc ?? "").StartsWith(b.Data)
                                            join c in lstCreditAcc on 0 equals 0
                                            select new
                                            {
                                                AmountWithoutVat = a.AmountWithoutVat ?? 0,
                                                AmountVat = a.AmountVat ?? 0,
                                            }).ToList();
                            amountWithoutVat = dataJoin.Sum(p => Math.Round(p.AmountWithoutVat));
                            amountVat = dataJoin.Sum(p => Math.Round(p.AmountVat));
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in dataStatementTax on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            AmountWithoutVat = (a.Math == "+") ? b?.AmountWithoutVat ?? 0 : -1 * b?.AmountWithoutVat ?? 0,
                                            AmountVat = (a.Math == "+") ? b?.AmountVat ?? 0 : -1 * b?.AmountVat ?? 0
                                        }).ToList();
                        amountWithoutVat = dataJoin.Sum(p => p.AmountWithoutVat);
                        amountVat = dataJoin.Sum(p => p.AmountVat);
                    }

                    if (item.Condition == "amountVat>0" && amountVat < 0)
                    {
                        amountWithoutVat = 0;
                        amountVat = 0;
                    }
                    if (item.Condition == "amountVat<0")
                    {
                        if (amountVat > 0)
                        {
                            amountWithoutVat = 0;
                            amountVat = 0;
                        }
                        else
                        {
                            amountWithoutVat = amountWithoutVat * (-1);
                            amountVat = amountVat * (-1);
                        }
                    }
                    // update
                    foreach (var itemStatementTax in dataStatementTax)
                    {
                        if (itemStatementTax.NumberCode == item.NumberCode)
                        {
                            itemStatementTax.AmountWithoutVat = (item.Printable == "K" || item.NumberCode1 != "") ? Math.Round(amountWithoutVat, 0) : 0;
                            itemStatementTax.AmountVat = (item.Printable == "K" || item.NumberCode2 != "") ? Math.Round(amountVat, 0) : 0;
                        }
                        itemStatementTax.TaxPeriod = taxPeriod;
                    }
                }
                rankMax--;
            }
            var toDate = dto.Parameters.ToDate.AddDays(1);
            deductPre = dataStatementTax.Where(p => p.NumberCode2 == "[43]").Select(p => p.AmountVat).FirstOrDefault() ?? 0;
            // update bộ lọc
            statementTaxData = await _tenantStatementTaxDataService.GetQueryableAsync();
            lstStatementTaxData = statementTaxData.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (!lstStatementTaxData.Any(p => p.BeginDate == null && p.EndDate == toDate))
            {
                var itemStatementTaxData = new TenantStatementTaxDataDto
                {
                    Id = GetNewObjectId(),
                    OrgCode = _webHelper.GetCurrentOrgUnit(),
                    BeginDate = null,
                    EndDate = toDate,
                    DeductPre = deductPre
                };
                var entity = _objectMapper.Map<TenantStatementTaxDataDto, TenantStatementTaxData>(itemStatementTaxData);
                await _tenantStatementTaxDataService.CreateAsync(entity, true);
            }
            else
            {
                var itemStatementTaxData = lstStatementTaxData.Where(p => p.BeginDate == null && p.EndDate == toDate).FirstOrDefault();
                if (itemStatementTaxData != null)
                {
                    itemStatementTaxData.DeductPre = deductPre;
                    await _tenantStatementTaxDataService.UpdateAsync(itemStatementTaxData, true);
                }
            }

            var reportResponse = new ReportResponseDto<DataStatementTaxDto>();
            reportResponse.Data = dataStatementTax.Where(p => p.Printable == "C").OrderBy(p => p.Ord).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.OrgUnit.Year = _webHelper.GetCurrentYear();
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }

        public async Task<FileContentResult> PrintAsync(ReportRequestDto<StatementOfValueAddedTaxParameterDto> dto)
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

        public async Task<TenantStatementTaxDataDto> PostGetFilter(StatementOfValueAddedTaxParameterDto dto)
        {
            var statementTaxData = await _tenantStatementTaxDataService.GetQueryableAsync();
            var lstStatementTaxData = statementTaxData.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var itemStatementTaxDataPre = lstStatementTaxData.Where(p => p.BeginDate == null && p.EndDate == dto.FromDate).Select(p => _objectMapper.Map<TenantStatementTaxData, TenantStatementTaxDataDto>(p)).FirstOrDefault();
            var itemStatementTaxData = lstStatementTaxData.Where(p => p.BeginDate == dto.FromDate && p.EndDate == dto.ToDate).Select(p => _objectMapper.Map<TenantStatementTaxData, TenantStatementTaxDataDto>(p)).FirstOrDefault();
            if (itemStatementTaxData == null)
            {
                itemStatementTaxData = new TenantStatementTaxDataDto()
                {
                    BeginDate = dto.FromDate,
                    EndDate = dto.ToDate,
                    DeductPre = itemStatementTaxDataPre?.DeductPre ?? 0,
                    IncreasePre = 0,
                    ReducePre = 0,
                    SuggestionReturn = 0,
                    OrgCode = _webHelper.GetCurrentOrgUnit(),
                    Id = GetNewObjectId(),
                };
            }
            return itemStatementTaxData;
        }
        #endregion
        #region Private
        private List<FormularDto> GetFormular(string formular) // lấy list công thức
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

        private List<LstAccDto> GetSplit(string str, char spt) // lấy list
        {
            var lst = new List<LstAccDto>();
            var lstAcc = str.Split(spt).ToList();
            for (var i = 0; i < lstAcc.Count; i++)
            {
                lst.Add(new LstAccDto
                {
                    Id = i + 1,
                    Data = lstAcc[i],
                });
            }
            return lst;
        }

        private async Task<List<JsonObject>> AddCondition(List<JsonObject> jsonObject, string condition) // Add điều kiện
        {
            var jsonCondition = (JsonObject)JsonObject.Parse(condition);
            var lstJsonCondition = (JsonArray)jsonCondition["data"];
            foreach (var item in lstJsonCondition)
            {
                switch (item["Type"].ToString())
                {
                    case "Contains":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString().Contains(p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case "!Contains":
                        jsonObject = jsonObject.Where(p => !item["Value"].ToString().Contains(p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case "!=":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString() != (p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case "==":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString() == (p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case ">":
                        jsonObject = jsonObject.Where(p => decimal.Parse((p[item["FieldName"].ToString()].ToString())) > decimal.Parse(item["Value"].ToString())).ToList();
                        break;
                    case "<":
                        jsonObject = jsonObject.Where(p => decimal.Parse((p[item["FieldName"].ToString()].ToString())) < decimal.Parse(item["Value"].ToString())).ToList();
                        break;
                    default:
                        break;
                }
            }
            return jsonObject;
        }

        private async Task<List<CashFlowDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var ledgerData = await GetDataledger(dic);
            var incurredData = ledgerData.GroupBy(g => new
            {
                g.VoucherDate,
                g.DebitAcc,
                g.CreditAcc,
                g.SectionCode
            }).Select(p => new CashFlowDto()
            {
                VoucherDate = p.Key.VoucherDate,
                DebitAcc = p.Key.DebitAcc,
                CreditAcc = p.Key.CreditAcc,
                SectionCode0 = p.Key.SectionCode,
                Amount0 = p.Sum(p => p.Amount0),
                AmountCur0 = p.Sum(p => p.AmountCur0),
            }).ToList();
            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private Dictionary<string, object> GetLedgerParameter(StatementOfValueAddedTaxParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.Year, _webHelper.GetCurrentYear());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccBuy);
            return dic;
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
        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return _objectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
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
            return _objectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        #endregion
    }
}
