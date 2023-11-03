using Accounting.Caching;
using Accounting.Categories.Partners;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.Partners;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Report.Constants;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.DebitBooks
{
    public class DebtBalanceSheetAppService : BaseReportAppService
    {
        #region Privates
        private readonly ReportDataService _reportDataService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly AccPartnerService _accPartnerService;
        #endregion
        #region Ctor
        public DebtBalanceSheetAppService(WebHelper webHelper,
                ReportTemplateService reportTemplateService,
                IWebHostEnvironment hostingEnvironment,
                YearCategoryService yearCategoryService,
                TenantSettingService tenantSettingService,
                OrgUnitService orgUnitService,
                CircularsService circularsService,
                IStringLocalizer<AccountingResource> localizer,
                ReportDataService reportDataService,
                AccountingCacheManager accountingCacheManager,
                AccPartnerService accPartnerService
            ) : base(webHelper, reportTemplateService, hostingEnvironment, yearCategoryService,
                    tenantSettingService, orgUnitService, circularsService, localizer)
        {
            _reportDataService = reportDataService;
            _accountingCacheManager = accountingCacheManager;
            _accPartnerService = accPartnerService;
        }
        #endregion
        #region Methods
        public async Task<ReportResponseDto<DebtBalanceSheetDto>> CreateDataAsync(ReportRequestDto<DebtBalanceSheetParameterDto> dto)
        {
            await this.CheckPermission(dto.ReportTemplateCode, ReportPermissions.ActionView);
            var treeParterGroup = await this.GetPartnerGroupTreeAsync(dto.Parameters);
            var accountingSystems = await this.GetAccountSystems(dto.Parameters);
            if (!string.IsNullOrEmpty(dto.Parameters.AccCode))
            {
                var attachPartner = accountingSystems.Where(p => p.AccCode == dto.Parameters.AccCode).FirstOrDefault().AttachPartner;
                if (attachPartner != "C")
                {
                    throw new Exception("Bạn phải chọn tài khoản bắt theo mã đối tượng!");
                }
            }
            var partners = await this.GetAccPartnersAsync(dto.Parameters, treeParterGroup);
            var balanceSheet = await this.GetBalaceSheet(dto.Parameters, accountingSystems, partners);

            List<DebtBalanceSheetDto> result = null;
            if (dto.Parameters.Sort == 2)
            {
                result = this.GetDebtSheetByAcc(balanceSheet);
            }
            else
            {
                result = this.GetTreeBalanceSheet(balanceSheet, treeParterGroup, partners);
            }

            if (dto.Parameters.Type == "DK")
            {
                result = result.Where(p => p.Debit1 > 0 || p.Credit1 > 0).ToList();
            }

            var reportResponse = await this.CreateReportResponseDto<DebtBalanceSheetDto, DebtBalanceSheetParameterDto>(result, dto);
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<DebtBalanceSheetParameterDto> dto)
        {
            string type = dto.Type.ToLower();
            string action = type switch
            {
                ReportTypeConst.Pdf => ReportPermissions.ActionPrint,
                ReportTypeConst.Xlsx => ReportPermissions.ActionExportExcel,
                _ => ""
            };
            await this.CheckPermission(dto.ReportTemplateCode, action);
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
        #region Privates
        private List<DebtBalanceSheetDto> GetTreeBalanceSheet(List<DebtBalanceSheetDto> balanceSheetAccs,
                                        List<PartnerGroupTreeItemDto> treeParterGroup, List<AccPartner> partners)
        {
            var total = balanceSheetAccs.GroupBy(s => s.OrgCode)
                            .Select(p => new DebtBalanceSheetDto()
                            {
                                Bold = "C",
                                PartnerName = "Tổng cộng",
                                Debit1 = p.Sum(s => s.Debit1),
                                Credit1 = p.Sum(s => s.Credit1),
                                Debit2 = p.Sum(s => s.Debit2),
                                Credit2 = p.Sum(s => s.Credit2),
                                DebitCur1 = p.Sum(s => s.DebitCur1),
                                DebitCur2 = p.Sum(s => s.DebitCur2),
                                CreditCur1 = p.Sum(s => s.CreditCur1),
                                CreditCur2 = p.Sum(s => s.CreditCur2),
                                DebitIncurred = p.Sum(s => s.DebitIncurred),
                                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                                CreditIncurred = p.Sum(s => s.CreditIncurred),
                                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
                            }).FirstOrDefault();

            var treeBalanceSheet = (from c in balanceSheetAccs
                                    join d in partners on c.PartnerCode equals d.Code
                                    join e in treeParterGroup on d.PartnerGroupId equals e.Id
                                    select new DebtBalanceSheetDto()
                                    {
                                        PartnerCode = c.PartnerCode,
                                        PartnerName = new string(' ', e.Rank.Value + 1) + d.Name,
                                        Debit1 = c.Debit1,
                                        Credit1 = c.Credit1,
                                        Debit2 = c.Debit2,
                                        Credit2 = c.Credit2,
                                        DebitCur1 = c.DebitCur1,
                                        DebitCur2 = c.DebitCur2,
                                        CreditCur1 = c.CreditCur1,
                                        CreditCur2 = c.CreditCur2,
                                        DebitIncurred = c.DebitIncurred,
                                        DebitIncurredCur = c.DebitIncurredCur,
                                        CreditIncurred = c.CreditIncurred,
                                        CreditIncurredCur = c.CreditIncurredCur,
                                        RankPartnerGroup = e.Rank.Value + 1,
                                        ParentId = d.PartnerGroupId,
                                        SortPath = e.SortPath + "|" + c.PartnerCode
                                    }).ToList();
            foreach (var item in treeParterGroup)
            {
                treeBalanceSheet.Add(new DebtBalanceSheetDto()
                {
                    Bold = "C",
                    PartnerCode = item.Code,
                    PartnerName = item.Rank > 1 ? new string(' ', item.Rank.Value) + item.Name : item.Name,
                    RankPartnerGroup = item.Rank.Value,
                    ParentId = item.ParentId,
                    SortPath = item.SortPath,
                    PartnerGroupId = item.Id
                });
            }

            int? maxRank = treeBalanceSheet.Max(p => p.RankPartnerGroup);
            while (maxRank > 1)
            {
                var groupData = treeBalanceSheet.Where(p => p.RankPartnerGroup == maxRank)
                                            .GroupBy(g => new
                                            {
                                                g.ParentId
                                            }).Select(p => new DebtBalanceSheetDto()
                                            {
                                                PartnerGroupId = p.Key.ParentId,
                                                Credit1 = p.Sum(s => s.Credit1),
                                                CreditCur1 = p.Sum(s => s.CreditCur1),
                                                Debit1 = p.Sum(s => s.Debit1),
                                                DebitCur1 = p.Sum(s => s.DebitCur1),
                                                CreditIncurred = p.Sum(s => s.CreditIncurred),
                                                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur),
                                                DebitIncurred = p.Sum(s => s.DebitIncurred),
                                                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                                                Credit2 = p.Sum(s => s.Credit2),
                                                CreditCur2 = p.Sum(s => s.CreditCur2),
                                                Debit2 = p.Sum(s => s.Debit2),
                                                DebitCur2 = p.Sum(s => s.DebitCur2)
                                            }).ToList();

                foreach (var item in groupData)
                {
                    var treeItem = treeBalanceSheet.Where(p => p.PartnerGroupId == item.PartnerGroupId)
                                        .FirstOrDefault();
                    if (treeItem == null) continue;

                    treeItem.Credit1 = item.Credit1 ?? null;
                    treeItem.CreditCur1 = item.CreditCur1 ?? null;
                    treeItem.Debit1 = item.Debit1 ?? null;
                    treeItem.DebitCur1 = item.DebitCur1 ?? null;
                    treeItem.CreditIncurred = item.CreditIncurred;
                    treeItem.CreditIncurredCur = item.CreditIncurredCur;
                    treeItem.DebitIncurred = item.DebitIncurred;
                    treeItem.DebitIncurredCur = item.DebitIncurredCur;
                    treeItem.Credit2 = item.Credit2 ?? null;
                    treeItem.CreditCur2 = item.CreditCur2 ?? null;
                    treeItem.Debit2 = item.Debit2 ?? null;
                    treeItem.DebitCur2 = item.DebitCur2 ?? null;
                }

                maxRank--;
            }

            treeBalanceSheet = treeBalanceSheet.Where(p => p.Debit2 != null || p.Credit2 != null
                                    || p.DebitCur2 != null || p.CreditCur2 != null)
                                .OrderBy(p => p.SortPath).ToList();
            if (total != null)
            {
                treeBalanceSheet.Add(total);
            }


            return treeBalanceSheet;
        }
        private List<DebtBalanceSheetDto> GetDebtSheetByAcc(List<DebtBalanceSheetDto> balanceSheetAccs)
        {
            var total = balanceSheetAccs.GroupBy(g => g.OrgCode)
                            .Select(p => new DebtBalanceSheetDto()
                            {
                                Bold = "C",
                                PartnerName = "Tổng cộng",
                                Debit1 = p.Sum(s => s.Debit1),
                                Credit1 = p.Sum(s => s.Credit1),
                                DebitCur1 = p.Sum(s => s.DebitCur1),
                                CreditCur1 = p.Sum(s => s.CreditCur1),
                                Debit2 = p.Sum(s => s.Debit2),
                                Credit2 = p.Sum(s => s.Credit2),
                                DebitCur2 = p.Sum(s => s.DebitCur2),
                                CreditCur2 = p.Sum(s => s.CreditCur2),
                                DebitIncurred = p.Sum(s => s.DebitIncurred),
                                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                                CreditIncurred = p.Sum(s => s.CreditIncurred),
                                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
                            }).FirstOrDefault();
            if (total != null)
            {
                balanceSheetAccs.Add(total);
            }
            return balanceSheetAccs;
        }
        private async Task<List<AccPartner>> GetAccPartnersAsync(DebtBalanceSheetParameterDto dto,
                                List<PartnerGroupTreeItemDto> treePartnerGroup)
        {
            if (!string.IsNullOrEmpty(dto.PartnerGroupCode))
            {
                //  string orgCode = _webHelper.GetCurrentOrgUnit();
                return await _accPartnerService.GetByParentIdAsync(dto.PartnerGroupCode);
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
            if (!string.IsNullOrEmpty(dto.PartnerGroupCode))
            {
                dic.Add(LedgerParameterConst.PartnerGroup, dto.PartnerGroupCode);
            }
            return dic;
        }
        private async Task<List<DebtBalanceSheetDto>> GetBalaceSheet(DebtBalanceSheetParameterDto dto,
                            List<AccountSystemDto> accountSystems, List<AccPartner> partners)
        {
            var balanceSheet = new List<BalanceSheetAccDto>();
            var openings = await this.GetOpeningBalance(dto, accountSystems);
            balanceSheet.AddRange(openings);
            if (dto.Type == "DK")
            {
                dto.ToDate = dto.ToDate.Value.AddDays(-1);
            }
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
        private List<BalanceSheetAccDto> ProcessBalanceValue(List<BalanceSheetAccDto> sheets)
        {
            Parallel.ForEach(sheets, item =>
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
            });

            return sheets;
        }
        #endregion
    }
}
