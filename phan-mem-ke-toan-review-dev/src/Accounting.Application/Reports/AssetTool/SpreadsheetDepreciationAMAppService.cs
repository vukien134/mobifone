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
    public class SpreadsheetDepreciationAMAppService : AccountingAppService
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
        public SpreadsheetDepreciationAMAppService(ReportDataService reportDataService,
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
        public async Task<ReportResponseDto<SpreadsheetDepreciationAMDto>> CreateDataAsync(ReportRequestDto<SpreadsheetDepreciationParameterAMDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, _webHelper.GetCurrentYear());
            // get list AssetToolGroup - danh mục nhóm ts ccdc đã xử lý rank và stt nhóm
            var lstAssetToolGroup = await _assetGroupAppService.GetRankGroup("");
            lstAssetToolGroup = lstAssetToolGroup.Where(p => p.AssetOrTool == dto.Parameters.AssetOrTool).ToList();
            string ordGroupAssetTool = "";
            if (!string.IsNullOrEmpty(dto.Parameters.AssetGroupCode))
            {
                ordGroupAssetTool = lstAssetToolGroup.Where(p => p.Id == dto.Parameters.AssetGroupCode).Select(p => p.OrdGroup).First();
            }

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
            var lstAssetToolDepreciation = assetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() 
                                                                         && p.Year == _webHelper.GetCurrentYear()).ToList();

            // get list cttscd group
            var lstAssetToolDetailGroup = (from a in lstAssetToolDetail
                                           join b in lstReason on a.UpDownCode equals b.Code
                                           where a.UpDownDate <= yearCategory.EndDate
                                           group new { a, b } by new
                                           {
                                               a.OrgCode,
                                               a.AssetToolId
                                           } into gr
                                           select new SpreadsheetDepreciationAMDto
                                           {
                                               OrgCode = gr.Key.OrgCode,
                                               AssetToolId = gr.Key.AssetToolId,
                                               DepartmentCode = gr.Max(p => p.a.DepartmentCode),
                                               OriginalPrice = gr.Sum(p => (p.a.OriginalPrice ?? 0) * (p.b.ReasonType == "T" ? 1 : -1)),
                                           }).ToList();
            // get list ctkhts group
            var lstAssetToolDepreciationGroup = (from a in lstAssetToolDepreciation
                                                 group new { a } by new
                                                 {
                                                     a.OrgCode,
                                                     a.AssetToolId
                                                 } into gr
                                                 select new SpreadsheetDepreciationAMDto
                                                 {
                                                     OrgCode = gr.Key.OrgCode,
                                                     AssetToolId = gr.Key.AssetToolId,
                                                     DepreciationAmount = gr.Sum(p => (p.a.DepreciationAmount ?? 0)),
                                                     DepreciationAmount01 = gr.Sum(p => (p.a.Month == 1)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount02 = gr.Sum(p => (p.a.Month == 2)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount03 = gr.Sum(p => (p.a.Month == 3)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount04 = gr.Sum(p => (p.a.Month == 4)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount05 = gr.Sum(p => (p.a.Month == 5)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount06 = gr.Sum(p => (p.a.Month == 6)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount07 = gr.Sum(p => (p.a.Month == 7)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount08 = gr.Sum(p => (p.a.Month == 8)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount09 = gr.Sum(p => (p.a.Month == 9)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount10 = gr.Sum(p => (p.a.Month == 10)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount11 = gr.Sum(p => (p.a.Month == 11)? (p.a.DepreciationAmount ?? 0) : 0),
                                                     DepreciationAmount12 = gr.Sum(p => (p.a.Month == 12)? (p.a.DepreciationAmount ?? 0) : 0),
                                                 }).ToList();

            var dataSpreadsheetDepreciation = (from a in lstAssetTool
                                 join d in lstAssetToolDetailGroup on a.Id equals d.AssetToolId into ajd
                                 from d in ajd.DefaultIfEmpty()
                                 join b in lstAssetToolDepreciationGroup on a.Id equals b.AssetToolId into ajb
                                 from b in ajb.DefaultIfEmpty()
                                 join c in lstAssetToolGroup on a.AssetToolGroupId equals c.Id
                                 where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                    && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                    && ((dto.Parameters.AssetGroupCode ?? "") == "" || (c.OrdGroup ?? "").StartsWith(ordGroupAssetTool))
                                    && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                    && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                    && (a.ReduceDate == null || a.ReduceDate >= yearCategory.BeginDate)
                                 group new { a, b, c, d } by new
                                 {
                                     a.Code,
                                     DepartmentCode = d?.DepartmentCode ?? ""
                                 } into gr
                                 orderby gr.Key.Code
                                 select new SpreadsheetDepreciationAMDto
                                 {
                                     Bold = "K",
                                     OrgCode = _webHelper.GetCurrentOrgUnit(),
                                     AssetToolCode = gr.Key.Code,
                                     DepartmentCode = gr.Key.DepartmentCode,
                                     AssetToolName = gr.Max(p => p.a.Name),
                                     AssetGroupCode = gr.Max(p => p.c.Code),
                                     AssetToolRank = gr.Max(p => p.c.Rank + 1),
                                     OriginalPrice = gr.Sum(p => p.d.OriginalPrice ?? 0),
                                     DepreciationRate = gr.Sum(p => (p.d.OriginalPrice ?? 0) != 0 ? (p.d.DepreciationAmount ?? 0/p.d.OriginalPrice)*100 : 0),
                                     DepreciationAmount = gr.Sum(p => (p.b?.DepreciationAmount ?? 0)),
                                     DepreciationAmount01 = gr.Sum(p => (p.b?.DepreciationAmount01 ?? 0)),
                                     DepreciationAmount02 = gr.Sum(p => (p.b?.DepreciationAmount02 ?? 0)),
                                     DepreciationAmount03 = gr.Sum(p => (p.b?.DepreciationAmount03 ?? 0)),
                                     DepreciationAmount04 = gr.Sum(p => (p.b?.DepreciationAmount04 ?? 0)),
                                     DepreciationAmount05 = gr.Sum(p => (p.b?.DepreciationAmount05 ?? 0)),
                                     DepreciationAmount06 = gr.Sum(p => (p.b?.DepreciationAmount06 ?? 0)),
                                     DepreciationAmount07 = gr.Sum(p => (p.b?.DepreciationAmount07 ?? 0)),
                                     DepreciationAmount08 = gr.Sum(p => (p.b?.DepreciationAmount08 ?? 0)),
                                     DepreciationAmount09 = gr.Sum(p => (p.b?.DepreciationAmount09 ?? 0)),
                                     DepreciationAmount10 = gr.Sum(p => (p.b?.DepreciationAmount10 ?? 0)),
                                     DepreciationAmount11 = gr.Sum(p => (p.b?.DepreciationAmount11 ?? 0)),
                                     DepreciationAmount12 = gr.Sum(p => (p.b?.DepreciationAmount12 ?? 0)),
                                     }).ToList();
            if((dto.Parameters.DepartmentCode ?? "") != "")
            {
                dataSpreadsheetDepreciation = dataSpreadsheetDepreciation.Where(p => p.DepartmentCode == dto.Parameters.DepartmentCode).ToList();
            }
            // Tạo cây dữ liệu
            var dataGroup = (from a in dataSpreadsheetDepreciation
                             join b in lstAssetToolGroup on a.AssetGroupCode equals b.Code into ajb
                             from b in ajb.DefaultIfEmpty()
                             group new { a, b } by new
                             {
                                 a.OrgCode,
                                 AssetToolGroupCode = b?.Code ?? "",
                                 AssetToolGroupName = b?.Name ?? "",
                                 Rank = b?.Rank ?? 0,
                             } into gr
                             select new SpreadsheetDepreciationAMDto
                             {
                                 Bold = "C",
                                 OrgCode = gr.Key.OrgCode,
                                 AssetToolCode = gr.Key.AssetToolGroupCode,
                                 AssetGroupCode = gr.Key.AssetToolGroupCode,
                                 AssetToolName = gr.Key.AssetToolGroupName,
                                 AssetToolRank = gr.Key.Rank,
                                 OriginalPrice = gr.Sum(p => p.a.OriginalPrice ?? 0),
                                 DepreciationRate = gr.Sum(p => (p.a.OriginalPrice ?? 0)) != 0 ? (gr.Sum(p => p.a.DepreciationAmount ?? 0) / gr.Sum(p => p.a.OriginalPrice)) * 100 : 0,
                                 DepreciationAmount = gr.Sum(p => (p.a.DepreciationAmount ?? 0)),
                                 DepreciationAmount01 = gr.Sum(p => (p.a.DepreciationAmount01 ?? 0)),
                                 DepreciationAmount02 = gr.Sum(p => (p.a.DepreciationAmount02 ?? 0)),
                                 DepreciationAmount03 = gr.Sum(p => (p.a.DepreciationAmount03 ?? 0)),
                                 DepreciationAmount04 = gr.Sum(p => (p.a.DepreciationAmount04 ?? 0)),
                                 DepreciationAmount05 = gr.Sum(p => (p.a.DepreciationAmount05 ?? 0)),
                                 DepreciationAmount06 = gr.Sum(p => (p.a.DepreciationAmount06 ?? 0)),
                                 DepreciationAmount07 = gr.Sum(p => (p.a.DepreciationAmount07 ?? 0)),
                                 DepreciationAmount08 = gr.Sum(p => (p.a.DepreciationAmount08 ?? 0)),
                                 DepreciationAmount09 = gr.Sum(p => (p.a.DepreciationAmount09 ?? 0)),
                                 DepreciationAmount10 = gr.Sum(p => (p.a.DepreciationAmount10 ?? 0)),
                                 DepreciationAmount11 = gr.Sum(p => (p.a.DepreciationAmount11 ?? 0)),
                                 DepreciationAmount12 = gr.Sum(p => (p.a.DepreciationAmount12 ?? 0)),
                             }).ToList();
            var dataGroupClone = dataGroup.Select(p => ObjectMapper.Map<SpreadsheetDepreciationAMDto, SpreadsheetDepreciationAMDto>(p)).ToList();
            dataSpreadsheetDepreciation.AddRange(dataGroupClone);
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
                                    where a.AssetToolRank == rankMax - 1 && !dataSpreadsheetDepreciation.Select(p => p.AssetGroupCode).Contains(a.AssetGroupCode)
                                    group new { a, b } by new
                                    {
                                        a.OrgCode,
                                        Id = b?.Id ?? "",
                                        Rank = b?.Rank ?? 0,
                                        OrdGroup = b?.OrdGroup ?? "",
                                        AssetToolGroupCode = b?.Code ?? "",
                                        AssetToolGroupName = b?.Name ?? "",
                                    } into gr
                                    select new SpreadsheetDepreciationAMDto
                                    {
                                        Bold = "C",
                                        OrgCode = gr.Key.OrgCode,
                                        AssetToolCode = gr.Key.AssetToolGroupCode,
                                        AssetToolName = gr.Key.AssetToolGroupName,
                                        AssetToolRank = gr.Key.Rank,
                                        AssetToolOrd = gr.Key.OrdGroup,
                                        OriginalPrice = gr.Sum(p => p.a.OriginalPrice ?? 0),
                                        DepreciationRate = gr.Sum(p => (p.a.DepreciationRate ?? 0)),
                                        DepreciationAmount = gr.Sum(p => (p.a.DepreciationAmount ?? 0)),
                                        DepreciationAmount01 = gr.Sum(p => (p.a.DepreciationAmount01 ?? 0)),
                                        DepreciationAmount02 = gr.Sum(p => (p.a.DepreciationAmount02 ?? 0)),
                                        DepreciationAmount03 = gr.Sum(p => (p.a.DepreciationAmount03 ?? 0)),
                                        DepreciationAmount04 = gr.Sum(p => (p.a.DepreciationAmount04 ?? 0)),
                                        DepreciationAmount05 = gr.Sum(p => (p.a.DepreciationAmount05 ?? 0)),
                                        DepreciationAmount06 = gr.Sum(p => (p.a.DepreciationAmount06 ?? 0)),
                                        DepreciationAmount07 = gr.Sum(p => (p.a.DepreciationAmount07 ?? 0)),
                                        DepreciationAmount08 = gr.Sum(p => (p.a.DepreciationAmount08 ?? 0)),
                                        DepreciationAmount09 = gr.Sum(p => (p.a.DepreciationAmount09 ?? 0)),
                                        DepreciationAmount10 = gr.Sum(p => (p.a.DepreciationAmount10 ?? 0)),
                                        DepreciationAmount11 = gr.Sum(p => (p.a.DepreciationAmount11 ?? 0)),
                                        DepreciationAmount12 = gr.Sum(p => (p.a.DepreciationAmount12 ?? 0)),
                                    }).ToList();
                dataSpreadsheetDepreciation.AddRange(itemGroupAdd);
                rankMax--;
            }
            foreach (var item in dataSpreadsheetDepreciation)
            {
                var space = "";
                for (var i = 1; i < item.AssetToolRank; i++)
                {
                    space += "  ";
                }
                item.AssetToolName = space + item.AssetToolName;
                item.AssetToolOrd = lstAssetToolGroup.Where(p => p.Code == item.AssetGroupCode).Select(p => p.OrdGroup).FirstOrDefault();

                item.OriginalPrice = item.OriginalPrice == 0 ? null : item.OriginalPrice;
                item.DepreciationAmount = item.DepreciationAmount == 0 ? null : item.DepreciationAmount;
                item.DepreciationAmount01 = item.DepreciationAmount01 == 0 ? null : item.DepreciationAmount01;
                item.DepreciationAmount02 = item.DepreciationAmount02 == 0 ? null : item.DepreciationAmount02;
                item.DepreciationAmount03 = item.DepreciationAmount03 == 0 ? null : item.DepreciationAmount03;
                item.DepreciationAmount04 = item.DepreciationAmount04 == 0 ? null : item.DepreciationAmount04;
                item.DepreciationAmount05 = item.DepreciationAmount05 == 0 ? null : item.DepreciationAmount05;
                item.DepreciationAmount06 = item.DepreciationAmount06 == 0 ? null : item.DepreciationAmount06;
                item.DepreciationAmount07 = item.DepreciationAmount07 == 0 ? null : item.DepreciationAmount07;
                item.DepreciationAmount08 = item.DepreciationAmount08 == 0 ? null : item.DepreciationAmount08;
                item.DepreciationAmount09 = item.DepreciationAmount09 == 0 ? null : item.DepreciationAmount09;
                item.DepreciationAmount10 = item.DepreciationAmount10 == 0 ? null : item.DepreciationAmount10;
                item.DepreciationAmount11 = item.DepreciationAmount11 == 0 ? null : item.DepreciationAmount11;
                item.DepreciationAmount12 = item.DepreciationAmount12 == 0 ? null : item.DepreciationAmount12;
            }
            var reportResponse = new ReportResponseDto<SpreadsheetDepreciationAMDto>();
            reportResponse.Data = dataSpreadsheetDepreciation.OrderBy(p => p.AssetToolOrd).ThenBy(p => p.AssetToolRank).ThenBy(p => p.AssetToolCode).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<SpreadsheetDepreciationParameterAMDto> dto)
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
