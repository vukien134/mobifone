using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Units;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Categories.Units
{
    public class UnitAppService : AccountingAppService, IUnitAppService
    {
        #region Fields
        private readonly UnitService _unitService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly CacheManager _cacheManager;
        private readonly ProductService _productService;
        #endregion
        #region Ctor
        public UnitAppService(UnitService unitService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IStringLocalizer<AccountingResource> localizer,
                            CacheManager cacheManager,
                            ProductService productService)
        {
            _unitService = unitService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _localizer = localizer;
            _cacheManager = cacheManager;
            _productService = productService;
        }
        #endregion
        [Authorize(AccountingPermissions.UnitManagerCreate)]
        public async Task<UnitDto> CreateAsync(CrudUnitDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Code = dto.Code.Trim();
            if (dto.Code.IsNullOrWhiteSpace())
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Unit, ErrorCode.Other),
                            _localizer["Err:UnitNotNull"]);
            }
            var entity = ObjectMapper.Map<CrudUnitDto, Unit>(dto);
            var result = await _unitService.CreateAsync(entity);
            return ObjectMapper.Map<Unit, UnitDto>(result);
        }

        [Authorize(AccountingPermissions.UnitManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var dataCheck = await _productService.GetByProductAsync(_webHelper.GetCurrentOrgUnit());
            var dataUnitDel = await _unitService.GetAsync(id);
            if(dataCheck.Where(x=>x.UnitCode == dataUnitDel.Code).Count() > 0)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Unit, ErrorCode.IsUsing),
                            _localizer["Err:UnitIsUsing"]);
            }
            await _unitService.DeleteAsync(id);
            await this.RemoveAllCache();
        }
        [Authorize(AccountingPermissions.UnitManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }
            string orgCode = _webHelper.GetCurrentOrgUnit();          
            string[] deleteIds = dto.ListId.ToArray();
            foreach(string deleteId in deleteIds)
            {
                var dataCheck = await _productService.GetByProductAsync(_webHelper.GetCurrentOrgUnit());
                var dataUnitDel = await _unitService.GetAsync(deleteId);
                if (dataCheck.Where(x => x.UnitCode == dataUnitDel.Code).Count() > 0)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Unit, ErrorCode.IsUsing),
                                _localizer["Err:UnitIsUsing"]);
                }
            }
            await _unitService.DeleteManyAsync(deleteIds);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.UnitManagerView)]
        public Task<PageResultDto<UnitDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.UnitManagerView)]
        public async Task<PageResultDto<UnitDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<UnitDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Unit, UnitDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<UnitDto> GetByIdAsync(string unitId)
        {
            var accountSystem = await _unitService.GetAsync(unitId);
            return ObjectMapper.Map<Unit, UnitDto>(accountSystem);
        }

        [Authorize(AccountingPermissions.UnitManagerUpdate)]
        public async Task UpdateAsync(string id, CrudUnitDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _unitService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _unitService.UpdateAsync(entity);
        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            var queryable = await _unitService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Code
            }).ToList();
        }

        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudUnitDto>(bytes, dto.WindowId);
            lstImport = lstImport.Where(p => !p.Code.IsNullOrWhiteSpace()).ToList();
            foreach (var item in lstImport)
            {
                var checkUnit = await _unitService.GetByUnitAsync(item.Code, _webHelper.GetCurrentOrgUnit());
                if (checkUnit.Count == 0)
                {
                    item.Code = item.Code.Trim();
                    item.Id = this.GetNewObjectId();
                    item.OrgCode = _webHelper.GetCurrentOrgUnit();
                    item.CreatorName = await _userService.GetCurrentUserNameAsync();
                }
                else
                {
                    throw new Exception("Mã  đơn vị tính   " + item.Code + " đã tồn tại");
                }

            }

            var lstfdepartment = lstImport.Select(p => ObjectMapper.Map<CrudUnitDto, Unit>(p))
                                .ToList();
            await _unitService.CreateManyAsync(lstfdepartment);
            return new UploadFileResponseDto() { Ok = true };
        }
        #region Private
        private async Task<IQueryable<Unit>> Filter(PageRequestDto dto)
        {
            var queryable = await _unitService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<SelectOrgUnitLoginDto>();
        }
        #endregion
    }
}
