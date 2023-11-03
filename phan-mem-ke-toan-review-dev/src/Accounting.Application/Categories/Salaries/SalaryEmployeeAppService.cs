using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Salaries.Employees;
using Accounting.Common.Extensions;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Salaries;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Vouchers.ExportXmls;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Uow;

namespace Accounting.Categories.Salaries
{
    public class SalaryEmployeeAppService : AccountingAppService, ISalaryEmployeeAppService
    {
        #region Fields
        private readonly SalaryEmployeeService _salaryEmployeeService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<SalaryEmployeeDto> _cache;
        private readonly IDistributedCache<PageResultDto<SalaryEmployeeDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly SalaryCategoryService _salaryCategory;
        private readonly EmployeeService _employee;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public SalaryEmployeeAppService(SalaryEmployeeService salaryEmployeeService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<SalaryEmployeeDto> cache,
                            IDistributedCache<PageResultDto<SalaryEmployeeDto>> pageCache,
                            CacheManager cacheManager,
                            SalaryCategoryService salaryCategory,
                            EmployeeService employee,
                            IStringLocalizer<AccountingResource> localizer
                            )
        {
            _salaryEmployeeService = salaryEmployeeService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _salaryCategory = salaryCategory;
            _employee = employee;
            _localizer = localizer;
        }
        #endregion

        [Authorize(AccountingPermissions.SalaryEmployeeManagerCreate)]
        public async Task<SalaryEmployeeDto> CreateAsync(CrudSalaryEmployeeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();

            var dataemployee = await _employee.GetQueryableAsync();
            var dataSalaryCategory = await _salaryCategory.GetQueryableAsync();
            if (dataemployee.Where(x => x.Code == dto.EmployeeCode).Count() > 0 && dataSalaryCategory.Where(y => y.Code == dto.SalaryCode).Count() > 0)
            {
                var entity = ObjectMapper.Map<CrudSalaryEmployeeDto, SalaryEmployee>(dto);
                var status = await this.CheckExisted(entity);
                if (status == false)
                {
                    var result = await _salaryEmployeeService.CreateAsync(entity);
                    await RemoveAllCache();
                    return ObjectMapper.Map<SalaryEmployee, SalaryEmployeeDto>(result);
                }
                else
                {                    
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SalaryCategory, ErrorCode.Duplicate),
                               _localizer["SalaryDataExisted"]);
                }
            }
            else
            {               
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SalaryCategory, ErrorCode.NotFoundEntity),
                               _localizer["Error:SalaryData"]);
            }          
        }

        [Authorize(AccountingPermissions.SalaryEmployeeManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _salaryEmployeeService.DeleteAsync(id);
            await RemoveAllCache();
        }

        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string[] idsDelete = dto.ListId.ToArray();
            await _salaryEmployeeService.DeleteManyAsync(idsDelete);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }


        [Authorize(AccountingPermissions.SalaryEmployeeManagerView)]
        public async Task<PageResultDto<SalaryEmployeeDto>> PagesAsync(PageRequestDto dto)
        {
            await RemoveAllCache();
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<SalaryEmployeeDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }
        [Authorize(AccountingPermissions.SalaryEmployeeManagerView)]
        public async Task<PageResultDto<SalaryEmployeeDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<SalaryEmployeeDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.EmployeeCode).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<SalaryEmployee, SalaryEmployeeDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.SalaryEmployeeManagerUpdate)]
        public async Task UpdateAsync(string id, CrudSalaryEmployeeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _salaryEmployeeService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _salaryEmployeeService.UpdateAsync(entity);
            await RemoveAllCache();
        }

        public async Task<SalaryEmployeeDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () =>
                {
                    var accCase = await _salaryEmployeeService.GetAsync(caseId);
                    return ObjectMapper.Map<SalaryEmployee, SalaryEmployeeDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );

        }
        [Authorize(AccountingPermissions.SalaryEmployeeManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {          
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();       
            var org =  this._webHelper.GetCurrentOrgUnit();
            var datatest = await _salaryEmployeeService.GetallbyOrgcode(org);
            var lstImport = await _excelService.ImportFileToList<ExcelSalaryEmployee>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {   
                var id = this.GetNewObjectId();
                item.Id = id;
                item.OrgCode = org;
                item.CreatorName = await _userService.GetCurrentUserNameAsync();   
            }

            var lstSalaryEmployee = lstImport.Select(p => ObjectMapper.Map<ExcelSalaryEmployee, SalaryEmployee>(p)).ToList();
            foreach(var salary in lstSalaryEmployee)
            {
                var dataemployee = await _employee.GetQueryableAsync();
                var dataSalaryCategory = await _salaryCategory.GetQueryableAsync();
                if (dataemployee.Where(x=>x.Code == salary.EmployeeCode).Count()>0 && dataSalaryCategory.Where(y=>y.Code == salary.SalaryCode).Count()>0)
                {
                    var status = await this.CheckExisted(salary);
                    if (status == false)
                    {
                        var tenants = _webHelper.GetTenantId();
                        salary.TenantId = Guid.Parse(tenants);
                        try
                        {
                            await _salaryEmployeeService.CreateAsync(salary);
                            await this.RemoveAllCache();
                        }
                         catch (Exception) {               
                            throw;
                        }
                    }
                    else
                    {
                        var data = await _salaryEmployeeService.GetallbyOrgcode(org);
                        var temp = data.Where(x=>x.EmployeeCode == salary.EmployeeCode && x.SalaryCode == salary.SalaryCode).FirstOrDefault();
                        var entity = await _salaryEmployeeService.GetAsync(temp.Id);    
                        await _salaryEmployeeService.DeleteAsync(entity);             
                        var tenants = _webHelper.GetTenantId();
                        salary.TenantId = Guid.Parse(tenants);
                        await _salaryEmployeeService.CreateAsync(salary);
                        await RemoveAllCache();
                    }
                }       
            }
            return new UploadFileResponseDto() { Ok = true };      
        }

        #region Private
        private async Task<bool> CheckExisted(SalaryEmployee entity)
        
        {
            var dataQuery = await _salaryEmployeeService.GetQueryableAsync();
            var data = dataQuery.Where(x => x.EmployeeCode == entity.EmployeeCode && x.SalaryCode == entity.SalaryCode).Any();        
            return data;
        }
        private async Task<IQueryable<SalaryEmployee>> Filter(PageRequestDto dto)
        {
            var queryable = await _salaryEmployeeService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _salaryEmployeeService.GetQueryableQuickSearch(queryable, filterValue);
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<SalaryEmployeeDto>();
        }
        #endregion
    }
}
