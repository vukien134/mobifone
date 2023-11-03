using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Reports.GeneralDiaries
{
    public class AccountAnalysisReportAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccountSystemService _accountSystemService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public AccountAnalysisReportAppService(ReportDataService reportDataService,
                            WebHelper webHelper,
                            ReportTemplateService reportTemplateService,
                            IWebHostEnvironment hostingEnvironment,
                            TenantSettingService tenantSettingService,
                            OrgUnitService orgUnitService,
                            CircularsService circularsService,
                            YearCategoryService yearCategoryService,
                            AccountSystemService accountSystemService,
                            AccountingCacheManager accountingCacheManager
                        )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.AccountAnalysisReportView)]
        public async Task<ReportResponseDto<JsonObject>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var data = await GetDataledger(dto.Parameters);
            var accounts = await GetAccounts();
            var groupData = GetGroupData(data);
            var reportResponse = new ReportResponseDto<JsonObject>();
            var result = new List<JsonObject>();
            decimal amount0 = 0;
            foreach(var item in groupData)
            {
                var obj = this.GetRowData(item, data,accounts,dto.Parameters.AccRank.Value);
                result.Add(obj);
                if (item.Amount0 != null)
                {
                    amount0 = amount0 + item.Amount0.Value;
                }
            }

            var objTotal = new JsonObject();
            objTotal.Add("bold", "C");
            objTotal.Add("description", "Tổng cộng");
            objTotal.Add("amount0", amount0);
            var lstTotal = GetTotalByAcc(data,accounts,dto.Parameters.AccRank.Value);
            foreach (var item in lstTotal)
            {
                objTotal.Add($"acc_{item.ReciprocalAcc}", item.Amount0);
            }
            result.Add(objTotal);

            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.AccountAnalysisReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var data = await GetDataledger(dto.Parameters);
            var accounts = await GetAccounts();
            var groupData = GetGroupDataForPrint(data,accounts,dto.Parameters.AccRank.Value);

            var reportResponse = new ReportResponseDto<LedgerGeneralDto>();
            reportResponse.Data = groupData;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());

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
                DataSource = reportResponse,
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
        private async Task<List<LedgerGeneralDto>> GetDataledger(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, dto.DebitCredit);
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
            dic.Add(LedgerParameterConst.ReciprocalAcc, dto.ReciprocalAcc);
            dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            dic.Add(LedgerParameterConst.PartnerCode, dto.PartnerCode);
            dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            dic.Add(LedgerParameterConst.CurrencyCode, dto.CurrencyCode);

            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private List<LedgerGeneralDto> GetGroupData(List<LedgerGeneralDto> ledgerData)
        {
            var groupData = ledgerData.GroupBy(p => new
            {
                p.VoucherId,
                p.VoucherDate,
                p.VoucherNumber,
                p.VoucherCode,
                p.VoucherGroup,
                p.Description,
                p.Acc
            }).Select(p => new LedgerGeneralDto()
            {
                VoucherId = p.Key.VoucherId,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                VoucherCode = p.Key.VoucherCode,
                VoucherGroup = p.Key.VoucherGroup,
                Description = p.Key.Description,
                Acc = p.Key.Acc,
                Amount0 = p.Sum(s => s.Amount0),
                AmountCur0 = p.Sum(s => s.AmountCur0)
            }).OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                .ToList();
            return groupData;
        }
        private List<LedgerGeneralDto> GetGroupDataForPrint(List<LedgerGeneralDto> ledgerData,List<AccountSystemDto> accounts,int rank)
        {
            var groupData = ledgerData.GroupBy(p => new
            {
                p.VoucherId,
                p.VoucherDate,
                p.VoucherNumber,
                p.VoucherCode,
                p.VoucherGroup,
                p.Description,
                p.Acc,
                p.ReciprocalAcc
            }).Select(p => new LedgerGeneralDto()
            {
                VoucherId = p.Key.VoucherId,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                VoucherCode = p.Key.VoucherCode,
                VoucherGroup = p.Key.VoucherGroup,
                Description = p.Key.Description,
                Acc = p.Key.Acc,
                ReciprocalAcc = GetAccountCode(accounts,p.Key.ReciprocalAcc,rank),
                Amount0 = p.Sum(s => s.Amount0),
                AmountCur0 = p.Sum(s => s.AmountCur0)
            }).ToList();            
            return groupData;
        }
        private JsonObject GetRowData(LedgerGeneralDto groupRow, List<LedgerGeneralDto> ledgerData,
                            List<AccountSystemDto> accounts,int rank)
        {
            var obj = new JsonObject();
            obj.Add("voucherId", groupRow.VoucherId);
            obj.Add("voucherDate", $"{groupRow.VoucherDate:dd/MM/yyyy}");
            obj.Add("voucherNumber", groupRow.VoucherNumber);
            obj.Add("voucherCode", groupRow.VoucherCode);
            obj.Add("voucherGroup", groupRow.VoucherGroup);
            obj.Add("description", groupRow.Description);
            obj.Add("acc", groupRow.Acc);
            obj.Add("amount0", groupRow.Amount0);
            obj.Add("amountCur0", groupRow.AmountCur0);

            var details = GetDetailData(ledgerData, groupRow.VoucherId, accounts, rank);
            foreach(var item in details)
            {
                obj.Add($"acc_{item.ReciprocalAcc}", item.Amount0);
            }
            return obj;
        }
        private List<LedgerGeneralDto> GetTotalByAcc(List<LedgerGeneralDto> ledgerData,
                        List<AccountSystemDto> accounts,int rank)
        {
            var details = ledgerData.GroupBy(p => new
            {
                p.ReciprocalAcc
            }).Select(p => new LedgerGeneralDto()
            {
                ReciprocalAcc = p.Key.ReciprocalAcc,
                Amount0 = p.Sum(s => s.Amount0),
                AmountCur0 = p.Sum(s => s.AmountCur0)
            }).ToList();

            foreach(var item in details)
            {
                item.ReciprocalAcc = GetAccountCode(accounts, item.ReciprocalAcc, rank);
            }

            var result = details.GroupBy(p => new
            {
                p.ReciprocalAcc
            }).Select(p => new LedgerGeneralDto()
            {
                ReciprocalAcc = p.Key.ReciprocalAcc,
                Amount0 = p.Sum(s => s.Amount0),
                AmountCur0 = p.Sum(s => s.AmountCur0)
            }).OrderBy(p => p.ReciprocalAcc).ToList();
            return result;
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
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private async Task<List<AccountSystemDto>> GetAccounts()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var accounts = await _accountSystemService.GetAccounts(orgCode, year);
            int maxRank = accounts.Max(p => p.AccRank);
            var result = new List<AccountSystemDto>();
            for(int i = 1; i <= maxRank; i++)
            {
                var lstAccount = accounts.Where(p => p.AccRank == i).ToList();
                var lstDto = lstAccount.Select(p => ObjectMapper.Map<AccountSystem, AccountSystemDto>(p))
                                    .ToList();
                foreach(var dto in lstDto)
                {
                    if (string.IsNullOrEmpty(dto.ParentAccId))
                    {
                        dto.SortPath = dto.AccCode;
                        continue;
                    };

                    var parentDto = result.Where(p => p.Id == dto.ParentAccId)
                                        .FirstOrDefault();
                    if (parentDto == null) continue;
                    dto.SortPath = parentDto.SortPath + "|" + dto.AccCode;
                }
                result.AddRange(lstDto);
            }
            return result.OrderBy(p => p.SortPath).ToList();
        }
        private string GetAccountCode(List<AccountSystemDto> accounts,string accCode,int rank)
        {
            var account = accounts.Where(p => p.AccCode.Equals(accCode)).FirstOrDefault();
            if (account == null) return null;
            if (account.AccRank <= rank) return account.AccCode;
            string[] parts = account.SortPath.Split("|");
            return parts[rank - 1];
        }
        private List<LedgerGeneralDto> GetDetailData(List<LedgerGeneralDto> ledgerData, string voucherId,
                                    List<AccountSystemDto> accounts,int rank)
        {
            var details = ledgerData.Where(p => p.VoucherId == voucherId).ToList();
            foreach(var item in details)
            {
                item.ReciprocalAcc = GetAccountCode(accounts, item.ReciprocalAcc, rank);
            }
            var result = details.GroupBy(p => new
                            {
                                p.Acc,
                                p.ReciprocalAcc
                            })
                            .Select(p => new LedgerGeneralDto()
                            {
                                Acc = p.Key.Acc,
                                ReciprocalAcc = p.Key.ReciprocalAcc,
                                Amount0 = p.Sum(s => s.Amount0),
                                AmountCur0 = p.Sum(s => s.AmountCur0)
                            })
                            .OrderBy(p => p.ReciprocalAcc).ToList();
            return result;
        }
        #endregion
    }
}
