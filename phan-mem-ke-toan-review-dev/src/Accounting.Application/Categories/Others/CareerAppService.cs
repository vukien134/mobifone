using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Others.Careers;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.Others
{
    public class CareerAppService : AccountingAppService, ICareerAppService
    {
        #region Fields
        private readonly CareerService _careerService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public CareerAppService(CareerService careerService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ICurrentTenant currentTenant,
                            TenantExtendInfoService tenantExtendInfoService
            )
        {
            _careerService = careerService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _currentTenant = currentTenant;
            _tenantExtendInfoService = tenantExtendInfoService;
        }
        #endregion
        public async Task<CareerDto> CreateAsync(CrudCareerDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudCareerDto, Career>(dto);
            var result = await _careerService.CreateAsync(entity);
            return ObjectMapper.Map<Career, CareerDto>(result);
        }

        public async Task<CareerDto> GetById(string careerId)
        {
            var career = await _careerService.GetById(careerId);
            if (career == null) return null;
            return ObjectMapper.Map<Career, CareerDto>(career);            
        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            var careers = await _careerService.GetListAsync();
            if (careers.Count > 0)
            {
                var dtos = careers.Select(p =>
                {
                    var dto = new BaseComboItemDto()
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        Value = p.Name,
                        Display = p.Name
                    };
                    return dto;
                }).ToList();
                return dtos;
            }

            int? tenantType = await this.GetTenantType();
            var defaultCareers = await _careerService.GetByTenantTypeAsync(tenantType);
            return defaultCareers.Select(p =>
            {
                var dto = new BaseComboItemDto()
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Value = p.Name,
                    Display = p.Name
                };
                return dto;
            }).ToList();
        }
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _careerService.DeleteAsync(id);
        }

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

        public async Task<PageResultDto<CareerDto>> GetListAsync(PageRequestDto dto)
        {
            await this.InsertDefaultAsync();
            var result = new PageResultDto<CareerDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Career, CareerDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudCareerDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _careerService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _careerService.UpdateAsync(entity);
        }
        #region Private
        private async Task<IQueryable<Career>> Filter(PageRequestDto dto)
        {
            var queryable = await _careerService.GetQueryableAsync();
            return queryable;
        }
        private async Task InsertDefaultAsync()
        {
            bool isExists = await _careerService.IsExistListAsync();
            if (isExists) return;
            int? tenantType = await this.GetTenantType();
            var defaultCareers = await _careerService.GetByTenantTypeAsync(tenantType);
            if (defaultCareers.Count == 0) return;

            var entities = defaultCareers.Select(p => ObjectMapper.Map<DefaultCareer, Career>(p))
                               .ToList();
            await _careerService.CreateManyAsync(entities);
        }
        private async Task<int?> GetTenantType()
        {
            var tenantInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantInfo == null) return null;
            return tenantInfo.TenantType;
        }
        #endregion
    }
}
