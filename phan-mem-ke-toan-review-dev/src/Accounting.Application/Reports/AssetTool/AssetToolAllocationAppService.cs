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
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.ImportExports.Parameters;
using Microsoft.AspNetCore.Authorization;
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
    public class AssetToolAllocationAppService : AccountingAppService
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
        private readonly DepartmentService _departmentService;
        private readonly CapitalService _capitalService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccSectionService _accSectionService;
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
        public AssetToolAllocationAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        AssetGroupAppService assetGroupAppService,
                        AssetToolAccountService assetToolAccountService,
                        DepartmentService departmentService,
                        CapitalService capitalService,
                        FProductWorkService fProductWorkService,
                        AccSectionService accSectionService,
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
            _departmentService = departmentService;
            _capitalService = capitalService;
            _fProductWorkService = fProductWorkService;
            _accSectionService = accSectionService;
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
        public async Task<ReportResponseDto<AssetToolAllocationDto>> CreateDataAsync(ReportRequestDto<AssetToolAllocationParameterDto> dto)
         {
            var department = await _departmentService.GetQueryableAsync();
            var lstDepartment = department.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var capital = await _capitalService.GetQueryableAsync();
            var lstCapital = capital.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            var accPartner = await _accPartnerService.GetQueryableAsync();
            var lstAccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var accSection = await _accSectionService.GetQueryableAsync();
            var lstAccSection = accSection.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstFProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // get list AssetTool - đầu phiếu ts, ccdc
            var assetTool = await _assetToolService.GetQueryableAsync();
            var lstAssetTool = assetTool.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetToolDetail - chi tiết ts, ccdc
            var assetToolDetail = await _assetToolDetailService.GetQueryableAsync();
            var lstAssetToolDetail = assetToolDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetToolGroup - danh mục nhóm ts ccdc đã xử lý rank và stt nhóm
            var lstAssetToolGroup = await _assetGroupAppService.GetRankGroup("");
            lstAssetToolGroup = lstAssetToolGroup.Where(p => p.AssetOrTool == dto.Parameters.AssetOrTool).ToList();
            string ordGroupAssetTool = "";
            if (!string.IsNullOrEmpty(dto.Parameters.AssetGroupCode))
            {
                ordGroupAssetTool = lstAssetToolGroup.Where(p => p.Id == dto.Parameters.AssetGroupCode).Select(p => p.OrdGroup).First();
            }
            // get list AssetToolDepreciation - chi tiết khấu hao ts
            var assetToolDepreciation = await _assetToolDepreciationService.GetQueryableAsync();
            var lstAssetToolDepreciation = assetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                         && p.AssetOrTool == dto.Parameters.AssetOrTool).ToList();

            

            var lst = new List<AssetToolAllocationDto>();
            switch (dto.Parameters.Type)
            {
                case 1: // Department
                    // get list ctkhts group theo DepartmentCode
                    var lstGroupDepartmentCode = (from a in lstAssetToolDepreciation
                                                         join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                                         where a.DepreciationDate >= dto.Parameters.FromDate && a.DepreciationDate <= dto.Parameters.ToDate
                                                         group new { a, b } by new
                                                         {
                                                             a.OrgCode,
                                                             a.AssetToolId,
                                                             b.DepartmentCode,          
                                                             a.DebitAcc,
                                                             a.CreditAcc,
                                                         } into gr
                                                         select new AssetToolAllocationDto
                                                         {
                                                             OrgCode = gr.Key.OrgCode,
                                                             AssetToolId = gr.Key.AssetToolId,
                                                             Code = gr.Key.DepartmentCode,
                                                             DebitAcc = gr.Key.DebitAcc,
                                                             CreditAcc = gr.Key.CreditAcc,
                                                             ImpoverishmentPrice = gr.Sum(p => p.a.DepreciationAmount ?? 0)
                                                         }).ToList();

                    lst.AddRange((from a in lstAssetTool
                                  join b in lstGroupDepartmentCode on a.Id equals b.AssetToolId
                                  join c in lstDepartment on b.Code equals c.Code into bjc 
                                  from c in bjc.DefaultIfEmpty()
                                  join d in lstAccountSystem on b.DebitAcc equals d.AccCode into bjd
                                  from d in bjd.DefaultIfEmpty()
                                  join e in lstAccountSystem on b.CreditAcc equals e.AccCode into bje
                                  from e in bje.DefaultIfEmpty()
                                  where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                     && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                     && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Select(p => p.Id).Contains(a.AssetToolGroupId))
                                     && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                     && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                  group new { a, b, c, d, e } by new
                                  {
                                      a.Id,
                                      AssetToolCode = a.Code,
                                      AssetToolName = a.Name,
                                      Code = c?.Code ?? "",
                                      Name = c?.Name ?? "",
                                      b.DebitAcc,
                                      b.CreditAcc,
                                      DebitAccName = d?.AccName ?? "",
                                      CreditAccName = e?.AccName ?? "",
                                  } into gr
                                  orderby gr.Key.AssetToolCode, gr.Key.AssetToolName, gr.Key.Code, gr.Key.DebitAcc, gr.Key.CreditAcc
                                  select new AssetToolAllocationDto
                                  {
                                      Bold = "K",
                                      Sort = 2,
                                      Id = gr.Key.Id,
                                      AssetToolCode = gr.Key.AssetToolCode,
                                      AssetToolName = gr.Key.AssetToolName,
                                      Code = gr.Key.Code,
                                      Name = gr.Key.Name,
                                      DebitAcc = gr.Key.DebitAcc,
                                      CreditAcc = gr.Key.CreditAcc,
                                      DebitAccName = gr.Key.DebitAccName,
                                      CreditAccName = gr.Key.CreditAccName,
                                      ImpoverishmentPrice = gr.Sum(p => p.b.ImpoverishmentPrice ?? 0)
                                  }).ToList());
                    break;
                case 2:// Capital
                    // get list ctkhts group theo Capital
                    var lstGroupCapital = (from a in lstAssetToolDepreciation
                                            join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                            where a.DepreciationDate >= dto.Parameters.FromDate && a.DepreciationDate <= dto.Parameters.FromDate
                                            group new { a, b } by new
                                            {
                                                a.OrgCode,
                                                a.AssetToolId,
                                                b.CapitalCode,
                                                a.DebitAcc,
                                                a.CreditAcc,
                                            } into gr
                                            select new AssetToolAllocationDto
                                            {
                                                OrgCode = gr.Key.OrgCode,
                                                AssetToolId = gr.Key.AssetToolId,
                                                Code = gr.Key.CapitalCode,
                                                DebitAcc = gr.Key.DebitAcc,
                                                CreditAcc = gr.Key.CreditAcc,
                                                ImpoverishmentPrice = gr.Sum(p => p.b.Impoverishment ?? 0)
                                            }).ToList();

                    lst.AddRange((from a in lstAssetTool
                                  join b in lstGroupCapital on a.Id equals b.AssetToolId
                                  join c in lstCapital on b.Code equals c.Code into bjc
                                  from c in bjc.DefaultIfEmpty()
                                  join d in lstAccountSystem on b.DebitAcc equals d.AccCode into bjd
                                  from d in bjd.DefaultIfEmpty()
                                  join e in lstAccountSystem on b.CreditAcc equals e.AccCode into bje
                                  from e in bje.DefaultIfEmpty()
                                  where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                     && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                     && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Select(p => p.Id).Contains(a.AssetToolGroupId))
                                     && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                     && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                  group new { a, b, c, d, e } by new
                                  {
                                      a.Id,
                                      AssetToolCode = a.Code,
                                      AssetToolName = a.Name,
                                      Code = c.Code ?? "",
                                      Name = c.Name ?? "",
                                      b.DebitAcc,
                                      b.CreditAcc,
                                      DebitAccName = d.AccName ?? "",
                                      CreditAccName = e.AccName ?? "",
                                  } into gr
                                  orderby gr.Key.AssetToolCode, gr.Key.AssetToolName, gr.Key.Code, gr.Key.DebitAcc, gr.Key.CreditAcc
                                  select new AssetToolAllocationDto
                                  {
                                      Bold = "K",
                                      Sort = 2,
                                      Id = gr.Key.Id,
                                      AssetToolCode = gr.Key.AssetToolCode,
                                      AssetToolName = gr.Key.AssetToolName,
                                      Code = gr.Key.Code,
                                      Name = gr.Key.Name,
                                      DebitAcc = gr.Key.DebitAcc,
                                      CreditAcc = gr.Key.CreditAcc,
                                      DebitAccName = gr.Key.DebitAccName,
                                      CreditAccName = gr.Key.CreditAccName,
                                      ImpoverishmentPrice = gr.Sum(p => p.b.ImpoverishmentPrice ?? 0)
                                  }).ToList());
                    break;
                case 3:// AccountSystem
                    // get list ctkhts group theo AccountSystem
                    var lstGroupAccountSystem = (from a in lstAssetToolDepreciation
                                           join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                           where a.DepreciationDate >= dto.Parameters.FromDate && a.DepreciationDate <= dto.Parameters.FromDate
                                           group new { a, b } by new
                                           {
                                               a.OrgCode,
                                               a.AssetToolId,
                                               a.DebitAcc,
                                               a.CreditAcc,
                                           } into gr
                                           select new AssetToolAllocationDto
                                           {
                                               OrgCode = gr.Key.OrgCode,
                                               AssetToolId = gr.Key.AssetToolId,
                                               DebitAcc = gr.Key.DebitAcc,
                                               CreditAcc = gr.Key.CreditAcc,
                                               ImpoverishmentPrice = gr.Sum(p => p.b.Impoverishment ?? 0)
                                           }).ToList();

                    lst.AddRange((from a in lstAssetTool
                                  join b in lstGroupAccountSystem on a.Id equals b.AssetToolId
                                  join c in lstAccountSystem on b.DebitAcc equals c.AccCode into bjc
                                  from c in bjc.DefaultIfEmpty()
                                  join d in lstAccountSystem on b.DebitAcc equals d.AccCode into bjd
                                  from d in bjd.DefaultIfEmpty()
                                  join e in lstAccountSystem on b.CreditAcc equals e.AccCode into bje
                                  from e in bje.DefaultIfEmpty()
                                  where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                     && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                     && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Select(p => p.Id).Contains(a.AssetToolGroupId))
                                     && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                     && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                  group new { a, b, c, d, e } by new
                                  {
                                      a.Id,
                                      AssetToolCode = a.Code,
                                      AssetToolName = a.Name,
                                      Code = c.AccCode ?? "",
                                      Name = c.AccName ?? "",
                                      b.DebitAcc,
                                      b.CreditAcc,
                                      DebitAccName = d.AccName ?? "",
                                      CreditAccName = e.AccName ?? "",
                                  } into gr
                                  orderby gr.Key.AssetToolCode, gr.Key.AssetToolName, gr.Key.Code, gr.Key.DebitAcc, gr.Key.CreditAcc
                                  select new AssetToolAllocationDto
                                  {
                                      Bold = "K",
                                      Sort = 2,
                                      Id = gr.Key.Id,
                                      AssetToolCode = gr.Key.AssetToolCode,
                                      AssetToolName = gr.Key.AssetToolName,
                                      Code = gr.Key.Code,
                                      Name = gr.Key.Name,
                                      DebitAcc = gr.Key.DebitAcc,
                                      CreditAcc = gr.Key.CreditAcc,
                                      DebitAccName = gr.Key.DebitAccName,
                                      CreditAccName = gr.Key.CreditAccName,
                                      ImpoverishmentPrice = gr.Sum(p => p.b.ImpoverishmentPrice ?? 0)
                                  }).ToList());
                    break;
                case 4:// Partner
                    // get list ctkhts group theo Partner
                    var lstGroupPartner = (from a in lstAssetToolDepreciation
                                                 join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                                 where a.DepreciationDate >= dto.Parameters.FromDate && a.DepreciationDate <= dto.Parameters.FromDate
                                                 group new { a, b } by new
                                                 {
                                                     a.OrgCode,
                                                     a.AssetToolId,
                                                     b.PartnerCode,
                                                     a.DebitAcc,
                                                     a.CreditAcc,
                                                 } into gr
                                                 select new AssetToolAllocationDto
                                                 {
                                                     OrgCode = gr.Key.OrgCode,
                                                     AssetToolId = gr.Key.AssetToolId,
                                                     Code = gr.Key.PartnerCode,
                                                     DebitAcc = gr.Key.DebitAcc,
                                                     CreditAcc = gr.Key.CreditAcc,
                                                     ImpoverishmentPrice = gr.Sum(p => p.b.Impoverishment ?? 0)
                                                 }).ToList();

                    lst.AddRange((from a in lstAssetTool
                                  join b in lstGroupPartner on a.Id equals b.AssetToolId
                                  join c in lstAccPartner on b.Code equals c.Code into bjc
                                  from c in bjc.DefaultIfEmpty()
                                  join d in lstAccountSystem on b.DebitAcc equals d.AccCode into bjd
                                  from d in bjd.DefaultIfEmpty()
                                  join e in lstAccountSystem on b.DebitAcc equals e.AccCode into bje
                                  from e in bje.DefaultIfEmpty()
                                  where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                     && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                     && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Select(p => p.Id).Contains(a.AssetToolGroupId))
                                     && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                     && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                  group new { a, b, c, d, e } by new
                                  {
                                      a.Id,
                                      AssetToolCode = a.Code,
                                      AssetToolName = a.Name,
                                      Code = c.Code ?? "",
                                      Name = c.Name ?? "",
                                      b.DebitAcc,
                                      b.CreditAcc,
                                      DebitAccName = d.AccName ?? "",
                                      CreditAccName = e.AccName ?? "",
                                  } into gr
                                  orderby gr.Key.AssetToolCode, gr.Key.AssetToolName, gr.Key.Code, gr.Key.DebitAcc, gr.Key.CreditAcc
                                  select new AssetToolAllocationDto
                                  {
                                      Bold = "K",
                                      Sort = 2,
                                      Id = gr.Key.Id,
                                      AssetToolCode = gr.Key.AssetToolCode,
                                      AssetToolName = gr.Key.AssetToolName,
                                      Code = gr.Key.Code,
                                      Name = gr.Key.Name,
                                      DebitAcc = gr.Key.DebitAcc,
                                      CreditAcc = gr.Key.CreditAcc,
                                      DebitAccName = gr.Key.DebitAccName,
                                      CreditAccName = gr.Key.CreditAccName,
                                      ImpoverishmentPrice = gr.Sum(p => p.b.ImpoverishmentPrice ?? 0)
                                  }).ToList());
                    break;
                case 5:// AccSection
                    // get list ctkhts group theo AccSection
                    var lstGroupAccSection = (from a in lstAssetToolDepreciation
                                           join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                           where a.DepreciationDate >= dto.Parameters.FromDate && a.DepreciationDate <= dto.Parameters.FromDate
                                           group new { a, b } by new
                                           {
                                               a.OrgCode,
                                               a.AssetToolId,
                                               b.SectionCode,
                                               a.DebitAcc,
                                               a.CreditAcc,
                                           } into gr
                                           select new AssetToolAllocationDto
                                           {
                                               OrgCode = gr.Key.OrgCode,
                                               AssetToolId = gr.Key.AssetToolId,
                                               Code = gr.Key.SectionCode,
                                               DebitAcc = gr.Key.DebitAcc,
                                               CreditAcc = gr.Key.CreditAcc,
                                               ImpoverishmentPrice = gr.Sum(p => p.b.Impoverishment ?? 0)
                                           }).ToList();

                    lst.AddRange((from a in lstAssetTool
                                  join b in lstGroupAccSection on a.Id equals b.AssetToolId
                                  join c in lstAccSection on b.Code equals c.Code into bjc
                                  from c in bjc.DefaultIfEmpty()
                                  join d in lstAccountSystem on b.DebitAcc equals d.AccCode into bjd
                                  from d in bjd.DefaultIfEmpty()
                                  join e in lstAccountSystem on b.DebitAcc equals e.AccCode into bje
                                  from e in bje.DefaultIfEmpty()
                                  where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                     && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                     && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Select(p => p.Id).Contains(a.AssetToolGroupId))
                                     && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                     && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                  group new { a, b, c, d, e } by new
                                  {
                                      a.Id,
                                      AssetToolCode = a.Code,
                                      AssetToolName = a.Name,
                                      Code = c.Code ?? "",
                                      Name = c.Name ?? "",
                                      b.DebitAcc,
                                      b.CreditAcc,
                                      DebitAccName = d.AccName ?? "",
                                      CreditAccName = e.AccName ?? "",
                                  } into gr
                                  orderby gr.Key.AssetToolCode, gr.Key.AssetToolName, gr.Key.Code, gr.Key.DebitAcc, gr.Key.CreditAcc
                                  select new AssetToolAllocationDto
                                  {
                                      Bold = "K",
                                      Sort = 2,
                                      Id = gr.Key.Id,
                                      AssetToolCode = gr.Key.AssetToolCode,
                                      AssetToolName = gr.Key.AssetToolName,
                                      Code = gr.Key.Code,
                                      Name = gr.Key.Name,
                                      DebitAcc = gr.Key.DebitAcc,
                                      CreditAcc = gr.Key.CreditAcc,
                                      DebitAccName = gr.Key.DebitAccName,
                                      CreditAccName = gr.Key.CreditAccName,
                                      ImpoverishmentPrice = gr.Sum(p => p.b.ImpoverishmentPrice ?? 0)
                                  }).ToList());
                    break;
                default:// FProductWork
                    // get list ctkhts group theo FProductWork
                    var lstGroupFProductWork = (from a in lstAssetToolDepreciation
                                              join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                              where a.DepreciationDate >= dto.Parameters.FromDate && a.DepreciationDate <= dto.Parameters.FromDate
                                              group new { a, b } by new
                                              {
                                                  a.OrgCode,
                                                  a.AssetToolId,
                                                  b.FProductWorkCode,
                                                  a.DebitAcc,
                                                  a.CreditAcc,
                                              } into gr
                                              select new AssetToolAllocationDto
                                              {
                                                  OrgCode = gr.Key.OrgCode,
                                                  AssetToolId = gr.Key.AssetToolId,
                                                  Code = gr.Key.FProductWorkCode,
                                                  DebitAcc = gr.Key.DebitAcc,
                                                  CreditAcc = gr.Key.CreditAcc,
                                                  ImpoverishmentPrice = gr.Sum(p => p.b.Impoverishment ?? 0)
                                              }).ToList();

                    lst.AddRange((from a in lstAssetTool
                                  join b in lstGroupFProductWork on a.Id equals b.AssetToolId
                                  join c in lstFProductWork on b.Code equals c.Code into bjc
                                  from c in bjc.DefaultIfEmpty()
                                  join d in lstAccountSystem on b.DebitAcc equals d.AccCode into bjd
                                  from d in bjd.DefaultIfEmpty()
                                  join e in lstAccountSystem on b.DebitAcc equals e.AccCode into bje
                                  from e in bje.DefaultIfEmpty()
                                  where a.OrgCode == _webHelper.GetCurrentOrgUnit() && a.AssetOrTool == dto.Parameters.AssetOrTool
                                     && ((dto.Parameters.PurposeCode ?? "") == "" || a.PurposeCode == dto.Parameters.PurposeCode)
                                     && ((dto.Parameters.AssetGroupCode ?? "") == "" || lstAssetToolGroup.Select(p => p.Id).Contains(a.AssetToolGroupId))
                                     && ((dto.Parameters.AssetToolCode ?? "") == "" || a.Code == dto.Parameters.AssetToolCode)
                                     && ((dto.Parameters.AssetToolAcc ?? "") == "" || (a.AssetToolAcc ?? "").StartsWith(dto.Parameters.AssetToolAcc))
                                  group new { a, b, c, d, e } by new
                                  {
                                      a.Id,
                                      AssetToolCode = a.Code,
                                      AssetToolName = a.Name,
                                      Code = c.Code ?? "",
                                      Name = c.Name ?? "",
                                      b.DebitAcc,
                                      b.CreditAcc,
                                      DebitAccName = d.AccName ?? "",
                                      CreditAccName = e.AccName ?? "",
                                  } into gr
                                  orderby gr.Key.AssetToolCode, gr.Key.AssetToolName, gr.Key.Code, gr.Key.DebitAcc, gr.Key.CreditAcc
                                  select new AssetToolAllocationDto
                                  {
                                      Bold = "K",
                                      Sort = 2,
                                      Id = gr.Key.Id,
                                      AssetToolCode = gr.Key.AssetToolCode,
                                      AssetToolName = gr.Key.AssetToolName,
                                      Code = gr.Key.Code,
                                      Name = gr.Key.Name,
                                      DebitAcc = gr.Key.DebitAcc,
                                      CreditAcc = gr.Key.CreditAcc,
                                      DebitAccName = gr.Key.DebitAccName,
                                      CreditAccName = gr.Key.CreditAccName,
                                      ImpoverishmentPrice = gr.Sum(p => p.b.ImpoverishmentPrice ?? 0)
                                  }).ToList());
                    break;
            }
            lst.AddRange(lst.GroupBy(g => new {g.Code, g.Name}).OrderBy(p => p.Key.Code)
                            .Select(p => new AssetToolAllocationDto 
                            {
                                Bold = "C",
                                Sort = 1,
                                Code = p.Key.Code,
                                Name = p.Key.Name,
                                DebitAccName = p.Key.Code + " - " + p.Key.Name,
                                ImpoverishmentPrice = p.Sum(a => a.ImpoverishmentPrice)
                            }).ToList() );

            // Thêm dòng tổng
            var totalImpoverishmentPrice = lst.Where(p => p.Bold == "K").Sum(p => p.ImpoverishmentPrice);
            lst.Add(new AssetToolAllocationDto
                    {
                        Bold = "C",
                        Sort = 9999,
                        Code = "zzzz",
                        DebitAccName = "Tổng cộng",
                        ImpoverishmentPrice = totalImpoverishmentPrice
                    });

            var reportResponse = new ReportResponseDto<AssetToolAllocationDto>();
            reportResponse.Data = lst.OrderBy(p => p.Code).ThenBy(p => p.Sort).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<AssetToolAllocationParameterDto> dto)
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
