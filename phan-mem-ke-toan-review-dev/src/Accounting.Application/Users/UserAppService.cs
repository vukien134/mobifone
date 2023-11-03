using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Menus;
using Accounting.Catgories.OrgUnits;
using Accounting.Common;
using Accounting.Common.Extensions;
using Accounting.Configs;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Migrations.HostDbMigration;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Accounting.Users
{
    public class UserAppService : AccountingAppService, IUserAppService
    {
        #region Fields
        private readonly UserService _userService;
        private readonly ICurrentTenant _currentTenant;
        private readonly RoleService _roleService;
        private readonly UserRoleService _userRoleService;
        private readonly OrgUnitService _orgUnitService;
        private readonly MenuAccountingService _menuAccountingService;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly ITenantManager _tenantManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly IEmailSender _emailSender;
        private readonly CacheManager _cacheManager;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly RegLicenseService _regLicenseService;
        #endregion
        #region Ctor
        public UserAppService(UserService userService,
                        ICurrentTenant currentTenant,
                        RoleService roleService,
                        UserRoleService userRoleService,
                        OrgUnitService orgUnitService,
                        MenuAccountingService menuAccountingService,
                        IPermissionDefinitionManager permissionDefinitionManager,
                        ITenantManager tenantManager,
                        ITenantRepository tenantRepository,
                        IEmailSender emailSender,
                        CacheManager cacheManager,
                        IStringLocalizer<AccountingResource> localizer,
                        LicenseBusiness licenseBusiness,
                        RegLicenseService regLicenseService,
                        WebHelper webHelper
            )
        {
            _userService = userService;
            _currentTenant = currentTenant;
            _roleService = roleService;
            _userRoleService = userRoleService;
            _orgUnitService = orgUnitService;
            _menuAccountingService = menuAccountingService;
            _permissionDefinitionManager = permissionDefinitionManager;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _emailSender = emailSender;
            _cacheManager = cacheManager;
            _localizer = localizer;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _regLicenseService = regLicenseService;
        }
        #endregion
        [Authorize(AccountingPermissions.UserManagerView)]
        public Task<PageResultDto<UserDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.UserManagerView)]
        public async Task<PageResultDto<UserDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<UserDto>();
            var query = await Filter(dto);
            int count = dto.Count == 0 ? 10 : dto.Count;
            var querysort = query.OrderBy(p => p.UserName).Skip(dto.Start).Take(count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<IdentityUser, UserDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.UserManagerCreate)]
        public async Task<UserDto> CreateAsync(CrudUserDto dto)
        {
            var checkReg = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            var numUserReg = checkReg.UserQuantity;
            var datas = await _userService.GetQueryableAsync();
            var curTenant = this._currentTenant.Id.GetValueOrDefault();
            var tenantData = await _tenantRepository.FindAsync(curTenant);
            if (datas.Count() >= numUserReg)
            {
                throw new Exception(_localizer["Err:OverUserRegister"]);
            }
            if(datas.Where(x=>x.Email == dto.Email).Count()>0)
            {
                throw new Exception(_localizer["Err:EmailExisted"]);
            }
            if (!IsEmail(dto.Email))
            {
                throw new Exception(_localizer["Err:EmailInvalidate"]);
            }
            var curUrl = _webHelper.GetCurrentUrl();
            await _licenseBusiness.ValidRegUserQuantity();
            dto.Id = GuidGenerator.Create();
            dto.TenantId = _currentTenant.Id;
            var entity = ObjectMapper.Map<CrudUserDto, IdentityUser>(dto);
            await this.SendMailForProcessing(dto.Email);
            var identityUser = await _userService.CreateAsync(entity, dto.Password);
            await this.SendMailForCompleted(dto.Email, dto.Password, dto.UserName, curUrl,tenantData.Name);
            return ObjectMapper.Map<IdentityUser, UserDto>(identityUser);
        }
        [Authorize(AccountingPermissions.UserManagerUpdate)]
        public async Task UpdateAsync(Guid id, CrudUserDto dto)
        {
            if (!IsEmail(dto.Email))
            {
                throw new Exception(_localizer["Err:EmailInvalidate"]);
            }
            var entity = await _userService.GetIdentityUserByIdAsync(id);
            ObjectMapper.Map(dto, entity);
            await _userService.UpdateAsync(entity);            
        }
        [Authorize(AccountingPermissions.UserManagerDelete)]
        public async Task DeleteAsync(Guid id)
        {
            await _userService.DeleteAsync(id);
        }
        public async Task<UserDto> GetByIdAsync(Guid userId)
        {
            var identityUser = await _userService.GetIdentityUserByIdAsync(userId);
            return ObjectMapper.Map<IdentityUser, UserDto>(identityUser);
        }
        public async Task<List<SelectRoleDto>> GetSelectRolesAsync(Guid userId)
        {
            var roles = await _roleService.GetAllAsync();
            string[] userRoles = await _userRoleService.GetRolesAsync(userId);

            return roles.Select(p => new SelectRoleDto()
            {
                Id = p.Id,
                Name = p.Name,
                IsSelect = userRoles.Contains(p.Name) ? true : false
            }).ToList();
        }
        public async Task<List<SelectRoleDto>> GetRolesAsync(Guid userId)
        {
            var selectRoles = new List<SelectRoleDto>();
            string[] userRoles = await _userRoleService.GetRolesAsync(userId);
            foreach(string name in userRoles)
            {
                var item = new SelectRoleDto()
                {
                    IsSelect = true,
                    Name = name
                };
                selectRoles.Add(item);
            }
            return selectRoles;
        }
        public async Task<List<SelectOrgUnitDto>> GetOrgUnitByUserIdAsync(Guid userId)
        {
            var orgUnitPermissions = await _orgUnitService.GetOrgUnitByUserIdAsync(userId);
            var orgUnits = await _orgUnitService.GetAllAsync();

            return orgUnitPermissions.Join(orgUnits,
                    p => p.OrgUnitId,
                    m => m.Id,
                    (p,m) => new SelectOrgUnitDto()
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        OrgUnitId = p.OrgUnitId,
                        Name = m.Name,
                        Code = m.Code
                    }).ToList();
        }
        public async Task<List<SelectOrgUnitDto>> GetSelectOrgUnitsAsync(Guid userId)
        {
            var orgUnits = await _orgUnitService.GetAllAsync();
            var orgUnitPermissions = await _orgUnitService.GetOrgUnitByUserIdAsync(userId);

            var selectOrgUnits = new List<SelectOrgUnitDto>();            
            foreach (var orgUnit in orgUnits)
            {
                bool isSelect = orgUnitPermissions.Any(p => p.OrgUnitId == orgUnit.Id);
                var item = new SelectOrgUnitDto()
                {
                    IsSelect = isSelect,
                    Name = orgUnit.Name,
                    Code = orgUnit.Code,
                    UserId = userId,
                    OrgUnitId = orgUnit.Id,
                    Id = this.GetNewObjectId()
                };
                selectOrgUnits.Add(item);
            }
            return selectOrgUnits;
        }
        public async Task SaveRolesAsync(SaveRoleDto dto)
        {
            string[] names = dto.Roles.Where(p => p.IsSelect == true)
                                .Select(p => p.Name).ToArray();
            await _userService.UpdateRolesAsync(dto.UserId.Value, names);
            await SaveOrgUnitPermission(dto.UserId.Value, dto.OrgUnits);
        }
        public async Task<UrlPermissionDto> CheckClientPath(UrlPermissionDto dto)
        {
            dto.IsGranted = false;
            var menu = await _menuAccountingService.GetMenuAccountingByUrl(dto.Url);
            if (menu == null) return dto;

            bool isGranted = false;
            string[] viewPermissions = await _userService.GetViewPermissions();
            if (menu.JavaScriptCode.Equals(JavaScriptCodeType.Report))
            {
                isGranted = IsGrantedReportGroup(menu, viewPermissions);
            }
            else
            {
                isGranted = viewPermissions.Contains(menu.ViewPermission);
            }            

            dto.IsGranted = isGranted;
            return dto;
        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            var partnes = await _userService.GetIdentityUsersAsync();
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.UserName,
                Value = p.UserName,
                Code = p.UserName,
                Name = p.UserName
            }).ToList();
        }
        [AllowAnonymous]
        public async Task<ResultDto> RecoverPass(JsonObject parameter)
        {
            if (!parameter.ContainsKey("accessCode") || !parameter.ContainsKey("email"))
            {
                throw new ArgumentException("Parameter accessCode or email not found");
                
            }
            var resultDto = new ResultDto();
            string tenantName = parameter["accessCode"].ToString();
            var tenant = await _tenantRepository.FindByNameAsync(tenantName);
            if (tenant == null)
            {
                /*throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Tenant, ErrorCode.NotFoundEntity),
                        $"Access Code ['{tenantName}'] not found ");*/
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Tenant, ErrorCode.NotFoundEntity),
                        _localizer["Err:AccCodeNotExist"]);
            }
            using (_currentTenant.Change(tenant.Id))
            {
                string email = parameter["email"].ToString();
                var identityUser = await _userService.GetByEmailAsync(email);
                if (identityUser == null)
                {
                    /*throw new AccountingException(ErrorCode.Get(GroupErrorCodes.IdentityUser, ErrorCode.NotFoundEntity),
                        $"Email ['{email}'] not found ");*/
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.IdentityUser, ErrorCode.NotFoundEntity),
                        _localizer["Err:EmailNotExist"]);
                }
                string randomPass = RandomPassword.Generate(8, 10);
                await _userService.ResetPassWordAsync(randomPass, identityUser);
                var curUrl = _webHelper.GetCurrentUrl();
                await _emailSender.SendAsync(
                    email,     // target email address
                    "Khôi phục mật khẩu",         // subject
                    $"Kính gửi Quý khách hàng, <br>" +  // email body
                    $"Chúng tôi vừa nhận được yêu cầu tạo mới mật khẩu cho tài khoản của Quý khách truy cập Phần mềm kế toán MobiFone Accounting Solution của Tổng Công ty Viễn thông MobiFone với thông tin như sau: <br>" +
                    $"Tên đăng nhập:  {identityUser.UserName} <br>" +
                    $"Mật khẩu: {randomPass} <br>" +
                    $"Quý khách vui lòng không cung cấp mật khẩu cho bất kỳ ai. <br>" +
                    $"Quý khách vui lòng truy cập Hệ thống theo đường dẫn: https://{curUrl}/ <br>" + 
                    $"Quý khách lưu ý, đây là email tự động từ hệ thống, vui lòng không trả lời lại email này! <br>" + 
                    $"MobiFone xin chân thành cảm ơn sự hợp tác và tin dùng của Quý khách hàng. <br>" +
                    $"Trân trọng."
                );
            }
            resultDto.Ok = true;
            return resultDto;
        }
        #region Privates
        private async Task<IQueryable<IdentityUser>> Filter(PageRequestDto dto)
        {
            var queryable = await _userService.GetQueryableAsync();

            if (dto.FilterRows == null) return queryable;

            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task SaveOrgUnitPermission(Guid userId, List<SelectOrgUnitDto> dtos)
        {
            await _orgUnitService.DeleteOrgUnitByUserIdAsync(userId);
            var orgUnitPermissions = dtos.Where(p => p.IsSelect == true)
                                        .Select(p => ObjectMapper.Map<SelectOrgUnitDto,OrgUnitPermission>(p))
                                        .ToList();
            await _orgUnitService.SaveOrgUnitByUserIdAsync(orgUnitPermissions);
            await this.RemoveCacheOrgUnitAttachUser(userId);
        }
        private bool IsGrantedReportGroup(MenuAccounting menu, string[] viewPermissions)
        {
            var reportGroup = _permissionDefinitionManager.GetGroups()
                                .Where(p => p.Name.Equals(PermissionGroup.Report))
                                .FirstOrDefault();

            var permissionDefines = reportGroup.Permissions.Where(p => p.Name.Equals(menu.ViewPermission))
                                    .FirstOrDefault();
            if (permissionDefines == null) return false;

            foreach (var reportPermission in permissionDefines.Children)
            {
                var viewPermission = reportPermission.Children
                                        .Where(p => p.Name.EndsWith($"_{ActionType.View}"))
                                        .FirstOrDefault();
                if (viewPermission == null) continue;
                if (viewPermissions.Contains(viewPermission.Name)) return true;
            }

            return false;
        }
        private async Task RemoveCacheOrgUnitAttachUser(Guid userId)
        {
            string key = string.Format(CacheKeyManager.ListByUserId, userId);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<SelectOrgUnitLoginDto>(key);
            await _cacheManager.RemoveAsync(cacheKey);
        }
        private async Task SendMailForProcessing(string to)
        {
            await _emailSender.SendAsync(
                to,     
                "Cảm ơn đã đăng ký sử dụng dịch vụ",         
                "Chúng tôi đang tiến hành xử lý yêu cầu của bạn"  
            );
        }
        private async Task SendMailForCompleted(string to, string pass, string userName, string url, string accessCode)
        {
            string body = $"Kính gửi Quý khách hàng,<br>" +
                $"Chúng tôi vừa tạo tài khoản cho Quý khách để truy cập Phần mềm kế toán MobiFone Accounting Solution của Tổng Công ty Viễn thông MobiFone với thông tin như sau:<br>" +                              
                $"Tên đăng nhập: {userName}<br>" +
                $"Mật khẩu:  {pass}<br>" +
                $"Quý khách vui lòng truy cập Hệ thống theo đường dẫn: https://{url}/ <br>" +
                $"Quý khách lưu ý, đây là email tự động từ hệ thống, vui lòng không trả lời lại email này!<br>" +
                $"MobiFone xin chân thành cảm ơn sự hợp tác và tin dùng của Quý khách hàng.<br>" +
                $"Trân trọng <br>";
            string subject = "Thông báo kết quả đăng ký";
            await _emailSender.SendAsync(
                to,     
                subject,         
                body  
            );
        }
        private static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        }

        #endregion
    }
}
