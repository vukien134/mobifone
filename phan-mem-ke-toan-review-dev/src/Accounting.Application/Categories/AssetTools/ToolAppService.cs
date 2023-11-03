using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.AssetTools;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Excels;
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
using Volo.Abp.Uow;

namespace Accounting.Categories.AssetTools
{
    public class ToolAppService : AccountingAppService, IToolAppService
    {
        #region Field
        private readonly AssetToolService _assetToolService;
        private readonly AssetToolGroupService _assetToolGroupService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly AssetToolStoppingDepreciationService _assetToolStoppingDepreciationService;
        private readonly AssetToolAccessoryService _assetToolAccessoryService;
        private readonly ExcelService _excelService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly CapitalService _capitalService;
        private readonly DepartmentService _departmentService;
        #endregion
        #region Ctor
        public ToolAppService(AssetToolService assetToolService,
                                AssetToolGroupService assetToolGroupService,
                                AssetToolDetailService assetToolDetailService,
                                AssetToolStoppingDepreciationService assetToolStoppingDepreciationService,
                                AssetToolAccessoryService assetToolAccessoryService,
                                ExcelService excelService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                CapitalService capitalService,
                                LinkCodeBusiness linkCodeBusiness,
                                DepartmentService departmentService,
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _assetToolService = assetToolService;
            _assetToolGroupService = assetToolGroupService;
            _assetToolDetailService = assetToolDetailService;
            _assetToolStoppingDepreciationService = assetToolStoppingDepreciationService;
            _assetToolAccessoryService = assetToolAccessoryService;
            _excelService = excelService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _capitalService = capitalService;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _departmentService = departmentService;
        }
        #endregion
        [Authorize(AccountingPermissions.ToolsManagerCreate)]
        public async Task<AssetToolDto> CreateAsync(CrudAssetToolDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Tool;
            var entity = ObjectMapper.Map<CrudAssetToolDto, AssetTool>(dto);

            var dataDetails = entity.AssetToolDetails.ToList();
            var capitalData = await _capitalService.GetQueryableAsync();
            var departmentData = await _departmentService.GetQueryableAsync();
            foreach (var data in dataDetails)
            {
                if (data.CapitalCode.IsNullOrEmpty() == false)
                {
                    if (capitalData.Where(x => x.Code == data.CapitalCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Capital, ErrorCode.NotFoundEntity),
                            _localizer["Err:CapitalData"]);
                    }
                }
                if(data.DepartmentCode.IsNullOrEmpty() == false)
                {
                    if (departmentData.Where(x => x.Code == data.DepartmentCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.NotFoundEntity),
                            _localizer["Err:DepartmentData"]);
                    }
                }               
            }
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _assetToolService.CreateAsync(entity);
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AssetTool, AssetToolDto>(result);
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }
        [Authorize(AccountingPermissions.ToolsManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _assetToolService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.AssetToolCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetTool, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _assetToolService.DeleteAsync(id);
            try
            {
                var assetToolDetails = await _assetToolDetailService.GetByAssetToolIdAsync(id);
                var assetToolAccessories = await _assetToolAccessoryService.GetByAssetToolIdAsync(id);
                var assetToolStoppingDepreciations = await _assetToolStoppingDepreciationService.GetByAssetToolIdAsync(id);

                using var unitOfWork = _unitOfWorkManager.Begin();
                if (assetToolDetails != null)
                {
                    await _assetToolDetailService.DeleteManyAsync(assetToolDetails);
                }
                if (assetToolAccessories != null)
                {
                    await _assetToolAccessoryService.DeleteManyAsync(assetToolAccessories);
                }
                if (assetToolStoppingDepreciations != null)
                {
                    await _assetToolStoppingDepreciationService.DeleteManyAsync(assetToolStoppingDepreciations);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }
        [Authorize(AccountingPermissions.ToolsManagerView)]
        public Task<PageResultDto<AssetToolDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.ToolsManagerView)]
        public async Task<PageResultDto<AssetToolDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AssetToolDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AssetTool, AssetToolDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.ToolsManagerUpdate)]
        public async Task UpdateAsync(string id, CrudAssetToolDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Tool;
            dto.Id = id;
            var entity = await _assetToolService.GetAsync(id);

            var dataDetails = dto.AssetToolDetails.ToList();
            var capitalData = await _capitalService.GetQueryableAsync();
            var departmentData = await _departmentService.GetQueryableAsync();
            foreach (var data in dataDetails)
            {
                if (data.CapitalCode.IsNullOrEmpty() == false)
                {
                    if (capitalData.Where(x => x.Code == data.CapitalCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Capital, ErrorCode.NotFoundEntity),
                            _localizer["Err:CapitalData"]);
                    }
                }
                if (data.DepartmentCode.IsNullOrEmpty() == false)
                {
                    if (departmentData.Where(x => x.Code == data.DepartmentCode).Any() == false)
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.NotFoundEntity),
                            _localizer["Err:DepartmentData"]);
                    }
                }
            }

            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            try
            {
                var assetToolDetails = await _assetToolDetailService.GetByAssetToolIdAsync(id);
                var assetToolAccessories = await _assetToolAccessoryService.GetByAssetToolIdAsync(id);
                var assetToolStoppingDepreciations = await _assetToolStoppingDepreciationService.GetByAssetToolIdAsync(id);

                using var unitOfWork = _unitOfWorkManager.Begin();
                if (assetToolDetails != null)
                {
                    await _assetToolDetailService.DeleteManyAsync(assetToolDetails);
                }
                if (assetToolAccessories != null)
                {
                    await _assetToolAccessoryService.DeleteManyAsync(assetToolAccessories);
                }
                if (assetToolStoppingDepreciations != null)
                {
                    await _assetToolStoppingDepreciationService.DeleteManyAsync(assetToolStoppingDepreciations);
                }

                await _assetToolService.UpdateAsync(entity);
                if (isChangeCode)
                {
                    await _linkCodeBusiness.UpdateCode(LinkCodeConst.AssetToolCode, dto.Code, oldCode, entity.OrgCode);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<AssetToolDto> GetByIdAsync(string assetToolId)
        {
            var assetTool = await _assetToolService.GetAsync(assetToolId);
            return ObjectMapper.Map<AssetTool, AssetToolDto>(assetTool);
        }

        public async Task<List<AssetToolDetailDto>> GetAssetToolDetailAsync(string productId)
        {
            var productUnits = await _assetToolDetailService.GetByAssetToolIdAsync(productId);
            var dtos = productUnits.Select(p => ObjectMapper.Map<AssetToolDetail, AssetToolDetailDto>(p)).ToList();
            return dtos;
        }

        public async Task<List<AssetToolAccessoryDto>> GetAssetToolAccessoryAsync(string productId)
        {
            var productPrices = await _assetToolAccessoryService.GetByAssetToolIdAsync(productId);
            var dtos = productPrices.Select(p => ObjectMapper.Map<AssetToolAccessory, AssetToolAccessoryDto>(p)).ToList();
            return dtos;
        }

        public async Task<List<AssetToolStoppingDepreciationDto>> GetAssetToolStoppingDepreciationAsync(string productId)
        {
            var productPrices = await _assetToolStoppingDepreciationService.GetByAssetToolIdAsync(productId);
            var dtos = productPrices.Select(p => ObjectMapper.Map<AssetToolStoppingDepreciation, AssetToolStoppingDepreciationDto>(p)).ToList();
            return dtos;
        }

        [Authorize(AccountingPermissions.AssetManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                var entity = await _assetToolService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.AssetToolCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetTool, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _assetToolService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            var partnes = await _assetToolService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue,
                                            AssetToolConst.Tool);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name,
                DataId = p.Id
            }).ToList();
        }

        #region Private
        private async Task<IQueryable<AssetTool>> Filter(PageRequestDto dto)
        {
            var queryable = await _assetToolService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                          && p.AssetOrTool == AssetToolConst.Tool);
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnName.Equals("assetToolGroupId"))
                {
                    string value = item.Value.ToString();
                    queryable = queryable.Where(p => p.AssetToolGroupId == value);
                    continue;
                }
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;            
        }
        #endregion
    }
}
