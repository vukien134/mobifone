using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Others.Warehouses;
using Accounting.Common.Extensions;
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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;

namespace Accounting.Categories.Others
{
    public class WarehouseAppService : AccountingAppService, IWarehouseAppService
    {
        #region Fields
        private readonly WarehouseService _warehouseService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<WarehouseDto> _cache;
        private readonly IDistributedCache<PageResultDto<WarehouseDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public WarehouseAppService(WarehouseService warehouseService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<WarehouseDto> cache,
                            IDistributedCache<PageResultDto<WarehouseDto>> pageCache,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
            )
        {
            _warehouseService = warehouseService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.WarehouseManagerCreate)]
        public async Task<WarehouseDto> CreateAsync(CrudWarehouseDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.WarehouseType = "C";
            var lstWarehouse = await _warehouseService.GetQueryableAsync();
            lstWarehouse = lstWarehouse.Where(p => p.Id == dto.ParentId);
            if (lstWarehouse != null)
            {
                foreach (var item in lstWarehouse.ToList())
                {
                    item.WarehouseType = "K";

                    await _warehouseService.UpdateAsync(item);
                }

            }
            var entity = ObjectMapper.Map<CrudWarehouseDto, Warehouse>(dto);
            var result = await _warehouseService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<Warehouse, WarehouseDto>(result);
        }

        [Authorize(AccountingPermissions.WarehouseManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _warehouseService.GetAsync(id);
            bool isParentGroup = await _warehouseService.IsParentGroup(entity.Id);
            if (isParentGroup)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Warehouse, ErrorCode.Other),
                        _localizer["Err:GroupIsParent", entity.Code]);
            }
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.WarehouseCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Warehouse, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _warehouseService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.WarehouseManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _warehouseService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.WarehouseCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Warehouse, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _warehouseService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.WarehouseManagerView)]
        public async Task<PageResultDto<WarehouseDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<WarehouseDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }

        [Authorize(AccountingPermissions.WarehouseManagerView)]
        public async Task<PageResultDto<WarehouseDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<WarehouseDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Warehouse, WarehouseDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.WarehouseManagerUpdate)]
        public async Task UpdateAsync(string id, CrudWarehouseDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _warehouseService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            dto.WarehouseType = "C";
            var lstWarehouse = await _warehouseService.GetQueryableAsync();
            lstWarehouse = lstWarehouse.Where(p => p.Id == dto.ParentId);
            if (lstWarehouse != null)
            {
                foreach (var item in lstWarehouse.ToList())
                {
                    item.WarehouseType = "K";

                    await _warehouseService.UpdateAsync(item);
                }

            }
            ObjectMapper.Map(dto, entity);
            await _warehouseService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.WarehouseCode, dto.Code, oldCode, entity.OrgCode);
            }
            await RemoveAllCache();
        }
        public async Task<List<BaseComboItemDto>> GetViewListAsync()
        {
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<WarehouseDto>("ViewList");

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnerGroups = await _warehouseService.GetRepository()
                        .GetListAsync(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                    return partnerGroups.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        ParentId = p.ParentId,
                        Value = p.Name
                    })
                    .OrderBy(p => p.Code).ToList();
                }
            );
        }
        public async Task<WarehouseDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () =>
                {
                    var accCase = await _warehouseService.GetAsync(caseId);
                    return ObjectMapper.Map<Warehouse, WarehouseDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );
        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<WarehouseDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _warehouseService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
                    return partnes.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name
                    }).ToList();
                }
            );
        }
        [Authorize(AccountingPermissions.WarehouseManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudWarehouseDto>(bytes, dto.WindowId);

            var dataCheck = await _warehouseService.GetListAsync(_webHelper.GetCurrentOrgUnit());
            List<CrudWarehouseDto> listtemp = new List<CrudWarehouseDto>();
            foreach (var item in lstImport)
            {
                if (listtemp.Where(x => x.Code == item.Code).Count() > 0)
                {
                    break;
                }
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
                var dataCode = dataCheck.Where(x => x.Code == item.Code);
                if (dataCode.Count() > 0)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Warehouse, ErrorCode.NotFoundEntity),
                            _localizer["Err:WarehouseCodeExisted", item.Code]);
                }
                if (item.ParentId.IsNullOrEmpty() == false)
                {
                    var Parnert_Id = await _warehouseService.GetByWarehouseAsync(_webHelper.GetCurrentOrgUnit(), item.ParentId);
                    if (Parnert_Id.Count() > 0)
                    {
                        List<Warehouse> warehouses = new List<Warehouse>();
                        warehouses = Parnert_Id;
                        for (int i = 0; i < warehouses.Count; i++)
                        {
                            item.ParentId = warehouses[i].Id;
                        }
                    }
                    else
                    {
                        throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Warehouse, ErrorCode.NotFoundEntity),
                            _localizer["Err:ParentCodeNotExist", item.ParentId]);
                    }
                    item.WarehouseType = "K";
                }
                item.WarehouseType = "C";
                listtemp.Add(item);
            }

            var lstfdepartment = listtemp.Select(p => ObjectMapper.Map<CrudWarehouseDto, Warehouse>(p))
                                .ToList();
            await _warehouseService.CreateManyAsync(lstfdepartment);
            await RemoveAllCache();
            return new UploadFileResponseDto() { Ok = true };
        }
        public async Task<List<Warehouse>> GetChildGroup(string code)
        {
            var lstWarehouse = new List<Warehouse>();
            var lstWarehouseGroup = new List<Warehouse>();
            var iQPartnerGroup = await _warehouseService.GetQueryableAsync();
            var partnerGroup = iQPartnerGroup.Where(p => ((code ?? "") == "" && (p.ParentId ?? "") == "" || p.Code == code) && p.OrgCode == _webHelper.GetCurrentOrgUnit()).FirstOrDefault();
            if (partnerGroup == null) return lstWarehouse;
            lstWarehouse.Add(partnerGroup);
            lstWarehouseGroup.Add(partnerGroup);
            while (iQPartnerGroup.Any(p => lstWarehouseGroup.Select(p => p.Id).Contains(p.ParentId)))
            {
                lstWarehouseGroup = iQPartnerGroup.Where(p => lstWarehouseGroup.Select(p => p.Id).Contains(p.ParentId)).ToList();
                lstWarehouse.AddRange(lstWarehouseGroup);
            }
            return lstWarehouse;
        }
        #region Private
        private async Task<IQueryable<Warehouse>> Filter(PageRequestDto dto)
        {
            var queryable = await _warehouseService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _warehouseService.GetQueryableQuickSearch(queryable, filterValue);
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<WarehouseDto>();
        }
        #endregion
    }
}
