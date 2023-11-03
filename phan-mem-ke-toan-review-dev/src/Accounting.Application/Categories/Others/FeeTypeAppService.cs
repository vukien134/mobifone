using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Others.FeeTypes;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class FeeTypeAppService : AccountingAppService, IFeeTypeAppService
    {
        #region Fields
        private readonly FeeTypeService _feeTypeService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        #endregion
        #region Ctor
        public FeeTypeAppService(FeeTypeService feeTypeService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper
                            )
        {
            _feeTypeService = feeTypeService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion

        [Authorize(AccountingPermissions.FeeTypeManagerCreate)]
        public async Task<FeeTypeDto> CreateAsync(CrudFeeTypeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudFeeTypeDto, FeeType>(dto);
            var result = await _feeTypeService.CreateAsync(entity);
            return ObjectMapper.Map<FeeType, FeeTypeDto>(result);
        }

        [Authorize(AccountingPermissions.FeeTypeManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _feeTypeService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.FeeTypeManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }

        [Authorize(AccountingPermissions.FeeTypeManagerView)]
        public Task<PageResultDto<FeeTypeDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.FeeTypeManagerView)]
        public async Task<PageResultDto<FeeTypeDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<FeeTypeDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<FeeType, FeeTypeDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.FeeTypeManagerUpdate)]
        public async Task UpdateAsync(string id, CrudFeeTypeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _feeTypeService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _feeTypeService.UpdateAsync(entity);
        }
        public async Task<FeeTypeDto> GetByIdAsync(string feeTypeId)
        {
            var entity = await _feeTypeService.GetAsync(feeTypeId);
            return ObjectMapper.Map<FeeType, FeeTypeDto>(entity);
        }
        #region Private
        private async Task<IQueryable<FeeType>> Filter(PageRequestDto dto)
        {
            var queryable = await _feeTypeService.GetQueryableAsync();

            if (dto.FilterRows == null) return queryable;

            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());


            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                queryable = queryable.Where(p => p.Code.Contains(dto.QuickSearch) || p.Name.Contains(dto.QuickSearch));
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
            }
            return queryable;
        }
        #endregion
    }
}
