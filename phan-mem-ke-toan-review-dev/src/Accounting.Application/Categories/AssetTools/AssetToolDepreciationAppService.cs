using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines.AssetTool;
using Accounting.Business;
using Accounting.Catgories.AssetTools;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolDepreciationAppService : AccountingAppService, IAssetToolDepreciationAppService
    {
        #region Field
        private readonly AssetToolService _assetToolService;
        private readonly AssetToolDepreciationService _assetToolDepreciationService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly AssetToolStoppingDepreciationService _assetToolStoppingDepreciationService;
        private readonly AssetToolAccessoryService _assetToolAccessoryService;
        private readonly ExcelService _excelService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        #endregion
        #region Ctor
        public AssetToolDepreciationAppService(AssetToolService assetToolService,
                                AssetToolDepreciationService assetToolDepreciationService,
                                ExcelService excelService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper)
        {
            _assetToolService = assetToolService;
            _assetToolDepreciationService = assetToolDepreciationService;
            _excelService = excelService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<AssetToolDepreciationDto> CreateAsync(CrudAssetToolDepreciationDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            var entity = ObjectMapper.Map<CrudAssetToolDepreciationDto, AssetToolDepreciation>(dto);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _assetToolDepreciationService.CreateAsync(entity);
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AssetToolDepreciation, AssetToolDepreciationDto>(result);
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolDepreciationService.DeleteAsync(id);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public Task<PageResultDto<AssetToolDepreciationDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<AssetToolDepreciationDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AssetToolDepreciationDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.AssetToolId).OrderBy(p => p.Year).OrderBy(p => p.Month).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AssetToolDepreciation, AssetToolDepreciationDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudAssetToolDepreciationDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Asset;
            dto.Id = id;
            var entity = await _assetToolDepreciationService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolDepreciationService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<AssetToolDepreciationDto> GetByIdAsync(string assetToolDepreciationId)
        {
            var assetToolDepreciation = await _assetToolDepreciationService.GetAsync(assetToolDepreciationId);
            return ObjectMapper.Map<AssetToolDepreciation, AssetToolDepreciationDto>(assetToolDepreciation);
        }
        #region Private
        private async Task<IQueryable<AssetToolDepreciation>> Filter(PageRequestDto dto)
        {
            var queryable = await _assetToolDepreciationService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                          && p.Year == _webHelper.GetCurrentYear()
                                          && p.AssetOrTool == AssetToolConst.Asset);
            return queryable;
        }
        #endregion
    }
}
