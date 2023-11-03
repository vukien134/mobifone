using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Others;
using Accounting.Catgories.Sections;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Accounting.Categories.Sections
{
    public class AccSectionAppService : AccountingAppService, IAccSectionAppService
    {
        #region Fields
        private readonly AccSectionService _accSectionService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly RegLicenseService _regLicenseService;
        private readonly ExcelService _excelService;
        private readonly CacheManager _cacheManager;
        private readonly IDistributedCache<AccSectionDto> _cache;
        private readonly ICurrentTenant _currentTenant;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly DefaultAccSectionService _defualtAccSectionService;
        #endregion
        #region Ctor
        public AccSectionAppService(AccSectionService accSectionService,
                                UserService userService,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                RegLicenseService regLicenseService,
                                ExcelService excelService,
                                CacheManager cacheManager,
                                IDistributedCache<AccSectionDto> cache,
                                ICurrentTenant currentTenant,
                                TenantExtendInfoService tenantExtendInfoService,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer,
                                DefaultAccSectionService    defaultAccSectionService
            )
        {
            _accSectionService = accSectionService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _regLicenseService = regLicenseService;
            _excelService = excelService;
            _cacheManager = cacheManager;
            _cache = cache;
            _currentTenant = currentTenant;
            _tenantExtendInfoService = tenantExtendInfoService;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _defualtAccSectionService = defaultAccSectionService;
        }


        #endregion
        #region Public Methods
        [Authorize(AccountingPermissions.SectionManagerView)]
        public async Task<PageResultDto<AccSectionDto>> PagesAsync(PageRequestDto dto)
        {
            await InsertDefaultData();
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<AccSectionDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }
        [Authorize(AccountingPermissions.SectionManagerView)]
        public async Task<PageResultDto<AccSectionDto>> GetListAsync(PageRequestDto dto)
        {
            await this.InsertDefaultAsync();
            var result = new PageResultDto<AccSectionDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AccSection, AccSectionDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.SectionManagerCreate)]
        public async Task<AccSectionDto> CreateAsync(CruAccSectionDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var section = ObjectMapper.Map<CruAccSectionDto, AccSection>(dto);
            var result = await _accSectionService.CreateAsync(section);
            await _cacheManager.RemoveClassCache<AccSectionDto>();
            return ObjectMapper.Map<AccSection, AccSectionDto>(result);
        }
        [Authorize(AccountingPermissions.SectionManagerUpdate)]
        public async Task UpdateAsync(string id, CruAccSectionDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var section = await _accSectionService.GetAsync(id);
            string oldCode = section.Code;
            bool isChangeCode = dto.Code == section.Code ? false : true;
            ObjectMapper.Map(dto, section);
            await _accSectionService.UpdateAsync(section);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.SectionCode, dto.Code, oldCode, section.OrgCode);
            }
            await _cacheManager.RemoveClassCache<AccSectionDto>();
        }
        [Authorize(AccountingPermissions.SectionManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _accSectionService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.SectionCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccSection, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _accSectionService.DeleteAsync(id);
            await _cacheManager.RemoveClassCache<AccSectionDto>();
        }

        [Authorize(AccountingPermissions.SectionManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                var entity = await _accSectionService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.SectionCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccSection, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _accSectionService.DeleteManyAsync(deleteIds);
            await RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }
        public async Task<AccSectionDto> GetByIdAsync(string sectionId)
        {
            return await _cache.GetOrAddAsync(
                sectionId,
                async () =>
                {
                    var section = await _accSectionService.GetAsync(sectionId);
                    return ObjectMapper.Map<AccSection, AccSectionDto>(section);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(CacheConst.CacheTime)
                }
            );
            
        }
        public async Task<List<AccSectionComboItemDto>> DataReference(ComboRequestDto dto)
        {
            await this.InsertDefaultAsync();
            string filterValue = $"%{dto.FilterValue}%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<AccSectionDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _accSectionService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
                    return partnes.Select(p => new AccSectionComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name,
                        AttachProductCost = p.AttachProductCost,
                        SectionType = p.SectionType
                    }).ToList();
                }
            );            
        }
        public async Task<List<AccSectionComboItemDto>> GetDataReference()
        {
            var partnes = await _accSectionService.GetAll(_webHelper.GetCurrentOrgUnit());
            return partnes.Select(p => new AccSectionComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name,
                AttachProductCost = p.AttachProductCost,
                SectionType = p.SectionType
            }).ToList();
        }
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CruAccSectionDto>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();


            }

            var lstAccSection = lstImport.Select(p => ObjectMapper.Map<CruAccSectionDto, AccSection>(p))
                                .ToList();
            await _accSectionService.CreateManyAsync(lstAccSection);
            await _cacheManager.RemoveClassCache<AccSectionDto>();
            return new UploadFileResponseDto() { Ok = true };
        }


        #endregion
        #region Private Methods
        private async Task<IQueryable<AccSection>> Filter(PageRequestDto dto)
        {
            var queryable = await _accSectionService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _accSectionService.GetQueryableQuickSearch(queryable, filterValue);
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExists = await _accSectionService.IsExistListAsync(orgCode);
            if (isExists) return;

            int? tenantType = await this.GetTenantType();
            if (tenantType != 2) return;

            foreach(var item in SectionConst.Default)
            {
                var dto = new CruAccSectionDto()
                {
                    Id = this.GetNewObjectId(),
                    AttachProductCost = "K",
                    Code = item.Key,
                    Name = item.Value,
                    OrgCode = orgCode
                };
                var entity = ObjectMapper.Map<CruAccSectionDto, AccSection>(dto);
                await _accSectionService.CreateAsync(entity);
            }
        }
        private async Task<int?> GetTenantType()
        {
            var tenantInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantInfo == null) return null;
            return tenantInfo.TenantType;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<AccSectionDto>();
        }

        private async Task InsertDefaultData()
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var regLic = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            if (await IsDataCategoryExist(orgCode) == false && regLic.CompanyType == "Household")
            {
                var data = await this._defualtAccSectionService.GetList();
                var dtos = data.Select(p => new CruAccSectionDto()
                {
                    AttachProductCost = p.AttachProductCost,
                    Code = p.Code,
                    Id = p.Id,
                    Name = p.Name,
                    OrgCode=orgCode,
                    SectionType = p.SectionType
                }).ToList();
                foreach (var item in dtos)
                {
                    var entity = ObjectMapper.Map<CruAccSectionDto, AccSection>(item);
                    await _accSectionService.CreateAsync(entity);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
                await this.RemoveAllCache();
            };
        }
        private async Task<bool> IsDataCategoryExist(string orgCode)
        {
            var queryable = await _accSectionService.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode);
        }
        #endregion
    }
}
