using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.OrgUnits;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Users;
using Accounting.EntityFrameworkCore;
using Accounting.Exceptions;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;

namespace Accounting.Categories.OrgUnits
{
    public class OrgUnitAppService : AccountingAppService, IOrgUnitAppService
    {
        #region Fields
        private readonly OrgUnitService _orgUnitService;
        private readonly UserService _userService;
        private readonly CategoryDeleteService _categoryDeleteService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly DefaultAccountSystemService _defaultAccountSystemService;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly AccountingDb _accountingDb;
        private readonly WebHelper _webHelper;
        private readonly RegLicenseService _regLicenseService;
        private readonly ICurrentTenant _currentTenant;
        private readonly CustomerRegisterService _customerRegisterService;
        #endregion
        #region Ctor
        public OrgUnitAppService(OrgUnitService orgUnitService,
                            UserService userService,
                            CategoryDeleteService categoryDeleteService,
                            YearCategoryService yearCategoryService,
                            AccountSystemService accountSystemService,
                            DefaultAccountSystemService defaultAccountSystemService,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer,
                            LicenseBusiness licenseBusiness,
                            AccountingDb accountingDb,
                            WebHelper webHelper,
                            RegLicenseService regLicenseService,
                            ICurrentTenant currentTenant,
                            CustomerRegisterService customerRegisterService
                        )
        {
            _orgUnitService = orgUnitService;
            _userService = userService;
            _categoryDeleteService = categoryDeleteService;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _defaultAccountSystemService = defaultAccountSystemService;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _licenseBusiness = licenseBusiness;
            _accountingDb = accountingDb;
            _webHelper = webHelper;
            _regLicenseService = regLicenseService;
            _currentTenant = currentTenant;
            _customerRegisterService = customerRegisterService;
        }
        #endregion
        #region Methods
        [Authorize(AccountingPermissions.OrgUnitManagerCreate)]
        public async Task<OrgUnitDto> CreateAsync(CrudOrgUnitDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var checkReg = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            var numCompanyReg = checkReg.CompanyQuantity;
            var numCompanyCurrent = await _orgUnitService.GetAllAsync();
            var message = new AccountingException(ErrorCode.Get(GroupErrorCodes.OrgUnit, ErrorCode.Other),
                            _localizer["Err:Overload"]);
            if (numCompanyReg > 1)
            {
                if (numCompanyCurrent.Count() < numCompanyReg)
                {
                    return await this.Create(dto);
                }
                else
                {
                    throw message;
                }               
            }
            if(numCompanyReg == 1)
            {
                if (numCompanyCurrent.Count() < 2)
                {
                    if(numCompanyCurrent.FirstOrDefault().TaxCode == dto.TaxCode && numCompanyCurrent.FirstOrDefault().Name.Trim() == dto.Name.Trim())
                    {
                       return await this.Create(dto);
                    }
                    else
                    {
                        throw message;
                    }
                }
                else
                {
                    throw message;
                }      
            }
            return null;           
        }
        [Authorize(AccountingPermissions.OrgUnitManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _orgUnitService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.OrgUnitCode, entity.Code, "");
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.OrgUnit, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            if (entity.Code.Equals(orgCode))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.OrgUnit, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            var orgCodeDelete = entity.Code;
            string sqlTable = $"select table_name from information_schema.Columns WHERE Column_Name='OrgCode'";
            var dataTable = await _accountingDb.GetDataTableAsync(sqlTable);
            var dataRow = dataTable.Rows;
            var lstTabNameCheckDelete = (await _categoryDeleteService.GetQueryableAsync()).Select(p => p.TabName).ToList();
            for (int i = 0; i < dataRow.Count; i++)
            {
                var dict = new Dictionary<string, object>();
                var table_name = dataRow[i][0];
                if (lstTabNameCheckDelete.Contains(table_name))
                {
                    string sqlCheckData = $"select 1 from \"{table_name}\" WHERE \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCodeDelete);
                    var dataCheckData = await _accountingDb.GetDataTableAsync(sqlCheckData, dict);
                    if (dataCheckData.Rows.Count > 0) throw new Exception($"Đơn vị cơ sở {orgCodeDelete} đã có phát sinh ở bảng {table_name}");
                }
                else
                {
                    string sqlCheckData = $"delete from \"{table_name}\" WHERE \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCodeDelete);
                    await _accountingDb.ExecuteSQLAsync(sqlCheckData, dict);
                }
            }
            await _orgUnitService.DeleteAsync(id);
            await this.RemoveAllCache();
        }
        [Authorize(AccountingPermissions.OrgUnitManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }
            string orgCode = _webHelper.GetCurrentOrgUnit();
            foreach (var item in dto.ListId)
            {
                var entity = await _orgUnitService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.OrgUnitCode, entity.Code, "");
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.OrgUnit, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
                if (entity.Code.Equals(orgCode))
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.OrgUnit, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _orgUnitService.DeleteManyAsync(deleteIds);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }
        [Authorize(AccountingPermissions.OrgUnitManagerView)]
        public Task<PageResultDto<OrgUnitDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.OrgUnitManagerView)]
        public async Task<PageResultDto<OrgUnitDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<OrgUnitDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<OrgUnit,OrgUnitDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<OrgUnitDto> GetByIdAsync(string orgUnitId)
        {
            var orgUnit = await _orgUnitService.GetByIdAsync(orgUnitId);
            return ObjectMapper.Map<OrgUnit,OrgUnitDto>(orgUnit);
        }
        public async Task<List<OrgUnitDto>> GetByCurrentUser()
        {
            var queryable = await _orgUnitService.GetQueryableAsync();
            var orgunits = await AsyncExecuter.ToListAsync(queryable);
            return orgunits.Select(p => ObjectMapper.Map<OrgUnit, OrgUnitDto>(p)).ToList();
        }
        public async Task<List<SelectOrgUnitLoginDto>> GetSelectToLoginAsync()
        {
            var userId = _userService.GetCurrentUserId();
            //string key = string.Format(CacheKeyManager.ListByUserId, userId);
            //string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<SelectOrgUnitLoginDto>(key);
            //return await _cacheManager.GetOrAddAsync(
            //    cacheKey,
            //    async () =>
            //    {
            //        var orgunits = await _orgUnitService.GetOrgUnitForLogin(userId.Value);
            //        return orgunits.Select(p => new SelectOrgUnitLoginDto()
            //        {
            //            Id = p.Code,
            //            Name = p.Name,
            //            TaxCode = p.TaxCode,
            //            Address = p.Address
            //        }).OrderBy(p => p.Id).ToList();
            //    }
            //);

            var orgunits = await _orgUnitService.GetOrgUnitForLogin(userId.Value);
            return orgunits.Select(p => new SelectOrgUnitLoginDto()
            {
                Id = p.Code,
                Name = p.Name,
                TaxCode = p.TaxCode,
                Address = p.Address
            }).OrderBy(p => p.Id).ToList();
        }
        [Authorize(AccountingPermissions.OrgUnitManagerUpdate)]
        public async Task UpdateAsync(string id, CrudOrgUnitDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var checkReg = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            var numCompanyReg = checkReg.CompanyQuantity;
            var numCompanyCurrent = await _orgUnitService.GetAllAsync();           
            if (numCompanyReg > 1)
            {
                await this.Update(id,dto);
            }
            if (numCompanyReg == 1)
            {
                if(numCompanyCurrent.Count() == 1)
                {
                    await this.Update(id, dto);
                }             
                else
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.OrgUnit, ErrorCode.Other),
                            _localizer["Err:Overload"]); ;
                }  
            }
        }        
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            var partnes = await _orgUnitService.GetAllAsync();
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        #endregion
        #region Private Methods
        private async Task Update(string id, CrudOrgUnitDto dto)
        {
            await this.ValidLicAsync("Update", dto);
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            var orgUnit = await _orgUnitService.GetAsync(id);
            ObjectMapper.Map(dto, orgUnit);
            await _orgUnitService.UpdateAsync(orgUnit);
            await this.RemoveAllCache();
        }
        private async Task<OrgUnitDto> Create(CrudOrgUnitDto dto)
        {
            await this.ValidLicAsync("Insert", dto);
            {
                dto.CreatorName = await _userService.GetCurrentUserNameAsync();
                dto.Id = this.GetNewObjectId();
                var orgUnit = ObjectMapper.Map<CrudOrgUnitDto, OrgUnit>(dto);
                var result = await _orgUnitService.CreateAsync(orgUnit);
                await this.InsertOrgUnitPermission(dto);
                await this.RemoveAllCache();
                return ObjectMapper.Map<OrgUnit, OrgUnitDto>(result);
            }
        }
        private async Task<IQueryable<OrgUnit>> Filter(PageRequestDto dto)
        {
            var queryable = await _orgUnitService.GetQueryableAsync();
            if (dto.FilterRows == null) return queryable;

            foreach (var item in dto.FilterRows)
            {
                FilterOperator filterOperator = item.ColumnType == "guid" ? FilterOperator.Equal
                                                    : FilterOperator.Contains;
                queryable = queryable.Where(item.ColumnName, item.Value, filterOperator);
            }
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<SelectOrgUnitLoginDto>();
        }
        private async Task InsertOrgUnitPermission(CrudOrgUnitDto dto)
        {
            if (dto.AutoOrgPermission != true) return;

            bool isAssign = await _orgUnitService.IsAssignOrgPermissionAsync(dto.Id);
            if (isAssign) return;

            var userId = _userService.GetCurrentUserId();
            var orgPermissionDto = new CrudOrgUnitPermissionDto()
            {
                OrgUnitId = dto.Id,
                UserId = userId.Value,
                Id = this.GetNewObjectId()
            };
            var entity = ObjectMapper.Map<CrudOrgUnitPermissionDto, OrgUnitPermission>(orgPermissionDto);
            await _orgUnitService.InsertOrgUnitPermissionAsync(entity);
            await this.RemoveCacheOrgUnitAttachUser(userId.Value);
        }
        private async Task RemoveCacheOrgUnitAttachUser(Guid userId)
        {
            string key = string.Format(CacheKeyManager.ListByUserId, userId);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<SelectOrgUnitLoginDto>(key);
            await _cacheManager.RemoveAsync(cacheKey);
        }
        private async Task ValidLicAsync(string action,CrudOrgUnitDto dto)
        {
            bool isDemo = await _licenseBusiness.IsDemo();
            if (isDemo) return;
            var regDto = await _licenseBusiness.GetInfoRegLicense();
            var orgUnits = await _orgUnitService.GetAllAsync();
            var groupUnits = orgUnits.GroupBy(g => new { g.TaxCode, g.Name })
                                .Select(p => new OrgUnitDto()
                                {
                                    TaxCode = p.Key.TaxCode,
                                    Name = p.Key.Name
                                }).ToList();
            if (action.Equals("Insert"))
            {             
                if (groupUnits.Count >= regDto.CompanyQuantity)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.Other),
                            _localizer["Err:OverCompanyRegister"]);
                }
            }
            if (action.Equals("Update"))
            {
                int countVoucher = await _orgUnitService.CountAync();
                if (regDto.CompanyQuantity == 1 && groupUnits.Count == 1
                && (regDto.TaxCode != groupUnits[0].TaxCode || regDto.Name != groupUnits[0].Name))
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.Other),
                            _localizer["Err:TaxCodeOrNameNotSameInOrgUnit"]);
                }
            }
        }
        #endregion
    }
}
