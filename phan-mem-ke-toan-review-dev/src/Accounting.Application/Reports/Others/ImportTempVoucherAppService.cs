using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Generals;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Accounting.Categories.Partners;
using Accounting.DomainServices.Reports;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Hosting;
using Accounting.Vouchers.AccVouchers;
using System.Linq;
using Accounting.DomainServices.Vouchers;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.Others
{
    public class ImportTempVoucherAppService : AccountingAppService
    {
        #region Fields

        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ICurrentTenant _currentTenant;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly AccVoucherService _accVoucherAppService;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ImportTempVoucherAppService(WebHelper webHelper,
                                            IUnitOfWorkManager unitOfWorkManager,
                                            OrgUnitService orgUnitService,
                                            YearCategoryService yearCategoryService,
                                            CircularsService circularsService,
                                            TenantSettingService tenantSettingService,
                                            ProductVoucherService productVoucherService,
                                            AccVoucherService accVoucherService,
                                            ReportTemplateService reportTemplateService,
                                            AccountingCacheManager accountingCacheManager
                )
        {
            _webHelper = webHelper;
            _unitOfWorkManager = unitOfWorkManager;
            _orgUnitService = orgUnitService;
            _yearCategoryService = yearCategoryService;
            _circularsService = circularsService;
            _tenantSettingService = tenantSettingService;
            _productVoucherService = productVoucherService;
            _accVoucherAppService = accVoucherService;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        [Authorize(ReportPermissions.ImportTempVoucherReportView)]
        public async Task<ReportResponseDto<ImportTempVoucherDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var producVoucher = await _productVoucherService.GetQueryableAsync();
            producVoucher = producVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                        && p.VoucherDate >= dto.Parameters.FromDate 
                                                        && p.VoucherDate <= dto.Parameters.ToDate);
            if (!string.IsNullOrEmpty(dto.Parameters.Status))
            {
                producVoucher = producVoucher.Where(p => p.Status.Equals(dto.Parameters.Status));
            }
            if (!string.IsNullOrEmpty(dto.Parameters.LstVoucherCode))
            {
                producVoucher = producVoucher.Where(p => dto.Parameters.LstVoucherCode.Contains(p.VoucherCode));                
            }

            var lstProducVoucher = await AsyncExecuter.ToListAsync(producVoucher);

            var accVoucher = await _accVoucherAppService.GetQueryableAsync();
            accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                 && p.VoucherDate >= dto.Parameters.FromDate
                                                 && p.VoucherDate <= dto.Parameters.ToDate);
            if (!string.IsNullOrEmpty(dto.Parameters.Status))
            {
                accVoucher = accVoucher.Where(p => p.Status.Equals(dto.Parameters.Status));
            }
            if (!string.IsNullOrEmpty(dto.Parameters.LstVoucherCode))
            {
                accVoucher = accVoucher.Where(p => dto.Parameters.LstVoucherCode.Contains(p.VoucherCode));                
            }

            var lstAccVoucher = await AsyncExecuter.ToListAsync(accVoucher);

            var result = (from a in lstAccVoucher
                          select new ImportTempVoucherDto
                          {
                              DocId = a.Id,
                              VoucherId = a.Id,
                              VoucherCode = a.VoucherCode,
                              VoucherDate = a.VoucherDate,
                              VoucherNumber = a.VoucherNumber,
                              Description = a.Description,
                              TotalAmount = a.TotalAmount,
                              TotalAmountCur = a.TotalAmountCur,
                              Status = a.Status == "2" ? "Chưa ghi sổ" : "Đã ghi sổ"
                          }).ToList();
            var result2 = (from a in lstProducVoucher
                           select new ImportTempVoucherDto
                           {
                               DocId = a.Id,
                               VoucherId = a.Id,
                               VoucherCode = a.VoucherCode,
                               VoucherDate = a.VoucherDate,
                               VoucherNumber = a.VoucherNumber,
                               Description = a.Description,
                               TotalAmount = a.TotalAmount,
                               TotalAmountCur = a.TotalAmountCur,
                               Status = a.Status == "2" ? "Chưa ghi sổ" : "Đã ghi sổ"
                           }).ToList();
            result.AddRange(result2);
            result = result.OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                            .ToList();
            var reportResponse = new ReportResponseDto<ImportTempVoucherDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return ObjectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
        }
        [Authorize(ReportPermissions.ImportTempVoucherReportPrint)]
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
        private List<FormularDto> GetFormular(string formular)
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
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, FolderConst.Report,
                                        templateFile);
            return filePath;
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
        private async Task<string> GetPartnerName(string orgCode, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var partner = await _accPartnerService.GetAccPartnerByCodeAsync(code, orgCode);
            if (partner == null) return null;

            return partner.Name;
        }
    }
}

