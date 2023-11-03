using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Salaries;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Salaries;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Reports.Salaries
{
    public class SalarySheetReportAppService : BaseReportAppService
    {
        #region Privates
        private readonly SalaryBookService _salaryBookService;        
        private readonly EmployeeService _employeeService;        
        private readonly DepartmentService _departmentService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public SalarySheetReportAppService(
                    WebHelper webHelper,
                    ReportTemplateService reportTemplateService,
                    IWebHostEnvironment hostingEnvironment,
                    YearCategoryService yearCategoryService,
                    TenantSettingService tenantSettingService,
                    OrgUnitService orgUnitService,
                    CircularsService circularsService,
                    IStringLocalizer<AccountingResource> localizer,
                    DepartmentService departmentService,
                    SalaryBookService salaryBookService,                    
                    EmployeeService employeeService,
                    AccountingCacheManager accountingCacheManager
            ) : base(webHelper,reportTemplateService,hostingEnvironment,yearCategoryService,
                            tenantSettingService,orgUnitService,circularsService, localizer)
        {
            _salaryBookService = salaryBookService;            
            _employeeService = employeeService;            
            _departmentService = departmentService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.SalarySheetHkdReportView)]
        public async Task<ReportResponseDto<JsonObject>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            var salaryBooks = await this.GetSalaryBooksAsync(dto.Parameters);

            var totalAmounts = this.GetTotalAmount(salaryBooks);
            var data = new List<JsonObject>();
            foreach(var item in totalAmounts)
            {
                string employeeName = await this.GetEmployeeNameAsync(item.EmployeeCode);
                var obj = new JsonObject();
                obj.Add("employeeCode", item.EmployeeCode);
                obj.Add("employeeName", employeeName);
                obj.Add("bold", "K");
                obj = this.AddSalaryColumn(salaryBooks, item.EmployeeCode, obj);
                data.Add(obj);
            }

            var totalBySalaryCode = this.GetTotalAmountBySalaryCode(salaryBooks);
            var objTotal = new JsonObject();
            objTotal.Add("employeeName", "Tổng cộng");
            objTotal.Add("bold", "C");
            foreach(var item in totalBySalaryCode)
            {
                objTotal.Add(item.SalaryCode, item.NumberValue);
            }
            data.Add(objTotal);

            var reportResponse = await this.CreateReportResponseDto<JsonObject>(data, dto);
            return reportResponse;
        }
        [Authorize(ReportPermissions.SalarySheetHkdReportPrint)]
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
                CurrencyFormats = currencyFormats,
                IsJsonObject = true
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
        private async Task<List<SalaryBook>> GetSalaryBooksAsync(ReportBaseParameterDto dto)
        {
            if (string.IsNullOrEmpty(dto.SalarySheetTypeId))
            {
                throw new ArgumentNullException(nameof(dto.SalarySheetTypeId));
            }
            if (string.IsNullOrEmpty(dto.SalaryPeriodId))
            {
                throw new ArgumentNullException(nameof(dto.SalaryPeriodId));
            }
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var queryable = await _salaryBookService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode)
                        && p.SalarySheetTypeId.Equals(dto.SalarySheetTypeId)
                        && p.SalaryPeriodId.Equals(dto.SalaryPeriodId));
            if (!string.IsNullOrEmpty(dto.DepartmentId))
            {
                string departmentCode = await this.GetDepartmentCode(dto.DepartmentId);
                queryable = queryable.Where(p => p.DepartmentCode.Equals(departmentCode));
            }
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private List<SalaryBook> GetTotalAmount(List<SalaryBook> salaryBooks)
        {
            var totals = salaryBooks.GroupBy(s => new
            {
                s.OrgCode,
                s.SalarySheetTypeId,
                s.SalaryPeriodId,
                s.DepartmentCode,
                s.PositionCode,
                s.EmployeeCode
            }).Select(p => new SalaryBook()
            {
                OrgCode = p.Key.OrgCode,
                SalarySheetTypeId = p.Key.SalarySheetTypeId,
                SalaryPeriodId = p.Key.SalaryPeriodId,
                DepartmentCode = p.Key.DepartmentCode,
                PositionCode = p.Key.PositionCode,
                EmployeeCode = p.Key.EmployeeCode,
                NumberValue = p.Sum(s => s.NumberValue)
            }).OrderBy(p => p.DepartmentCode).ThenBy(p => p.EmployeeCode).ToList();
            return totals;
        }
        private List<SalaryBook> GetTotalAmountBySalaryCode(List<SalaryBook> salaryBooks)
        {
            var totals = salaryBooks.GroupBy(s => new
            {
                s.OrgCode,
                s.SalarySheetTypeId,
                s.SalaryPeriodId,                
                s.SalaryCode
            }).Select(p => new SalaryBook()
            {
                OrgCode = p.Key.OrgCode,
                SalarySheetTypeId = p.Key.SalarySheetTypeId,
                SalaryPeriodId = p.Key.SalaryPeriodId,
                SalaryCode = p.Key.SalaryCode,                
                NumberValue = p.Sum(s => s.NumberValue)
            }).OrderBy(p => p.SalaryCode).ToList();
            return totals;
        }
        private async Task<string> GetEmployeeNameAsync(string code)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var employee = await _employeeService.GetByCodeAsync(code, orgCode);
            if (employee == null) return null;
            return employee.Name;
        }
        private JsonObject AddSalaryColumn(List<SalaryBook> salaryBooks,string code,JsonObject obj)
        {
            var books = salaryBooks.Where(p => p.EmployeeCode.Equals(code))
                                .OrderBy(p => p.SalaryCode).ToList();
            foreach(var item in books)
            {
                obj.Add(item.SalaryCode, item.NumberValue);
            }
            return obj;
        }        
        private async Task<string> GetDepartmentCode(string departmentId)
        {
            var department = await _departmentService.GetAsync(departmentId);
            return department.Code;
        }
        #endregion
    }
}
