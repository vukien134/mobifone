using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.Catgories.AccCases;
using Accounting.Catgories.CostProductions.SoTHZs;
using Accounting.Catgories.Others.Departments;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.CostProduction;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Extensions;
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

namespace Accounting.Categories.CostProductions
{
    public class SoTHZAppService : AccountingAppService, ISoTHZAppService
    {
        #region Fields
        private readonly SoTHZService _SoTHZService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public SoTHZAppService(SoTHZService SoTHZService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            YearCategoryService yearCategoryService,
                            IStringLocalizer<AccountingResource> localizer,
                            CacheManager cacheManager
                            )
        {
            _SoTHZService = SoTHZService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _yearCategoryService = yearCategoryService;
            _localizer = localizer;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<SoTHZDto> CreateAsync(CrudSoTHZDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var yearCategory = await GetCurrentYear();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.UsingDecision = yearCategory.UsingDecision.Value;
            dto.Year = yearCategory.Year;

            var entity = ObjectMapper.Map<CrudSoTHZDto, SoTHZ>(dto);
            var result = await _SoTHZService.CreateAsync(entity);
            return ObjectMapper.Map<SoTHZ, SoTHZDto>(result);
        }        
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _SoTHZService.DeleteAsync(id);
        }
        
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string[] idsDelete = dto.ListId.ToArray();
            await _SoTHZService.DeleteManyAsync(idsDelete);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }
        public async Task<PageResultDto<SoTHZDto>> PagesAsync(string productOrWork,PageRequestDto dto)
        {
            var requestDto = new SoTHZRequestDto()
            {
                Continue = dto.Continue,
                Count = dto.Count,
                FilterAdvanced = dto.FilterAdvanced,
                FilterRows = dto.FilterRows,
                ProductOrWork = productOrWork,
                QuickSearch = dto.QuickSearch,
                Start = dto.Start,
                WindowId = dto.WindowId
            };
            return await GetListAsync(requestDto);
        }
        
        public async Task<PageResultDto<SoTHZDto>> GetListAsync(SoTHZRequestDto dto)
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var year = _webHelper.GetCurrentYear();
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            var result = new PageResultDto<SoTHZDto>();
            var query = await Filter(dto);
            var querysort = query.Where(p => p.UsingDecision == yearCategory.UsingDecision).OrderBy(p => p.Ord).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<SoTHZ, SoTHZDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(querysort);
            }
            return result;
        }
        
        public async Task UpdateAsync(string id, CrudSoTHZDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _SoTHZService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _SoTHZService.UpdateAsync(entity);
        }        
        public async Task<SoTHZDto> GetByIdAsync(string SoTHZId)
        {
            var SoTHZ = await _SoTHZService.GetAsync(SoTHZId);
            return ObjectMapper.Map<SoTHZ, SoTHZDto>(SoTHZ);
        }        
        #region Private
        private async Task<IQueryable<SoTHZ>> Filter(SoTHZRequestDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await GetCurrentYear();
            var queryable = await _SoTHZService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit())
                            && p.Year == yearCategory.Year && p.FProductOrWork.Equals(dto.ProductOrWork));
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
            }
            return queryable;
        }
        private async Task<YearCategory> GetCurrentYear()
        {
            int year = _webHelper.GetCurrentYear();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.YearCategory, ErrorCode.NotFoundEntity),
                        $"YearCategory Code ['{year}'] not found ");
            }
            if (yearCategory.UsingDecision == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.YearCategory, ErrorCode.Other),
                        $"UsingDecision is null ");
            }
            return yearCategory;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<DepartmentDto>();
        }
        #endregion
    }
}
