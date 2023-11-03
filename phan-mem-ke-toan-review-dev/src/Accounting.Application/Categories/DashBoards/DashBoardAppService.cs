using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.DashBoards;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Catgories.Partners;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Reports;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.ImportExports;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.DebitBooks
{
    public class DashBoardAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly LedgerService _ledgerService;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly PartnerGroupAppService _partnerGroupAppService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly BalanceSheetAccAppService _balanceSheetAccAppService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly ProductService _productService;
        #endregion
        #region Ctor
        public DashBoardAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        LedgerService ledgerService,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        AccPartnerAppService accPartnerAppService,
                        PartnerGroupAppService partnerGroupAppService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager,
                        BalanceSheetAccAppService balanceSheetAccAppService,
                        IStringLocalizer<AccountingResource> localizer,
                        ProductService productService
                        )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _ledgerService = ledgerService;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _accPartnerAppService = accPartnerAppService;
            _partnerGroupAppService = partnerGroupAppService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
            _balanceSheetAccAppService = balanceSheetAccAppService;
            _localizer = localizer;
            _productService = productService;
        }
        #endregion
        #region Methods
        public async Task<List<FinancialSituationDto>> GetFinancialSituationAsync()
        {
            var accSystem = (await _accountSystemService.GetAccountByRank(1, _webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear()))
                            .Select(p => ObjectMapper.Map<AccountSystem, AccountSystemDto>(p)).ToList();
            List<string> lstAcc = accSystem.Select(p => p.AccCode).ToList();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter();
            var reportBaseParameterDto = new ReportBaseParameterDto
            {
                AccCode = "",
                AccRank = 3,
                CleaningAcc = false,
                CleaningFProductWork = true,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };

            var openingBalances = await this.GetBalaceSheetFS(reportBaseParameterDto, accSystem);
            var res = new List<FinancialSituationDto>();
            foreach (var acc in lstAcc)
            {
                var infoAcc = accSystem.Where(p => p.AccCode == acc).FirstOrDefault();
                var debit = openingBalances.Where(p => (p.AccCode ?? "").StartsWith(acc)).Sum(p => p.Debit2 ?? 0);
                var credit = openingBalances.Where(p => (p.AccCode ?? "").StartsWith(acc)).Sum(p => p.Credit2 ?? 0);
                res.Add(new FinancialSituationDto
                {
                    Code = infoAcc?.AccCode ?? "",
                    Name = infoAcc?.AccName ?? "Không có tên tài khoản",
                    Debit = debit,
                    Credit = credit
                });
            }
            return res.OrderBy(p => p.Code).Where(p => p.Debit != 0 || p.Credit != 0).Take(100).ToList();
        }

        public async Task<List<TurnoverDto>> GetTurnoverAsync()
        {
            var accSystem = await _accountSystemService.GetAccountAllRank(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            List<string> lstAcc = new List<string>(new string[] { "511", "521", "711" });
            var ledger = await _ledgerService.GetQueryableAsync();
            var dataLedger = (from a in ledger
                              where a.Status == "1" && (a.CheckDuplicate == "" || a.CheckDuplicate == "N")
                                 && a.OrgCode == _webHelper.GetCurrentOrgUnit()
                                 && a.VoucherDate >= DateTime.Parse(_webHelper.GetCurrentYear() + "/01/01")
                                 && a.VoucherDate <= (_webHelper.GetCurrentYear() != DateTime.Now.Year ? DateTime.Parse(_webHelper.GetCurrentYear() + "/12/31") : DateTime.Now)
                              group new { a } by new
                              {
                                  Month = a.VoucherDate.Month,
                                  a.DebitAcc,
                                  a.CreditAcc
                              } into gr
                              select new LedgerTurnoverCostDto
                              {
                                  Month = gr.Key.Month,
                                  DebitAcc = gr.Key.DebitAcc,
                                  CreditAcc = gr.Key.CreditAcc,
                                  Amount0 = gr.Sum(p => p.a.Amount)
                              }).ToList();
            var dataTurnover = new List<TurnoverDto>();
            foreach (var acc in lstAcc)
            {
                dataTurnover.AddRange((from a in dataLedger
                                       where (a.CreditAcc ?? "").StartsWith(acc)
                                       select new TurnoverDto
                                       {
                                           Month = "T" + a.Month.ToString().PadLeft(2, '0'),
                                           Amount = a.Amount0,
                                       }).ToList());
            }
            int i = 1;
            dataTurnover = dataTurnover.GroupBy(p => new { Month = p.Month }).OrderBy(p => p.Key.Month)
                                       .Select(p =>
                                       {
                                           var id = i;
                                           i++;
                                           return new TurnoverDto
                                           {
                                               Id = id,
                                               Month = p.Key.Month,
                                               Amount = p.Sum(a => a.Amount ?? 0) / 1000000
                                           };
                                       }).ToList();
            return dataTurnover;
        }

        public async Task<List<CostDto>> GetCostAsync()
        {
            var accSystem = await _accountSystemService.GetAccountAllRank(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            List<string> lstAcc = new List<string>(new string[] { "632", "641", "642", "811", "821" });
            var ledger = await _ledgerService.GetQueryableAsync();
            var dataLedger = (from a in ledger
                              where a.Status == "1" && (a.CheckDuplicate == "" || a.CheckDuplicate == "N")
                                 && a.OrgCode == _webHelper.GetCurrentOrgUnit()
                                 && a.VoucherDate >= DateTime.Parse(_webHelper.GetCurrentYear() + "/01/01")
                                 && a.VoucherDate <= (_webHelper.GetCurrentYear() != DateTime.Now.Year ? DateTime.Parse(_webHelper.GetCurrentYear() + "/12/31") : DateTime.Now)
                              group new { a } by new
                              {
                                  Month = a.VoucherDate.Month,
                                  a.DebitAcc,
                                  a.CreditAcc
                              } into gr
                              select new LedgerTurnoverCostDto
                              {
                                  Month = gr.Key.Month,
                                  DebitAcc = gr.Key.DebitAcc,
                                  CreditAcc = gr.Key.CreditAcc,
                                  Amount0 = gr.Sum(p => p.a.Amount)
                              }).ToList();
            var dataTurnover = new List<CostDto>();
            foreach (var acc in lstAcc)
            {
                dataTurnover.AddRange((from a in dataLedger
                                       where (a.DebitAcc ?? "").StartsWith(acc)
                                       select new CostDto
                                       {
                                           Month = "T" + a.Month.ToString().PadLeft(2, '0'),
                                           Amount = a.Amount0,
                                       }).ToList());
            }
            List<string> lstColor = new List<string>(new string[] { "#FFFFFF", "#FFFF99", "#FFFF33", "#99FFFF", "#00FF33",
                                                                    "#FFCCFF", "#FFCCCC", "#FFCC33", "#66CCFF", "#66CC99",
                                                                    "#00CC99", "#FF9900"});
            var i = 0;
            dataTurnover = dataTurnover.GroupBy(p => new { Month = p.Month }).OrderBy(p => p.Key.Month)
                                       .Select(p =>
                                       {
                                           var color = lstColor[i];
                                           i++;
                                           return new CostDto
                                           {
                                               Color = color,
                                               Month = p.Key.Month,
                                               Amount = Math.Round(p.Sum(a => a.Amount ?? 0) / 1000000, 2)
                                           };
                                       }).ToList();
            return dataTurnover.Take(100).ToList();
        }

        public async Task<List<ResultReceivablePayableDto>> GetReceivableAsync()
        {
            var data = await ReceivablePayable("131");
            var receivable = (from a in data
                              where a.Receivable != 0
                              orderby a.Receivable descending
                              select new ResultReceivablePayableDto
                              {
                                  Code = a.Code,
                                  Name = a.Name,
                                  Value = a.Receivable,
                              }).ToList();
            return receivable;
        }

        public async Task<List<ResultReceivablePayableDto>> GetPayableAsync()
        {
            var data = await ReceivablePayable("331");
            var payable = (from a in data
                           where a.Payable != 0
                           orderby a.Payable descending
                           select new ResultReceivablePayableDto
                           {
                               Code = a.Code,
                               Name = a.Name,
                               Value = a.Payable,
                           }).ToList();
            return payable.Take(100).ToList();
        }

        public async Task<List<ReceivablePayableDto>> ReceivablePayable(string accCode)
        {
            var para = new DebtBalanceSheetParameterDto()
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Type = "*",
                AccCode = accCode,
                PartnerGroupCode = "",
                PartnerCode = "",
                FProductWorkCode = "",
                SectionCode = "",
                Sort = 1,
                CleaningFProductWork = true,
            };
            var treeParterGroup = await this.GetPartnerGroupTreeAsync(para);
            var accountingSystems = await this.GetAccountSystems(para);
            var partners = await this.GetAccPartnersAsync(para, treeParterGroup);
            var balanceSheet = await this.GetBalaceSheet(para, accountingSystems, partners);
            // group theo đối tượng
            var receivablePayable = (from a in balanceSheet
                                     group new { a } by new
                                     {
                                         a.PartnerCode,
                                         a.PartnerName
                                     } into gr
                                     select new ReceivablePayableDto
                                     {
                                         Code = gr.Key.PartnerCode,
                                         Name = gr.Key.PartnerName,
                                         Receivable = gr.Sum(p => p.a.Debit2 ?? 0),
                                         Payable = gr.Sum(p => p.a.Credit2 ?? 0),
                                     }).ToList();
            return receivablePayable;
        }

        public async Task<List<ProductAlmostOutDto>> GetProductAlmostOut()
        {

            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetWarehouseBookParameter();
            var lst = new List<InventorySummaryBookDto>();
            var iEInventoryData = await GetIEInventoryData(dic);
            var inventorySummaryBookData0 = (from a in iEInventoryData
                                             join b in lstProduct on a.ProductCode equals b.Code into ajb
                                             from b in ajb.DefaultIfEmpty()
                                             group new { a, b } by new
                                             {
                                                 a.OrgCode,
                                                 a.AccCode,
                                                 a.ProductCode,
                                                 ProductName = b?.Name ?? "",
                                                 UnitCode = b?.UnitCode ?? "",
                                             } into gr
                                             select new InventorySummaryBookDto
                                             {
                                                 OrgCode = gr.Key.OrgCode,
                                                 Acc = gr.Key.AccCode,
                                                 ProductCode = gr.Key.ProductCode,
                                                 ProductName = gr.Key.ProductName,
                                                 UnitCode = gr.Key.UnitCode,
                                                 Bold = "K",
                                                 ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1),
                                                 ImportAmount1 = gr.Sum(p => p.a.ImportAmount1),
                                                 ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1),
                                                 ImportPrice1 = (gr.Sum(p => p.a.ImportQuantity1) != 0) ? gr.Sum(p => p.a.ImportAmount1) / gr.Sum(p => p.a.ImportQuantity1) : null,
                                                 ImportQuantity = gr.Sum(p => p.a.ImportQuantity),
                                                 ImportAmount = gr.Sum(p => p.a.ImportAmount),
                                                 ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur),
                                                 ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                                 ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                                 ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur),
                                                 ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2),
                                                 ImportAmount2 = gr.Sum(p => p.a.ImportAmount2),
                                                 ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2),
                                                 ImportPrice2 = (gr.Sum(p => p.a.ImportQuantity2) != 0) ? gr.Sum(p => p.a.ImportAmount2) / gr.Sum(p => p.a.ImportQuantity2) : null,
                                                 Amount2 = gr.Sum(p => p.a.Amount2),
                                                 AmountCur2 = gr.Sum(p => p.a.AmountCur2),
                                             }).ToList();
            var inventorySummaryBookData = (from a in inventorySummaryBookData0
                                            group new { a } by new
                                            {
                                                a.OrgCode,
                                                a.ProductCode,
                                                a.ProductName,

                                            } into gr
                                            select new InventorySummaryBookDto
                                            {
                                                OrgCode = gr.Key.OrgCode,
                                                ProductCode = gr.Key.ProductCode,
                                                ProductOriginCode = "",
                                                ProductLotCode = "",
                                                ProductName = gr.Key.ProductName,
                                                Bold = "K",
                                                ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1),
                                                ImportAmount1 = gr.Sum(p => p.a.ImportAmount1),
                                                ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1),
                                                ImportPrice1 = gr.Max(p => p.a.ImportPrice1),
                                                ImportQuantity = gr.Sum(p => p.a.ImportQuantity),
                                                ImportAmount = gr.Sum(p => p.a.ImportAmount),
                                                ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur),
                                                ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                                ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                                ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur),
                                                ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2),
                                                ImportAmount2 = gr.Sum(p => p.a.ImportAmount2),
                                                ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2),
                                                ImportPrice2 = gr.Max(p => p.a.ImportPrice2),
                                                Amount2 = gr.Sum(p => p.a.Amount2),
                                                AmountCur2 = gr.Sum(p => p.a.AmountCur2),
                                            }).ToList();

            //data_NXT
            var dataIEI = (from a in inventorySummaryBookData0
                           join b in lstProduct on a.ProductCode equals b.Code
                           group new { a, b } by new
                           {
                               a.OrgCode,
                               a.ProductCode,
                               a.ProductName,
                           } into gr
                           select new ProductAlmostOutDto
                           {
                               Code = gr.Key.ProductCode,
                               Name = gr.Key.ProductName,
                               Quantity = gr.Sum(p => p.a.ImportQuantity2),
                               Value = gr.Sum(p => p.a.ImportAmount2)
                           }).ToList();
            return dataIEI.Where(p => p.Quantity > 0).OrderBy(p => p.Quantity).Take(100).ToList();
        }
        #endregion
        #region Private
        private async Task<List<DebtBalanceSheetDto>> GetIncurredData(Dictionary<string, object> dic)
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
            }).Select(p => new DebtBalanceSheetDto()
            {
                OrgCode = p.Key.OrgCode,
                Year = p.Key.Year,
                VoucherCode = p.Key.VoucherCode,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                VoucherId = p.Key.VoucherId,
                Note = p.Key.Note,
                PartnerCode = p.Key.PartnerCode,
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
                DebitIncurredCur = p.Sum(g => g.DebitIncurredCur ?? 0),
                DebitIncurred = p.Sum(g => g.DebitIncurred ?? 0),
                CreditIncurredCur = p.Sum(g => g.CreditIncurredCur ?? 0),
                CreditIncurred = p.Sum(g => g.CreditIncurred ?? 0)
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
        private Dictionary<string, object> GetLedgerParameter()
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, DateTime.Now.ToString("yyyy/MM/dd"));
            dic.Add(LedgerParameterConst.ToDate, DateTime.Now.ToString("yyyy/MM/dd"));
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
            return openingBalances.Where(p => (p.AccCode ?? "") != "").ToList();
        }

        private Dictionary<string, object> GetWarehouseBookParameter()
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(WarehouseBookParameterConst.FromDate, DateTime.Now.ToString(_webHelper.GetCurrentYear() + "/01/01"));
            dic.Add(WarehouseBookParameterConst.ToDate, (_webHelper.GetCurrentYear() != DateTime.Now.Year ? _webHelper.GetCurrentYear() + "/12/31" : DateTime.Now.ToString("yyyy/MM/dd")));
            dic.Add(WarehouseBookParameterConst.Year, _webHelper.GetCurrentYear());
            return dic;
        }

        private async Task<List<IEInventoryDto>> GetIEInventoryData(Dictionary<string, object> dic)
        {
            var incurredData = await _reportDataService.GetIEInventoryAsync(dic);
            return incurredData;
        }

        private async Task<List<AccPartner>> GetAccPartnersAsync(DebtBalanceSheetParameterDto dto,
                                List<PartnerGroupTreeItemDto> treePartnerGroup)
        {
            if (string.IsNullOrEmpty(dto.PartnerGroupId))
            {
                string orgCode = _webHelper.GetCurrentOrgUnit();
                return await _accPartnerService.GetAccPartnerAsync(orgCode);
            }

            var result = new List<AccPartner>();

            string[] partnerGroupIds = treePartnerGroup.Where(p => p.HasChild == false)
                                        .Select(p => p.Id).ToArray();
            if (partnerGroupIds.Length == 0) return result;

            foreach (var id in partnerGroupIds)
            {
                var partners = await _accPartnerService.GetByParentIdAsync(id);
                result.AddRange(partners);
            }
            return result;
        }
        private async Task<List<AccountSystemDto>> GetAccountSystems(DebtBalanceSheetParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, dto.FromDate.Value);
            return await _accountingCacheManager.GetAccountSystemsAsync(yearCategory.Year);
        }
        private async Task<List<PartnerGroupTreeItemDto>> GetPartnerGroupTreeAsync(DebtBalanceSheetParameterDto dto)
        {
            var partnerGroups = await _accountingCacheManager.GetPartnerGroupAsync();
            var tree = new List<PartnerGroupTreeItemDto>();
            BuildTreeView(partnerGroups, "", tree, null, null);
            if (!string.IsNullOrEmpty(dto.PartnerGroupId))
            {
                var partnerGroup = partnerGroups.Where(p => p.Id == dto.PartnerGroupId)
                                            .FirstOrDefault();
                if (partnerGroup == null)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.PartnerGroup, ErrorCode.NotFoundEntity),
                        _localizer["Err:PartnerGroupNotFound"]);
                }
                string code = "|" + partnerGroup.Code;
                tree = tree.Where(p => p.SortPath.Contains(code))
                            .ToList();
            }
            return tree;
        }
        private int BuildTreeView(List<PartnerGroupDto> partnerGroups, string parentId,
                            List<PartnerGroupTreeItemDto> tree, int? rank, string sortPath)
        {
            var groups = partnerGroups.Where(p => p.ParentId == parentId)
                                .OrderBy(p => p.Code).ToList();
            if (groups.Count == 0) return 0;

            int count = 0;
            foreach (var item in groups)
            {
                var child = new PartnerGroupTreeItemDto()
                {
                    Id = item.Id,
                    ParentId = item.ParentId,
                    Value = item.Code,
                    Code = item.Code,
                    Name = item.Name,
                    Rank = rank == null ? 1 : rank + 1,
                    SortPath = string.IsNullOrEmpty(sortPath) ? "|" + item.Code
                                : $"{sortPath}|{item.Code}",
                    HasChild = false
                };
                int treeCount = BuildTreeView(partnerGroups, item.Id, tree, child.Rank, child.SortPath);
                if (treeCount > 0)
                {
                    child.HasChild = true;
                }
                tree.Add(child);
                count++;
            }
            return count;
        }
        private Dictionary<string, object> GetLedgerParameter(DebtBalanceSheetParameterDto dto)
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
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            return dic;
        }
        private async Task<List<DebtBalanceSheetDto>> GetBalaceSheet(DebtBalanceSheetParameterDto dto,
                            List<AccountSystemDto> accountSystems, List<AccPartner> partners)
        {
            var balanceSheet = new List<BalanceSheetAccDto>();
            var openings = await this.GetOpeningBalance(dto, accountSystems);
            balanceSheet.AddRange(openings);
            var incurreds = await this.GetIncurredData(dto, accountSystems);
            balanceSheet.AddRange(incurreds);

            var groupData = balanceSheet.GroupBy(g => new
            {
                g.AccCode,
                g.ContractCode,
                g.CurrencyCode,
                g.FProductWorkCode,
                g.PartnerCode,
                g.SectionCode,
                g.WorkPlaceCode
            }).Select(p => new BalanceSheetAccDto()
            {
                AccCode = p.Key.AccCode,
                ContractCode = p.Key.ContractCode,
                CurrencyCode = p.Key.CurrencyCode,
                FProductWorkCode = p.Key.FProductWorkCode,
                PartnerCode = p.Key.PartnerCode,
                SectionCode = p.Key.SectionCode,
                WorkPlaceCode = p.Key.WorkPlaceCode,
                Credit1 = p.Sum(s => s.Credit1),
                CreditCur1 = p.Sum(s => s.CreditCur1),
                Debit1 = p.Sum(s => s.Debit1),
                DebitCur1 = p.Sum(s => s.DebitCur1),
                Credit2 = p.Sum(s => s.Credit2),
                CreditCur2 = p.Sum(s => s.CreditCur2),
                Debit2 = p.Sum(s => s.Debit2),
                DebitCur2 = p.Sum(s => s.DebitCur2),
                DebitIncurred = p.Sum(s => s.DebitIncurred),
                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                CreditIncurred = p.Sum(s => s.CreditIncurred),
                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
            }).ToList();
            groupData = this.ProcessBalanceValue(groupData);

            var query = from c in groupData
                        join d in partners on c.PartnerCode equals d.Code
                        orderby c.PartnerCode
                        select new DebtBalanceSheetDto()
                        {
                            PartnerCode = c.PartnerCode,
                            PartnerName = d.Name,
                            Debit1 = c.Debit1,
                            DebitCur1 = c.DebitCur1,
                            Credit1 = c.Credit1,
                            CreditCur1 = c.CreditCur1,
                            Debit2 = c.Debit2,
                            DebitCur2 = c.DebitCur2,
                            Credit2 = c.Credit2,
                            CreditCur2 = c.CreditCur2,
                            DebitIncurred = c.DebitIncurred,
                            DebitIncurredCur = c.DebitIncurredCur,
                            CreditIncurred = c.CreditIncurred,
                            CreditIncurredCur = c.CreditIncurredCur
                        };
            return query.ToList();
        }
        private async Task<List<BalanceSheetAccDto>> GetOpeningBalance(DebtBalanceSheetParameterDto dto,
                                List<AccountSystemDto> accountSystems)
        {
            var dic = this.GetLedgerParameter(dto);

            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime fromDate = Convert.ToDateTime(dic[LedgerParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, yearCategory.Year);
            }

            dic[LedgerParameterConst.Year] = yearCategory.Year;
            dic[LedgerParameterConst.FromDate] = yearCategory.BeginDate;
            dic[LedgerParameterConst.ToDate] = fromDate.AddDays(-1);

            var dtos = await _reportDataService.GetAccBalancesAsync(dic);
            var query = from p in dtos
                        join d in accountSystems on p.AccCode equals d.AccCode into gj
                        from df in gj.DefaultIfEmpty()
                        select new BalanceSheetAccDto()
                        {
                            AccCode = dto.AccCode,
                            ContractCode = !this.IsAttachContract(df) ? null : this.DefaultNullIfEmpty(p.ContractCode),
                            CurrencyCode = !this.IsAttachCurrency(df) ? null : this.DefaultNullIfEmpty(p.CurrencyCode),
                            FProductWorkCode = dto.CleaningFProductWork == true || !this.IsAttachFProductWork(df) ? null : this.DefaultNullIfEmpty(p.FProductCode),
                            PartnerCode = !this.IsAttachPartner(df) ? null : this.DefaultNullIfEmpty(p.PartnerCode),
                            SectionCode = !this.IsAttachSection(df) ? null : this.DefaultNullIfEmpty(p.SectionCode),
                            WorkPlaceCode = !this.IsAttachWorkPlace(df) ? null : this.DefaultNullIfEmpty(p.WorkPlaceCode),
                            Credit1 = p.Credit,
                            CreditCur1 = p.CreditCur,
                            Debit1 = p.Debit,
                            DebitCur1 = p.DebitCur,
                            Credit2 = p.Credit,
                            CreditCur2 = p.CreditCur,
                            Debit2 = p.Debit,
                            DebitCur2 = p.DebitCur
                        };

            return query.ToList();
        }
        private async Task<List<BalanceSheetAccDto>> GetIncurredData(DebtBalanceSheetParameterDto dto,
                    List<AccountSystemDto> accountSystems)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = this.GetLedgerParameter(dto);
            DateTime fromDate = Convert.ToDateTime(dic[LedgerParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            var dtos = await _reportDataService.GetAccBalancesAsync(dic);
            var query = from p in dtos
                        join d in accountSystems on p.AccCode equals d.AccCode into gj
                        from df in gj.DefaultIfEmpty()
                        select new BalanceSheetAccDto()
                        {
                            AccCode = dto.AccCode,
                            ContractCode = !this.IsAttachContract(df) ? null : this.DefaultNullIfEmpty(p.ContractCode),
                            CurrencyCode = !this.IsAttachCurrency(df) ? null : this.DefaultNullIfEmpty(p.CurrencyCode),
                            FProductWorkCode = dto.CleaningFProductWork == true || !this.IsAttachFProductWork(df) ? null : this.DefaultNullIfEmpty(p.FProductCode),
                            PartnerCode = !this.IsAttachPartner(df) ? null : this.DefaultNullIfEmpty(p.PartnerCode),
                            SectionCode = !this.IsAttachSection(df) ? null : this.DefaultNullIfEmpty(p.SectionCode),
                            WorkPlaceCode = !this.IsAttachWorkPlace(df) ? null : this.DefaultNullIfEmpty(p.WorkPlaceCode),
                            CreditIncurred = p.CreditIncurred,
                            CreditIncurredCur = p.CreditIncurredCur,
                            DebitIncurred = p.DebitIncurred,
                            DebitIncurredCur = p.DebitIncurredCur,
                            Credit2 = p.CreditIncurred,
                            CreditCur2 = p.CreditIncurredCur,
                            Debit2 = p.DebitIncurred,
                            DebitCur2 = p.DebitIncurredCur
                        };

            return query.ToList();
        }
        private bool IsAttachContract(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachContract == "C" ? true : false;
        }
        private bool IsAttachFProductWork(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachProductCost == "C" ? true : false;
        }
        private bool IsAttachSection(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachAccSection == "C" ? true : false;
        }
        private bool IsAttachPartner(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachPartner == "C" ? true : false;
        }
        private bool IsAttachCurrency(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachCurrency == "C" ? true : false;
        }
        private bool IsAttachWorkPlace(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachWorkPlace == "C" ? true : false;
        }
        private string DefaultNullIfEmpty(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return value;
        }

        private async Task<List<BalanceSheetAccDto>> GetBalaceSheetFS(ReportBaseParameterDto dto,
                            List<AccountSystemDto> accountSystems)
        {
            var balanceSheet = new List<BalanceSheetAccDto>();

            var openings = await this.GetOpeningBalanceFS(dto, accountSystems);
            balanceSheet.AddRange(openings);

            var groupData = balanceSheet.GroupBy(g => new
            {
                g.AccCode,
                g.ContractCode,
                g.CurrencyCode,
                g.FProductWorkCode,
                g.PartnerCode,
                g.SectionCode,
                g.WorkPlaceCode
            }).Select(p => new BalanceSheetAccDto()
            {
                AccCode = p.Key.AccCode,
                ContractCode = p.Key.ContractCode,
                CurrencyCode = p.Key.CurrencyCode,
                FProductWorkCode = p.Key.FProductWorkCode,
                PartnerCode = p.Key.PartnerCode,
                SectionCode = p.Key.SectionCode,
                WorkPlaceCode = p.Key.WorkPlaceCode,
                Credit1 = p.Sum(s => s.Credit1),
                CreditCur1 = p.Sum(s => s.CreditCur1),
                Debit1 = p.Sum(s => s.Debit1),
                DebitCur1 = p.Sum(s => s.DebitCur1),
                Credit2 = p.Sum(s => s.Credit2),
                CreditCur2 = p.Sum(s => s.CreditCur2),
                Debit2 = p.Sum(s => s.Debit2),
                DebitCur2 = p.Sum(s => s.DebitCur2),
                DebitIncurred = p.Sum(s => s.DebitIncurred),
                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                CreditIncurred = p.Sum(s => s.CreditIncurred),
                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
            }).ToList();
            groupData = this.ProcessBalanceValue(groupData);

            return groupData.GroupBy(g => new
            {
                g.AccCode

            }).Select(p => new BalanceSheetAccDto()
            {
                AccCode = p.Key.AccCode,
                Credit1 = p.Sum(s => s.Credit1),
                CreditCur1 = p.Sum(s => s.CreditCur1),
                Debit1 = p.Sum(s => s.Debit1),
                DebitCur1 = p.Sum(s => s.DebitCur1),
                Credit2 = p.Sum(s => s.Credit2),
                CreditCur2 = p.Sum(s => s.CreditCur2),
                Debit2 = p.Sum(s => s.Debit2),
                DebitCur2 = p.Sum(s => s.DebitCur2),
                DebitIncurred = p.Sum(s => s.DebitIncurred),
                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                CreditIncurred = p.Sum(s => s.CreditIncurred),
                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
            }).ToList();

        }
        private async Task<List<BalanceSheetAccDto>> GetOpeningBalanceFS(ReportBaseParameterDto dto,
                                List<AccountSystemDto> accountSystems)
        {
            var dic = this.GetLedgerParameter();

            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime fromDate = Convert.ToDateTime(dic[LedgerParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, yearCategory.Year);
            }

            dic[LedgerParameterConst.Year] = yearCategory.Year;
            dic[LedgerParameterConst.FromDate] = yearCategory.BeginDate;
            dic[LedgerParameterConst.ToDate] = fromDate.AddDays(-1);

            var dtos = await _reportDataService.GetAccBalancesAsync(dic);
            var query = from p in dtos
                        join d in accountSystems on p.AccCode equals d.AccCode into gj
                        from df in gj.DefaultIfEmpty()
                        select new BalanceSheetAccDto()
                        {
                            AccCode = p.AccCode,
                            ContractCode = dto.CleaningAcc == true || !this.IsAttachContract(df) ? null : this.DefaultNullIfEmpty(p.ContractCode),
                            CurrencyCode = dto.CleaningAcc == true || this.IsAttachCurrency(df) == false ?
                                                null : this.DefaultNullIfEmpty(p.CurrencyCode),
                            FProductWorkCode = dto.CleaningAcc == true || dto.CleaningFProductWork == true
                                               || !this.IsAttachFProductWork(df) ? null : this.DefaultNullIfEmpty(p.FProductCode),
                            PartnerCode = dto.CleaningAcc == true || !this.IsAttachPartner(df) ? null : this.DefaultNullIfEmpty(p.PartnerCode),
                            SectionCode = dto.CleaningAcc == true || !this.IsAttachSection(df) ? null : this.DefaultNullIfEmpty(p.SectionCode),

                            WorkPlaceCode = dto.CleaningAcc == true || !this.IsAttachWorkPlace(df) ? null : this.DefaultNullIfEmpty(p.WorkPlaceCode),
                            Credit1 = p.Credit,
                            CreditCur1 = p.CreditCur,
                            Debit1 = p.Debit,
                            DebitCur1 = p.DebitCur,
                            Credit2 = p.Credit,
                            CreditCur2 = p.CreditCur,
                            Debit2 = p.Debit,
                            DebitCur2 = p.DebitCur
                        };

            return query.ToList();
        }
        private List<BalanceSheetAccDto> ProcessBalanceValue(List<BalanceSheetAccDto> sheets)
        {
            foreach (var item in sheets)
            {
                decimal debit1 = item.Debit1 == null ? 0 : item.Debit1.Value;
                decimal credit1 = item.Credit1 == null ? 0 : item.Credit1.Value;
                decimal debit2 = item.Debit2 == null ? 0 : item.Debit2.Value;
                decimal credit2 = item.Credit2 == null ? 0 : item.Credit2.Value;
                decimal debitCur1 = item.DebitCur1 == null ? 0 : item.DebitCur1.Value;
                decimal creditCur1 = item.CreditCur1 == null ? 0 : item.CreditCur1.Value;
                decimal debitCur2 = item.DebitCur2 == null ? 0 : item.DebitCur2.Value;
                decimal creditCur2 = item.CreditCur2 == null ? 0 : item.CreditCur2.Value;

                item.Debit1 = debit1 >= credit1 ? debit1 - credit1 : null;
                item.Credit1 = credit1 > debit1 ? credit1 - debit1 : null;
                item.DebitCur1 = debitCur1 >= creditCur1 ? debitCur1 - creditCur1 : null;
                item.CreditCur1 = creditCur1 > debitCur1 ? creditCur1 - debitCur1 : null;
                item.Debit2 = debit2 >= credit2 ? debit2 - credit2 : null;
                item.Credit2 = credit2 > debit2 ? credit2 - debit2 : null;
                item.DebitCur2 = debitCur2 >= creditCur2 ? debitCur2 - creditCur2 : null;
                item.CreditCur2 = creditCur2 > debitCur2 ? creditCur2 - debitCur2 : null;
            }
            return sheets;
        }
        #endregion
    }
}
