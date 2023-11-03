using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.Catgories.AccCases;
using Accounting.Catgories.CostProductions;
using Accounting.Catgories.CostProductions.AllotmentForwardCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.CostProduction;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Jobs.CalcPrices;
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
using Volo.Abp.Authorization;
using Volo.Abp.BackgroundJobs;

namespace Accounting.Categories.CostProductions
{
    public class AllotmentForwardCategoryAppService : AccountingAppService, IAllotmentForwardCategoryAppService
    {
        #region Fields
        private readonly AllotmentForwardCategoryService _allotmentForwardCategoryService;
        private readonly DefaultAllotmentForwardCategoryService _defaultAllotmentForwardCategoryService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly YearCategoryService _yearCategoryService;
        private readonly MenuAccountingService _menuAccountingService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public AllotmentForwardCategoryAppService(AllotmentForwardCategoryService allotmentForwardCategoryService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IBackgroundJobManager backgroundJobManager,
                            YearCategoryService yearCategoryService,
                            MenuAccountingService menuAccountingService,
                            IStringLocalizer<AccountingResource> localizer,
                            DefaultAllotmentForwardCategoryService defaultAllotmentForwardCategoryService
                            )
        {
            _allotmentForwardCategoryService = allotmentForwardCategoryService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _backgroundJobManager = backgroundJobManager;
            _yearCategoryService = yearCategoryService;
            _menuAccountingService = menuAccountingService;
            _localizer = localizer;
            _defaultAllotmentForwardCategoryService = defaultAllotmentForwardCategoryService;
        }
        #endregion        
        public async Task<AllotmentForwardCategoryDto> CreateAsync(CrudAllotmentForwardCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionCreate);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            var yearCategory = await GetCurrentYear();

            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.DecideApply = yearCategory.UsingDecision.Value;
            dto.Type = dto.Type;
            dto.FProductWork = dto.FProductWork;
            dto.Year = yearCategory.Year;
            dto.OrdGrp = dto.OrdGrp;
            if (dto.FProductWork == "S" && dto.Type == "T")
            {
                dto.OrdGrp = "1";
            }
            else if (dto.FProductWork == "S" && dto.Type == "D")
            {
                dto.OrdGrp = "2";
            }
            else if (dto.FProductWork == "S" && dto.Type == "H")
            {
                dto.OrdGrp = "3";
            }
            else if (dto.FProductWork == "S" && dto.Type == "L")
            {
                dto.OrdGrp = "4";
            }
            else if (dto.FProductWork == "C" && dto.Type == "T")
            {
                dto.OrdGrp = "1";
            }
            else if (dto.FProductWork == "C" && dto.Type == "H")
            {
                dto.OrdGrp = "2";
            }
            else if (dto.FProductWork == "C" && dto.Type == "L")
            {
                dto.OrdGrp = "3";
            }
            var entity = ObjectMapper.Map<CrudAllotmentForwardCategoryDto, AllotmentForwardCategory>(dto);
            var result = await _allotmentForwardCategoryService.CreateAsync(entity);
            return ObjectMapper.Map<AllotmentForwardCategory, AllotmentForwardCategoryDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionDelete);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
            await _allotmentForwardCategoryService.DeleteAsync(id);
        }

        public async Task<PageResultDto<AllotmentForwardCategoryDto>> PagesAsync(string type, string productOrWork, string ordGrp, PageRequestDto dto)
        {
            var requestDto = new AllotmentForwardRequestDto()
            {
                Type = type,
                FProductWork = productOrWork,
                Continue = dto.Continue,
                Count = dto.Count,
                FilterAdvanced = dto.FilterAdvanced,
                FilterRows = dto.FilterRows,
                QuickSearch = dto.QuickSearch,
                Start = dto.Start,
                WindowId = dto.WindowId,
                OrdGrp = ordGrp
            };
            return await GetListAsync(requestDto);
        }

        public async Task<PageResultDto<AllotmentForwardCategoryDto>> GetListAsync(AllotmentForwardRequestDto dto)
        {
            await this.InsertDefaultAsync(dto);
            var result = new PageResultDto<AllotmentForwardCategoryDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AllotmentForwardCategory, AllotmentForwardCategoryDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<AllotmentGetOrdGrpDto>> GetDataOrdGrpAsync(string fProductWork)
        {
            var iQYearCategory = await _yearCategoryService.GetQueryableAsync(); 
            var yearCategory = iQYearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault(); //Test-2023, data = null
            var orgCode = _webHelper.GetCurrentOrgUnit();      
            var result = new List<AllotmentGetOrdGrpDto>();
            var query = await _allotmentForwardCategoryService.GetQueryableAsync();
            var data = query.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                     && p.Year == _webHelper.GetCurrentYear()
                                     && p.FProductWork == fProductWork
                                     && p.DecideApply == yearCategory.UsingDecision
                                     && p.Active == 1).GroupBy(g => new { g.FProductWork, g.Type, g.OrdGrp })
                                     .Select(p => new AllotmentGetOrdGrpDto
                                     {
                                         Id = p.Key.OrdGrp,
                                         FProductWork = p.Key.FProductWork,
                                         Type = p.Key.Type,
                                         OrdGrp = p.Key.OrdGrp,
                                         SelectRow = true     
                                     }).OrderBy(p => p.OrdGrp).ToList();
            if (data.Count > 0)
            {
                int i = 1;
                foreach (var item in data)
                {
                    item.Ord = i;
                    i++;
                }
                return data;
            }
            return await this.GetDefaultAsync(fProductWork, yearCategory.UsingDecision.Value);         
        }

        public async Task UpdateAsync(string id, CrudAllotmentForwardCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionUpdate);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            var yearCategory = await GetCurrentYear();
            if (dto.FProductWork == "S" && dto.Type == "T")
            {
                dto.OrdGrp = "1";
            }
            else if (dto.FProductWork == "S" && dto.Type == "D")
            {
                dto.OrdGrp = "2";
            }
            else if (dto.FProductWork == "S" && dto.Type == "H")
            {
                dto.OrdGrp = "3";
            }
            else if (dto.FProductWork == "S" && dto.Type == "L")
            {
                dto.OrdGrp = "4";
            }
            else if (dto.FProductWork == "C" && dto.Type == "T")
            {
                dto.OrdGrp = "1";
            }
            else if (dto.FProductWork == "C" && dto.Type == "H")
            {
                dto.OrdGrp = "2";
            }
            else if (dto.FProductWork == "C" && dto.Type == "L")
            {
                dto.OrdGrp = "3";
            }
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _allotmentForwardCategoryService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _allotmentForwardCategoryService.UpdateAsync(entity);
        }

        public async Task<AllotmentForwardCategoryDto> GetByIdAsync(string caseId)
        {
            var allotmentForwardCategory = await _allotmentForwardCategoryService.GetAsync(caseId);
            return ObjectMapper.Map<AllotmentForwardCategory, AllotmentForwardCategoryDto>(allotmentForwardCategory);
        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            var queryable = await _allotmentForwardCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        && (p.Code.Contains(dto.FilterValue) || p.DecideApply.ToString().Contains(dto.FilterValue)))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.DecideApply.ToString()
            }).ToList();
        }
        public async Task<List<BaseComboItemDto>> GetDataReference(string productOrWork, string lstType)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await GetCurrentYear();
            var queryable = await _allotmentForwardCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode) && p.Year == yearCategory.Year
                                    && p.DecideApply == yearCategory.UsingDecision
                                    && p.FProductWork.Equals(productOrWork)
                                    && lstType.Contains(p.Type));
            var allots = await AsyncExecuter.ToListAsync(queryable);
            return allots.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.DecideApply.ToString()
            }).ToList();
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
            res.Message = _localizer["success"];
            return res;
        }
        #region Private
        private async Task<IQueryable<AllotmentForwardCategory>> Filter(AllotmentForwardRequestDto dto)
        {
            var yearCategory = await this.GetCurrentYear();
            var queryable = await _allotmentForwardCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                    && p.Year == yearCategory.Year
                                    && p.DecideApply == yearCategory.UsingDecision);
            if (!string.IsNullOrEmpty(dto.Type))
            {
                queryable = queryable.Where(p => p.Type.Equals(dto.Type));
            }
            if (!string.IsNullOrEmpty(dto.FProductWork))
            {
                queryable = queryable.Where(p => p.FProductWork.Equals(dto.FProductWork));
            }
            if (!string.IsNullOrEmpty(dto.OrdGrp))
            {
                queryable = queryable.Where(p => p.OrdGrp.Equals(dto.OrdGrp));
            }
            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                queryable = queryable.Where(p => p.Code.Contains(dto.QuickSearch) || p.DecideApply.ToString().Contains(dto.QuickSearch));
            }

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
        private async Task<bool> IsGrantPermission(string menuAccountingId, string action)
        {
            var menuAccounting = await _menuAccountingService.FindAsync(menuAccountingId);
            if (menuAccounting == null) return false;
            string permissionName = menuAccounting.ViewPermission.Replace(AccountingPermissions.ActionView,
                                                    action);
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            return result.Succeeded;
        }
        private async Task InsertDefaultAsync(AllotmentForwardRequestDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await this.GetCurrentYear();
            bool isExistedList = await _allotmentForwardCategoryService.IsExistListAsync(dto.Type, dto.FProductWork,
                                            yearCategory.UsingDecision.Value, dto.OrdGrp, orgCode, yearCategory.Year);
            if (isExistedList) return;

            var defaultAllotments = await _defaultAllotmentForwardCategoryService.GetListAsync(dto.Type, dto.FProductWork,
                                        yearCategory.UsingDecision.Value, dto.OrdGrp);
            var entities = defaultAllotments.Select(p =>
            {
                var dto = ObjectMapper.Map<DefaultAllotmentForwardCategory, CrudAllotmentForwardCategoryDto>(p);
                dto.OrgCode = orgCode;
                dto.Year = yearCategory.Year;
                dto.Id = this.GetNewObjectId();
                var entity = ObjectMapper.Map<CrudAllotmentForwardCategoryDto, AllotmentForwardCategory>(dto);
                return entity;
            });
            await _allotmentForwardCategoryService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }       
        private async Task<List<AllotmentGetOrdGrpDto>> GetDefaultAsync(string fProductWork, int decideApply)
        {  
            var dataDefaultAlloment = await _defaultAllotmentForwardCategoryService.GetQueryableAsync();
            var dataTemp = dataDefaultAlloment.Where(p => p.FProductWork == fProductWork 
                                                           && p.DecideApply == decideApply
                                                           && p.Active == 1).ToList(); 
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var datas = dataTemp.GroupBy(g => new { g.FProductWork, g.Type, g.OrdGrp })
                                     .Select(p => new AllotmentGetOrdGrpDto
                                     {
                                         Id = p.Key.OrdGrp,
                                         FProductWork = p.Key.FProductWork,
                                         Type = p.Key.Type,
                                         OrdGrp = p.Key.OrdGrp,
                                         SelectRow = true,
                                         OrgCode = orgCode,                                      
                                     }).OrderBy(p => p.OrdGrp).ToList();
            int a = 1;
            foreach (var item in datas)
            {
                item.Ord = a;
                a++;
            }
            return datas;
        }
            #endregion
    }
}
