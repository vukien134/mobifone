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
    public class AssetToolBookAppService : AccountingAppService
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
        public AssetToolBookAppService(ReportDataService reportDataService,
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
        public async Task<ReportResponseDto<AssetBookDto>> CreateDataAsync(ReportRequestDto<AssetBookParameterDto> dto)
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

            // get list AssetToolAccount - chi tiết htts
            var assetToolAccount = await _assetToolAccountService.GetQueryableAsync();
            var lstAssetToolAccount = assetToolAccount.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() 
                                                               && p.Year == toYear && p.Month == toMonth).ToList();

            // get list AssetTool - đầu phiếu ts, ccdc
            var assetTool = await _assetToolService.GetQueryableAsync();
            var lstAssetTool = assetTool.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetToolDetail - chi tiết ts, ccdc
            var assetToolDetail = await _assetToolDetailService.GetQueryableAsync();
            var lstAssetToolDetail = assetToolDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list Reason - danh mục lý do
            var reason = await _reasonService.GetQueryableAsync();
            var lstReason = reason.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetToolDepreciation - chi tiết khấu hao ts
            var assetToolDepreciation = await _assetToolDepreciationService.GetQueryableAsync();
            var lstAssetToolDepreciation = assetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

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
                                          where dto.Parameters.DepartmentCode == "" || a.DepartmentCode == dto.Parameters.DepartmentCode
                                          group new { a, b } by new
                                          {
                                              a.OrgCode,
                                              a.AssetToolId
                                          } into gr
                                          select new AssetBookDto
                                          {
                                              OrgCode = gr.Key.OrgCode,
                                              AssetToolId = gr.Key.AssetToolId,
                                              Ord0 = gr.Max(p => p.a.Ord0),
                                              VoucherDate = gr.Max(p => p.a.VoucherDate),
                                              VoucherNumber = gr.Max(p => p.a.VoucherNumber),
                                              UpDownDate = gr.Min(p => p.a.UpDownDate),
                                              UpDownCode = gr.Max(p => p.a.UpDownCode),
                                              DepreciationBeginDate = gr.Max(p => p.a.BeginDate),
                                              MonthNumber = gr.Max(p => p.a.MonthNumber),
                                              MonthNumber0 = gr.Max(p => p.a.MonthNumber0),
                                              DepartmentCode = gr.Max(p => p.a.DepartmentCode),
                                              CapitalCode = gr.Max(p => p.a.CapitalCode),
                                              IncreaseReduced = gr.Max(p => p.b.ReasonType),
                                              DepreciationAmount = gr.Sum(p => p.a.DepreciationAmount),
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
                                                group new { a, b } by new
                                                {
                                                    a.OrgCode,
                                                    a.AssetToolId
                                                } into gr
                                                select new AssetBookDto
                                                {
                                                    OrgCode = gr.Key.OrgCode,
                                                    AssetToolId = gr.Key.AssetToolId,
                                                    Ord0 = gr.Max(p => p.a.Ord0),
                                                    DepartmentCode = gr.Max(p => p.a.DepartmentCode),
                                                    CapitalCode = gr.Max(p => p.a.CapitalCode),
                                                    DebitAcc = gr.Max(p => p.a.DebitAcc),
                                                    CreditAcc = gr.Max(p => p.a.CreditAcc),
                                                    DepreciationAccumulated = gr.Sum(p => (p.a.DepreciationBeginDate < dto.Parameters.FromDate) ? p.a.DepreciationAmount : 0),
                                                    DepreciationAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationAmount : 0),
                                                    DepreciationUpAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationUpAmount : 0),
                                                    DepreciationDownAmount = gr.Sum(p => (p.a.DepreciationBeginDate >= dto.Parameters.FromDate
                                                                                 && p.a.DepreciationBeginDate <= dto.Parameters.ToDate) ? p.a.DepreciationDownAmount : 0),
                                                }).ToList();
            // get list so_thang group theo AssetToolId
            var monthNumberGroup = lstAssetToolDepreciation.GroupBy(g => new { g.Year, g.AssetToolId, g.Month, g.Ord0 })
                                                              .Select(p => new
                                                              {
                                                                  AssetToolId = p.Key.AssetToolId,
                                                                  MonthNumber = (p.Key.Ord0.Substring(p.Key.Ord0.Length-1) == "1" && p.Sum(p => p.DepreciationAmount ?? 0) > 0) ? 1 : 0,
                                                              }).ToList();
            monthNumberGroup = monthNumberGroup.GroupBy(g => new { g.AssetToolId })
                                                              .Select(p => new
                                                              {
                                                                  AssetToolId = p.Key.AssetToolId,
                                                                  MonthNumber = p.Sum(p => p.MonthNumber),
                                                              }).ToList();

            var dataAssetBook = (from a in lstAssetTool
                                 join d in lstAssetToolAccountGroup on a.Id equals d.AssetToolId into ajd
                               from d in ajd.DefaultIfEmpty()
                               join c in lstAssetToolDetailGroup on a.Id equals c.AssetToolId
                               join b in lstAssetToolDepreciationGroup on a.Id equals b.AssetToolId into ajb
                               from b in ajb.DefaultIfEmpty()
                               join e in lstAssetToolGroup on a.AssetToolGroupId equals e.Id
                               join f in monthNumberGroup on a.Id equals f.AssetToolId into ajf
                               from f in ajf.DefaultIfEmpty()
                               where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                  && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                  && ((dto.Parameters.AssetGroupCode ?? "") == "" || (e.OrdGroup ?? "").StartsWith(ordGroupAssetTool))
                                  && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                  && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                  && (c.UpDownDate <= dto.Parameters.ToDate)
                                  && (a.ReduceDate == null || a.ReduceDate >= dto.Parameters.FromDate)
                               group new { a, b, c, d, e, f } by new
                               {
                                   a.Code
                               } into gr
                               orderby gr.Key.Code
                               select new AssetBookDto
                               {
                                   Bold = "K",
                                   OrgCode = _webHelper.GetCurrentOrgUnit(),
                                   AssetToolCode = gr.Key.Code,
                                   Country = gr.Max(p => p.a.Country),
                                   UpDownCode0 = gr.Max(p => p.a.UpDownCode),
                                   ReduceDetail = gr.Max(p => p.a.ReduceDetail),
                                   ReduceDate0 = gr.Max(p => p.a.ReduceDate),
                                   Content = gr.Max(p => p.a.Content),
                                   AssetToolName = gr.Max(p => p.a.Name),
                                   Quantity = gr.Max(p => p.a.Quantity),
                                   AssetGroupCode = gr.Max(p => p.e.Code),
                                   AssetToolRank = gr.Max(p => p.e.Rank + 1),
                                   AssetToolOrd = gr.Max(p => p.e.OrdGroup),
                                   ReduceDate = gr.Max(p => p.a.ReduceDate),
                                   VoucherDate = gr.Max(p => p.c.VoucherDate),
                                   VoucherNumber = gr.Max(p => p.c.VoucherNumber),
                                   UpDownDate = gr.Max(p => p.c.UpDownDate),
                                   UpDownCode = gr.Max(p => p.c.UpDownCode),
                                   DepreciationBeginDate = gr.Max(p => p.c.DepreciationBeginDate),
                                   MonthNumber = gr.Max(p => p.c.MonthNumber),
                                   DepreciationAmount = gr.Sum(p => p.c.DepreciationAmount ?? 0),
                                   DepartmentCode = gr.Max(p => p.c.DepartmentCode),
                                   CapitalCode = gr.Max(p => p.c.CapitalCode),
                                   IncreaseReduced = gr.Max(p => p.c.IncreaseReduced),
                                   DebitAcc = gr.Max(p => (p.d?.DebitAcc ?? "")),
                                   CreditAcc = gr.Max(p => (p.d?.CreditAcc ?? "" )),
                                   MonthNumber0 = gr.Sum(p => p.c.MonthNumber0 + ((p.f == null) ? 0 : p.f.MonthNumber)),
                                   OriginalPrice1 = gr.Sum(p => p.c.OriginalPrice1),
                                   ImpoverishmentPrice1 = gr.Sum(p => p.c.ImpoverishmentPrice1 + (p.b?.DepreciationAccumulated ?? 0)),
                                   RemainingPrice1 = gr.Sum(p => p.c.OriginalPrice1 - p.c.ImpoverishmentPrice1 - (p.b?.DepreciationAccumulated ?? 0)),
                                   Depreciation = gr.Sum(p => p.b?.DepreciationAmount ?? 0),
                                   OriginalPriceIncreased = gr.Sum(p => (p.c.OriginalPriceIncreased ?? 0)),
                                   ImpoverishmentIncrease = gr.Sum(p => (p.c.ImpoverishmentIncrease ?? 0)),
                                   DepreciationUpAmount = gr.Sum(p => (p.b?.DepreciationUpAmount ?? 0 )),
                                   OriginalPriceReduced = gr.Sum(p => (p.c.OriginalPriceReduced ?? 0)),
                                   ImpoverishmentReduced = gr.Sum(p => (p.c.ImpoverishmentReduced ?? 0)),
                                   DepreciationDownAmount = gr.Sum(p => gr.Sum(p => p.b?.DepreciationDownAmount ?? 0)),
                                   OriginalPrice2 = gr.Sum(p => (p.c.OriginalPrice1 ?? 0) + (p.c.OriginalPriceIncreased ?? 0) - (p.c.OriginalPriceReduced ?? 0)),
                                   ImpoverishmentPrice2 = gr.Sum(p => (p.c.ImpoverishmentPrice1 ?? 0) + (p.b?.DepreciationAccumulated ?? 0) + (p.c.ImpoverishmentIncrease ?? 0) - (p.c.ImpoverishmentReduced ?? 0) + (p.b?.DepreciationAmount ?? 0)),
                                   RemainingPrice2 = gr.Sum(p => (p.c.OriginalPrice1 ?? 0) + (p.c.OriginalPriceIncreased ?? 0) - (p.c.OriginalPriceReduced ?? 0) - ((p.c.ImpoverishmentPrice1 ?? 0) + (p.b?.DepreciationAccumulated ?? 0) + (p.c.ImpoverishmentIncrease ?? 0) - (p.c.ImpoverishmentReduced ?? 0) + (p.b?.DepreciationAmount ?? 0))),
                               }).ToList();
            foreach (var itemAssetBook in dataAssetBook)
            {
                if (itemAssetBook.ReduceDate != null && itemAssetBook.ReduceDate <= dto.Parameters.ToDate)
                {
                    itemAssetBook.OriginalPriceReduced = itemAssetBook.OriginalPriceReduced + itemAssetBook.OriginalPrice2;
                    itemAssetBook.OriginalPriceIncreased = (itemAssetBook.UpDownDate >= dto.Parameters.FromDate
                                                            && itemAssetBook.UpDownDate <= dto.Parameters.ToDate) ? itemAssetBook.OriginalPriceIncreased : 0;
                    itemAssetBook.DepreciationDownAmount = itemAssetBook.DepreciationDownAmount + itemAssetBook.ImpoverishmentPrice2;
                    itemAssetBook.ImpoverishmentReduced = itemAssetBook.ImpoverishmentPrice1 + itemAssetBook.Depreciation;
                    itemAssetBook.ImpoverishmentIncrease = 0;
                    itemAssetBook.OriginalPrice2 = 0;
                    itemAssetBook.ImpoverishmentPrice2 = 0;
                    itemAssetBook.RemainingPrice2 = 0;
                }
            }
            dataAssetBook = dataAssetBook.Where(p => p.OriginalPrice1 + p.Depreciation + p.OriginalPrice2 + p.OriginalPriceIncreased
                                                   + p.OriginalPriceReduced + p.ImpoverishmentIncrease + p.ImpoverishmentReduced != 0).ToList();
            var dataAssetBookGroup = (from a in dataAssetBook
                                      join b in lstAssetToolGroup on a.AssetGroupCode equals b.Code
                                      group new { a, b } by new
                                     {
                                         a.OrgCode,
                                         Id = b?.Id ?? "",
                                         Rank = b?.Rank ?? 0,
                                         OrdGroup = b?.OrdGroup ?? "",
                                         AssetGroupCode = b?.Code ?? "",
                                         AssetGroupName = b?.Name ?? "",
                                     } into gr
                                     select new AssetBookDto
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         AssetToolRank = gr.Key.Rank,
                                         AssetToolOrd = gr.Key.OrdGroup,
                                         AssetToolCode = gr.Key.AssetGroupCode,
                                         AssetToolName = gr.Key.AssetGroupName,
                                         AssetGroupCode = gr.Key.AssetGroupCode,
                                         Bold = "C",
                                         OriginalPrice1 = gr.Sum(p => p.a.OriginalPrice1 ?? 0),
                                         ImpoverishmentPrice1 = gr.Sum(p => p.a.ImpoverishmentPrice1 ?? 0),
                                         RemainingPrice1 = gr.Sum(p => p.a.RemainingPrice1 ?? 0),
                                         Depreciation = gr.Sum(p => p.a.Depreciation ?? 0),
                                         DepreciationAmount = gr.Sum(p => p.a.DepreciationAmount ?? 0),
                                         OriginalPriceIncreased = gr.Sum(p => p.a.OriginalPriceIncreased ?? 0),
                                         OriginalPriceReduced = gr.Sum(p => p.a.OriginalPriceReduced ?? 0),
                                         ImpoverishmentIncrease = gr.Sum(p => p.a.ImpoverishmentIncrease ?? 0),
                                         ImpoverishmentReduced = gr.Sum(p => p.a.ImpoverishmentReduced ?? 0),
                                         DepreciationUpAmount = gr.Sum(p => p.a.DepreciationUpAmount ?? 0),
                                         DepreciationDownAmount = gr.Sum(p => p.a.DepreciationDownAmount ?? 0),
                                         OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2 ?? 0),
                                         ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2 ?? 0),
                                         RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2 ?? 0),
                                     }).ToList();
            var dataAssetBookGroupClone = dataAssetBookGroup.Select(p => ObjectMapper.Map<AssetBookDto, AssetBookDto>(p)).ToList();
            dataAssetBook.AddRange(dataAssetBookGroupClone);
            var rankMin = 1;
            var rankMax = (dataAssetBookGroup.Count > 0) ? dataAssetBookGroup.Max(p => p.AssetToolRank) : 0;
            while (rankMax > rankMin)
            {
                foreach (var itemAssetBookGroup in dataAssetBookGroup)
                {
                    if (itemAssetBookGroup.AssetToolRank == rankMax)
                    {
                        var partnerGroupId = lstAssetToolGroup.Where(p => p.Code == itemAssetBookGroup.AssetGroupCode)
                                                                 .Select(p => p.ParentId).FirstOrDefault();
                        itemAssetBookGroup.AssetGroupCode = lstAssetToolGroup.Where(p => p.Id == partnerGroupId).Select(p => p.Code).FirstOrDefault();
                        itemAssetBookGroup.AssetToolRank--;
                    }
                }
                var itemAssetBookGroupAdd = (from a in dataAssetBookGroup
                                             join b in lstAssetToolGroup on a.AssetGroupCode equals b.Code into ajb
                                             from b in ajb.DefaultIfEmpty()
                                             where a.AssetToolRank == rankMax - 1 && !dataAssetBook.Select(p => p.AssetGroupCode).Contains(a.AssetGroupCode)
                                             group new { a, b } by new
                                             {
                                                 a.OrgCode,
                                                 Id = b?.Id ?? "",
                                                 Rank = b?.Rank ?? 0,
                                                 OrdGroup = b?.OrdGroup ?? "",
                                                 AssetGroupCode = b?.Code ?? "",
                                                 AssetGroupName = b?.Name ?? "",
                                             } into gr
                                             select new AssetBookDto
                                             {
                                                 OrgCode = gr.Key.OrgCode,
                                                 AssetToolRank = gr.Key.Rank,
                                                 AssetToolOrd = gr.Key.OrdGroup,
                                                 AssetToolCode = gr.Key.AssetGroupCode,
                                                 AssetToolName = gr.Key.AssetGroupName,
                                                 Bold = "C",
                                                 OriginalPrice1 = gr.Sum(p => p.a.OriginalPrice1),
                                                 ImpoverishmentPrice1 = gr.Sum(p => p.a.ImpoverishmentPrice1),
                                                 RemainingPrice1 = gr.Sum(p => p.a.RemainingPrice1),
                                                 Depreciation = gr.Sum(p => p.a.Depreciation ?? 0),
                                                 DepreciationAmount = gr.Sum(p => p.a.DepreciationAmount ?? 0),
                                                 OriginalPriceIncreased = gr.Sum(p => p.a.OriginalPriceIncreased),
                                                 OriginalPriceReduced = gr.Sum(p => p.a.OriginalPriceReduced),
                                                 ImpoverishmentIncrease = gr.Sum(p => p.a.ImpoverishmentIncrease),
                                                 ImpoverishmentReduced = gr.Sum(p => p.a.ImpoverishmentReduced),
                                                 DepreciationUpAmount = gr.Sum(p => p.a.DepreciationUpAmount),
                                                 DepreciationDownAmount = gr.Sum(p => p.a.DepreciationDownAmount),
                                                 OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2),
                                                 ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2 ?? 0),
                                                 RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2),
                                             }).ToList();
                dataAssetBook.AddRange(itemAssetBookGroupAdd);
                rankMax--;
            }
            foreach (var item in dataAssetBook)
            {
                var space = "";
                for (var i = 1; i < item.AssetToolRank; i++)
                {
                    space += "  ";
                }
                item.AssetToolName = space + item.AssetToolName;
                if ((item.AssetGroupCode ?? "") != "")
                {
                    item.AssetToolOrd = lstAssetToolGroup.Where(p => p.Code == item.AssetGroupCode).Select(p => p.OrdGroup).FirstOrDefault();
                }
                else
                {
                    item.AssetToolOrd = lstAssetToolGroup.Where(p => p.Code == item.AssetToolCode).Select(p => p.OrdGroup).FirstOrDefault();
                }

                item.OriginalPrice1 = item.OriginalPrice1 == 0 ? null : item.OriginalPrice1;
                item.ImpoverishmentPrice1 = item.ImpoverishmentPrice1 == 0 ? null : item.ImpoverishmentPrice1;
                item.RemainingPrice1 = item.RemainingPrice1 == 0 ? null : item.RemainingPrice1;
                item.Depreciation = item.Depreciation == 0 ? null : item.Depreciation;
                item.OriginalPriceIncreased = item.OriginalPriceIncreased == 0 ? null : item.OriginalPriceIncreased;
                item.OriginalPriceReduced = item.OriginalPriceReduced == 0 ? null : item.OriginalPriceReduced;
                item.ImpoverishmentIncrease = item.ImpoverishmentIncrease == 0 ? null : item.ImpoverishmentIncrease;
                item.ImpoverishmentReduced = item.ImpoverishmentReduced == 0 ? null : item.ImpoverishmentReduced;
                item.DepreciationUpAmount = item.DepreciationUpAmount == 0 ? null : item.DepreciationUpAmount;
                item.DepreciationDownAmount = item.DepreciationDownAmount == 0 ? null : item.DepreciationDownAmount;
                item.OriginalPrice2 = item.OriginalPrice2 == 0 ? null : item.OriginalPrice2;
                item.ImpoverishmentPrice2 = item.ImpoverishmentPrice2 == 0 ? null : item.ImpoverishmentPrice2;
                item.RemainingPrice2 = item.RemainingPrice2 == 0 ? null : item.RemainingPrice2;
            }
            // thêm dòng tổng
            dataAssetBook.AddRange((from a in dataAssetBookGroup
                                    group new { a } by new
                                    {
                                        a.OrgCode
                                    } into gr
                                    select new AssetBookDto
                                    {
                                        OrgCode = gr.Key.OrgCode,
                                        AssetToolRank = 1,
                                        AssetToolOrd = "zzzzz",
                                        AssetToolCode = "",
                                        AssetToolName = "Tổng cộng",
                                        Bold = "C",
                                        OriginalPrice1 = gr.Sum(p => p.a.OriginalPrice1),
                                        ImpoverishmentPrice1 = gr.Sum(p => p.a.ImpoverishmentPrice1),
                                        RemainingPrice1 = gr.Sum(p => p.a.RemainingPrice1),
                                        Depreciation = gr.Sum(p => p.a.Depreciation),
                                        OriginalPriceIncreased = gr.Sum(p => p.a.OriginalPriceIncreased),
                                        OriginalPriceReduced = gr.Sum(p => p.a.OriginalPriceReduced),
                                        ImpoverishmentIncrease = gr.Sum(p => p.a.ImpoverishmentIncrease),
                                        ImpoverishmentReduced = gr.Sum(p => p.a.ImpoverishmentReduced),
                                        DepreciationUpAmount = gr.Sum(p => p.a.DepreciationUpAmount),
                                        DepreciationDownAmount = gr.Sum(p => p.a.DepreciationDownAmount),
                                        OriginalPrice2 = gr.Sum(p => p.a.OriginalPrice2),
                                        ImpoverishmentPrice2 = gr.Sum(p => p.a.ImpoverishmentPrice2),
                                        RemainingPrice2 = gr.Sum(p => p.a.RemainingPrice2),
                                    }).ToList());
            var reportResponse = new ReportResponseDto<AssetBookDto>();
            reportResponse.Data = dataAssetBook.OrderBy(p => p.AssetToolOrd).ThenBy(p => p.AssetToolRank).ThenBy(p => p.AssetToolCode).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<AssetBookParameterDto> dto)
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
