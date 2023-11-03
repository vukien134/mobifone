using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.AssetTools;
using Accounting.Constants;
using Accounting.DomainServices.Categories.AssetTools;
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
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.AssetTools
{
    public class AssetGroupAppService : AccountingAppService, IAssetGroupAppService
    {
        #region Fields
        private readonly AssetToolGroupService _assetToolGroupService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly AssetToolService _assetToolService;
        #endregion
        #region Ctor
        public AssetGroupAppService(AssetToolGroupService assetToolGroupService,
                            UserService userService,
                            WebHelper webHelper,
                            AccountingCacheManager accountingCacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            LicenseBusiness licenseBusiness,
                            IStringLocalizer<AccountingResource> localizer,
                            AssetToolService assetToolService
            )
        {
            _assetToolGroupService = assetToolGroupService;
            _userService = userService;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _licenseBusiness = licenseBusiness;
            _localizer = localizer;
            _assetToolService = assetToolService;
        }
        [Authorize(AccountingPermissions.AssetGroupManagerCreate)]
        public async Task<AssetToolGroupDto> CreateAsync(CrudAssetToolGroupDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.AssetOrTool = AssetToolConst.Asset;
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();            
            var entity = ObjectMapper.Map<CrudAssetToolGroupDto, AssetToolGroup>(dto);
            var result = await _assetToolGroupService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<AssetToolGroup, AssetToolGroupDto>(result);
        }
        [Authorize(AccountingPermissions.AssetGroupManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _assetToolGroupService.GetAsync(id);
            bool parentGroup = await _assetToolGroupService.IsParentGroup(id);
            if (parentGroup)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetToolGroup, ErrorCode.IsParentGroup),
                        _localizer["Err:CodeIsParentGroup", entity.Code]);
            }
            parentGroup = await _assetToolService.IsParentGroup(id);
            if (parentGroup)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetToolGroup, ErrorCode.IsParentGroup),
                        _localizer["Err:CodeIsParentGroup", entity.Code]);
            }
            await _assetToolGroupService.DeleteAsync(id);
            await this.RemoveAllCache();
        }
        [Authorize(AccountingPermissions.AssetGroupManagerView)]
        public async Task<PageResultDto<AssetToolGroupDto>> GetListAsync(PageRequestDto dto)
        {
            await this.InsertDefaultAsync();
            var result = new PageResultDto<AssetToolGroupDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AssetToolGroup, AssetToolGroupDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<BaseComboItemDto>> GetViewListAsync()
        {
            await this.InsertDefaultAsync();
            var assetToolGroups = await _accountingCacheManager.GetAssetToolGroupAsync(AssetToolConst.Asset);
            return assetToolGroups.Select(p => new BaseComboItemDto()
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                ParentId = p.ParentId,
                Value = p.Code                
            })
            .OrderBy(p => p.Code).ToList();
        }

        public Task<List<BaseComboItemDto>> GetViewListByCodeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<BaseComboItemDto>> GetViewTreeByCodeAsync()
        {
            throw new NotImplementedException();
        }
        [Authorize(AccountingPermissions.AssetGroupManagerView)]
        public Task<PageResultDto<AssetToolGroupDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.AssetGroupManagerUpdate)]
        public async Task UpdateAsync(string id, CrudAssetToolGroupDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.AssetOrTool = AssetToolConst.Asset;
            var entity = await _assetToolGroupService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _assetToolGroupService.UpdateAsync(entity);
            await this.RemoveAllCache();
        }
        public async Task<AssetToolGroupDto> GetByIdAsync(string assetGroupId)
        {
            var assetGroup = await _assetToolGroupService.GetAsync(assetGroupId);
            return ObjectMapper.Map<AssetToolGroup, AssetToolGroupDto>(assetGroup);
        }
        public async Task<List<AssetToolGroupCustomineDto>> GetRankGroup(string code)
        {
            var lstAssetToolGroup = new List<AssetToolGroupCustomineDto>();
            var lstAssetToolChildGroup = new List<AssetToolGroupCustomineDto>();
            var iQAssetToolGroup = await _assetToolGroupService.GetQueryableAsync();
            var assetToolGroup = ObjectMapper.Map(iQAssetToolGroup.Where(p => (((code ?? "") == "" && (p.ParentId ?? "") == "") || p.Code == code) && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList(), new List<AssetToolGroupCustomineDto>());
            if (assetToolGroup == null) return lstAssetToolGroup;
            lstAssetToolGroup.AddRange(assetToolGroup);
            lstAssetToolChildGroup.AddRange(assetToolGroup);
            foreach (var item in lstAssetToolChildGroup)
            {
                item.Rank = 1;
                item.OrdGroup = @"\" + item.Code + @"\";
            }
            int rank = 2;
            while (iQAssetToolGroup.Any(p => lstAssetToolChildGroup.Select(p => p.Id).Contains(p.ParentId)))
            {
                lstAssetToolChildGroup = iQAssetToolGroup.Where(p => lstAssetToolChildGroup.Select(p => p.Id).Contains(p.ParentId)).Select(p => ObjectMapper.Map<AssetToolGroup, AssetToolGroupCustomineDto>(p)).ToList();
                foreach (var item in lstAssetToolChildGroup)
                {
                    var ordGroupName = lstAssetToolGroup.Where(p => p.Id == item.ParentId).FirstOrDefault();
                    item.OrdGroup = ordGroupName.OrdGroup + item.Code + @"\";
                    item.Rank = rank;
                }
                lstAssetToolGroup.AddRange(lstAssetToolChildGroup);
                rank++;
            }
            return lstAssetToolGroup;
        }
        #endregion
        #region Private
        private async Task<IQueryable<AssetToolGroup>> Filter(PageRequestDto dto)
        {
            var queryable = await _assetToolGroupService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && p.AssetOrTool.Equals(AssetToolConst.Asset));
            return queryable;
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExists = await _assetToolGroupService.IsExistListAsync(orgCode,
                                            AssetToolConst.Asset);
            if (isExists) return;

            var defaultAssets = await _accountingCacheManager.GetDefaultAssetToolGroupAsync(AssetToolConst.Asset);
            defaultAssets = this.GetNewTreeId(defaultAssets);
            var entities = defaultAssets.Select(p =>
            {
                var entity = ObjectMapper.Map<DefaultAssetToolGroupDto, AssetToolGroup>(p);
                entity.OrgCode = orgCode;
                return entity;
            }).ToList();
            await _assetToolGroupService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        private List<DefaultAssetToolGroupDto> GetNewTreeId(List<DefaultAssetToolGroupDto> dtos)
        {
            foreach (var item in dtos)
            {
                string oldId = item.Id;
                item.Id = this.GetNewObjectId();
                var lstChilds = dtos.Where(p => p.ParentId == oldId);
                foreach (var child in lstChilds)
                {
                    child.ParentId = item.Id;
                }
            }
            return dtos;
        }
        private async Task RemoveAllCache()
        {
            await _accountingCacheManager.RemoveClassCache<AssetToolGroupDto>();
        }
        #endregion
    }
}
