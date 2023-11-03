using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Others;
using Accounting.Catgories.AccCases;
using Accounting.Common;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Jobs.CalcPrices;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.TenantEntities;
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
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Categories.AccCases
{
    public class AccCaseAppService : AccountingAppService, IAccCaseAppService
    {
        #region Fields
        private readonly AccCaseService _accCaseService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<AccCaseDto> _cache;
        private readonly IDistributedCache<PageResultDto<AccCaseDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly IServiceProvider _serviceProvider;
        #endregion
        #region Ctor
        public AccCaseAppService(AccCaseService accCaseService,
                            UserService userService,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<AccCaseDto> cache,
                            IDistributedCache<PageResultDto<AccCaseDto>> pageCache,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            LicenseBusiness licenseBusiness,
                            IStringLocalizer<AccountingResource> localizer,
                            IServiceProvider serviceProvider
                            )
        {
            _accCaseService = accCaseService;
            _userService = userService;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _licenseBusiness = licenseBusiness;
            _localizer = localizer;
            _serviceProvider = serviceProvider;
        }
        #endregion

        [Authorize(AccountingPermissions.CaseManagerCreate)]
        public async Task<AccCaseDto> CreateAsync(CrudAccCaseDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudAccCaseDto, AccCase>(dto);
            var result = await _accCaseService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<AccCase, AccCaseDto>(result);
        }

        [Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _accCaseService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.CaseCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccCase, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _accCaseService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                var entity = await _accCaseService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.CaseCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccCase, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }                
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _accCaseService.DeleteManyAsync(deleteIds);
            await RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<AccCaseDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<AccCaseDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );            
        }
        [Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<AccCaseDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AccCaseDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AccCase, AccCaseDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.CaseManagerUpdate)]
        public async Task UpdateAsync(string id, CrudAccCaseDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _accCaseService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _accCaseService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.CaseCode, dto.Code, oldCode, entity.OrgCode);
            }
            await RemoveAllCache();
        }
        [Authorize(AccountingPermissions.CaseManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudAccCaseDto>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {
                var checkAccase = await _accCaseService.GetByAccCaseAsync(item.Code, _webHelper.GetCurrentOrgUnit());
                if (checkAccase.Count == 0)
                {
                    item.Id = this.GetNewObjectId();
                    item.OrgCode = _webHelper.GetCurrentOrgUnit();
                    item.CreatorName = await _userService.GetCurrentUserNameAsync();
                }
                else
                {
                    throw new Exception("Mã vụ việc " + item.Code + " đã tồn tại");
                }

            }

            var lstAccCase = lstImport.Select(p => ObjectMapper.Map<CrudAccCaseDto, AccCase>(p))
                                .ToList();
            await _accCaseService.CreateManyAsync(lstAccCase);
            await RemoveAllCache();
            return new UploadFileResponseDto() { Ok = true };
        }
        public async Task<AccCaseDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () => 
                {
                    var accCase = await _accCaseService.GetAsync(caseId);
                    return ObjectMapper.Map<AccCase, AccCaseDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );
            
        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<AccCaseDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _accCaseService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
                    return partnes.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name
                    }).ToList();
                }
            );            
        }
        #region Private
        private async Task<IQueryable<AccCase>> Filter(PageRequestDto dto)
        {
            var queryable = await _accCaseService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _accCaseService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<AccCaseDto>();            
        }        
        #endregion
    }
}
