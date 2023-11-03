﻿using Accounting.Caching;
using Accounting.Categories.CostProductions;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.CostReports.ProductionCostBooks;
using Accounting.Reports.Others;
using Accounting.Vouchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Costs
{
    public class ProductionCostBookReportAppService : AccountingAppService
    {
        #region Fields
        private readonly LedgerService _ledgerService;
        private readonly InfoZService _infoZService;
        private readonly SoTHZService _soTHZService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ProductionCostBookReportAppService(LedgerService ledgerService,
                    SoTHZService soTHZService,
                    WebHelper webHelper,
                    YearCategoryService yearCategoryService,
                    InfoZService infoZService,
                    CircularsService circularsService,
                    OrgUnitService orgUnitService,
                    TenantSettingService tenantSettingService,
                    IWebHostEnvironment hostingEnvironment,
                    ReportTemplateService reportTemplateService,
                    FProductWorkService fProductWorkService,
                    AccountingCacheManager accountingCacheManager
            )
        {
            _soTHZService = soTHZService;
            _webHelper = webHelper;
            _yearCategoryService = yearCategoryService;
            _infoZService = infoZService;
            _circularsService = circularsService;
            _orgUnitService = orgUnitService;
            _tenantSettingService = tenantSettingService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _fProductWorkService = fProductWorkService;
            _ledgerService = ledgerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.ProductionCostBookReportView)]
        public async Task<ReportResponseDto<FProductWorkCostDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var detailData = await this.GetCostDetailData(dto.Parameters);
            var groupData = detailData.GroupBy(g => new
            {
                g.FProductWorkCode,
                g.OrgCode
            })
            .Select(p => new FProductWorkCostDto()
            {
                Bold = "C",
                Note = "Tổng cộng",
                FProductWorkCode = p.Key.FProductWorkCode,
                OrgCode = p.Key.OrgCode,
                Quantity = p.Sum(s => s.Quantity),
                Ps621 = p.Sum(s => s.Ps621),
                Ps622 = p.Sum(s => s.Ps622),
                Ps623 = p.Sum(s => s.Ps623),
                Ps627 = p.Sum(s => s.Ps627),
                TotalZ = p.Sum(s => s.TotalZ),
                Ps511 = p.Sum(s => s.Ps511),
                Opening154 = p.Sum(s => s.Opening154),
                Ending154 = p.Sum(s => s.Ending154)
            }).OrderBy(p => p.FProductWorkCode).ToList();

            var result = new List<FProductWorkCostDto>();
            foreach(var group in groupData)
            {
                result.Add(new FProductWorkCostDto()
                {
                    Bold = "C",
                    FProductWorkCode = group.FProductWorkCode,
                    Note = await this.GetFProductWorkName(group.FProductWorkCode, group.OrgCode),
                    Balance = group.Opening154
                });
                var lst = detailData.Where(p => p.FProductWorkCode == group.FProductWorkCode)
                                .OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber).ToList();
                result.AddRange(lst);
                group.TotalZ = group.Opening154.GetValueOrDefault(0) + group.TotalExpenseAmount.GetValueOrDefault(0)
                                            - group.Ending154.GetValueOrDefault(0);
                group.Balance = group.Ending154;
                result.Add(group);
            }
            var reportResponse = new ReportResponseDto<FProductWorkCostDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.ProductionCostBookReportPrint)]
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
        #region Privates
        private async Task<List<FProductWorkCostDto>> GetCostDetailData(ReportBaseParameterDto dto)
        {
            var soTHZs = await this.GetSoTHZsAsync(dto);
            var infoZs = await this.GetInfoZsAsync(dto);
            var filterSoTHZ = soTHZs.Where(p => p.FieldName.StartsWith("PS_621") || p.FieldName.StartsWith("PS_622")
                                            || p.FieldName.StartsWith("PS_623") || p.FieldName.StartsWith("PS_627")
                                            || p.FieldName.StartsWith("DK_154") || p.FieldName.StartsWith("CK_154")
                                            || p.FieldName.StartsWith("TONG_Z") || p.FieldName.StartsWith("PS_TONG_CF")
                                            || p.FieldName.StartsWith("PS_511")
                                            )
                                .OrderBy(p => p.Ord).ToList();
            foreach (var item in infoZs)
            {
                item.BeginQuantity = item.BeginM == dto.FromDate ? item.BeginQuantity : 0;
                item.BeginAmount = item.BeginM == dto.FromDate ? item.BeginAmount : 0;
                item.EndQuantity = item.EndM == dto.ToDate ? item.EndQuantity : 0;
                item.EndAmount = item.EndM == dto.ToDate ? item.EndAmount : 0;
            }

            var result = new List<FProductWorkCostDto>();
            var sumSoThz = new List<SumSoThz>();

            foreach (var item in filterSoTHZ)
            {
                if ((item.TGet ?? "").Equals("SOCAI"))
                {
                    var ledgers = await this.GetDataLedger(item, dto);
                    foreach(var p in ledgers)
                    {
                        result.Add(new FProductWorkCostDto()
                        {
                            VoucherGroup = p.VoucherGroup,
                            VoucherCode = p.VoucherCode,
                            OrgCode = p.OrgCode,
                            VoucherId = p.VoucherId,
                            VoucherDate = p.VoucherDate,
                            VoucherNumber = p.VoucherNumber,
                            Note = p.Note,
                            FProductWorkCode = p.FProductWorkCode,
                            DebitAcc = p.DebitAcc,
                            CreditAcc = p.CreditAcc,                            
                            ProductCode = p.ProductCode,
                            ProductName = p.ProductName0,
                            UnitCode = p.UnitCode,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            Ps621 = item.FieldName.Equals("PS_621") ? p.Amount : null,
                            Ps622 = item.FieldName.Equals("PS_622") ? p.Amount : null,
                            Ps623 = item.FieldName.Equals("PS_623") ? p.Amount : null,
                            Ps627 = item.FieldName.Equals("PS_627") ? p.Amount : null,
                            TotalZ = item.FieldName.Equals("TONG_Z") ? p.Amount : null,
                            Ps511 = item.FieldName.Equals("PS_511") ? p.Amount : null,
                            CreditFProductWorkCode = p.CreditFProductWorkCode,
                            DebitFProductWorkCode = p.DebitFProductWorkCode
                        });
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(item.TSum))
                    {
                        var infoZ = this.GetInfoZ(item, infoZs.AsQueryable());
                        foreach (var p in infoZ)
                        {
                            result.Add(new FProductWorkCostDto()
                            {
                                VoucherGroup = null,
                                VoucherCode = null,
                                OrgCode = p.OrgCode,
                                VoucherId = null,
                                VoucherDate = p.EndM,
                                VoucherNumber = p.AllotmentForwardCode,
                                Note = null,
                                FProductWorkCode = p.FProductWorkCode,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                ProductCode = null,
                                ProductName = null,
                                UnitCode = null,
                                Price = null,
                                Quantity = null,
                                Ps621 = item.FieldName.Equals("PS_621") ? p.Amount : null,
                                Ps622 = item.FieldName.Equals("PS_622") ? p.Amount : null,
                                Ps623 = item.FieldName.Equals("PS_623") ? p.Amount : null,
                                Ps627 = item.FieldName.Equals("PS_627") ? p.Amount : null,
                                TotalZ = item.FieldName.Equals("TONG_Z") ? p.Amount : null,
                                Ps511 = item.FieldName.Equals("PS_511") ? p.Amount : null,
                                Opening154 = item.FieldName.Equals("DK_154") ? p.BeginAmount : null,
                                Ending154 = item.FieldName.Equals("CK_154") ? p.EndAmount : null,
                                CreditFProductWorkCode = p.CreditFProductWorkCode,
                                DebitFProductWorkCode = p.DebitFProductWorkCode
                            });
                            sumSoThz.Add(new SumSoThz()
                            {
                                OrgCode = p.OrgCode,
                                FProductWorkCode = p.FProductWorkCode,
                                FieldName = item.FieldName,
                                Amount = item.FieldName.Equals("DK_154") ? p.BeginAmount :
                                            item.FieldName.Equals("CK_154") ? p.EndAmount :
                                            p.Amount
                            });
                        }
                    }
                    else
                    {
                        string tSum = item.TSum.Replace("+", ",+").Replace("-", ",-");
                        string[] parts = tSum.Split(',');

                        var lstTotal = new List<SumSoThz>();
                        foreach (string part in parts)
                        {
                            decimal sign = 1;
                            if (part.StartsWith("-")) sign = -1;
                            var fieldName = part.StartsWith("+") || part.StartsWith("-") ? part.Substring(1) : part;
                            var lstSoThz = sumSoThz.Where(p => p.FieldName == fieldName)
                                                    .Select(p => new SumSoThz()
                                                    {
                                                        OrgCode = item.OrgCode,
                                                        FProductWorkCode = p.FProductWorkCode,
                                                        Amount = p.Amount * sign
                                                    })
                                                    .ToList();
                            lstTotal.AddRange(lstSoThz);
                        }

                        lstTotal = lstTotal.GroupBy(g => new
                        {
                            g.OrgCode,
                            g.FProductWorkCode
                        }).Select(p => new SumSoThz()
                        {
                            FieldName = item.FieldName,
                            OrgCode = p.Key.OrgCode,
                            FProductWorkCode = p.Key.FProductWorkCode,
                            Amount = p.Sum(s => s.Amount)
                        }).ToList();
                        sumSoThz.AddRange(lstTotal);                        
                    }
                }
            }
            result = result.GroupBy(g => new
            {
                g.OrgCode,
                g.VoucherCode,
                g.VoucherGroup,
                g.VoucherId,
                g.VoucherNumber,
                g.VoucherDate,
                g.ProductCode,
                g.ProductName,
                g.FProductWorkCode,
                g.CreditAcc,
                g.DebitAcc,
                g.UnitCode,
                g.Price,
                g.Note,
                g.CreditFProductWorkCode,
                g.DebitFProductWorkCode
            }).Select(p => new FProductWorkCostDto()
            {
                VoucherGroup = p.Key.VoucherGroup,
                VoucherCode = p.Key.VoucherCode,
                OrgCode = p.Key.OrgCode,
                VoucherId = p.Key.VoucherId,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                Note = p.Key.Note,
                FProductWorkCode = p.Key.FProductWorkCode,
                DebitAcc = p.Key.DebitAcc,
                CreditAcc = p.Key.CreditAcc,
                ProductCode = p.Key.ProductCode,
                ProductName = p.Key.ProductName,
                UnitCode = p.Key.UnitCode,
                Price = p.Key.Price,
                Quantity = p.Sum(s => s.Quantity),
                Ps621 = p.Sum(s => s.Ps621),
                Ps622 = p.Sum(s => s.Ps622),
                Ps623 = p.Sum(s => s.Ps623),
                Ps627 = p.Sum(s => s.Ps627),
                TotalZ = p.Sum(s => s.TotalZ),
                Ps511 = p.Sum(s => s.Ps511),
                Opening154 = p.Sum(s => s.Opening154),
                Ending154 = p.Sum(s => s.Ending154),
                CreditFProductWorkCode = p.Key.CreditFProductWorkCode,
                DebitFProductWorkCode = p.Key.DebitFProductWorkCode
            }).ToList();
            return result;
        }
        private async Task<List<Ledger>> GetDataLedger(SoTHZ item,ReportBaseParameterDto dto)
        {
            var queryable = await this.GetQueryableLegder(item, dto);
            return await AsyncExecuter.ToListAsync<Ledger>(queryable);
        }
        private async Task<List<SoTHZ>> GetSoTHZsAsync(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;
            var queryable = await _soTHZService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                && p.Year == year
                                && p.FProductOrWork == dto.FProductOrWork
                                && p.UsingDecision == yearCategory.UsingDecision
                            )
                .OrderBy(p => p.Ord);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private async Task<List<InfoZ>> GetInfoZsAsync(ReportBaseParameterDto dto)
        {
            var queryable = await this.GetQueryableInfoZ(dto);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private async Task<IQueryable<InfoZ>> GetQueryableInfoZ(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var infoZ = await _infoZService.GetQueryableAsync();
            infoZ = infoZ.Where(p => p.OrgCode == orgCode && p.Year == year
                            && p.BeginM >= dto.FromDate && p.EndM <= dto.ToDate
                            && p.DebitAcc.StartsWith("154")
                            && p.FProductWork.Equals(dto.FProductOrWork));
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                infoZ = infoZ.Where(p => p.FProductWorkCode.Equals(dto.FProductWorkCode));
            }
            return infoZ;
        }
        private async Task<IQueryable<Ledger>> GetQueryableLegder(SoTHZ item,ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var queryable = await _ledgerService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Year == year
                            && p.VoucherDate >= dto.FromDate && p.VoucherDate <= dto.ToDate);
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                queryable = queryable.Where(p => p.FProductWorkCode == dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(item.DebitAcc))
            {
                queryable = queryable.Where(p => p.DebitAcc.StartsWith(item.DebitAcc));
            }
            if (!string.IsNullOrEmpty(item.DebitSection))
            {
                queryable = queryable.Where(p => p.DebitSectionCode == item.DebitSection);
            }            
            if (!string.IsNullOrEmpty(item.CreditAcc))
            {
                queryable = queryable.Where(p => p.CreditAcc.StartsWith(item.CreditAcc));
            }
            if (!string.IsNullOrEmpty(item.CreditSection))
            {
                queryable = queryable.Where(p => p.CreditSectionCode == item.CreditSection);
            }
            queryable = queryable.Select(p => new Ledger()
            {
                VoucherGroup = p.VoucherGroup,
                VoucherCode = p.VoucherCode,
                OrgCode = p.OrgCode,
                VoucherId = p.VoucherId,
                VoucherDate = p.VoucherDate,
                VoucherNumber = p.VoucherNumber,
                Note = p.Note,
                FProductWorkCode = p.FProductWorkCode,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                Amount = p.Amount,
                ProductCode = p.ProductCode,
                ProductName0 = p.ProductName0,
                UnitCode = p.UnitCode,
                Price = p.Price,
                Quantity = p.Quantity,
                CreditFProductWorkCode = item.CreditFProductWork == "C" ? p.CreditFProductWorkCode : null,
                DebitFProductWorkCode = item.DebitFProductWork == "C" ? p.DebitFProductWorkCode : null
            });
            return queryable;
        }
        private List<InfoZ> GetInfoZ(SoTHZ soTHZ, IQueryable<InfoZ> queryableInfoz)
        {
            var queryable = this.GetQueryableSum(soTHZ, queryableInfoz);            
            return queryable.ToList();
        }
        private IQueryable<InfoZ> GetQueryableSum(SoTHZ soTHZ, IQueryable<InfoZ> queryableInfoz)
        {
            if (!string.IsNullOrEmpty(soTHZ.DebitAcc))
            {
                queryableInfoz = queryableInfoz.Where(p => p.DebitAcc.StartsWith(soTHZ.DebitAcc));
            }
            if (!string.IsNullOrEmpty(soTHZ.DebitSection))
            {
                queryableInfoz = queryableInfoz.Where(p => p.DebitSectionCode == soTHZ.DebitSection);
            }
            if (!string.IsNullOrEmpty(soTHZ.CreditAcc))
            {
                queryableInfoz = queryableInfoz.Where(p => p.CreditAcc.StartsWith(soTHZ.CreditAcc));
            }
            if (!string.IsNullOrEmpty(soTHZ.CreditSection))
            {
                queryableInfoz = queryableInfoz.Where(p => p.CreditSectionCode == soTHZ.CreditSection);
            }
            
            return queryableInfoz;
        }
        private async Task<string> GetFProductWorkName(string code,string orgCode)
        {
            var fProductWork = await _fProductWorkService.GetByFProductWorkAsync(code, orgCode);
            if (fProductWork == null) return null;
            return fProductWork.Name;
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
        #endregion
    }
}
