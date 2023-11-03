using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Purposes;
using Accounting.Common.Extensions;
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

namespace Accounting.Categories.AssetTools
{
    public class PurposeAppService : AccountingAppService, IPurposeAppService
    {
        #region Fields
        private readonly PurposeService _purposeService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public PurposeAppService(PurposeService purposeService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
            )
        {
            _purposeService = purposeService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.PurposeManagerCreate)]
        public async Task<PurposeDto> CreateAsync(CrudPurposeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudPurposeDto, Purpose>(dto);
            var result = await _purposeService.CreateAsync(entity);
            return ObjectMapper.Map<Purpose, PurposeDto>(result);
        }

        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            string orgCode = _webHelper.GetCurrentOrgUnit();

            var partnes = await _purposeService.GetDataReference(orgCode, filterValue);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        [Authorize(AccountingPermissions.PurposeManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _purposeService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.PurposeCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Purpose, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _purposeService.DeleteAsync(id);
        }

        public async Task<PurposeDto> GetByIdAsync(string purposeId)
        {
            var purpose = await _purposeService.GetAsync(purposeId);
            return ObjectMapper.Map<Purpose, PurposeDto>(purpose);
        }
        [Authorize(AccountingPermissions.PurposeManagerView)]
        public async Task<PageResultDto<PurposeDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<PurposeDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Purpose, PurposeDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.PurposeManagerView)]
        public Task<PageResultDto<PurposeDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.PurposeManagerUpdate)]
        public async Task UpdateAsync(string id, CrudPurposeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _purposeService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _purposeService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.PurposeCode, dto.Code, oldCode, entity.OrgCode);
            }
        }
        #region Private
        private async Task<IQueryable<Purpose>> Filter(PageRequestDto dto)
        {
            var queryable = await _purposeService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        #endregion
    }
}
