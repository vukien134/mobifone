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
    public class ToolAllocationAccountAppService : AccountingAppService, IAssetToolAccountAppService
    {
        #region Field
        private readonly AssetToolService _assetToolService;
        private readonly AssetToolAccountService _assetToolAccountService;
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
        public ToolAllocationAccountAppService(AssetToolService assetToolService,
                                AssetToolAccountService assetToolAccountService,
                                ExcelService excelService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper)
        {
            _assetToolService = assetToolService;
            _assetToolAccountService = assetToolAccountService;
            _excelService = excelService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<AssetToolAccountDto> CreateAsync(CrudAssetToolAccountDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Tool;
            var entity = ObjectMapper.Map<CrudAssetToolAccountDto, AssetToolAccount>(dto);
            try
            {
                var assetToolDetails = await _assetToolAccountService.GetByAssetToolIdAsync(dto.AssetToolId);
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolAccountService.DeleteManyAsync(assetToolDetails);
                var result = await _assetToolAccountService.CreateAsync(entity);
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(result);
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public async Task<List<AssetToolAccountDto>> CreateListAsync(ObjectListAssetToolAccountDto listDto)
        {
            await _licenseBusiness.CheckExpired();
            List<AssetToolAccountDto> listResult = new List<AssetToolAccountDto>();
            var listAssetToolAccount = from a in listDto.Data
                              group a.AssetToolId by a.AssetToolId into g
                              select new { AssetToolId = g.Key };
            var assetToolAccounts = await _assetToolAccountService.GetByAssetToolIdAsync(listDto.AssetToolId);
            if (assetToolAccounts != null)
            {
                await _assetToolAccountService.DeleteManyAsync(assetToolAccounts);
            }
            int ord = 1;
            foreach (var dto in listDto.Data)
            {
                dto.CreatorName = await _userService.GetCurrentUserNameAsync();
                dto.Id = GetNewObjectId();
                dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                dto.Year = _webHelper.GetCurrentYear();
                dto.Ord0 = "B" + ord.ToString().PadLeft(9, '0');
                var entity = ObjectMapper.Map<CrudAssetToolAccountDto, AssetToolAccount>(dto);
                var result = await _assetToolAccountService.CreateAsync(entity);
                listResult.Add(ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(result));
                ord++;
            }
            return listResult;
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolAccountService.DeleteAsync(id);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public Task<PageResultDto<AssetToolAccountDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<AssetToolAccountDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AssetToolAccountDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.AssetToolId).OrderBy(p => p.Year).OrderBy(p => p.Month).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudAssetToolAccountDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Tool;
            dto.Id = id;
            var entity = await _assetToolAccountService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolAccountService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<AssetToolAccountDto> GetByIdAsync(string assetToolAccountId)
        {
            var assetToolAccount = await _assetToolAccountService.GetAsync(assetToolAccountId);
            return ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(assetToolAccount);
        }
        public async Task<List<AssetToolAccountDto>> GetListByAssetToolIdAsync(string assetToolId)
        {
            var result = new List<AssetToolAccountDto>();
            var query = await _assetToolAccountService.GetQueryableAsync();
            query = query.Where(p => p.AssetToolId == assetToolId && p.Year == _webHelper.GetCurrentYear());
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(p)).ToList();
            return result;
        }
        #region Private
        private async Task<IQueryable<AssetToolAccount>> Filter(PageRequestDto dto)
        {
            var queryable = await _assetToolAccountService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                          && p.Year == _webHelper.GetCurrentYear()
                                          && p.AssetOrTool == AssetToolConst.Asset);
            return queryable;
        }
        #endregion
    }
}
