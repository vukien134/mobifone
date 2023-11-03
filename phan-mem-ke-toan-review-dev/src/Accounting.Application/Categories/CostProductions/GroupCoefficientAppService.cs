using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Contracts;
using Accounting.Catgories.CostProductions;
using Accounting.Catgories.Customines;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.CostProductions
{
    public class GroupCoefficientAppService : AccountingAppService, IGroupCoefficientAppService
    {
        #region Field
        private readonly GroupCoefficientService _groupCoefficientService;
        private readonly GroupCoefficientDetailService _groupCoefficientDetailService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly UserService _userService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;        
        #endregion
        #region Ctor
        public GroupCoefficientAppService(GroupCoefficientService groupCoefficientService,
                                GroupCoefficientDetailService groupCoefficientDetailService,
                                FProductWorkService fProductWorkService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ExcelService excelService
                                )
        {
            _groupCoefficientService = groupCoefficientService;
            _groupCoefficientDetailService = groupCoefficientDetailService;
            _fProductWorkService = fProductWorkService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;            
        }
        #endregion
        public async Task<GroupCoefficientDto> CreateAsync(CrudGroupCoefficientDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Code = dto.Code.ToUpper();
            var entity = ObjectMapper.Map<CrudGroupCoefficientDto, GroupCoefficient>(dto);
            var result = await _groupCoefficientService.CreateAsync(entity);
            return ObjectMapper.Map<GroupCoefficient, GroupCoefficientDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _groupCoefficientService.DeleteAsync(id);
        }

        public Task<PageResultDto<GroupCoefficientDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<GroupCoefficientDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<GroupCoefficientDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<GroupCoefficient, GroupCoefficientDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<GroupCoefficientDetailCustomineDto>> GetGroupCoefficientDetailAsync(string groupCoefficientId)
        {
            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstfProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var groupCoefficientDetails = await _groupCoefficientDetailService.GetByGroupCoefficientIdAsync(groupCoefficientId);
            var iQgroupCoefficientDetails = from a in groupCoefficientDetails
                                            join b in lstfProductWork on a.FProductWorkCode equals b.Code into ajb
                                            from b in ajb.DefaultIfEmpty()
                                            select new GroupCoefficientDetailCustomineDto
                                            {
                                                Id = a.Id,
                                                OrgCode = a.OrgCode,
                                                Year = a.Year,
                                                FProductWorkCode = a.FProductWorkCode,
                                                GroupCoefficientCode = a.GroupCoefficientCode,
                                                January = a.January,
                                                February = a.February,
                                                March = a.March,
                                                April = a.April,
                                                May = a.May,
                                                June = a.June,
                                                July = a.July,
                                                August = a.August,
                                                September = a.September,
                                                October = a.October,
                                                November = a.November,
                                                December = a.December,
                                                FProductWorkName = (b != null) ? b.Name : ""
                                            };
            var dtos = iQgroupCoefficientDetails.ToList();            
            return dtos;
        }

        public async Task UpdateAsync(string id, CrudGroupCoefficientDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Code = dto.Code.ToUpper();
            dto.Id = id;
            var entity = await _groupCoefficientService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                var groupCoefficientDetails = await _groupCoefficientDetailService.GetByGroupCoefficientIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (groupCoefficientDetails != null)
                {
                    await _groupCoefficientDetailService.DeleteManyAsync(groupCoefficientDetails);
                }
                await _groupCoefficientService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            var queryable = await _groupCoefficientService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        && (p.Code.Contains(dto.FilterValue.ToUpper()) || p.Name.ToLower().Contains(dto.FilterValue.ToLower())))
                                .OrderBy(p => p.Code);
            var groupCoefficients = await AsyncExecuter.ToListAsync(queryable);
            return groupCoefficients.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        public async Task<GroupCoefficientDto> GetByIdAsync(string partnerId)
        {
            var partner = await _groupCoefficientService.GetAsync(partnerId);
            return ObjectMapper.Map<GroupCoefficient, GroupCoefficientDto>(partner);
        }
        #region Private
        private async Task<IQueryable<GroupCoefficient>> Filter(PageRequestDto dto)
        {
            var queryable = await _groupCoefficientService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnName == "code")
                {
                    var checkCode = item.Value.ToString().ToUpper();
                    queryable = queryable.Where(item.ColumnName, checkCode, FilterOperator.Contains);                   
                }
                if (item.ColumnName == "name")
                {
                    var checkName = item.Value.ToString().ToLower();                  
                    queryable = queryable.Where(x => x.Name.ToLower().Contains(checkName));
                }


            }
            return queryable;
        }
        private async Task<string> GetFProductName(string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var queryable = await _fProductWorkService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit())
                                    && p.Code.Equals(code));
            var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable);
            if (entity == null) return null;
            return entity.Name;
        }
        #endregion
    }
}
