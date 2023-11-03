using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Capitals;
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
using Volo.Abp.MultiTenancy;

namespace Accounting.Categories.AssetTools
{
    public class CapitalAppService : AccountingAppService, ICapitalAppService
    {
        #region Fields
        private readonly CapitalService _capitalService;
        private readonly DefaultCapitalService _defaultCapitalService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly CurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public CapitalAppService(CapitalService capitalService,
                                UserService userService,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer,
                                DefaultCapitalService defaultCapitalService,
                                CurrentTenant currentTenant
            )
        {
            _capitalService = capitalService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _defaultCapitalService = defaultCapitalService;
            _currentTenant = currentTenant;
        }
        #endregion
        [Authorize(AccountingPermissions.CapitalManagerCreate)]
        public async Task<CapitalDto> CreateAsync(CrudCapitalDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudCapitalDto, Capital>(dto);
            var result = await _capitalService.CreateAsync(entity);
            return ObjectMapper.Map<Capital, CapitalDto>(result);
        }

        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            var queryable = await _capitalService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        && (p.Code.Contains(dto.FilterValue.ToUpper()) || p.Name.Replace(" ","").ToUpper().Contains(dto.FilterValue.Replace(" ","").ToUpper())))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        [Authorize(AccountingPermissions.CapitalManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _capitalService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.CapitalCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Capital, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _capitalService.DeleteAsync(id);
        }

        public async Task<CapitalDto> GetByIdAsync(string capitalId)
        {
            var capital = await _capitalService.GetAsync(capitalId);
            return ObjectMapper.Map<Capital, CapitalDto>(capital);
        }
        [Authorize(AccountingPermissions.CapitalManagerView)]
        public async Task<PageResultDto<CapitalDto>> GetListAsync(PageRequestDto dto)
        {
            await InsertDefaultAsync();
            var result = new PageResultDto<CapitalDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Capital, CapitalDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.CapitalManagerView)]
        public Task<PageResultDto<CapitalDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.CapitalManagerUpdate)]
        public async Task UpdateAsync(string id, CrudCapitalDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _capitalService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _capitalService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.CapitalCode, dto.Code, oldCode, entity.OrgCode);
            }
        }
        #region Private
        private async Task<IQueryable<Capital>> Filter(PageRequestDto dto)
        {
            var queryable = await _capitalService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
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
            bool isExists = await _capitalService.IsExistListAsync(orgCode);
            if (isExists) return;

            var defaultCapitals = await _defaultCapitalService.GetListAsync();            
            var entities = defaultCapitals.Select(p =>
            {
                var dto = new CrudCapitalDto()
                {
                    Code = p.Code,
                    Name = p.Name,
                    Id = this.GetNewObjectId(),
                    OrgCode = orgCode
                };
                return ObjectMapper.Map<CrudCapitalDto,Capital>(dto);
            }).ToList();
            await _capitalService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        #endregion
    }
}
