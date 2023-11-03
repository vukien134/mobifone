using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Partners;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Partners
{
    public class PartnerGroupAppService : AccountingAppService, IPartnerGroupAppService
    {
        #region Fields
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerService _accPartnerService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly CacheManager _cacheManager;        
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public PartnerGroupAppService(PartnerGroupService partnerGroupService,
                                UserService userService,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                CacheManager cacheManager,
                                AccPartnerService accPartnerService,                                
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _partnerGroupService = partnerGroupService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _cacheManager = cacheManager;
            _accPartnerService = accPartnerService;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.PartnerGroupManagerCreate)]
        public async Task<PartnerGroupDto> CreateAsync(CrudPartnerGroupDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();            
            var entity = ObjectMapper.Map<CrudPartnerGroupDto, PartnerGroup>(dto);
            var result = await _partnerGroupService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<PartnerGroup, PartnerGroupDto>(result);
        }
        [Authorize(AccountingPermissions.PartnerGroupManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _partnerGroupService.GetAsync(id);
            bool isUsing = await this.IsPartnerGroupUsing(id);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.PartnerGroup, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }

            var partnerGroup = await _partnerGroupService.GetQueryableAsync();
            var lstPartnerGroupChild = partnerGroup.Where(p => p.ParentId == id).ToList();
            if (lstPartnerGroupChild.Count > 0)
            {
                foreach (var item in lstPartnerGroupChild)
                {
                    item.ParentId = null;
                    await _partnerGroupService.UpdateAsync(item, true);
                }
            }
            await _partnerGroupService.DeleteAsync(id,true);
            await this.RemoveAllCache();
        }
        [Authorize(AccountingPermissions.PartnerGroupManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                var entity = await _partnerGroupService.GetAsync(item);
                bool isUsing = await this.IsPartnerGroupUsing(item);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.PartnerGroup, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _partnerGroupService.DeleteManyAsync(deleteIds);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        public async Task<PartnerGroupDto> GetByIdAsync(string partnerGroupId)
        {
            var partnerGroup = await _partnerGroupService.GetAsync(partnerGroupId);
            return ObjectMapper.Map<PartnerGroup,PartnerGroupDto>(partnerGroup);
        }
        [Authorize(AccountingPermissions.PartnerGroupManagerView)]
        public async Task<PageResultDto<PartnerGroupDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<PartnerGroupDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<PartnerGroup, PartnerGroupDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<PartnerGroupTreeItemDto>> GetViewTreeAsync()
        {
            var partnerGroups = await _partnerGroupService.GetRepository().GetListAsync();
            var tree = new List<PartnerGroupTreeItemDto>();
            BuildTreeView(partnerGroups, null, tree);
            return tree;
        }
        public async Task<List<PartnerGroupComboItemDto>> GetViewListAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListByOrgCode, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<PartnerGroupComboItemDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {

                    var partnerGroups = await _partnerGroupService.GetRepository()
                                            .GetListAsync(p => p.OrgCode == orgCode);
                    return partnerGroups.Select(p => new PartnerGroupComboItemDto()
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
        [Authorize(AccountingPermissions.PartnerGroupManagerView)]
        public Task<PageResultDto<PartnerGroupDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.PartnerGroupManagerUpdate)]
        public async Task UpdateAsync(string id, CrudPartnerGroupDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if(dto.ParentId == id)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.Other),
                        _localizer["Err:ParentData"]);
            }
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();            
            var entity = await _partnerGroupService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _partnerGroupService.UpdateAsync(entity);
            await this.RemoveAllCache();
        }

        public async Task<List<PartnerGroup>> GetChildGroup(string code)
        {
            var lstPartnerGroup = new List<PartnerGroup>();
            var lstPartnerChildGroup = new List<PartnerGroup>();
            var iQPartnerGroup = await _partnerGroupService.GetQueryableAsync();
            var partnerGroup = iQPartnerGroup.Where(p => ((code ?? "") == "" && (p.ParentId ?? "") == "" || p.Code == code) && p.OrgCode == _webHelper.GetCurrentOrgUnit()).FirstOrDefault();
            if (partnerGroup == null) return lstPartnerGroup;
            lstPartnerGroup.Add(partnerGroup);
            lstPartnerChildGroup.Add(partnerGroup);
            while (iQPartnerGroup.Any(p => lstPartnerChildGroup.Select(p => p.Id).Contains(p.ParentId)))
            {
                lstPartnerChildGroup = iQPartnerGroup.Where(p => lstPartnerChildGroup.Select(p => p.Id).Contains(p.ParentId)).ToList();
                lstPartnerGroup.AddRange(lstPartnerChildGroup);
            }
            return lstPartnerGroup;
        }

        public async Task<List<PartnerGroupCustomineDto>> GetRankGroup(string code)
        {
            var lstPartnerGroup = new List<PartnerGroupCustomineDto>();
            var lstPartnerChildGroup = new List<PartnerGroupCustomineDto>();
            var iQPartnerGroup = await _partnerGroupService.GetQueryableAsync();
            var partnerGroup = ObjectMapper.Map(iQPartnerGroup.Where(p => ((code ?? "") == "" || p.Code == code) 
                                                                       && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                       && (p.ParentId ?? "") == "").ToList(), new List<PartnerGroupCustomineDto>());
            if (partnerGroup == null) return lstPartnerGroup;
            lstPartnerGroup.AddRange(partnerGroup);
            lstPartnerChildGroup.AddRange(partnerGroup);
            foreach (var item in lstPartnerChildGroup)
            {
                item.Rank = 1;
                item.OrdGroup = @"\" + item.Name + @"\";
            }
            int rank = 2;
            while (iQPartnerGroup.Any(p => lstPartnerChildGroup.Select(p => p.Id).Contains(p.ParentId)))
            {
                lstPartnerChildGroup = iQPartnerGroup.Where(p => lstPartnerChildGroup.Select(p => p.Id).Contains(p.ParentId)).Select(p => ObjectMapper.Map< PartnerGroup, PartnerGroupCustomineDto>(p)).ToList();
                foreach (var item in lstPartnerChildGroup)
                {
                    var ordGroupName = lstPartnerGroup.Where(p => p.Id == item.ParentId).FirstOrDefault();
                    item.OrdGroup = ordGroupName.OrdGroup + item.Name + @"\";
                    item.Rank = rank;
                }
                lstPartnerGroup.AddRange(lstPartnerChildGroup);
                rank++;
            }
            return lstPartnerGroup;
        }

        #region Private
        private async Task<IQueryable<PartnerGroup>> Filter(PageRequestDto dto)
        {
            var queryable = await _partnerGroupService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }
        private void BuildTreeView(List<PartnerGroup> partnerGroups, string parentId, List<PartnerGroupTreeItemDto> tree)
        {
            var groups = partnerGroups.Where(p => p.ParentId == parentId)
                                .OrderBy(p => p.Code).ToList();
            if (groups.Count == 0) return;

            foreach (var item in groups)
            {
                var child = new PartnerGroupTreeItemDto()
                {
                    Id = item.Id,
                    Value = item.Code,
                    Open = true,
                    Code = item.Code,
                    Name = item.Name
                };

                child.Data = new List<PartnerGroupTreeItemDto>();
                BuildTreeView(partnerGroups, item.Id, child.Data);
                if (child.Data.Count == 0)
                {
                    child.Data = null;
                    child.Open = null;
                }

                tree.Add(child);
            }
        }
        private async Task RemoveAllCache()
        {
            string key = string.Format(CacheKeyManager.ListByOrgCode, _webHelper.GetCurrentOrgUnit());
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<PartnerGroupComboItemDto>(key);
            await _cacheManager.RemoveAsync(cacheKey);
            await _cacheManager.RemoveClassCache<PartnerGroupDto>();
        }
        private async Task<bool> IsPartnerGroupUsing(string groupId)
        {
            var result = await _accPartnerService.IsExistPartnerGroupIdAsync(groupId);
            if (result == true) return result;
            result = await _partnerGroupService.IsParentGroup(groupId);
            return result;
        }
        #endregion
    }
}
