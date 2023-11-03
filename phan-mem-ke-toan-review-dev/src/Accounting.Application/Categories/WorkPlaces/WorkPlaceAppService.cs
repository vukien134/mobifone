using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Others;
using Accounting.Catgories.WorkPlace;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.WorkPlaces
{
    public class WorkPlaceAppService : AccountingAppService, IWorkPlaceAppService
    {
        #region Fields
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly IDistributedCache<WorkPlaceDto> _cache;
        #endregion
        #region Ctor
        public WorkPlaceAppService(WorkPlaceSevice workPlaceSevice,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer,
                            IDistributedCache<WorkPlaceDto> cache
            )
        {
            _workPlaceSevice = workPlaceSevice;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _cache = cache;
        }
        #endregion
        [Authorize(AccountingPermissions.WorkPlaceManagerCreate)]
        public async Task<WorkPlaceDto> CreateAsync(CrudWokPlaceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudWokPlaceDto, WorkPlace>(dto);
            var result = await _workPlaceSevice.CreateAsync(entity);
            return ObjectMapper.Map<WorkPlace, WorkPlaceDto>(result);
        }

        [Authorize(AccountingPermissions.WorkPlaceManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _workPlaceSevice.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.WorkPlaceCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.WorkPlace, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _workPlaceSevice.DeleteAsync(id);
            await _cache.RemoveAsync(id);
        }

        [Authorize(AccountingPermissions.WorkPlaceManagerView)]
        public Task<PageResultDto<WorkPlaceDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }        
        [Authorize(AccountingPermissions.WorkPlaceManagerView)]
        public async Task<PageResultDto<WorkPlaceDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<WorkPlaceDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<WorkPlace, WorkPlaceDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.WorkPlaceManagerUpdate)]
        public async Task UpdateAsync(string id, CrudWokPlaceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _workPlaceSevice.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _workPlaceSevice.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.WorkPlaceCode, dto.Code, oldCode, entity.OrgCode);
            }
            await _cache.RemoveAsync(entity.Id);
        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            var queryable = await _workPlaceSevice.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                .OrderBy(p => p.Code);
            var sections = await AsyncExecuter.ToListAsync(queryable);
            return sections.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        public async Task<WorkPlaceDto> GetByIdAsync(string workPlaceId)
        {
            return await _cache.GetOrAddAsync(
                workPlaceId, //Cache key
                async () =>
                {
                    var accCase = await _workPlaceSevice.GetAsync(workPlaceId);
                    return ObjectMapper.Map<WorkPlace, WorkPlaceDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );
        }
        #region Private
        private async Task<IQueryable<WorkPlace>> Filter(PageRequestDto dto)
        {
            var queryable = await _workPlaceSevice.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }
        #endregion
    }
}
