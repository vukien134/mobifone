using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
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
    public class IncreaseReducedAssetToolAppService : AccountingAppService
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
        public IncreaseReducedAssetToolAppService(ReportDataService reportDataService,
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
        public async Task<ReportResponseDto<IncreaseReducedAssetToolDto>> CreateDataAsync(ReportRequestDto<IncreaseReducedAssetToolParameterDto> dto)
        {
            // get list AssetToolGroup - danh mục nhóm ts ccdc đã xử lý rank và stt nhóm
            var lstAssetToolGroup = await _assetGroupAppService.GetRankGroup("");
            lstAssetToolGroup = lstAssetToolGroup.Where(p => p.AssetOrTool == dto.Parameters.AssetOrTool).ToList();
            string ordGroupAssetTool = "";
            if (!string.IsNullOrEmpty(dto.Parameters.AssetGroupCode))
            {
                ordGroupAssetTool = lstAssetToolGroup.Where(p => p.Id == dto.Parameters.AssetGroupCode).Select(p => p.OrdGroup).First();
            }
            var reason = await _reasonService.GetQueryableAsync();
            var lstReason = reason.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstIncrease = lstReason.Where(p => ((dto.Parameters.UpDownCode ?? "") == "" || p.Code == dto.Parameters.UpDownCode) && p.ReasonType == "T").Select(p => p.Code);
            var lstReduced = lstReason.Where(p => ((dto.Parameters.UpDownCode ?? "") == "" || p.Code == dto.Parameters.UpDownCode) && p.ReasonType == "G").Select(p => p.Code);
            // get list AssetTool - đầu phiếu ts, ccdc
            var assetTool = await _assetToolService.GetQueryableAsync();
            var lstAssetTool = assetTool.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetToolDetail - chi tiết ts, ccdc
            var assetToolDetail = await _assetToolDetailService.GetQueryableAsync();
            var lstAssetToolDetail = assetToolDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var data = new List<IncreaseReducedAssetToolDto>();

            // get list AssetToolDepreciation - chi tiết khấu hao ts
            var assetToolDepreciation = await _assetToolDepreciationService.GetQueryableAsync();
            var lstAssetToolDepreciation = assetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // lấy nguyên giá
            if (dto.Parameters.ReasonType == "T" || dto.Parameters.ReasonType == "*")
            {
                data.AddRange((from a in lstAssetToolDetail
                               join b in lstAssetTool on a.AssetToolId equals b.Id
                               join c in lstReason on a.UpDownCode equals c.Code
                               join d in lstAssetToolGroup on b.AssetToolGroupId equals d.Id into bjd
                               from d in bjd.DefaultIfEmpty()
                               where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                   && ((dto.Parameters.PurposeCode ?? "") == "" || b.PurposeCode == dto.Parameters.PurposeCode)
                                   && (lstIncrease.Contains(a.UpDownCode) || lstIncrease.Contains(b.UpDownCode)) 
                                   && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Where(p => p.OrdGroup.StartsWith(ordGroupAssetTool)).Select(p => p.Id).Contains(b.AssetToolGroupId))
                                   && ((dto.Parameters.AssetToolCode ?? "") == "" || b.Code == dto.Parameters.AssetToolCode)
                                   && ((dto.Parameters.AssetToolAcc ?? "") == "" || (b.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                   && (b.ReduceDate == null || b.ReduceDate >= dto.Parameters.FromDate)
                               group new { a, b, c, d } by new
                               {
                                   b.OrgCode,
                                   a.Ord0,
                                   b.Code,
                                   b.ReduceDate
                               } into gr
                               select new IncreaseReducedAssetToolDto
                               {
                                   OrgCode = gr.Key.OrgCode,
                                   Ord0 = gr.Key.Ord0,
                                   AssetToolCode = gr.Key.Code,
                                   AssetToolName = gr.Max(p => p.b.Name),
                                   Country = gr.Max(p => p.b.Country),
                                   ProductionYear = gr.Max(p => p.b.ProductionYear),
                                   Wattage = gr.Max(p => p.b.Wattage),
                                   Quantity = gr.Max(p => p.b.Quantity),
                                   Note = gr.Max(p => p.b.Note),
                                   CalculatingMethod = gr.Max(p => p.b.CalculatingMethod),
                                   AssetToolAcc = gr.Max(p => p.b.AssetToolAcc),
                                   PurposeCode = gr.Max(p => p.b.PurposeCode),
                                   UpDownDate = gr.Min(p => p.a.UpDownDate),
                                   DepreciationBeginDate = gr.Min(p => p.a.BeginDate),
                                   AssetToolGroupCode = gr.Max(p => p.d?.Code ?? ""),
                                   ReduceDate = (gr.Key.ReduceDate != null && gr.Key.ReduceDate > dto.Parameters.ToDate) ? null : gr.Key.ReduceDate,
                                   MonthNumberDepreciation = gr.Max(p => p.a.MonthNumber),
                                   IncreaseOrReduced = "T",
                                   MonthNumber0 = gr.Sum(p => p.a.MonthNumber0),
                                   OriginalPrice1 = gr.Sum(p => (p.a.UpDownDate < dto.Parameters.FromDate) ? (p.a.OriginalPrice ?? 0) : 0),
                                   ImpoverishmentPrice1 = gr.Sum(p => (p.a.UpDownDate < dto.Parameters.FromDate) ? (p.a.Impoverishment ?? 0) : 0),
                                   OriginalPriceIncreased = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "T") ? p.a.OriginalPrice ?? 0 : 0),
                                   OriginalPriceReduced = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "G") ? p.a.OriginalPrice ?? 0 : 0),
                                   ImpoverishmentIncrease = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "T") ? p.a.Impoverishment ?? 0 : 0),
                                   ImpoverishmentReduced = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "G") ? p.a.Impoverishment ?? 0 : 0),
                               }).ToList());
            } 
            
            if (dto.Parameters.ReasonType == "G" || dto.Parameters.ReasonType == "*")
            {
                data.AddRange((from a in lstAssetToolDetail
                               join b in lstAssetTool on a.AssetToolId equals b.Id
                               join c in lstReason on a.UpDownCode equals c.Code
                               join d in lstAssetToolGroup on b.AssetToolGroupId equals d.Id into bjd
                               from d in bjd.DefaultIfEmpty()
                               where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                   && ((dto.Parameters.PurposeCode ?? "") == "" || b.PurposeCode == dto.Parameters.PurposeCode)
                                   && (lstReduced.Contains(a.UpDownCode) || (b.ReduceDate != null && b.ReduceDate < dto.Parameters.ToDate && lstReduced.Contains(b.UpDownCode)))
                                   && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Where(p => p.OrdGroup.StartsWith(ordGroupAssetTool)).Select(p => p.Id).Contains(b.AssetToolGroupId))
                                   && ((dto.Parameters.AssetToolCode ?? "") == "" || b.Code == dto.Parameters.AssetToolCode)
                                   && ((dto.Parameters.AssetToolAcc ?? "") == "" || (b.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                   && (b.ReduceDate == null || b.ReduceDate >= dto.Parameters.FromDate)
                               group new { a, b, c, d } by new
                               {
                                   b.OrgCode,
                                   a.Ord0,
                                   b.Code,
                                   b.ReduceDate
                               } into gr
                               select new IncreaseReducedAssetToolDto
                               {
                                   OrgCode = gr.Key.OrgCode,
                                   Ord0 = gr.Key.Ord0,
                                   AssetToolCode = gr.Key.Code,
                                   AssetToolName = gr.Max(p => p.b.Name),
                                   Country = gr.Max(p => p.b.Country),
                                   ProductionYear = gr.Max(p => p.b.ProductionYear),
                                   Wattage = gr.Max(p => p.b.Wattage),
                                   Quantity = gr.Max(p => p.b.Quantity),
                                   Note = gr.Max(p => p.b.Note),
                                   CalculatingMethod = gr.Max(p => p.b.CalculatingMethod),
                                   AssetToolAcc = gr.Max(p => p.b.AssetToolAcc),
                                   PurposeCode = gr.Max(p => p.b.PurposeCode),
                                   UpDownDate = gr.Min(p => p.a.UpDownDate),
                                   DepreciationBeginDate = gr.Min(p => p.a.BeginDate),
                                   AssetToolGroupCode = gr.Max(p => p.d?.Code ?? ""),
                                   ReduceDate = (gr.Key.ReduceDate != null && gr.Key.ReduceDate > dto.Parameters.ToDate) ? null : gr.Key.ReduceDate,
                                   MonthNumberDepreciation = gr.Max(p => p.a.MonthNumber),
                                   IncreaseOrReduced = "G",
                                   MonthNumber0 = gr.Sum(p => p.a.MonthNumber0 ?? 0),
                                   OriginalPrice1 = gr.Sum(p => (p.a.UpDownDate < dto.Parameters.FromDate) ? (p.a.OriginalPrice ?? 0) : 0),
                                   ImpoverishmentPrice1 = gr.Sum(p => (p.a.UpDownDate < dto.Parameters.FromDate) ? (p.a.Impoverishment ?? 0) : 0),
                                   OriginalPriceIncreased = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "T") ? p.a.OriginalPrice ?? 0 : 0),
                                   OriginalPriceReduced = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "G") ? p.a.OriginalPrice ?? 0 : 0),
                                   ImpoverishmentIncrease = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "T") ? p.a.Impoverishment ?? 0 : 0),
                                   ImpoverishmentReduced = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                       && p.a.UpDownDate <= dto.Parameters.ToDate
                                                                       && p.c.ReasonType == "G") ? p.a.Impoverishment ?? 0 : 0),
                               }).ToList());
            }

            // Lay Gt Khau hao TSCD
            var dataDepreciation = (from a in lstAssetToolDepreciation
                                    join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 } into ajb
                                    from b in ajb.DefaultIfEmpty()
                                    join c in lstAssetTool on a.AssetToolId equals c.Id into ajc
                                    from c in ajc.DefaultIfEmpty()
                                    join d in lstAssetToolGroup on c.AssetToolGroupId equals d.Id into ajd
                                    from d in ajd.DefaultIfEmpty()
                                    where (dto.Parameters.DepartmentCode ?? "") == "" || a.DepartmentCode == dto.Parameters.DepartmentCode
                                       && ((dto.Parameters.PurposeCode ?? "") == "" || c.PurposeCode == dto.Parameters.PurposeCode)
                                       && ((dto.Parameters.AssetGroupCode ?? "") == "" || (d?.OrdGroup ?? "").StartsWith(ordGroupAssetTool))
                                       && ((dto.Parameters.AssetToolAcc ?? "") == "" || (c.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                       && (c.ReduceDate == null || c.ReduceDate >= dto.Parameters.FromDate)
                                    group new { a, b, c, d } by new
                                    {
                                        a.OrgCode,
                                        AssetToolCode = c?.Code ?? "",
                                        Ord0 = b?.Ord0 ?? ""
                                    } into gr
                                    select new IncreaseReducedAssetToolDto
                                    {
                                        OrgCode = gr.Key.OrgCode,
                                        AssetToolCode = gr.Key.AssetToolCode,
                                        Ord0 = gr.Key.Ord0,
                                        AssetToolName = gr.Max(p => p.c.Name),
                                        MonthNumber = gr.Sum(p => (p.a.DepreciationBeginDate <= dto.Parameters.ToDate && p.a.DepreciationAmount > 0) ? 1 : 0),
                                        DepreciationAccumulated = gr.Sum(p => (p.a.DepreciationBeginDate <= dto.Parameters.FromDate) ? p.a.DepreciationAmount : 0),
                                        DepreciationAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationAmount : 0),
                                        DepreciationUpAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationUpAmount : 0),
                                        DepreciationDownAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationDownAmount : 0),
                                    }).ToList();
            var dataIncreasedReduced = (from a in data
                                       join b in dataDepreciation on new { AssetToolCode = a.AssetToolCode, Ord0 = a.Ord0 } equals new { AssetToolCode = b.AssetToolCode, Ord0 = b.Ord0 } into ajb
                                       from b in ajb.DefaultIfEmpty()
                                       join c in lstAssetToolGroup on a.AssetToolGroupCode equals c.Code into ajc
                                       from c in ajc.DefaultIfEmpty()
                                       where a.OriginalPrice1 + (a.OriginalPriceIncreased ?? 0) + (a.OriginalPriceReduced ?? 0) > 0
                                          && ((a.IncreaseOrReduced == "T" && a.UpDownDate >= dto.Parameters.FromDate && a.UpDownDate <= dto.Parameters.ToDate)
                                              || (a.IncreaseOrReduced == "G" && (a.ReduceDate ?? a.UpDownDate) >= dto.Parameters.FromDate && (a.ReduceDate ?? a.UpDownDate) <= dto.Parameters.ToDate))
                                       orderby a.AssetToolCode
                                       select new IncreaseReducedAssetToolDto
                                       {
                                           Bold = "K",
                                           Sort = "B",
                                           IncreaseOrReduced = a.IncreaseOrReduced,
                                           OrgCode = a.OrgCode,
                                           AssetToolCode = a.AssetToolCode,
                                           AssetToolName = a.AssetToolName,
                                           Country = a.Country,
                                           ProductionYear = a.ProductionYear,
                                           Wattage = a.Wattage,
                                           Quantity = a.Quantity,
                                           Note = a.Note,
                                           CalculatingMethod = a.CalculatingMethod,
                                           AssetToolAcc = a.AssetToolAcc,
                                           PurposeCode = a.PurposeCode,
                                           DepreciationBeginDate = a.DepreciationBeginDate,
                                           AssetToolGroupCode = a.AssetToolGroupCode,
                                           AssetToolRank = (c?.Rank ?? 0) + 1,
                                           AssetToolOrdGroup = c?.OrdGroup ?? null,
                                           ReduceDate = a.ReduceDate,
                                           UpDownDate = (a.IncreaseOrReduced == "T" ? a.UpDownDate : (a.ReduceDate ?? a.UpDownDate)),
                                           MonthNumberDepreciation = a.MonthNumberDepreciation,
                                           MonthNumber = a.MonthNumber0 + (b?.MonthNumber ?? 0),
                                           OriginalPrice2 = (a.OriginalPrice1 ?? 0) + (a.OriginalPriceIncreased ?? 0) + (a.OriginalPriceReduced ?? 0),
                                           ImpoverishmentPrice2 = (a.ImpoverishmentPrice1 ?? 0) + (b?.DepreciationAccumulated ?? 0) + (a.ImpoverishmentIncrease ?? 0) + (a.ImpoverishmentReduced ?? 0) + (b?.DepreciationAmount ?? 0),
                                           RemainingPrice2 = ((a.OriginalPrice1 ?? 0) + (a.OriginalPriceIncreased ?? 0) + (a.OriginalPriceReduced ?? 0)) 
                                                           - ((a.ImpoverishmentPrice1 ?? 0) + (b?.DepreciationAccumulated ?? 0) + (a.ImpoverishmentIncrease ?? 0) + (a.ImpoverishmentReduced ?? 0) + (b?.DepreciationAmount ?? 0)),
                                       }).ToList();
            // Tổng
            var dataTotal = (from a in dataIncreasedReduced
                            group new { a } by new { a.IncreaseOrReduced } into gr
                            select new IncreaseReducedAssetToolDto
                            {
                                IncreaseOrReduced = gr.Key.IncreaseOrReduced,
                                OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2),
                                ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2),
                                RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2),
                            }).ToList();

            var dataIncreasedReducedGroup = (from a in dataIncreasedReduced
                                             join b in lstAssetToolGroup on new { AssetToolGroupCode = a?.AssetToolGroupCode ?? "" } equals new { AssetToolGroupCode = b.Code } into ajb
                                             from b in ajb.DefaultIfEmpty()
                                             group new { a, b } by new
                                             {
                                                 a.OrgCode,
                                                 a.IncreaseOrReduced,
                                                 PartnerGroupRank = b.Rank,
                                                 AssetToolOrdGroup = b.OrdGroup,
                                                 AssetToolGroupCode = b.Code,
                                                 AssetToolGroupName = b.Name,
                                             } into gr
                                             orderby gr.Key.OrgCode
                                             select new IncreaseReducedAssetToolDto
                                             {
                                                 IncreaseOrReduced = gr.Key.IncreaseOrReduced,
                                                 Sort = "B",
                                                 Bold = "C",
                                                 OrgCode = gr.Key.OrgCode,
                                                 AssetToolRank = gr.Key.PartnerGroupRank,
                                                 AssetToolOrdGroup = gr.Key.AssetToolOrdGroup,
                                                 AssetToolGroupCode = gr.Key.AssetToolGroupCode,
                                                 AssetToolCode = gr.Key.AssetToolGroupCode,
                                                 AssetToolName = gr.Key.AssetToolGroupName,
                                                 OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2 ?? 0),
                                                 ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2 ?? 0),
                                                 RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2 ?? 0),
                                             }).ToList();
            int rankMin = 1;
            int rankMax = (dataIncreasedReducedGroup.Count > 0) ? dataIncreasedReducedGroup.Max(p => p.AssetToolRank) : 0;
            var dataIncreasedReducedGroupClone = dataIncreasedReducedGroup.Select(p => ObjectMapper.Map<IncreaseReducedAssetToolDto, IncreaseReducedAssetToolDto>(p)).ToList();
            dataIncreasedReduced.AddRange(dataIncreasedReducedGroupClone);
            while (rankMax > rankMin)
            {
                foreach (var itemDataIncreasedReduced in dataIncreasedReducedGroup)
                {
                    if (itemDataIncreasedReduced.AssetToolRank == rankMax)
                    {
                        var partnerGroupId = lstAssetToolGroup.Where(p => p.Code == itemDataIncreasedReduced.AssetToolGroupCode)
                                                                 .Select(p => p.ParentId).FirstOrDefault();
                        itemDataIncreasedReduced.AssetToolGroupCode = lstAssetToolGroup.Where(p => p.Id == partnerGroupId).Select(p => p.Code).FirstOrDefault();
                        itemDataIncreasedReduced.AssetToolRank--;
                    }
                }
                var itemIncurredDataGroupAdd = (from a in dataIncreasedReducedGroup
                                                join b in lstAssetToolGroup on new { AssetToolGroupCode = a?.AssetToolGroupCode ?? "" } equals new { AssetToolGroupCode = b.Code } into ajb
                                                from b in ajb.DefaultIfEmpty()
                                                where a.AssetToolRank == rankMax - 1 && !dataIncreasedReduced.Select(p => p.AssetToolGroupCode).Contains(a.AssetToolGroupCode)
                                                group new { a, b } by new
                                                {
                                                    a.OrgCode,
                                                    a.IncreaseOrReduced,
                                                    PartnerGroupRank = b.Rank,
                                                    AssetToolOrdGroup = b.OrdGroup,
                                                    AssetToolGroupCode = b.Code,
                                                    AssetToolGroupName = b.Name,
                                                } into gr
                                                select new IncreaseReducedAssetToolDto
                                                {
                                                    IncreaseOrReduced = gr.Key.IncreaseOrReduced,
                                                    Sort = "B",
                                                    Bold = "C",
                                                    OrgCode = gr.Key.OrgCode,
                                                    AssetToolRank = gr.Key.PartnerGroupRank,
                                                    AssetToolOrdGroup = gr.Key.AssetToolOrdGroup,
                                                    AssetToolCode = gr.Key.AssetToolGroupCode,
                                                    AssetToolName = gr.Key.AssetToolGroupName,
                                                    OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2),
                                                    ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2),
                                                    RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2),
                                                }).ToList();
                dataIncreasedReduced.AddRange(itemIncurredDataGroupAdd);
                rankMax--;
            }

            foreach (var item in dataIncreasedReduced)
            {
                var space = "";
                for (var i = 1; i < item.AssetToolRank; i++)
                {
                    space += "  ";
                }
            }
            dataIncreasedReduced.AddRange((from a in dataTotal
                                           select new IncreaseReducedAssetToolDto
                                           {
                                               IncreaseOrReduced = a.IncreaseOrReduced,
                                               Sort = "A",
                                               Bold = "C",
                                               AssetToolName = (a.IncreaseOrReduced == "T") ? 
                                                               (dto.Parameters.AssetOrTool == "TS" ? "I. TÀI SẢN CỐ ĐỊNH TĂNG" : "I. CÔNG CỤ, DỤNG CỤ TĂNG") :
                                                               (dto.Parameters.AssetOrTool == "TS" ? "I. TÀI SẢN CỐ ĐỊNH GIẢM" : "I. CÔNG CỤ, DỤNG CỤ GIẢM"),
                                           }).ToList());
            dataIncreasedReduced.AddRange((from a in dataTotal
                                           select new IncreaseReducedAssetToolDto
                                           {
                                               IncreaseOrReduced = a.IncreaseOrReduced,
                                               Sort = "C",
                                               Bold = "C",
                                               AssetToolName = "TỔNG CỘNG",
                                               OriginalPrice2 = a.OriginalPrice2,
                                               ImpoverishmentPrice2 = a.ImpoverishmentPrice2,
                                               RemainingPrice2 = a.RemainingPrice2,
                                           }).ToList());
            var reportResponse = new ReportResponseDto<IncreaseReducedAssetToolDto>();
            reportResponse.Data = dataIncreasedReduced.OrderByDescending(p => p.IncreaseOrReduced).ThenBy(p => p.Sort)
                                      .ThenBy(p => p.AssetToolOrdGroup).ThenBy(p => p.AssetToolRank).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<IncreaseReducedAssetToolParameterDto> dto)
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
