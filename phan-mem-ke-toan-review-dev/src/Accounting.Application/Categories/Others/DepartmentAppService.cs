using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Others.Departments;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Windows;
using Accounting.EntityFrameworkCore;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class DepartmentAppService : AccountingAppService, IDepartmentAppService
    {
        #region Fields
        private readonly DepartmentService _departmentService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly LinkCodeService _linkCodeService;
        private readonly AccountingDb _accountingDb;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public DepartmentAppService(DepartmentService departmentService,
                                UserService userService,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ExcelService excelService,
                                LinkCodeService linkCodeService,
                                AccountingDb accountingDb,
                                CacheManager cacheManager,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _departmentService = departmentService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _linkCodeService = linkCodeService;
            _accountingDb = accountingDb;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.DepartmentManagerCreate)]
        public async Task<DepartmentDto> CreateAsync(CrudDepartmentDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.ParentCode = await GetParentCode(dto.ParentId);
            if (string.IsNullOrEmpty(dto.Code))
            {
                throw new Exception("Chưa nhập mã bộ phận");
            }
            if (string.IsNullOrEmpty(dto.Name))
            {
                throw new Exception("Chưa nhập tên bộ phận");
            }
            if (dto.Code.Length <= 2)
            {
                throw new Exception("Mã bộ phận không được ít hơn 3 ký tự");
            }
            if (dto.Code.Length >= 3)
            {
                var department = await _departmentService.GetQueryableAsync();
                var lstDepartment = department.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.Code).ToList();
                if (lstDepartment.Count > 0)
                {
                    throw new Exception("Mã bộ phận " + dto.Code + " đã tồn tại!");
                }
            }

            var entity = ObjectMapper.Map<CrudDepartmentDto, Department>(dto);
            var result = await _departmentService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<Department, DepartmentDto>(result);
        }
        [Authorize(AccountingPermissions.DepartmentManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            bool isUsing = await this.IsDepartmentUsing(id);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.IsUsing),
                        _localizer["Err:IdIsUsing", id]);
            }
            await _departmentService.DeleteAsync(id);
            await this.RemoveAllCache();
        }

        [Authorize(AccountingPermissions.DepartmentManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                bool isUsing = await this.IsDepartmentUsing(item);
                if (isUsing)
                {
                    var itemData = await _departmentService.GetAsync(item);
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", itemData.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _departmentService.DeleteManyAsync(deleteIds);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        public async Task<DepartmentDto> GetByIdAsync(string departmentId)
        {
            var department = await _departmentService.GetAsync(departmentId);

            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }
        [Authorize(AccountingPermissions.DepartmentManagerView)]
        public async Task<PageResultDto<DepartmentDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<DepartmentDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Department, DepartmentDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.DepartmentManagerView)]
        public Task<PageResultDto<DepartmentDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.DepartmentManagerUpdate)]
        public async Task UpdateAsync(string id, CrudDepartmentDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.ParentCode = await GetParentCode(dto.ParentId);
            if (string.IsNullOrEmpty(dto.Code))
            {
                throw new Exception("Chưa nhập mã bộ phận");
            }
            if (string.IsNullOrEmpty(dto.Name))
            {
                throw new Exception("Chưa nhập tên bộ phận");
            }
            if (dto.Code.Length <= 2)
            {
                throw new Exception("Mã bộ phận không được ít hơn 3 ký tự");
            }

            var entity = await _departmentService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _departmentService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.DepartmentCode, dto.Code, oldCode, entity.OrgCode);
            }
            await this.RemoveAllCache();
        }
        public async Task<List<BaseComboItemDto>> GetViewListAsync()
        {
            string key = string.Format(CacheKeyManager.ListByOrgCode, _webHelper.GetCurrentOrgUnit());
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DepartmentDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var departments = await _departmentService.GetRepository()
                        .GetListAsync(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                    return departments.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        ParentId = p.ParentId,
                        Value = p.Name
                    })
                    .OrderBy(p => p.Code).ToList();
                }
            );            
        }
        public async Task<List<BaseComboItemDto>> GetViewListCodeAsync()
        {
            var departments = await _departmentService.GetRepository()
                        .GetListAsync(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return departments.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Code = p.Code,
                Name = p.Name,
                ParentId = p.ParentCode,
                Value = p.Name
            })
            .OrderBy(p => p.Code).ToList();
        }
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudDepartmentDto>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {

                var checkDepartment = await _departmentService.GetListDepartmentAsync(item.Code, _webHelper.GetCurrentOrgUnit());
                if (checkDepartment.Count == 0)
                {
                    item.Id = this.GetNewObjectId();
                    item.OrgCode = _webHelper.GetCurrentOrgUnit();
                    item.CreatorName = await _userService.GetCurrentUserNameAsync();                  
                    if(item.ParentCode.IsNullOrEmpty() == false)
                    {
                        var dataParent = await _departmentService.GetQueryableAsync();
                        if(dataParent.Where(x=>x.Code == item.ParentCode).Any() == false)
                        {
                            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.Duplicate),
                          _localizer["Err:DepartmentParentCodeNotExist", item.ParentCode]);
                        }
                        else
                        {
                            item.ParentId = dataParent.Where(x=>x.Code == item.ParentCode).Select(x=>x.Id).FirstOrDefault();
                        }
                    }
                }
                else
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.Duplicate),
                          _localizer["Err:DepartmentCodeExisted", item.Code]);
                }

            }

            var lstfdepartment = lstImport.Select(p => ObjectMapper.Map<CrudDepartmentDto, Department>(p))
                                .ToList();
            await _departmentService.CreateManyAsync(lstfdepartment);
            return new UploadFileResponseDto() { Ok = true };
        }
        #region Private
        private async Task<IQueryable<Department>> Filter(PageRequestDto dto)
        {
            var queryable = await _departmentService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }
        private async Task<string> GetParentCode(string parentId)
        {
            if (string.IsNullOrEmpty(parentId)) return null;
            var department = await _departmentService.FindAsync(parentId);
            if (department == null) return null;
            return department.Code;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<DepartmentDto>();
        }
        private async Task<bool> IsDepartmentUsing(string id)
        {
            var entity = await _departmentService.GetAsync(id);
            bool result = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.DepartmentCode, entity.Code, entity.OrgCode);
            if (result == true) return result;
            result = await _departmentService.IsParentGroup(id);
            return result;
        }
        #endregion
    }
}
