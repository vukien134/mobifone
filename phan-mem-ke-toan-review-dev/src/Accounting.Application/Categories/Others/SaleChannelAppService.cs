using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Others.SaleChannels;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Windows;
using Accounting.EntityFrameworkCore;
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

namespace Accounting.Categories.Others
{
    public class SaleChannelAppService : AccountingAppService, ISaleChannelAppService
    {
        #region Fields
        private readonly SaleChannelService _saleChannelService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly LinkCodeService _linkCodeService;
        private readonly AccountingDb _accountingDb;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public SaleChannelAppService(SaleChannelService saleChannelService,
                                UserService userService,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                LinkCodeService linkCodeService,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _saleChannelService = saleChannelService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _linkCodeService = linkCodeService;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.CaseManagerCreate)]
        public async Task<SaleChannelDto> CreateAsync(CrudSaleChannelDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudSaleChannelDto, SaleChannel>(dto);
            var result = await _saleChannelService.CreateAsync(entity);
            return ObjectMapper.Map<SaleChannel, SaleChannelDto>(result);
        }

        [Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _saleChannelService.GetAsync(id);
            bool isUsing = await _saleChannelService.IsParentGroup(entity.Id);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SaleChannel, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.SaleChannelCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SaleChannel, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _saleChannelService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _saleChannelService.GetAsync(item);
                bool isUsing = await _saleChannelService.IsParentGroup(entity.Id);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SaleChannel, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
                isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.SaleChannelCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SaleChannel, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _saleChannelService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        public async Task<SaleChannelDto> GetByIdAsync(string saleChannelId)
        {
            var saleChannel = await _saleChannelService.GetAsync(saleChannelId);
            return ObjectMapper.Map<SaleChannel, SaleChannelDto>(saleChannel);
        }

        [Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<SaleChannelDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<SaleChannelDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<SaleChannel, SaleChannelDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<BaseComboItemDto>> GetViewListAsync()
        {
            var items = await _saleChannelService.GetRepository()
                                .GetListAsync(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return items.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Code = p.Code,
                Name = p.Name,
                ParentId = p.ParentId,
                Value = p.Name
            })
            .OrderBy(p => p.Code).ToList();
        }

        [Authorize(AccountingPermissions.CaseManagerView)]
        public Task<PageResultDto<SaleChannelDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.CaseManagerUpdate)]
        public async Task UpdateAsync(string id, CrudSaleChannelDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _saleChannelService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _saleChannelService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.SaleChannelCode, dto.Code, oldCode, entity.OrgCode);
            }
        }
        #region Private
        private async Task<IQueryable<SaleChannel>> Filter(PageRequestDto dto)
        {
            var queryable = await _saleChannelService.GetQueryableAsync();
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
