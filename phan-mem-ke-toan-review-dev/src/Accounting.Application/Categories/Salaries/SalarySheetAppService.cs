using Accounting.Business;
using Accounting.Catgories.Salaries.SalaryBooks;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Salaries;
using Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.Salaries
{
    public class SalarySheetAppService : AccountingAppService
    {
        #region Fields
        private readonly WebHelper _webHelper;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly SalarySheetTypeDetailService _salarySheetTypeDetailService;
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly PositionService _positionService;
        private readonly SalaryCategoryService _salaryCategoryService;
        private readonly SalaryEmployeeService _salaryEmployeeService;
        private readonly SalaryBookService _salaryBookService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        #endregion
        #region Ctor
        public SalarySheetAppService(WebHelper webHelper,
                    LicenseBusiness licenseBusiness,
                    SalarySheetTypeDetailService salarySheetTypeDetailService,
                    DepartmentService departmentService,
                    EmployeeService employeeService,
                    PositionService positionService,
                    SalaryCategoryService salaryCategoryService,
                    SalaryEmployeeService salaryEmployeeService,
                    SalaryBookService salaryBookService,
                    IUnitOfWorkManager unitOfWorkManager
                )
        {
            _webHelper = webHelper;
            _licenseBusiness = licenseBusiness;
            _salarySheetTypeDetailService = salarySheetTypeDetailService;
            _departmentService = departmentService;
            _employeeService = employeeService;
            _positionService = positionService;
            _salaryCategoryService = salaryCategoryService;
            _salaryEmployeeService = salaryEmployeeService;
            _salaryBookService = salaryBookService;
            _unitOfWorkManager = unitOfWorkManager;
        }
        #endregion
        #region Methods
        public async Task<JsonArray> Create(JsonObject parameter)
        {
            await _licenseBusiness.CheckExpired();
            string salarySheetTypeId = parameter["salarySheetTypeId"].ToString();
            string salaryPeriodId = parameter["salaryPeriodId"].ToString();
            string departmentCode = parameter["departmentCode"].ToString();

            var result = new JsonArray();
            var employees = await this.GetEmployeesAsync(departmentCode);
            var salarySheetTypeDetails = await this.GetSalarySheetTypeDetails(salarySheetTypeId);
            foreach(var item in employees)
            {
                var obj = new JsonObject();
                obj.Add("employeeCode", item.Code);
                obj.Add("employeeName", item.Name);
                obj.Add("departmentCode", item.DepartmentCode);
                obj.Add("departmentName", await this.GetDepartmentName(item.DepartmentCode));
                obj.Add("positionCode", item.PositionCode);
                obj.Add("positionName", await this.GetPositionName(item.PositionCode));
                
                foreach(var detail in salarySheetTypeDetails)
                {
                    if (!string.IsNullOrEmpty(detail.Formular)) continue;
                    
                    var salaryEmployee = await this.GetSalaryEmployee(item.Code, detail.FieldName);
                    if (salaryEmployee != null)
                    {
                        obj.Add(detail.FieldName, salaryEmployee.Amount);
                    }
                    else
                    {
                        obj.Add(detail.FieldName, null);
                    }
                }

                result.Add(obj);
            }
            return result;
        }
        public async Task SaveAsync(JsonObject obj)
        {
            await _licenseBusiness.CheckExpired();
            var unitOfWork = _unitOfWorkManager.Begin();
            try
            {
                JsonObject parameter = (JsonObject)obj["parameter"];
                JsonArray data = (JsonArray)obj["data"];
                await this.CreateDeleteAsync(parameter);
                await this.SaveSalarySheetAsync(data, parameter["salarySheetTypeId"].ToString(),
                                            parameter["salaryPeriodId"].ToString());
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task CreateDeleteAsync(JsonObject parameter)
        {
            await _licenseBusiness.CheckExpired();
            string salarySheetTypeId = parameter["salarySheetTypeId"].ToString();
            string salaryPeriodId = parameter["salaryPeriodId"].ToString();
            string departmentCode = parameter["departmentCode"].ToString();

            var entities = await _salaryBookService.GetSalaryBooks(salarySheetTypeId, salaryPeriodId,departmentCode);
            await _salaryBookService.DeleteManyAsync(entities);
        }
        public async Task<JsonObject> LoadData(JsonObject parameter)
        {
            string salarySheetTypeId = parameter["salarySheetTypeId"].ToString();
            string salaryPeriodId = parameter["salaryPeriodId"].ToString();
            string departmentCode = parameter["departmentCode"].ToString();

            var entities = await _salaryBookService.GetSalaryBooks(salarySheetTypeId, salaryPeriodId, departmentCode);
            var groupDatas = entities.GroupBy(s => new
            {
                s.DepartmentCode,
                s.EmployeeCode,
                s.SalarySheetTypeId,
                s.SalaryPeriodId,
                s.PositionCode
            }).Select(p => new SalaryBook()
            {
                DepartmentCode = p.Key.DepartmentCode,
                EmployeeCode = p.Key.EmployeeCode,
                SalaryPeriodId = p.Key.SalaryPeriodId,
                SalarySheetTypeId = p.Key.SalarySheetTypeId,
                PositionCode = p.Key.PositionCode,
                NumberValue = p.Sum(s => s.NumberValue)
            }).ToList();

            var data = new JsonArray();
            foreach(var group in groupDatas)
            {
                JsonObject obj = new JsonObject();
                obj.Add("departmentCode", group.DepartmentCode);
                obj.Add("departmentName", await this.GetDepartmentName(group.DepartmentCode));
                obj.Add("employeeCode", group.EmployeeCode);
                obj.Add("employeeName", await this.GetEmployeeName(group.EmployeeCode));
                obj.Add("positionCode", group.PositionCode);
                obj.Add("positionName", await this.GetPositionName(group.PositionCode));
                obj.Add("salarySheetTypeId", group.SalarySheetTypeId);
                obj.Add("salaryPeriodId", group.SalaryPeriodId);                

                var details = entities.Where(p => p.SalarySheetTypeId == salarySheetTypeId
                                        && p.SalaryPeriodId == salaryPeriodId
                                        && p.DepartmentCode == group.DepartmentCode
                                        && p.PositionCode == group.PositionCode
                                        && p.EmployeeCode == group.EmployeeCode
                                        ).ToList();
                foreach(var detail in details)
                {
                    obj.Add(detail.SalaryCode, detail.NumberValue);
                }
                data.Add(obj);
            }

            var groupFormular = entities.Where(p => p.Formular != null).GroupBy(s => new
            {
                s.SalaryCode
            }).Select(p => new SalaryBook()
            {
                SalaryCode = p.Key.SalaryCode,
                Formular = p.Max(s => s.Formular)
            }).ToList();

            var formulars = new JsonArray();
            foreach(var group in groupFormular)
            {
                JsonObject obj = new JsonObject();
                obj.Add("salaryCode", group.SalaryCode);
                obj.Add("formular", group.Formular);
                formulars.Add(obj);
            }

            JsonObject result = new JsonObject();
            result.Add("formulars", formulars);
            result.Add("data", data);

            return result;
        }
        #endregion
        #region Privates
        private async Task<List<SalarySheetTypeDetail>> GetSalarySheetTypeDetails(string salarySheetTypeId)
        {
            var entities = await _salarySheetTypeDetailService.GetBySalarySheetTypeIdAsync(salarySheetTypeId);
            return entities;
        }
        private async Task<List<Employee>> GetEmployeesAsync(string departmentCode)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            return await _employeeService.GetByDepartmentCode(orgCode,departmentCode);
        }
        private async Task<string> GetDepartmentName(string code)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var department = await _departmentService.GetDepartmentByCodeAsync(code, orgCode);
            if (department == null) return null;
            return department.Name;
        }
        private async Task<string> GetEmployeeName(string code)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _employeeService.GetByCodeAsync(code, orgCode);
            if (entity == null) return null;
            return entity.Name;
        }
        private async Task<string> GetPositionName(string code)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var position = await _positionService.GetByCodeAsync(code, orgCode);
            if (position == null) return null;
            return position.Name;
        }
        private async Task<SalaryEmployee> GetSalaryEmployee(string employeeCode,string salaryCode)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            return await _salaryEmployeeService.GetSalary(employeeCode, salaryCode, orgCode);
        }
        private async Task SaveSalarySheetAsync(JsonArray data, string salarySheetTypeId,string salaryPeriodId)
        {
            var salarySheetTypeDetails = await this.GetSalarySheetTypeDetails(salarySheetTypeId);
            string orgCode = _webHelper.GetCurrentOrgUnit();
            foreach (var item in data)
            {
                string departmentCode = item["departmentCode"].ToString();
                string employeeCode = item["employeeCode"].ToString();
                string positionCode = item["positionCode"].ToString();

                foreach (var detail in salarySheetTypeDetails)
                {
                    var book = new CrudSalaryBookDto();
                    book.Id = this.GetNewObjectId();
                    book.DepartmentCode = departmentCode;
                    book.EmployeeCode = employeeCode;
                    book.SalarySheetTypeId = salarySheetTypeId;
                    book.SalaryPeriodId = salaryPeriodId;
                    book.SalaryCode = detail.FieldName;
                    book.PositionCode = positionCode;
                    book.OrgCode = orgCode;
                    if (detail.DataType == "Số")
                    {
                        book.NumberValue = item[detail.FieldName] == null ? 0
                                : Convert.ToDecimal(item[detail.FieldName].ToString());
                    }
                    else
                    {
                        book.StringValue = item[detail.FieldName].ToString();
                    }
                    if (!string.IsNullOrEmpty(detail.Formular))
                    {
                        book.Formular = detail.Formular;
                    }

                    var entity = ObjectMapper.Map<CrudSalaryBookDto, SalaryBook>(book);
                    await _salaryBookService.CreateAsync(entity);
                }
            }
        }
        #endregion
    }
}
