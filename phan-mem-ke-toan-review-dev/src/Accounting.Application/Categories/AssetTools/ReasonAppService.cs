using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Reasons;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
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
    public class ReasonAppService : AccountingAppService, IReasonAppService
    {
        #region Fields
        private readonly ReasonService _reasonService;
        private readonly DefaultReasonService _defaultReasonService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public ReasonAppService(ReasonService reasonService,
                        UserService userService,
                        LicenseBusiness licenseBusiness,
                        WebHelper webHelper,
                        LinkCodeBusiness linkCodeBusiness,
                        IStringLocalizer<AccountingResource> localizer,
                        DefaultReasonService defaultReasonService
            )
        {
            _reasonService = reasonService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _defaultReasonService = defaultReasonService;
        }
        #endregion
        [Authorize(AccountingPermissions.ReasonManagerCreate)]
        public async Task<ReasonDto> CreateAsync(CrudReasonDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudReasonDto, Reason>(dto);
            var result = await _reasonService.CreateAsync(entity);
            return ObjectMapper.Map<Reason, ReasonDto>(result);
        }

        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            var queryable = await _reasonService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        && (p.Code.Contains(dto.FilterValue) || p.Name.Contains(dto.FilterValue)))
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
        [Authorize(AccountingPermissions.ReasonManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _reasonService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ReasonCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Reason, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _reasonService.DeleteAsync(id);
        }

        public async Task<ReasonDto> GetByIdAsync(string reasonId)
        {
            var reason = await _reasonService.GetAsync(reasonId);
            return ObjectMapper.Map<Reason, ReasonDto>(reason);
        }
        [Authorize(AccountingPermissions.ReasonManagerView)]
        public async Task<PageResultDto<ReasonDto>> GetListAsync(PageRequestDto dto)
        {
            await InsertDefaultAsync();
            var result = new PageResultDto<ReasonDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Reason, ReasonDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.ReasonManagerView)]
        public Task<PageResultDto<ReasonDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.ReasonManagerUpdate)]
        public async Task UpdateAsync(string id, CrudReasonDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _reasonService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _reasonService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.ReasonCode, dto.Code, oldCode, entity.OrgCode);
            }
        }
        #region Private
        private async Task<IQueryable<Reason>> Filter(PageRequestDto dto)
        {
            var queryable = await _reasonService.GetQueryableAsync();
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
            bool isExists = await _reasonService.IsExistListAsync(orgCode);
            if (isExists) return;

            var defaultCapitals = await _defaultReasonService.GetListAsync();
            var entities = defaultCapitals.Select(p =>
            {
                var dto = new CrudReasonDto()
                {
                    Code = p.Code,
                    Name = p.Name,
                    Id = this.GetNewObjectId(),
                    OrgCode = orgCode,
                    ReasonType = p.ReasonType
                };
                return ObjectMapper.Map<CrudReasonDto, Reason>(dto);
            }).ToList();
            await _reasonService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        #endregion
    }
}
