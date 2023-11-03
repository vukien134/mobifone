using System;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Threading.Tasks;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using System.Collections.Generic;
using System.IO;
using Volo.Abp.ObjectMapping;
using System.Linq;
using System.Text.RegularExpressions;
using IdentityServer4.Extensions;
using Accounting.Categories.Partners;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.GeneralDiaries
{
    public class ListOfVoucherMultiBookAppService : AccountingAppService
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
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly ContractService _contractService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public ListOfVoucherMultiBookAppService(ReportDataService reportDataService,
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
                        WorkPlaceSevice workPlaceSevice,
                        ContractService contractService,
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
            _workPlaceSevice = workPlaceSevice;
            _contractService = contractService;
            _accPartnerAppService = accPartnerAppService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.ListOfVoucherMultiBookReportView)]
        public async Task<ReportResponseDto<ListOfVouchersBookdto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var incurredData = await GetIncurredData(dic);
            var totalAmount = incurredData.Sum(p => p.Amount);
            var totalAmountCur = incurredData.Sum(p => p.AmountCur);

            var result = new List<ListOfVouchersBookdto>();
            var groupData1 = await this.GroupData1(incurredData, dto.Parameters.Sort1.Value.ToString());
            foreach (var group1 in groupData1)
            {
                result.Add(group1);
                var groupData2 = await this.GroupData2(incurredData, dto.Parameters.Sort1.Value.ToString(),
                                            group1.GroupCode, dto.Parameters.Sort2.Value.ToString());
                result.AddRange(groupData2);
            }

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
        [Authorize(ReportPermissions.ListOfVoucherMultiBookReportPrint)]
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
                Acc = p.Acc,
                Note = p.Note,
                WorkPlaceCode = p.WorkPlaceCode,
                ContractCode = p.ContractCode
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
                            PartnerGroupId = df?.PartnerGroupId ?? null
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
        private Dictionary<string, object> GetLedgerParameter(ReportBaseParameterDto dto)
        {
            //string debitOrCredit = dto.DebitCredit.Equals("*") ? "N" : dto.DebitCredit;
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, dto.DebitCredit);
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

            if (!string.IsNullOrEmpty(dto.ReciprocalAcc))
            {
                dic.Add(LedgerParameterConst.ReciprocalAcc, dto.ReciprocalAcc);
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(LedgerParameterConst.DepartmentCode, dto.DepartmentCode);
            }

            if (!string.IsNullOrEmpty(dto.PartnerGroup))
            {
                dic.Add(LedgerParameterConst.PartnerGroup, dto.PartnerGroup);
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

            if (!string.IsNullOrEmpty(dto.CreationUser))
            {
                dic.Add(LedgerParameterConst.CreationUser, dto.CreationUser);
            }

            if (dto.CreationTime != null)
            {
                dic.Add(LedgerParameterConst.CreationTime, dto.CreationTime);
            }
            if (!string.IsNullOrEmpty(dto.EndVoucherNumber))
            {
                dic.Add(LedgerParameterConst.EndVoucherNumber, dto.EndVoucherNumber);
            }
            if (!string.IsNullOrEmpty(dto.BeginVoucherNumber))
            {
                dic.Add(LedgerParameterConst.BeginVoucherNumber, dto.BeginVoucherNumber);
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
        private async Task<List<ListOfVouchersBookdto>> GroupData1(List<ListOfVouchersBookdto> warehouseData, string group)
        {
            var queryable = warehouseData.AsQueryable();
            List<ListOfVouchersBookdto> result = group switch
            {
                "1" => await this.GroupByDepartment(queryable),
                "2" => this.GroupByPartner(queryable),
                "3" => await this.GroupByFProductWork(queryable),
                "4" => await this.GroupBySection(queryable),
                "5" => await this.GroupByAccCase(queryable),
                "6" => await this.GroupByDebitAcc(queryable),
                "7" => await this.GroupByCreditAcc(queryable),
                "8" => await this.GroupByWorkPlace(queryable),
                "9" => await this.GroupByContract(queryable),
                _ => null
            };
            return result;
        }
        private async Task<List<ListOfVouchersBookdto>> GroupData2(List<ListOfVouchersBookdto> warehouseData, string group1, string code, string group2)
        {
            var queryable = warehouseData.AsQueryable();
            queryable = this.FilterByGroupCode(queryable, group1, code);
            List<ListOfVouchersBookdto> result = group2 switch
            {
                "1" => await this.GroupByDepartment(queryable, true),
                "2" => this.GroupByPartner(queryable, true),
                "3" => await this.GroupByFProductWork(queryable, true),
                "4" => await this.GroupBySection(queryable, true),
                "5" => await this.GroupByAccCase(queryable, true),
                "6" => await this.GroupByDebitAcc(queryable, true),
                "7" => await this.GroupByCreditAcc(queryable, true),
                "8" => await this.GroupByWorkPlace(queryable, true),
                "9" => await this.GroupByContract(queryable, true),
                _ => null
            };
            return result;
        }
        private IQueryable<ListOfVouchersBookdto> FilterByGroupCode(IQueryable<ListOfVouchersBookdto> queryable,
                                        string group, string code)
        {
            var query = group switch
            {
                "1" => queryable.Where(p => p.DepartmentCode == code),
                "2" => queryable.Where(p => p.PartnerCode == code),
                "3" => queryable.Where(p => p.FProductWorkCode == code),
                "4" => queryable.Where(p => p.SectionCode == code),
                "5" => queryable.Where(p => p.CaseCode == code),
                "6" => queryable.Where(p => p.DebitAcc == code),
                "7" => queryable.Where(p => p.CreditAcc == code),
                "8" => queryable.Where(p => p.WorkPlaceCode == code),
                "9" => queryable.Where(p => p.ContractCode == code),
                _ => queryable
            };
            return query;
        }
        private async Task<List<ListOfVouchersBookdto>> GroupByDepartment(IQueryable<ListOfVouchersBookdto> queryable, bool attachDetail = false)
        {
            var departments = await _departmentService.GetListDepartmentAsync(null, _webHelper.GetCurrentOrgUnit());
            var queryableDepartment = departments.AsQueryable();

            var groupData = (from p in queryable
                             join c in queryableDepartment on p.DepartmentCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.DepartmentCode, Name = df == null ? null : df.Name, p.Amount, p.AmountCur }
                             by new
                             {
                                 DepartmentCode = p.DepartmentCode,
                                 Name = df == null ? null : df.Name
                             }
                             into gr
                             orderby gr.Key.DepartmentCode
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.DepartmentCode,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? "Không có mã bộ phận"
                                                                     : gr.Key.Name,
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(result, item =>
            {
                item.GroupCode = item.DepartmentCode;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private List<ListOfVouchersBookdto> GroupByPartner(IQueryable<ListOfVouchersBookdto> queryable,
                                                    bool attachDetail = false)
        {
            var groupData = queryable.GroupBy(p => new { p.PartnerCode, p.PartnerName })
                                .Select(p => new ListOfVouchersBookdto()
                                {
                                    Bold = "C",
                                    GroupCode = p.Key.PartnerCode,
                                    Note = string.IsNullOrEmpty(p.Key.PartnerName) ? "Không có mã đối tượng"
                                                                    : p.Key.PartnerName,
                                    Amount = p.Sum(s => s.Amount),
                                    AmountCur = p.Sum(s => s.AmountCur)
                                }).ToList();

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(result, item =>
            {
                item.GroupCode = item.PartnerCode;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> GroupByFProductWork(IQueryable<ListOfVouchersBookdto> queryable,
                                bool attachDetail = false)
        {
            var fProducts = await _fProductWorkService.GetListAsync(_webHelper.GetCurrentOrgUnit());
            var queryableFProductWork = fProducts.AsQueryable();

            var groupData = (from p in queryable
                             join c in queryableFProductWork on p.FProductWorkCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.DepartmentCode, Name = df == null ? null : df.Name, p.Amount, p.AmountCur }
                             by new
                             {
                                 FProductWorkCode = p.FProductWorkCode,
                                 Name = df == null ? null : df.Name
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

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(result, item =>
            {
                item.GroupCode = item.FProductWorkCode;
            });
            result.AddRange(groupData);

            return queryable.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> GroupBySection(IQueryable<ListOfVouchersBookdto> queryable,
                            bool attachDetail = false)
        {
            var accCases = await _accSectionService.GetAll(_webHelper.GetCurrentOrgUnit());
            var queryableSection = accCases.AsQueryable();

            var groupData = (from p in queryable
                             join c in queryableSection on p.SectionCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.SectionCode, Name = df == null ? null : df.Name, p.Amount, p.AmountCur }
                             by new
                             {
                                 SectionCode = p.SectionCode,
                                 Name = df == null ? null : df.Name
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

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(result, item =>
            {
                item.GroupCode = item.SectionCode;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> GroupByAccCase(IQueryable<ListOfVouchersBookdto> queryable,
                                    bool attachDetail = false)
        {
            var accCases = await _accCaseService.GetByAccCaseAsync(null, _webHelper.GetCurrentOrgUnit());
            var queryableAccCase = accCases.AsQueryable();

            var groupData = (from p in queryable
                             join c in accCases on p.CaseCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.CaseCode, Name = df == null ? null : df.Name, p.Amount, p.AmountCur }
                             by new
                             {
                                 CaseCode = p.CaseCode,
                                 Name = df == null ? null : df.Name
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

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(queryable, item =>
            {
                item.GroupCode = item.CaseCode;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> GroupByDebitAcc(IQueryable<ListOfVouchersBookdto> queryable,
                                bool attachDetail = false)
        {
            int year = _webHelper.GetCurrentYear();
            var accCases = await _accountingCacheManager.GetAccountSystemsAsync(year);
            var queryableAccountSystem = accCases.AsQueryable();

            var groupData = (from p in queryable
                             join c in queryableAccountSystem on p.DebitAcc equals c.AccCode into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.DebitAcc, Name = df == null ? null : df.AccName, p.Amount, p.AmountCur }
                             by new
                             {
                                 DebitAcc = p.DebitAcc,
                                 Name = df == null ? null : df.AccName
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.DebitAcc,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? $"{gr.Key.DebitAcc} - Không có mã tài khoản"
                                                                     : $"{gr.Key.DebitAcc} - {gr.Key.Name}",
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(queryable, item =>
            {
                item.GroupCode = item.DebitAcc;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> GroupByCreditAcc(IQueryable<ListOfVouchersBookdto> queryable,
                                bool attachDetail = false)
        {
            int year = _webHelper.GetCurrentYear();
            var accCases = await _accountingCacheManager.GetAccountSystemsAsync(year);
            var queryableAccountSystem = accCases.AsQueryable();

            var groupData = (from p in queryable
                             join c in queryableAccountSystem on p.CreditAcc equals c.AccCode into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.CreditAcc, Name = df == null ? null : df.AccName, p.Amount, p.AmountCur }
                             by new
                             {
                                 CreditAcc = p.CreditAcc,
                                 Name = df == null ? null : df.AccName
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

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(queryable, item =>
            {
                item.GroupCode = item.CreditAcc;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> GroupByWorkPlace(IQueryable<ListOfVouchersBookdto> queryable,
                            bool attachDetail = false)
        {
            var accCases = await _workPlaceSevice.GetListAsync(_webHelper.GetCurrentOrgUnit());
            var queryableSection = accCases.AsQueryable();

            var groupData = (from p in queryable
                             join c in queryableSection on p.WorkPlaceCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.WorkPlaceCode, Name = df == null ? null : df.Name, p.Amount, p.AmountCur }
                             by new
                             {
                                 WorkPlaceCode = p.WorkPlaceCode,
                                 Name = df == null ? null : df.Name
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.WorkPlaceCode,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? "Không có mã phân xưởng"
                                                                     : gr.Key.Name,
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(result, item =>
            {
                item.GroupCode = item.WorkPlaceCode;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        private async Task<List<ListOfVouchersBookdto>> GroupByContract(IQueryable<ListOfVouchersBookdto> queryable,
                            bool attachDetail = false)
        {
            var accCases = await _contractService.GetListAsync(_webHelper.GetCurrentOrgUnit());
            var queryableSection = accCases.AsQueryable();

            var groupData = (from p in queryable
                             join c in queryableSection on p.ContractCode equals c.Code into gj
                             from df in gj.DefaultIfEmpty()
                             group new { p.ContractCode, Name = df == null ? null : df.Name, p.Amount, p.AmountCur }
                             by new
                             {
                                 ContractCode = p.ContractCode,
                                 Name = df == null ? null : df.Name
                             }
                            into gr
                             select new ListOfVouchersBookdto()
                             {
                                 Bold = "C",
                                 GroupCode = gr.Key.ContractCode,
                                 Note = string.IsNullOrEmpty(gr.Key.Name) ? "Không có mã hợp đồng"
                                                                     : gr.Key.Name,
                                 Amount = gr.Sum(s => s.Amount),
                                 AmountCur = gr.Sum(s => s.AmountCur)
                             }).ToList();

            if (attachDetail == false) return groupData;

            var result = queryable.ToList();
            Parallel.ForEach(result, item =>
            {
                item.GroupCode = item.ContractCode;
            });
            result.AddRange(groupData);

            return result.OrderBy(p => p.GroupCode).ThenBy(p => p.VoucherDate)
                            .ThenBy(p => p.VoucherNumber)
                            .ToList();
        }
        #endregion
    }
}

