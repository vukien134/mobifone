using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Common.Extensions;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Invoices;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Invoices.Dtos;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Accounting.Categories.Invoices
{
    public class InvoiceStatusAppService : AccountingAppService, IInvoiceStatusAppService
    {
        #region Fields
        private readonly InvoiceStatusService _invoiceStatusService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<InvoiceStatusDto> _cache;
        private readonly IDistributedCache<PageResultDto<InvoiceStatusDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public InvoiceStatusAppService(InvoiceStatusService invoiceStatusService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<InvoiceStatusDto> cache,
                            IDistributedCache<PageResultDto<InvoiceStatusDto>> pageCache,
                            CacheManager cacheManager
                            )
        {
            _invoiceStatusService = invoiceStatusService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
        }
        #endregion

        [Authorize(AccountingPermissions.InvoiceStatusManagerCreate)]
        public async Task<InvoiceStatusDto> CreateAsync(CrudInvoiceStatusDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudInvoiceStatusDto, InvoiceStatus>(dto);
            var result = await _invoiceStatusService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<InvoiceStatus, InvoiceStatusDto>(result);
        }

        [Authorize(AccountingPermissions.InvoiceStatusManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _invoiceStatusService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.InvoiceStatusManagerDelete)]
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

        [Authorize(AccountingPermissions.InvoiceStatusManagerView)]
        public async Task<PageResultDto<InvoiceStatusDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<InvoiceStatusDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }
        [Authorize(AccountingPermissions.InvoiceStatusManagerView)]
        public async Task<PageResultDto<InvoiceStatusDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<InvoiceStatusDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<InvoiceStatus, InvoiceStatusDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.InvoiceStatusManagerUpdate)]
        public async Task UpdateAsync(string id, CrudInvoiceStatusDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _invoiceStatusService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _invoiceStatusService.UpdateAsync(entity);
            await RemoveAllCache();
        }
        
        public async Task<InvoiceStatusDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () =>
                {
                    var entity = await _invoiceStatusService.GetAsync(caseId);
                    return ObjectMapper.Map<InvoiceStatus, InvoiceStatusDto>(entity);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );

        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            string filterValue = $"List";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<InvoiceStatusDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _invoiceStatusService.GetList(_webHelper.GetCurrentOrgUnit());
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
        #region Private
        private async Task<IQueryable<InvoiceStatus>> Filter(PageRequestDto dto)
        {
            var queryable = await _invoiceStatusService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _invoiceStatusService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<InvoiceStatusDto>();
        }
        
        #endregion
    }
}
