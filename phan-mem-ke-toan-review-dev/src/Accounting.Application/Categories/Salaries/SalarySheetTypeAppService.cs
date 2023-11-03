using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Salaries.SalarySheetTypes;
using Accounting.Catgories.Salaries.SalaryTypes;
using Accounting.Common.Extensions;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Salaries;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Uow;

namespace Accounting.Categories.Salaries
{
    public class SalarySheetTypeAppService : AccountingAppService, ISalarySheetTypeAppService
    {
        #region Field
        private readonly SalarySheetTypeService _salarySheetTypeService;
        private readonly SalarySheetTypeDetailService _salarySheetTypeDetailService;        
        private readonly UserService _userService;
        private readonly ExcelService _excelService;        
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IDistributedCache<SalarySheetTypeDto> _cache;
        private readonly IDistributedCache<PageResultDto<SalarySheetTypeDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly DefaultSalarySheetTypeService _defaultSalarySheetTypeService;
        private readonly DefaultSalarySheetTypeDetailService _defaultSalarySheetTypeDetailService;
        #endregion
        #region Ctor
        public SalarySheetTypeAppService(SalarySheetTypeService salarySheetTypeService,
                                SalarySheetTypeDetailService salarySheetTypeDetailService,                                
                                UserService userService,
                                ExcelService excelService,                                
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                IDistributedCache<SalarySheetTypeDto> cache,
                                IDistributedCache<PageResultDto<SalarySheetTypeDto>> pageCache,
                                CacheManager cacheManager,
                                DefaultSalarySheetTypeService defaultSalarySheetType,
                                DefaultSalarySheetTypeDetailService defaultSalarySheetTypeDetailService

            )
        {
            _salarySheetTypeService = salarySheetTypeService;
            _salarySheetTypeDetailService = salarySheetTypeDetailService;            
            _userService = userService;
            _excelService = excelService;            
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _defaultSalarySheetTypeService = defaultSalarySheetType;
            _defaultSalarySheetTypeDetailService = defaultSalarySheetTypeDetailService; 
        }
        #endregion
        [Authorize(AccountingPermissions.SalarySheetTypeManagerCreate)]
        public async Task<SalarySheetTypeDto> CreateAsync(CrudSalarySheetTypeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();            
            var entity = ObjectMapper.Map<CrudSalarySheetTypeDto, SalarySheetType>(dto);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _salarySheetTypeService.CreateAsync(entity);                
                await unitOfWork.CompleteAsync();
                await RemoveAllCache();
                return ObjectMapper.Map<SalarySheetType, SalarySheetTypeDto>(result);
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        [Authorize(AccountingPermissions.SalarySheetTypeManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _salarySheetTypeService.DeleteAsync(id);
            await RemoveAllCache();           
        }

        [Authorize(AccountingPermissions.SalarySheetTypeManagerView)]
        public async Task<PageResultDto<SalarySheetTypeDto>> PagesAsync(PageRequestDto dto)
        {            
            await InsertDefaultData();
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<SalarySheetTypeDto>(dto); //salarysheettypeDto
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }

        [Authorize(AccountingPermissions.SalarySheetTypeManagerView)]
        public async Task<PageResultDto<SalarySheetTypeDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<SalarySheetTypeDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<SalarySheetType, SalarySheetTypeDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.SalarySheetTypeManagerUpdate)]
        public async Task UpdateAsync(string id, CrudSalarySheetTypeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();            
            var entity = await _salarySheetTypeService.GetAsync(id);

            try
            {
                var details = await _salarySheetTypeDetailService.GetBySalarySheetTypeIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();                
                ObjectMapper.Map(dto, entity);                
                await _salarySheetTypeDetailService.DeleteManyAsync(details);
                await _salarySheetTypeService.UpdateAsync(entity);                
                await unitOfWork.CompleteAsync();
                await RemoveAllCache();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<SalarySheetTypeDto> GetByIdAsync(string productId)
        {
            return await _cache.GetOrAddAsync(
                productId, //Cache key
                async () =>
                {
                    var product = await _salarySheetTypeService.GetAsync(productId);
                    return ObjectMapper.Map<SalarySheetType, SalarySheetTypeDto>(product);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );
        }
        public async Task<List<SalarySheetTypeDetailDto>> GetDetailsAsync(string salarySheetTypeId)
        {
            var entities = await _salarySheetTypeDetailService.GetBySalarySheetTypeIdAsync(salarySheetTypeId);          
            var dtos = entities.OrderBy(x => x.Ord).Select(p => ObjectMapper.Map<SalarySheetTypeDetail, SalarySheetTypeDetailDto>(p)).ToList();
            return dtos;
        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var partnes = await _salarySheetTypeService.GetDataReference(orgCode);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Id,
                Value = p.Name,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        #region Private
        private async Task<IQueryable<SalarySheetType>> Filter(PageRequestDto dto)
        {
            var queryable = await _salarySheetTypeService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _salarySheetTypeService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<SalarySheetTypeDto>();         
            await _cacheManager.RemoveClassCache<SalarySheetType>();
        }
        private async Task<bool> IsSalarySheetTypeExist(string orgCode)
        {
            var queryable = await _salarySheetTypeService.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode);
        }
        private async Task InsertDefaultData()
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            if (await IsSalarySheetTypeExist(orgCode) == false)
            {
                var data = await this._defaultSalarySheetTypeService.GetList();
                List<CrudSalarySheetTypeDto> temp = new List<CrudSalarySheetTypeDto>();
                foreach (var item in data)
                {
                    var id = this.GetNewObjectId();
                    temp.Add(new CrudSalarySheetTypeDto
                    {
                        Code = item.Code,
                        CreatorName = await _userService.GetCurrentUserNameAsync(),
                        OrgCode = orgCode,
                        CreditAcc = item.CreditAcc,
                        DebitAcc = item.DebitAcc,
                        VoucherCode = item.VoucherCode,
                        Name = item.Name,
                        Id = id,
                        SalarySheetTypeDetails = await this.GetDetails(id, item.Id)
                    });            
                }
                foreach(var item in temp)
                {
                    var entity = ObjectMapper.Map<CrudSalarySheetTypeDto, SalarySheetType>(item);                   
                    await _salarySheetTypeService.CreateAsync(entity);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
                await this.RemoveAllCache();           
            };
        }
        private async Task<List<CrudSalarySheetTypeDetailDto>> GetDetails(string id, string Id)
        {   
            var data = await this._defaultSalarySheetTypeDetailService.GetByDetailId(Id);
            List<CrudSalarySheetTypeDetailDto> temp = new List<CrudSalarySheetTypeDetailDto>();
              foreach (var item in data)
              {
                  temp.Add(new CrudSalarySheetTypeDetailDto
                  {
                      Caption = item.Caption,
                      DataType = item.DataType,
                      FieldName = item.FieldName,
                      Format = item.Format,
                      Width = item.Width,
                      SalarySheetTypeId = id,               
                      Ord = item.Ord,                      
                      Formular = item.Formular
                  });
              }
              return temp;            
      }
        #endregion
    }
}
