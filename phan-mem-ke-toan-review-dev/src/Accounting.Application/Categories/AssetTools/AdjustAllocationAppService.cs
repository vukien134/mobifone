using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Catgories.AdjustDepreciations;
using Accounting.Constants;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.AssetTools
{
    public class AdjustAllocationAppService : AccountingAppService, IAdjustDepreciationAppService
    {
        #region Field
        private readonly AdjustDepreciationService _adjustDepreciationService;
        private readonly AssetToolGroupService _assetToolGroupService;
        private readonly AdjustDepreciationDetailService _adjustDepreciationDetailService;
        private readonly AssetToolStoppingDepreciationService _assetToolStoppingDepreciationService;
        private readonly AssetToolAccessoryService _assetToolAccessoryService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly ExcelService _excelService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        #endregion
        #region Ctor
        public AdjustAllocationAppService(AdjustDepreciationService adjustDepreciationService,
                                AssetToolGroupService assetToolGroupService,
                                AdjustDepreciationDetailService adjustDepreciationDetailService,
                                AssetToolStoppingDepreciationService assetToolStoppingDepreciationService,
                                AssetToolAccessoryService assetToolAccessoryService,
                                AssetToolDetailService assetToolDetailService,
                                ExcelService excelService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper)
        {
            _adjustDepreciationService = adjustDepreciationService;
            _assetToolGroupService = assetToolGroupService;
            _adjustDepreciationDetailService = adjustDepreciationDetailService;
            _assetToolStoppingDepreciationService = assetToolStoppingDepreciationService;
            _assetToolAccessoryService = assetToolAccessoryService;
            _assetToolDetailService = assetToolDetailService;
            _excelService = excelService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<AdjustDepreciationDto> CreateAsync(CrudAdjustDepreciationDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.AssetOrTool = AssetToolConst.Tool;
            var entity = ObjectMapper.Map<CrudAdjustDepreciationDto, AdjustDepreciation>(dto);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _adjustDepreciationService.CreateAsync(entity);
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AdjustDepreciation, AdjustDepreciationDto>(result);
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
            await _adjustDepreciationService.DeleteAsync(id);
            try
            {
                var adjustDepreciationDetails = await _adjustDepreciationDetailService.GetByAdjustDepreciationIdAsync(id);

                using var unitOfWork = _unitOfWorkManager.Begin();
                if (adjustDepreciationDetails != null)
                {
                    await _adjustDepreciationDetailService.DeleteManyAsync(adjustDepreciationDetails);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public Task<PageResultDto<AdjustDepreciationDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<AdjustDepreciationDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AdjustDepreciationDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.AssetToolCode).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AdjustDepreciation, AdjustDepreciationDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudAdjustDepreciationDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.AssetOrTool = AssetToolConst.Tool;
            dto.Id = id;
            var entity = await _adjustDepreciationService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                var adjustDepreciationDetails = await _adjustDepreciationDetailService.GetByAdjustDepreciationIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (adjustDepreciationDetails != null)
                {
                    await _adjustDepreciationDetailService.DeleteManyAsync(adjustDepreciationDetails);
                }

                await _adjustDepreciationService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<AdjustDepreciationDto> GetByIdAsync(string adjustDepreciationId)
        {
            var adjustDepreciation = await _adjustDepreciationService.GetAsync(adjustDepreciationId);
            return ObjectMapper.Map<AdjustDepreciation, AdjustDepreciationDto>(adjustDepreciation);
        }

        public async Task<List<DataAdjustDepreciationDetailDto>> GetAdjustDepreciationDetailAsync(string productId)
        {
            var adjustDepreciationDetails = await _adjustDepreciationDetailService.GetByAdjustDepreciationIdAsync(productId);
            var assetToolDetails = await _assetToolDetailService.GetQueryableAsync();
            assetToolDetails = assetToolDetails.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var data = from a in adjustDepreciationDetails
                       join b in assetToolDetails on a.AssetToolDetailId equals b.Id into ajb
                       from b in ajb.DefaultIfEmpty()
                       select new DataAdjustDepreciationDetailDto
                       {
                           AdjustDepreciationId = a.AdjustDepreciationId,
                           OrgCode = a.OrgCode,
                           AssetToolDetailId = a.AssetToolDetailId,
                           Amount = a.Amount,
                           DepreciationAmount = (b != null) ? b.DepreciationAmount : 0,
                           VoucherDate = (b != null) ? b.VoucherDate : null,
                           VoucherNumber = (b != null) ? b.VoucherNumber : null,
                           DepartmentCode = (b != null) ? b.DepartmentCode : null,
                           UpDownCode = (b != null) ? b.UpDownCode : null,
                           CapitalCode = (b != null) ? b.CapitalCode : null,
                           OriginalPrice = (b != null) ? b.OriginalPrice : null,
                           Impoverishment = (b != null) ? b.Impoverishment : null,
                           MonthNumber0 = (b != null) ? b.MonthNumber0 : null
                       };
            var res = data.ToList();
            return res;
        }

        #region Private
        private async Task<IQueryable<AdjustDepreciation>> Filter(PageRequestDto dto)
        {
            var queryable = await _adjustDepreciationService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                    && p.AssetOrTool == AssetToolConst.Tool);
            return queryable;
        }
        #endregion
    }
}
