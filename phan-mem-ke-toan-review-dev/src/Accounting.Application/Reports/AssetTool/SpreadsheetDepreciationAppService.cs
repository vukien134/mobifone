using Accounting.BaseDtos.Customines;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.ImportExports.Parameters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.ImportExports
{
    public class SpreadsheetDepreciationAppService : AccountingAppService
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
        private readonly AssetToolAccountService _assetToolAccountService;
        private readonly AssetToolService _assetToolService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly AssetGroupAppService _assetGroupAppService;
        private readonly ReasonService _reasonService;
        private readonly AssetToolDepreciationService _assetToolDepreciationService;
        private readonly OrgUnitService _orgUnitService;        
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public SpreadsheetDepreciationAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        AssetGroupAppService assetGroupAppService,
                        AssetToolAccountService assetToolAccountService,
                        AssetToolDetailService assetToolDetailService,
                        AssetToolService assetToolService,
                        ReasonService reasonService,
                        AssetToolDepreciationService assetToolDepreciationService,
                        OrgUnitService orgUnitService,
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
            _assetGroupAppService = assetGroupAppService;
            _assetToolAccountService = assetToolAccountService;
            _assetToolDetailService = assetToolDetailService;
            _assetToolService = assetToolService;
            _reasonService = reasonService;
            _assetToolDepreciationService = assetToolDepreciationService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        public async Task<ReportResponseDto<SpreadsheetDepreciationDto>> CreateDataAsync(ReportRequestDto<SpreadsheetDepreciationParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var toMonth = dto.Parameters.ToDate.Value.Month;
            var toYear = dto.Parameters.ToDate.Value.Year;
            // get list AssetToolGroup - danh mục nhóm ts ccdc đã xử lý rank và stt nhóm
            var lstAssetToolGroup = await _assetGroupAppService.GetRankGroup("");
            lstAssetToolGroup = lstAssetToolGroup.Where(p => p.AssetOrTool == dto.Parameters.AssetOrTool).ToList();
            string ordGroupAssetTool = "";
            if (!string.IsNullOrEmpty(dto.Parameters.AssetGroupCode))
            {
                ordGroupAssetTool = lstAssetToolGroup.Where(p => p.Id == dto.Parameters.AssetGroupCode).Select(p => p.OrdGroup).First();
            }
            var lstAssetToolAccount = await _assetToolAccountService.GetListByOrgCode(orgCode);
            lstAssetToolAccount = lstAssetToolAccount.Where(p => p.Year == toYear && p.Month == toMonth).ToList();
            var lstAssetTool = await _assetToolService.GetListByOrgCode(orgCode);
            var lstAssetToolDetail = await _assetToolDetailService.GetListByOrgCode(orgCode);
            var lstReason = await _reasonService.GetListByOrgCode(orgCode);
            var lstAssetToolDepreciation = await _assetToolDepreciationService.GetListByOrgCode(orgCode);

            // get list cthtts group
            var lstAssetToolAccountGroup = lstAssetToolAccount.GroupBy(g => new { g.OrgCode, g.AssetToolId })
                                                              .Select(p => new AssetToolAccount
                                                              {
                                                                  OrgCode = p.Key.OrgCode,
                                                                  AssetToolId = p.Key.AssetToolId,
                                                                  DebitAcc = p.Max(p => p.DebitAcc),
                                                                  CreditAcc = p.Max(p => p.CreditAcc),
                                                              }).ToList();
            // get list cttscd group
            var lstAssetToolDetailGroup = (from a in lstAssetToolDetail
                                          join b in lstReason on a.UpDownCode equals b.Code
                                          where (string.IsNullOrEmpty(dto.Parameters.DepartmentCode) || a.DepartmentCode == dto.Parameters.DepartmentCode)
                                             && (string.IsNullOrEmpty(dto.Parameters.CapitalCode) || a.CapitalCode == dto.Parameters.CapitalCode)
                                          group new { a, b } by new
                                          {
                                              a.OrgCode,
                                              a.AssetToolId
                                          } into gr
                                          select new SpreadsheetDepreciationDto
                                          {
                                              OrgCode = gr.Key.OrgCode,
                                              AssetToolId = gr.Key.AssetToolId,
                                              Ord0 = gr.Max(p => p.a.Ord0),
                                              VoucherDate = gr.Max(p => p.a.VoucherDate),
                                              CapitalCode = gr.Max(p => p.a.CapitalCode),
                                              VoucherNumber = gr.Max(p => p.a.VoucherNumber),
                                              UpDownDate = gr.Min(p => p.a.UpDownDate),
                                              UpDownCode = gr.Max(p => p.a.UpDownCode),
                                              DepreciationBeginDate = gr.Max(p => p.a.BeginDate),
                                              MonthNumberDepreciation = gr.Max(p => p.a.MonthNumber),
                                              OriginalPrice1 = gr.Sum(p => (p.a.UpDownDate < dto.Parameters.FromDate) ? p.a.OriginalPrice * (p.b.ReasonType == "T" ? 1 : -1) : 0),
                                              ImpoverishmentPrice1 = gr.Sum(p => (p.a.UpDownDate < dto.Parameters.FromDate) ? p.a.Impoverishment * (p.b.ReasonType == "T" ? 1 : -1) : 0),
                                              OriginalPriceIncreased = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate 
                                                                                 && p.a.UpDownDate <= dto.Parameters.ToDate 
                                                                                 && p.b.ReasonType == "T") ? p.a.OriginalPrice : 0),
                                              OriginalPriceReduced = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                                 && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                                 && p.b.ReasonType == "G") ? p.a.OriginalPrice : 0),
                                              ImpoverishmentIncrease = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                                 && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                                 && p.b.ReasonType == "T") ? p.a.Impoverishment : 0),
                                              ImpoverishmentReduced = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                                 && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                                 && p.b.ReasonType == "G") ? p.a.Impoverishment : 0),
                                          }).ToList();

            // get list ctkhts group
            var lstAssetToolDepreciationGroup = (from a in lstAssetToolDepreciation
                                                join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                                where (dto.Parameters.DepartmentCode ?? "") == "" || a.DepartmentCode == dto.Parameters.DepartmentCode
                                                   && (dto.Parameters.CapitalCode ?? "") == "" || a.CapitalCode == dto.Parameters.CapitalCode
                                                group new { a, b } by new
                                                {
                                                    a.OrgCode,
                                                    a.AssetToolId,
                                                    a.Year,
                                                    a.Month
                                                } into gr
                                                select new SpreadsheetDepreciationDto
                                                {
                                                    OrgCode = gr.Key.OrgCode,
                                                    AssetToolId = gr.Key.AssetToolId,
                                                    Ord0 = gr.Max(p => p.a.Ord0),
                                                    DepartmentCode = gr.Max(p => p.a.DepartmentCode),
                                                    CapitalCode = gr.Max(p => p.a.CapitalCode),
                                                    DebitAcc = gr.Max(p => p.a.DebitAcc),
                                                    CreditAcc = gr.Max(p => p.a.CreditAcc),
                                                    MonthNumber0 = gr.Max(p => (p.a.DepreciationBeginDate <= dto.Parameters.ToDate && (p.a.DepreciationAmount ?? 0) != 0) ? 1 : 0),
                                                    DepreciationAccumulated = gr.Sum(p => (p.a.DepreciationBeginDate < dto.Parameters.FromDate) ? p.a.DepreciationAmount : 0),
                                                    DepreciationAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationAmount : 0),
                                                    DepreciationUpAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationUpAmount : 0),
                                                    DepreciationDownAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationDownAmount : 0),
                                                }).ToList();
            lstAssetToolDepreciationGroup = lstAssetToolDepreciationGroup
                                            .GroupBy(g => new { g.OrgCode, g.AssetToolId })
                                            .Select(p => new SpreadsheetDepreciationDto
                                            {
                                                OrgCode = p.Key.OrgCode,
                                                AssetToolId = p.Key.AssetToolId,
                                                Ord0 = p.Max(p => p.Ord0),
                                                DepartmentCode = p.Max(p => p.DepartmentCode),
                                                CapitalCode = p.Max(p => p.CapitalCode),
                                                DebitAcc = p.Max(p => p.DebitAcc),
                                                CreditAcc = p.Max(p => p.CreditAcc),
                                                MonthNumber0 = p.Sum(p => p.MonthNumber0),
                                                DepreciationAccumulated = p.Sum(p => p.DepreciationAccumulated ?? 0),
                                                DepreciationAmount = p.Sum(p => p.DepreciationAmount ?? 0),
                                                DepreciationUpAmount = p.Sum(p => p.DepreciationUpAmount ?? 0),
                                                DepreciationDownAmount = p.Sum(p => p.DepreciationDownAmount ?? 0),
                                            }).ToList();

            var data0 = (from a in lstAssetTool
                                 join b in lstAssetToolDepreciationGroup on a.Id equals b.AssetToolId into ajb
                                 from b in ajb.DefaultIfEmpty()
                                 join c in lstAssetToolDetailGroup on a.Id equals c.AssetToolId
                                 join d in lstAssetToolGroup on a.AssetToolGroupId equals d.Id into ajd
                                 from d in ajd.DefaultIfEmpty()
                                 where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                    && ((dto.Parameters.PurposeCode ?? "") == "" || (a.PurposeCode ?? "") == dto.Parameters.PurposeCode)
                                    && ((dto.Parameters.AssetGroupCode ?? "") == "" || (d?.OrdGroup ?? "").StartsWith(ordGroupAssetTool))
                                    && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                    && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                    && (c.UpDownDate <= dto.Parameters.ToDate)
                                    && (a.ReduceDate == null || a.ReduceDate >= dto.Parameters.FromDate)
                                 orderby a.Code
                                 select new SpreadsheetDepreciationDto
                                 {
                                     Year = a.Year,
                                     AssetOrTool = a.AssetOrTool,
                                     AssetGroupCode = d?.Code ?? "",
                                     AssetToolRank = d?.Rank ?? 0,
                                     AssetToolCode = a.Code,
                                     AssetToolName = a.Name,
                                     OrgCode = a.OrgCode,
                                     UpDownDate = c.UpDownDate,
                                     MonthNumber0 = b?.MonthNumber0 ?? 0,
                                     DepreciationBeginDate = c.DepreciationBeginDate,
                                     MonthNumberDepreciation = c.MonthNumberDepreciation,
                                     CapitalCode = c.CapitalCode,
                                     OriginalPrice1 = c.OriginalPrice1,
                                     ImpoverishmentPrice1 = c.ImpoverishmentPrice1 + b?.DepreciationAccumulated ?? 0,
                                     RemainingPrice1 = c.OriginalPrice1 - (c.ImpoverishmentPrice1 + b?.DepreciationAccumulated ?? 0) ,
                                     Depreciation = b?.DepreciationAmount ?? 0,
                                     OriginalPriceIncreased = c.OriginalPriceIncreased,
                                     ImpoverishmentIncrease = c.ImpoverishmentIncrease,
                                     DepreciationUpAmount = b?.DepreciationUpAmount ?? 0,
                                     OriginalPriceReduced = c.OriginalPriceReduced,
                                     ImpoverishmentReduced = c.ImpoverishmentReduced,
                                     DepreciationDownAmount = b?.DepreciationDownAmount ?? 0,
                                     OriginalPrice2 = c.OriginalPrice1 + (c.OriginalPriceIncreased ?? 0) - (c.OriginalPriceReduced ?? 0) ,
                                     ImpoverishmentPrice2 = c.ImpoverishmentPrice1 + (b?.DepreciationAccumulated ?? 0) + (c.ImpoverishmentIncrease ?? 0) - (c.ImpoverishmentReduced ?? 0) + (b?.DepreciationAmount ?? 0) ,
                                     RemainingPrice2 = (c.OriginalPrice1 + (c.OriginalPriceIncreased ?? 0) - (c.OriginalPriceReduced ?? 0))
                                                      - (c.ImpoverishmentPrice1 + (b?.DepreciationAccumulated ?? 0) + (c.ImpoverishmentIncrease ?? 0) - (c.ImpoverishmentReduced ?? 0) + (b?.DepreciationAmount ?? 0)), //OriginalPrice2 - Impoverishment2
                                     MonthNumber = (b?.MonthNumber0 ?? 0) + c.MonthNumber0,
                                     DepartmentCode = b?.DepartmentCode?? "",
                                     DebitAcc = b?.DebitAcc ?? "",
                                     CreditAcc = b?.CreditAcc ?? "",
                                 }).ToList();
            foreach (var itemData0 in data0)
            {
                if (itemData0.ReduceDate != null && itemData0.ReduceDate <= dto.Parameters.ToDate)
                {
                    itemData0.OriginalPriceReduced = itemData0.OriginalPriceReduced + itemData0.OriginalPrice2;
                    itemData0.DepreciationDownAmount = itemData0.DepreciationDownAmount + itemData0.ImpoverishmentPrice2;
                    itemData0.OriginalPrice2 = 0;
                    itemData0.ImpoverishmentPrice2 = 0;
                    itemData0.RemainingPrice2 = 0;
                }
            }

            var data = (from a in data0
                       join b in lstAssetToolGroup on a.AssetGroupCode equals b.Code
                       group new { a, b } by new
                       {
                           a.AssetToolCode
                       } into gr
                       orderby gr.Key.AssetToolCode
                       select new SpreadsheetDepreciationDto
                       {
                           Bold = "K",
                           OrgCode = gr.Max(p => p.a.OrgCode),
                           AssetToolCode = gr.Key.AssetToolCode,
                           AssetToolOrd = this.GetAssetToolOrd(lstAssetToolGroup, gr.Max(p => p.a.AssetGroupCode)),
                           AssetToolName = new string(' ', gr.Max(p => p.b.Rank + 1) * 2) + gr.Max(p => p.a.AssetToolName),
                           DepreciationBeginDate = gr.Max(p => p.a.DepreciationBeginDate),
                           AssetGroupCode = gr.Max(p => p.a.AssetGroupCode),
                           AssetToolRank = gr.Max(p => p.b.Rank + 1),
                           OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2 ?? 0),
                           ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2 ?? 0),
                           RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2 ?? 0),
                           Depreciation = gr.Sum(p => p.a.Depreciation ?? 0),
                           MonthNumber0 = gr.Sum(p => p.a.MonthNumber0 ?? 0),
                           MonthNumber = gr.Sum(p => p.a.MonthNumber ?? 0),
                           MonthNumberDepreciation = gr.Sum(p => p.a.MonthNumberDepreciation ?? 0),
                           OriginalPriceTotal = gr.Sum(p => p.a.OriginalPrice2 ?? 0),
                           DepreciationTotal = gr.Sum(p => p.a.Depreciation ?? 0),
                           DepreciationAccumulatedTotal = gr.Sum(p => p.a.ImpoverishmentPrice2 ?? 0),
                           RemainingTotal = gr.Sum(p => p.a.RemainingPrice2 ?? 0),
                           OriginalPriceReduced1 = gr.Sum(p => (p.a.CapitalCode == "NV1") ? p.a.OriginalPrice2 : 0),
                           Depreciation1 = gr.Sum(p => (p.a.CapitalCode == "NV1") ? p.a.Depreciation : 0),
                           DepreciationAccumulated1 = gr.Sum(p => (p.a.CapitalCode == "NV1") ? p.a.ImpoverishmentPrice2 : 0),
                           Remaining1 = gr.Sum(p => (p.a.CapitalCode == "NV1") ? p.a.RemainingPrice2 : 0),
                           OriginalPriceReduced2 = gr.Sum(p => (p.a.CapitalCode == "NV2") ? p.a.OriginalPrice2 : 0),
                           Depreciation2 = gr.Sum(p => (p.a.CapitalCode == "NV2") ? p.a.Depreciation : 0),
                           DepreciationAccumulated2 = gr.Sum(p => (p.a.CapitalCode == "NV2") ? p.a.ImpoverishmentPrice2 : 0),
                           Remaining2 = gr.Sum(p => (p.a.CapitalCode == "NV2") ? p.a.RemainingPrice2 : 0),
                           OriginalPriceReduced3 = gr.Sum(p => (p.a.CapitalCode == "NV3") ? p.a.OriginalPrice2 : 0),
                           Depreciation3 = gr.Sum(p => (p.a.CapitalCode == "NV3") ? p.a.Depreciation : 0),
                           DepreciationAccumulated3 = gr.Sum(p => (p.a.CapitalCode == "NV3") ? p.a.ImpoverishmentPrice2 : 0),
                           Remaining3 = gr.Sum(p => (p.a.CapitalCode == "NV3") ? p.a.RemainingPrice2 : 0),
                           OriginalPriceReduced4 = gr.Sum(p => (p.a.CapitalCode == "NV4") ? p.a.OriginalPrice2 : 0),
                           Depreciation4 = gr.Sum(p => (p.a.CapitalCode == "NV4") ? p.a.Depreciation : 0),
                           DepreciationAccumulated4 = gr.Sum(p => (p.a.CapitalCode == "NV4") ? p.a.ImpoverishmentPrice2 : 0),
                           Remaining4 = gr.Sum(p => (p.a.CapitalCode == "NV4") ? p.a.RemainingPrice2 : 0),
                           DepartmentCode = gr.Max(p => p.a.DepartmentCode),
                           DebitAcc = gr.Max(p => p.a.DebitAcc),
                           CreditAcc = gr.Max(p => p.a.CreditAcc),
                       }).ToList();

            // Tạo cây dữ liệu
            var dataGroup = (from a in data
                            join b in lstAssetToolGroup on a.AssetGroupCode equals b.Code into ajb
                            from b in ajb.DefaultIfEmpty()
                            group new { a, b } by new
                            {
                                a.OrgCode,
                                AssetToolGroupCode = b?.Code ?? "",
                                AssetToolGroupName = b?.Name ?? "",
                                Rank = b?.Rank ?? 0,
                            } into gr
                            select new SpreadsheetDepreciationDto
                            {
                                Bold = "C",
                                OrgCode = gr.Key.OrgCode,
                                AssetToolRank = gr.Key.Rank,
                                AssetToolCode = gr.Key.AssetToolGroupCode,
                                AssetGroupCode = gr.Key.AssetToolGroupCode,
                                AssetToolOrd = this.GetAssetToolOrd(lstAssetToolGroup, gr.Key.AssetToolGroupCode),
                                AssetToolName = new string(' ', gr.Key.Rank * 2) + gr.Key.AssetToolGroupName,
                                OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2),
                                ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2),
                                RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2),
                                Depreciation = gr.Sum(p => p.a.Depreciation),
                                MonthNumber0 = gr.Sum(p => p.a.MonthNumber0),
                                MonthNumber = gr.Sum(p => p.a.MonthNumber),
                                MonthNumberDepreciation = gr.Sum(p => p.a.MonthNumberDepreciation),
                                OriginalPriceTotal = gr.Sum(p => p.a.OriginalPriceTotal).GetDefaultNullIfZero(),
                                DepreciationTotal = gr.Sum(p => p.a.DepreciationTotal).GetDefaultNullIfZero(),
                                DepreciationAccumulatedTotal = gr.Sum(p => p.a.DepreciationAccumulatedTotal).GetDefaultNullIfZero(),
                                RemainingTotal = gr.Sum(p => p.a.RemainingTotal).GetDefaultNullIfZero(),
                                OriginalPriceReduced1 = gr.Sum(p => p.a.OriginalPriceReduced1).GetDefaultNullIfZero(),
                                Depreciation1 = gr.Sum(p => p.a.Depreciation1).GetDefaultNullIfZero(),
                                DepreciationAccumulated1 = gr.Sum(p => p.a.DepreciationAccumulated1).GetDefaultNullIfZero(),
                                Remaining1 = gr.Sum(p => p.a.Remaining1).GetDefaultNullIfZero(),
                                OriginalPriceReduced2 = gr.Sum(p => p.a.OriginalPriceReduced2).GetDefaultNullIfZero(),
                                Depreciation2 = gr.Sum(p => p.a.Depreciation2).GetDefaultNullIfZero(),
                                DepreciationAccumulated2 = gr.Sum(p => p.a.DepreciationAccumulated2).GetDefaultNullIfZero(),
                                Remaining2 = gr.Sum(p => p.a.Remaining2).GetDefaultNullIfZero(),
                                OriginalPriceReduced3 = gr.Sum(p => p.a.OriginalPriceReduced3).GetDefaultNullIfZero(),
                                Depreciation3 = gr.Sum(p => p.a.Depreciation3).GetDefaultNullIfZero(),
                                DepreciationAccumulated3 = gr.Sum(p => p.a.DepreciationAccumulated3).GetDefaultNullIfZero(),
                                Remaining3 = gr.Sum(p => p.a.Remaining3).GetDefaultNullIfZero(),
                                OriginalPriceReduced4 = gr.Sum(p => p.a.OriginalPriceReduced4).GetDefaultNullIfZero(),
                                Depreciation4 = gr.Sum(p => p.a.Depreciation4).GetDefaultNullIfZero(),
                                DepreciationAccumulated4 = gr.Sum(p => p.a.DepreciationAccumulated4).GetDefaultNullIfZero(),
                                Remaining4 = gr.Sum(p => p.a.Remaining4).GetDefaultNullIfZero(),
                            }).ToList();
            var dataGroupClone = dataGroup.Select(p => ObjectMapper.Map<SpreadsheetDepreciationDto, SpreadsheetDepreciationDto>(p)).ToList();
            data.AddRange(dataGroupClone);
            var rankMin = 1;
            var rankMax = (dataGroup.Count > 0) ? dataGroup.Max(p => p.AssetToolRank) : 0;
            while (rankMax > rankMin)
            {
                foreach (var itemGroup in dataGroup)
                {
                    if (itemGroup.AssetToolRank == rankMax)
                    {
                        var partnerGroupId = lstAssetToolGroup.Where(p => p.Code == itemGroup.AssetGroupCode)
                                                                 .Select(p => p.ParentId).FirstOrDefault();
                        itemGroup.AssetGroupCode = lstAssetToolGroup.Where(p => p.Id == partnerGroupId).Select(p => p.Code).FirstOrDefault();
                        itemGroup.AssetToolRank--;
                    }
                }
                var itemGroupAdd = (from a in dataGroup
                                             join b in lstAssetToolGroup on a.AssetGroupCode equals b.Code into ajb
                                             from b in ajb.DefaultIfEmpty()
                                             where a.AssetToolRank == rankMax - 1 && !data.Select(p => p.AssetGroupCode).Contains(a.AssetGroupCode)
                                    group new { a, b } by new
                                             {
                                                 a.OrgCode,
                                                 Id = b?.Id ?? "",
                                                 Rank = b?.Rank ?? 0,
                                                 OrdGroup = b?.OrdGroup ?? "",
                                                 AssetToolGroupCode = b?.Code ?? "",
                                                 AssetToolGroupName = b?.Name ?? "",
                                             } into gr
                                             select new SpreadsheetDepreciationDto
                                             {
                                                 Bold = "C",
                                                 OrgCode = gr.Key.OrgCode,
                                                 AssetToolRank = gr.Key.Rank,
                                                 AssetToolCode = gr.Key.AssetToolGroupCode,
                                                 AssetToolName = new string(' ', gr.Key.Rank * 2) + gr.Key.AssetToolGroupName,
                                                 AssetToolOrd = this.GetAssetToolOrd(lstAssetToolGroup, null),
                                                 OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2),
                                                 ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2),
                                                 RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2),
                                                 Depreciation = gr.Sum(p => p.a.Depreciation),
                                                 MonthNumber0 = gr.Sum(p => p.a.MonthNumber0),
                                                 MonthNumber = gr.Sum(p => p.a.MonthNumber),
                                                 MonthNumberDepreciation = gr.Sum(p => p.a.MonthNumberDepreciation),
                                                 OriginalPriceTotal = gr.Sum(p => p.a.OriginalPriceTotal).GetDefaultNullIfZero(),
                                                 DepreciationTotal = gr.Sum(p => p.a.DepreciationTotal).GetDefaultNullIfZero(),
                                                 DepreciationAccumulatedTotal = gr.Sum(p => p.a.DepreciationAccumulatedTotal).GetDefaultNullIfZero(),
                                                 RemainingTotal = gr.Sum(p => p.a.RemainingTotal).GetDefaultNullIfZero(),
                                                 OriginalPriceReduced1 = gr.Sum(p => p.a.OriginalPriceReduced1).GetDefaultNullIfZero(),
                                                 Depreciation1 = gr.Sum(p => p.a.Depreciation1).GetDefaultNullIfZero(),
                                                 DepreciationAccumulated1 = gr.Sum(p => p.a.DepreciationAccumulated1).GetDefaultNullIfZero(),
                                                 Remaining1 = gr.Sum(p => p.a.Remaining1).GetDefaultNullIfZero(),
                                                 OriginalPriceReduced2 = gr.Sum(p => p.a.OriginalPriceReduced2).GetDefaultNullIfZero(),
                                                 Depreciation2 = gr.Sum(p => p.a.Depreciation2).GetDefaultNullIfZero(),
                                                 DepreciationAccumulated2 = gr.Sum(p => p.a.DepreciationAccumulated2).GetDefaultNullIfZero(),
                                                 Remaining2 = gr.Sum(p => p.a.Remaining2),
                                                 OriginalPriceReduced3 = gr.Sum(p => p.a.OriginalPriceReduced3).GetDefaultNullIfZero(),
                                                 Depreciation3 = gr.Sum(p => p.a.Depreciation3).GetDefaultNullIfZero(),
                                                 DepreciationAccumulated3 = gr.Sum(p => p.a.DepreciationAccumulated3).GetDefaultNullIfZero(),
                                                 Remaining3 = gr.Sum(p => p.a.Remaining3),
                                                 OriginalPriceReduced4 = gr.Sum(p => p.a.OriginalPriceReduced4).GetDefaultNullIfZero(),
                                                 Depreciation4 = gr.Sum(p => p.a.Depreciation4).GetDefaultNullIfZero(),
                                                 DepreciationAccumulated4 = gr.Sum(p => p.a.DepreciationAccumulated4).GetDefaultNullIfZero(),
                                                 Remaining4 = gr.Sum(p => p.a.Remaining4).GetDefaultNullIfZero(),
                                             }).ToList();
                data.AddRange(itemGroupAdd);
                rankMax--;
            }
            
            // thêm dòng tổng
            data.AddRange((from a in dataGroup
                                    group new { a } by new
                                    {
                                        a.OrgCode
                                    } into gr
                                    select new SpreadsheetDepreciationDto
                                    {
                                        OrgCode = gr.Key.OrgCode,
                                        AssetToolRank = 1,
                                        AssetToolOrd = "zzz",
                                        AssetToolCode = "",
                                        AssetToolName = "Tổng cộng",
                                        Bold = "C",
                                        OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2 ?? 0),
                                        ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2 ?? 0),
                                        RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2 ?? 0),
                                        Depreciation = gr.Sum(p => p.a.Depreciation ?? 0),
                                        OriginalPriceTotal = gr.Sum(p => p.a.OriginalPriceTotal ?? 0),
                                        DepreciationTotal = gr.Sum(p => p.a.DepreciationTotal ?? 0),
                                        DepreciationAccumulatedTotal = gr.Sum(p => p.a.DepreciationAccumulatedTotal ?? 0),
                                        RemainingTotal = gr.Sum(p => p.a.RemainingTotal ?? 0),
                                        OriginalPriceReduced1 = gr.Sum(p => p.a.OriginalPriceReduced1 ?? 0),
                                        Depreciation1 = gr.Sum(p => p.a.Depreciation1 ?? 0),
                                        DepreciationAccumulated1 = gr.Sum(p => p.a.DepreciationAccumulated1 ?? 0),
                                        Remaining1 = gr.Sum(p => p.a.Remaining1 ?? 0),
                                        OriginalPriceReduced2 = gr.Sum(p => p.a.OriginalPriceReduced2 ?? 0),
                                        Depreciation2 = gr.Sum(p => p.a.Depreciation2 ?? 0),
                                        DepreciationAccumulated2 = gr.Sum(p => p.a.DepreciationAccumulated2 ?? 0),
                                        Remaining2 = gr.Sum(p => p.a.Remaining2 ?? 0),
                                        OriginalPriceReduced3 = gr.Sum(p => p.a.OriginalPriceReduced3 ?? 0),
                                        Depreciation3 = gr.Sum(p => p.a.Depreciation3 ?? 0),
                                        DepreciationAccumulated3 = gr.Sum(p => p.a.DepreciationAccumulated3 ?? 0),
                                        Remaining3 = gr.Sum(p => p.a.Remaining3 ?? 0),
                                        OriginalPriceReduced4 = gr.Sum(p => p.a.OriginalPriceReduced4 ?? 0),
                                        Depreciation4 = gr.Sum(p => p.a.Depreciation4 ?? 0),
                                        DepreciationAccumulated4 = gr.Sum(p => p.a.DepreciationAccumulated4 ?? 0),
                                        Remaining4 = gr.Sum(p => p.a.Remaining4 ?? 0),
                                    }).ToList());

            var reportResponse = new ReportResponseDto<SpreadsheetDepreciationDto>();
            reportResponse.Data = data.OrderBy(p => p.AssetToolOrd).ThenBy(p => p.AssetToolRank).ThenBy(p => p.AssetToolCode).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<SpreadsheetDepreciationParameterDto> dto)
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
        private string GetAssetToolOrd(List<AssetToolGroupCustomineDto> lstAssetToolGroup, string assetGroupCode)
        {
            return lstAssetToolGroup.Where(p => p.Code == assetGroupCode).Select(p => p.OrdGroup).FirstOrDefault();
        }
        #endregion
    }
}
