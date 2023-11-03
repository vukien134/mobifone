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
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Collections;
using System.Linq;
using Accounting.Categories.Partners;
using IdentityServer4.Extensions;
using System.Text.RegularExpressions;
using Accounting.Categories.Sections;
using NPOI.SS.Formula.Functions;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using Accounting.Caching;

namespace Accounting.Reports.GeneralDiaries
{
    public class ListOfVouchersBookAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly AccountSystemService _accountSystemService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccSectionService _accSectionService;
        private readonly DepartmentService _departmentService;
        private readonly AccCaseService _accCaseService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public ListOfVouchersBookAppService(ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccSectionService accSectionService,
                        DepartmentService departmentService,
                        AccCaseService accCaseService,
                        FProductWorkService fProductWorkService,
                        AccPartnerService accPartnerService,
                        PartnerGroupService partnerGroupService,
                        AccPartnerAppService accPartnerAppService,
                        AccountingCacheManager accountingCacheManager
            )
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accSectionService = accSectionService;
            _departmentService = departmentService;
            _accCaseService = accCaseService;
            _fProductWorkService = fProductWorkService;
            _accPartnerService = accPartnerService;
            _partnerGroupService = partnerGroupService;
            _accPartnerAppService = accPartnerAppService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.ListOfVouchersBookReportView)]
        public async Task<ReportResponseDto<ListOfVouchersBookdto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var incurredData = await GetIncurredData(dic);

            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var partnerGroup = await _partnerGroupService.GetQueryableAsync();
                var lst = partnerGroup.Where(p => p.Id == dto.Parameters.PartnerGroup).ToList();
                if (lst.Count > 0)
                {
                    var code = lst.FirstOrDefault().Code;
                    var partners = await _accPartnerAppService.GetListByPartnerGroupCode(code);
                    incurredData = incurredData.Where(p => partners.Select(p => p.Code).Contains(p.PartnerCode) == true).ToList();

                }
            }
            if (!string.IsNullOrEmpty(dto.Parameters.DepartmentCode))
            {
                var department = await _departmentService.GetQueryableAsync();
                var lstDepartment = department.Where(p => p.Id == dto.Parameters.DepartmentCode).ToList();
                if (lstDepartment.Count > 0)
                {
                    var departmentCode = lstDepartment.FirstOrDefault().Code;
                    incurredData = incurredData.Where(p => p.DepartmentCode == departmentCode).ToList();
                }
            }
            if (!string.IsNullOrEmpty(dto.Parameters.CreationUser))
            {
                incurredData = incurredData.Where(p => p.CreatorName == dto.Parameters.CreationUser).ToList();
            }

            if (!string.IsNullOrEmpty(dto.Parameters.CreationTime.ToString()))
            {
                incurredData = incurredData.Where(p => p.CreationTime == dto.Parameters.CreationTime).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = incurredData.Where(p => GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber) && GetVoucherNumber(dto.Parameters.EndVoucherNumber) <= GetVoucherNumber(p.VoucherNumber)).ToList();
            }
            var result = dto.Parameters.Sort switch
            {
                "1" => this.DataSortByDate(incurredData),
                "2" => this.DataSortByPartner(incurredData),
                "3" => await this.DataSortByAccCase(incurredData),
                "4" => await this.DataSortBySection(incurredData),
                "5" => await this.DataSortByDepartment(incurredData),
                "6" => await this.DataSortByFProductWork(incurredData),
                "7" => this.DataSortByCurrency(incurredData),
                "8" => await this.DataSortByAcc(incurredData),
                _ => this.DataSortByDate(incurredData)
            };
            var totalAmount = incurredData.Sum(p => p.Amount);
            var totalAmountCur = incurredData.Sum(p => p.AmountCur);
            result.Add(new ListOfVouchersBookdto()
            {
                Bold = "C",
                Note = "Tổng cộng",
                Amount = totalAmount,
                AmountCur = totalAmountCur
            });

            var reportResponse = new ReportResponseDto<ListOfVouchersBookdto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.ListOfVouchersBookReportPrint)]
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
        #region Private

        private decimal GetVoucherNumber(string VoucherNumber)
        {
            string[] numbers = Regex.Split(VoucherNumber, @"\D+");
            if (numbers.Length > 0)
            {
                return decimal.Parse(string.Join("", numbers));
            }
            else
            {
                return 0;
            }
        }
        private async Task<List<ListOfVouchersBookdto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var ledgerData = await GetDataledger(dic);
            var incurredData = ledgerData.Select(p => new ListOfVouchersBookdto()
            {
                OrgCode = p.OrgCode,
                Year = p.Year,
                VoucherNumber = p.VoucherNumber,
                VoucherDate = p.VoucherDate,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                ExchangeRate = p.ExchangeRate,
                Amount = p.Amount0,
                AmountCur = p.AmountCur0,
                VoucherId = p.VoucherId,
                Id = p.Id,
                VoucherCode = p.VoucherCode,
                CaseCode = p.CaseCode0,
                PartnerCode = string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode0 : p.PartnerCode,
                ClearingPartnerCode = p.ClearingPartnerCode,
                GroupCode = null,
                SectionCode = string.IsNullOrEmpty(p.SectionCode) ? p.SectionCode0 : p.SectionCode,
                FProductWorkCode = string.IsNullOrEmpty(p.FProductWorkCode) ? p.FProductWorkCode0 : p.FProductWorkCode,
                CurrencyCode = p.CurrencyCode,
                DepartmentCode = p.DepartmentCode,
                DebitIncurred = 0,
                CreditIncurred = 0,
                DebitIncurredCur = 0,
                CreditIncurredCur = 0,
                InvoiceDate = p.InvoiceDate,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceSymbol = p.InvoiceSymbol,
                DebitAmountCur = p.DebitAmountCur,
                CreditAmountCur = p.CreditAmountCur,
                CaseName = "",
                PartnerName = "",
                SectionName = "",
                DepartmentName = "",
                FProductWorkName = "",
                CurrencyName = "",
                AccName = "",
                CreationTime = p.CreationTime,
                CreatorName = p.CreatorName,
                ReciprocalAcc = p.ReciprocalAcc,
                Note = p.Note
            }).ToList();

            var partners = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partners.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var query = from p in incurredData
                        join d in lstPartner on p.PartnerCode equals d.Code into gj
                        from df in gj.DefaultIfEmpty()
                        select new ListOfVouchersBookdto()
                        {
                            Sort = 1,
                            Bold = "K",
                            OrgCode = p.OrgCode,
                            Year = p.Year,
                            VoucherNumber = p.VoucherNumber,
                            VoucherDate = p.VoucherDate,
                            DebitAcc = p.DebitAcc,
                            CreditAcc = p.CreditAcc,
                            ExchangeRate = p.ExchangeRate,
                            Amount = p.Amount,
                            AmountCur = p.AmountCur,
                            VoucherId = p.VoucherId,
                            Id = p.Id,
                            VoucherCode = p.VoucherCode,
                            CaseCode = this.DefaultNullIfEmpty(p.CaseCode),
                            PartnerCode = this.DefaultNullIfEmpty(p.PartnerCode),
                            ClearingPartnerCode = p.ClearingPartnerCode,
                            GroupCode = p.GroupCode,
                            SectionCode = this.DefaultNullIfEmpty(p.SectionCode),
                            FProductWorkCode = this.DefaultNullIfEmpty(p.FProductWorkCode),
                            CurrencyCode = this.DefaultNullIfEmpty(p.CurrencyCode),
                            DepartmentCode = this.DefaultNullIfEmpty(p.DepartmentCode),
                            InvoiceDate = p.InvoiceDate,
                            InvoiceNumber = p.InvoiceNumber,
                            InvoiceSymbol = p.InvoiceSymbol,
                            DebitIncurred = p.DebitIncurred,
                            CreditIncurred = p.CreditIncurred,
                            DebitIncurredCur = p.DebitIncurredCur,
                            CreditIncurredCur = p.CreditIncurredCur,
                            DebitAmountCur = p.DebitAmountCur,
                            CreditAmountCur = p.CreditAmountCur,
                            CaseName = p.CaseName,
                            PartnerName = df?.Name ?? null,
                            SectionName = p.SectionName,
                            DepartmentName = p.DepartmentName,
                            FProductWorkName = p.FProductWorkName,
                            CurrencyName = p.CurrencyName,
                            AccName = p.AccName,
                            ReciprocalAcc = p.ReciprocalAcc,
                            Note = p.Note,
                            PartnerGroupId = df?.PartnerGroupId ?? null,
                            CreatorName = p.CreatorName
                        };
            if (dic.ContainsKey(LedgerParameterConst.PartnerGroup))
            {
                string partnerGroupId = dic[LedgerParameterConst.PartnerGroup].ToString();
                query = query.Where(p => p.PartnerGroupId == partnerGroupId);
            }
            return query.ToList();
        }
        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }


        private Dictionary<string, object> GetLedgerParameter(ReportBaseParameterDto dto)
        {
            string debitOrCredit = dto.DebitCredit.Equals("*") ? dto.DebitCredit : "N";
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, debitOrCredit);
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccCode);

            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(LedgerParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            }
            if (!string.IsNullOrEmpty(dto.CaseCode))
            {
                dic.Add(LedgerParameterConst.CaseCode, dto.CaseCode);
            }
            if (!string.IsNullOrEmpty(dto.ReciprocalAcc))
            {
                dic.Add(LedgerParameterConst.ReciprocalAcc, dto.ReciprocalAcc);
            }

            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(LedgerParameterConst.PartnerCode, dto.PartnerCode);
            }
            if (!string.IsNullOrEmpty(dto.VoucherCode))
            {
                dic.Add(LedgerParameterConst.VoucherCode, dto.VoucherCode);
            }
            if (!string.IsNullOrEmpty(dto.Sort))
            {
                dic.Add(LedgerParameterConst.Sort, dto.Sort);
            }
            if (!string.IsNullOrEmpty(dto.LstVoucherCode))
            {
                dic.Add(LedgerParameterConst.LstVoucherCode, dto.LstVoucherCode);
            }

            if (!string.IsNullOrEmpty(dto.CreationUser))
            {
                dic.Add(LedgerParameterConst.CreationUser, dto.CreationUser);
            }

            if (!string.IsNullOrEmpty(dto.CreationTime.ToString()) != true)
            {
                dic.Add(LedgerParameterConst.CreationTime, dto.CreationTime);
            }
            return dic;
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
        private string DefaultNullIfEmpty(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return value;
        }
        private List<ListOfVouchersBookdto> DataSortByDate(List<ListOfVouchersBookdto> dtos)
        {
            dtos = dtos.OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                            .ToList();
            return dtos;
        }
        private List<ListOfVouchersBookdto> DataSortByPartner(List<ListOfVouchersBookdto> dtos)
        {
            var groupData = dtos.GroupBy(p => new { p.PartnerCode, p.PartnerName })
                                .Select(p => new ListOfVouchersBookdto()
                                {
                                    Bold = "C",
                                    GroupCode = p.Key.PartnerCode,
                                    Note = string.IsNullOrEmpty(p.Key.PartnerName) ? "Không có mã đối tượng"
                                                                    : p.Key.PartnerName,
                                    Amount = p.Sum(s => s.Amount),
                                    AmountCur = p.Sum(s => s.AmountCur)
                                }).ToList();
            Parallel.ForEach(dtos, item =>
            {
                item.GroupCode = item.PartnerCode;
            });

            dtos.AddRange(groupData);
            return dtos.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private List<ListOfVouchersBookdto> DataSortByCurrency(List<ListOfVouchersBookdto> dtos)
        {
            var groupData = dtos.GroupBy(p => new { p.CurrencyCode })
                                .Select(p => new ListOfVouchersBookdto()
                                {
                                    Bold = "C",
                                    GroupCode = p.Key.CurrencyCode,
                                    Note = p.Key.CurrencyCode,
                                    Amount = p.Sum(s => s.Amount),
                                    AmountCur = p.Sum(s => s.AmountCur)
                                }).ToList();
            Parallel.ForEach(dtos, item =>
            {
                item.GroupCode = item.PartnerCode;
            });

            dtos.AddRange(groupData);
            return dtos.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> DataSortByAccCase(List<ListOfVouchersBookdto> dtos)
        {
            var accCases = await _accCaseService.GetByAccCaseAsync(null, _webHelper.GetCurrentOrgUnit());

            var groupData = (from p in dtos
                             join c in accCases on p.CaseCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.CaseCode, Name = df?.Name ?? null, p.Amount, p.AmountCur }
                             by new
                             {
                                 CaseCode = p.CaseCode,
                                 Name = df?.Name ?? null
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.CaseCode,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? "Không có mã vụ việc"
                                                                     : gr.Key.Name,
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            Parallel.ForEach(dtos, item =>
            {
                item.GroupCode = item.CaseCode;
            });

            dtos.AddRange(groupData);
            return dtos.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> DataSortBySection(List<ListOfVouchersBookdto> dtos)
        {
            var accCases = await _accSectionService.GetAll(_webHelper.GetCurrentOrgUnit());

            var groupData = (from p in dtos
                             join c in accCases on p.SectionCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.SectionCode, Name = df?.Name ?? null, p.Amount, p.AmountCur }
                             by new
                             {
                                 SectionCode = p.SectionCode,
                                 Name = df?.Name ?? null
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.SectionCode,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? "Không có mã khoản mục"
                                                                     : gr.Key.Name,
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            Parallel.ForEach(dtos, item =>
            {
                item.GroupCode = item.SectionCode;
            });

            dtos.AddRange(groupData);
            return dtos.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> DataSortByDepartment(List<ListOfVouchersBookdto> dtos)
        {
            var accCases = await _departmentService.GetListDepartmentAsync(null, _webHelper.GetCurrentOrgUnit());

            var groupData = (from p in dtos
                             join c in accCases on p.DepartmentCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.DepartmentCode, Name = df?.Name ?? null, p.Amount, p.AmountCur }
                             by new
                             {
                                 DepartmentCode = p.DepartmentCode,
                                 Name = df?.Name ?? null
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.DepartmentCode,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? "Không có mã bộ phận"
                                                                     : gr.Key.Name,
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            Parallel.ForEach(dtos, item =>
            {
                item.GroupCode = item.DepartmentCode;
            });

            dtos.AddRange(groupData);
            return dtos.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> DataSortByFProductWork(List<ListOfVouchersBookdto> dtos)
        {
            var accCases = await _fProductWorkService.GetListAsync(_webHelper.GetCurrentOrgUnit());

            var groupData = (from p in dtos
                             join c in accCases on p.FProductWorkCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.DepartmentCode, Name = df?.Name ?? null, p.Amount, p.AmountCur }
                             by new
                             {
                                 FProductWorkCode = p.FProductWorkCode,
                                 Name = df?.Name ?? null
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.FProductWorkCode,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? "Không có mã công trình, sản phẩm"
                                                                     : gr.Key.Name,
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            Parallel.ForEach(dtos, item =>
            {
                item.GroupCode = item.FProductWorkCode;
            });

            dtos.AddRange(groupData);
            return dtos.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> DataSortByAcc(List<ListOfVouchersBookdto> dtos)
        {
            int year = _webHelper.GetCurrentYear();
            var accCases = await _accountingCacheManager.GetAccountSystemsAsync(year);

            var groupData = (from p in dtos
                             join c in accCases on p.CreditAcc equals c.AccCode into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.CreditAcc, Name = df?.AccName ?? null, p.Amount, p.AmountCur }
                             by new
                             {
                                 CreditAcc = p.CreditAcc,
                                 Name = df?.AccName ?? null
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.CreditAcc,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? $"{gr.Key.CreditAcc} - Không có mã tài khoản"
                                                                     : $"{gr.Key.CreditAcc} - {gr.Key.Name}",
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            Parallel.ForEach(dtos, item =>
            {
                item.GroupCode = item.CreditAcc;
            });

            dtos.AddRange(groupData);
            return dtos.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        #endregion
    }
}

