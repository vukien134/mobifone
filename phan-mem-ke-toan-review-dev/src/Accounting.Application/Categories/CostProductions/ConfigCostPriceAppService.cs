using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Others;
using Accounting.Catgories.AccCases;
using Accounting.Catgories.CostProductions;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Jobs.CalcPrices;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace Accounting.Categories.CostProductions
{
    public class ConfigCostPriceAppService : AccountingAppService, IConfigCostPriceAppService
    {
        #region Fields
        private readonly ConfigCostPriceService _configCostPriceService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly IBackgroundJobManager _backgroundJobManager;

        #endregion
        #region Ctor
        public ConfigCostPriceAppService(ConfigCostPriceService configCostPriceService,
                            UserService userService,
                            WebHelper webHelper,
                            ExcelService excelService,
                            LicenseBusiness licenseBusiness,
                            IBackgroundJobManager backgroundJobManager
                            )
        {
            _configCostPriceService = configCostPriceService;
            _userService = userService;
            _webHelper = webHelper;
            _excelService = excelService;
            _licenseBusiness = licenseBusiness;
            _backgroundJobManager = backgroundJobManager;
        }
        #endregion

        public async Task<ConfigCostPriceDto> CreateAsync(CrudConfigCostPriceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudConfigCostPriceDto, ConfigCostPrice>(dto);
            var result = await _configCostPriceService.CreateAsync(entity);
            return ObjectMapper.Map<ConfigCostPrice, ConfigCostPriceDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _configCostPriceService.DeleteAsync(id);
        }

        public async Task<PageResultDto<ConfigCostPriceDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }

        public async Task<PageResultDto<ConfigCostPriceDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ConfigCostPriceDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Type).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ConfigCostPrice, ConfigCostPriceDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<ConfigCostPrice> GetDataAsync()
        {
            var result = new PageResultDto<ConfigCostPriceDto>();
            var query = await _configCostPriceService.GetQueryableAsync();
            var data = query.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).FirstOrDefault();
            return data;
        }

        public async Task UpdateAsync(string id, CrudConfigCostPriceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _configCostPriceService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _configCostPriceService.UpdateAsync(entity);
        }

        public async Task<ConfigCostPriceDto> GetByIdAsync(string caseId)
        {
            var configCostPrice = await _configCostPriceService.GetAsync(caseId);
            return ObjectMapper.Map<ConfigCostPrice, ConfigCostPriceDto>(configCostPrice);
        }
        #region Private
        private async Task<IQueryable<ConfigCostPrice>> Filter(PageRequestDto dto)
        {
            var queryable = await _configCostPriceService.GetQueryableAsync();

            if (dto.FilterRows == null) return queryable;

            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

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
