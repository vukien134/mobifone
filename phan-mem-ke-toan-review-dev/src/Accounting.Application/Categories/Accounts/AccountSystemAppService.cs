using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.YearCategories;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Windows;
using Accounting.EntityFrameworkCore;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Categories.Accounts
{
    public class AccountSystemAppService : AccountingAppService, IAccountSystemAppService
    {
        #region Fields
        private readonly AccountSystemService _accountSystemService;
        private readonly DefaultAccountSystemService _defaultAccountSystemService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ICurrentTenant _currentTenant;
        private readonly YearCategoryService _yearCategoryService;
        private readonly LinkCodeService _linkCodeService;
        private readonly AccountingDb _accountingDb;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public AccountSystemAppService(AccountSystemService accountSystemService,
                            DefaultAccountSystemService defaultAccountSystemService,
                            UserService userService,
                            WebHelper webHelper,
                            TenantExtendInfoService tenantExtendInfoService,
                            ICurrentTenant currentTenant,
                            YearCategoryService yearCategoryService,
                            LinkCodeService linkCodeService,
                            AccountingDb accountingDb,
                            LicenseBusiness licenseBusiness,
                            IUnitOfWorkManager unitOfWorkManager,
                            AccountingCacheManager accountingCacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
            )
        {
            _accountSystemService = accountSystemService;
            _userService = userService;
            _webHelper = webHelper;
            _defaultAccountSystemService = defaultAccountSystemService;
            _tenantExtendInfoService = tenantExtendInfoService;
            _currentTenant = currentTenant;
            _yearCategoryService = yearCategoryService;
            _linkCodeService = linkCodeService;
            _accountingDb = accountingDb;
            _licenseBusiness = licenseBusiness;
            _unitOfWorkManager = unitOfWorkManager;
            _accountingCacheManager = accountingCacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        #region Methods

        [Authorize(AccountingPermissions.AccountSystemManagerCreate)]
        public async Task<AccountSystemDto> CreateAsync(CruAccountSystemDto dto)
        {

            await _licenseBusiness.CheckExpired();
            dto = await StandardDto(dto);
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            if ((dto.ParentCode ?? "" ) != "" || dto.ParentCode == null)
            {
                dto.AccType = "C";
            }
            if(dto.ParentCode.IsNullOrEmpty() == false)
            {
                var data = await _accountSystemService.GetAccountByAccCodeAsync(dto.ParentCode, dto.OrgCode, _webHelper.GetCurrentYear());
                data.AccType = "K";
                await _accountSystemService.UpdateAsync(data);
            }
            if (dto.Year != 0)
            {
                dto.Year = dto.Year;
            }
            else
            {
                dto.Year = _webHelper.GetCurrentYear();
            }

            //dto.ParentCode = await GetParentCodeAsync(dto.ParentAccId);
            var entity = ObjectMapper.Map<CruAccountSystemDto, AccountSystem>(dto);
            var result = await _accountSystemService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<AccountSystem, AccountSystemDto>(result);
        }

        [Authorize(AccountingPermissions.AccountSystemManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _accountSystemService.GetAsync(id);
            bool isUsing = await _accountSystemService.IsParentGroup(entity.Id);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.AccCode]);
            }
            isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.AccCode,
                                entity.AccCode, entity.OrgCode, entity.Year);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.AccCode]);
            }

            await _accountSystemService.DeleteAsync(id);
            await this.RemoveAllCache();
        }

        [Authorize(AccountingPermissions.AccountSystemManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _accountSystemService.GetAsync(item);
                bool isUsing = await _accountSystemService.IsParentGroup(entity.Id);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.AccCode]);
                }
                isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.AccCode,
                                    entity.AccCode, entity.OrgCode, entity.Year);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.AccCode]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _accountSystemService.DeleteManyAsync(deleteIds);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.AccountSystemManagerView)]
        public Task<PageResultDto<AccountSystemDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.AccountSystemManagerView)]
        public async Task<PageResultDto<AccountSystemDto>> GetListAsync(PageRequestDto dto)
        {
            await InsertDefaultAsync();
            var result = new PageResultDto<AccountSystemDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.AccCode).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AccountSystem, AccountSystemDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<List<AccountSystemComboItemDto>> GetViewListAsync()
        {
            var accountingSystem = await _accountSystemService.GetRepository()
                                .GetListAsync(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                            && p.Year == _webHelper.GetCurrentYear());
            return accountingSystem.Select(p => new AccountSystemComboItemDto()
            {
                Id = p.Id,
                Code = p.AccCode,
                Name = p.AccName,
                ParentId = p.ParentAccId,
                Value = p.AccCode,
                AttachAccSection = p.AttachAccSection,
                AccSectionCode = p.AccSectionCode,
                AttachContract = p.AttachContract,
                AttachCurrency = p.AttachCurrency,
                AttachProductCost = p.AttachProductCost,
                AttachPartner = p.AttachPartner,
                AttachWorkPlace = p.AttachWorkPlace,
                IsBalanceSheetAcc = p.IsBalanceSheetAcc
            })
            .OrderBy(p => p.Code).ToList();
        }
        public async Task<List<AccountSystemComboItemDto>> GetViewListByCodeAsync()
        {
            int year = _webHelper.GetCurrentYear();
            var accountingSystem = await _accountingCacheManager.GetAccountSystemsAsync(year);
            if (accountingSystem.Count == 0)
            {
                var yearCategory = await this.GetYearCategoryAsync();
                var defaultAccountSystems = await _accountingCacheManager.GetDefaultAccountSystemsAsync(yearCategory.UsingDecision.Value);
                return defaultAccountSystems.Select(p => new AccountSystemComboItemDto()
                {
                    Id = p.AccCode,
                    Code = p.AccCode,
                    Name = p.AccName,
                    ParentId = p.ParentCode,
                    Value = p.AccCode,
                    AttachAccSection = p.AttachAccSection,
                    AccSectionCode = p.AccSectionCode,
                    AttachContract = p.AttachContract,
                    AttachCurrency = p.AttachCurrency,
                    AttachProductCost = p.AttachProductCost,
                    AttachPartner = p.AttachPartner,
                    AttachWorkPlace = p.AttachWorkPlace,
                    IsBalanceSheetAcc = p.IsBalanceSheetAcc
                })
            .OrderBy(p => p.Code).ToList();
            }
            return accountingSystem.Select(p => new AccountSystemComboItemDto()
            {
                Id = p.AccCode,
                Code = p.AccCode,
                Name = p.AccName,
                ParentId = p.ParentCode,
                Value = p.AccCode,
                AttachAccSection = p.AttachAccSection,
                AccSectionCode = p.AccSectionCode,
                AttachContract = p.AttachContract,
                AttachCurrency = p.AttachCurrency,
                AttachProductCost = p.AttachProductCost,
                AttachPartner = p.AttachPartner,
                AttachWorkPlace = p.AttachWorkPlace,
                IsBalanceSheetAcc = p.IsBalanceSheetAcc
            })
            .OrderBy(p => p.Code).ToList();
        }

        public async Task<List<AccountSystemComboItemDto>> DataReferenceAsync(ComboRequestDto dto)
        {
            var accountSystem = await GetViewListByCodeAsync();
            accountSystem = accountSystem.Where(p => p.Code.StartsWith(dto.FilterValue)).ToList();
            return accountSystem;
        }

        public async Task<AccountSystemDto> GetByIdAsync(string accountSystemId)
        {
            var accountSystem = await _accountSystemService.GetAsync(accountSystemId);
            return ObjectMapper.Map<AccountSystem, AccountSystemDto>(accountSystem);
        }

        [Authorize(AccountingPermissions.AccountSystemManagerUpdate)]
        public async Task UpdateAsync(string id, CruAccountSystemDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (string.IsNullOrEmpty(dto.AccCode))
            {
                throw new Exception("Chưa nhập tài khoản!");
            }
            if (string.IsNullOrEmpty(dto.AccName))
            {
                throw new Exception("Chưa nhập tên tài khoản!");
            }
            if (dto.AccCode.Length < 3)
            {
                throw new Exception("Tài khoản phải lớn hơn 3 ký tự!");
            }
            var accountSys = await _accountSystemService.GetQueryableAsync();
            var lstAccountSys = accountSys.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == dto.AccCode).ToList();

            var linkCode = await _linkCodeService.GetQueryableAsync();
            var lstLinkcode = linkCode.Where(p => p.FieldCode == "AccCode").ToList();

            dto = await StandardDto(dto);
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            if ((dto.ParentCode ?? "") != "" || dto.ParentCode == null)
            {
                dto.AccType = "C";
            }
            if (dto.ParentCode.IsNullOrEmpty() == false)
            {
                var data = await _accountSystemService.GetAccountByAccCodeAsync(dto.ParentCode, dto.OrgCode, _webHelper.GetCurrentYear());
                data.AccType = "K";
                await _accountSystemService.UpdateAsync(data);
            }
            if (dto.Year != 0)
            {
                dto.Year = dto.Year;
            }
            else
            {
                dto.Year = _webHelper.GetCurrentYear();
            }
            //dto.ParentCode = await GetParentCodeAsync(dto.ParentAccId);
            var entity = await _accountSystemService.GetAsync(id);
            string oldCode = entity.AccCode;
            bool isChangeCode = dto.AccCode == entity.AccCode ? false : true;
            ObjectMapper.Map(dto, entity);
            await _accountSystemService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.AccCode, dto.AccCode, oldCode, entity.OrgCode, entity.Year);
            }
            await this.RemoveAllCache();
        }
        public async Task<List<AccountSystemTreeItemDto>> GetViewTreeByCodeAsync()
        {
            var accountingSystems = await _accountSystemService.GetRepository()
                                .GetListAsync(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                            && p.Year == _webHelper.GetCurrentYear());

            var tree = new List<AccountSystemTreeItemDto>();
            BuildTreeView(accountingSystems, null, tree);
            return tree;
        }
        public async Task<List<ComboItemDto>> GetListUsingDecision()
        {
            var lst = new List<ComboItemDto>();
            var tenantType = await this.GetTenantType();
            if (tenantType == null) return lst;
            if (tenantType == 2)
            {
                lst.Add(new ComboItemDto()
                {
                    Id = 88,
                    Value = "88",
                    Display = "88"
                });
                return lst;
            }
            lst.Add(new ComboItemDto()
            {
                Id = 133,
                Value = "133",
                Display = "133"
            });
            lst.Add(new ComboItemDto()
            {
                Id = 200,
                Value = "200",
                Display = "200"
            });
            return lst;
        }
        public async Task CreateDefaultAsync(int usingDecision)
        {
            await _licenseBusiness.CheckExpired();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                bool isExist = await _accountSystemService.IsExistAsync(orgCode, year);
                if (isExist)
                {
                    var lstAcc = await _accountSystemService.GetListAsync(orgCode, year);
                    await _accountSystemService.DeleteManyAsync(lstAcc);
                }
                var defaultAccounts = await _accountingCacheManager.GetDefaultAccountSystemsAsync(usingDecision);
                var dtos = defaultAccounts.Select(p => ObjectMapper.Map<DefaultAccountSystemDto, CrudDefaultAccountSystemDto>(p))
                                .ToList();
                dtos = this.GetNewTreeId(dtos);
                var entities = dtos.Select(p =>
                {
                    var dto = ObjectMapper.Map<CrudDefaultAccountSystemDto, AccountSystem>(p);
                    dto.OrgCode = orgCode;
                    dto.Year = year;
                    return dto;
                }).ToList();
                await _accountSystemService.CreateManyAsync(entities);
                await unitOfWork.CompleteAsync();
                await this.RemoveAllCache();
            }
            catch (Exception ex)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        #endregion
        #region Private Methods
        private async Task<IQueryable<AccountSystem>> Filter(PageRequestDto dto)
        {
            var queryable = await _accountSystemService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        && p.Year == _webHelper.GetCurrentYear());
            return queryable;
        }
        private async Task<CruAccountSystemDto> StandardDto(CruAccountSystemDto dto)
        {
            CruAccountSystemDto result = dto;
            result.OrgCode = _webHelper.GetCurrentOrgUnit();
            result.AccRank = 1;

            var parentAccount = await _accountSystemService.GetParentAccountByIdAsync(dto.ParentAccId);
            if (parentAccount != null)
            {
                result.AccRank = parentAccount.AccRank + 1;
                result.ParentCode = parentAccount.AccCode;
            }

            if (!string.IsNullOrEmpty(result.ParentCode) && !result.AccCode.StartsWith(result.ParentCode))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.NotStartWithParentCode),
                                    "Code is not valid.Code is not start with parent code");
            }

            string prefixName = new string('-', 2 * (result.AccRank - 1));
            result.AccNameTemp = $"{prefixName}{result.AccName}";
            result.AccNameTempE = $"{prefixName}{result.AccNameEn}";

            return result;
        }
        private async Task<string> GetParentCodeAsync(string parentId)
        {
            if (string.IsNullOrEmpty(parentId)) return null;
            var accountSystem = await _accountSystemService.FindAsync(parentId);
            if (accountSystem == null) return null;

            return accountSystem.AccCode;
        }
        private void BuildTreeView(List<AccountSystem> partnerGroups, string parentCode, List<AccountSystemTreeItemDto> tree)
        {
            var groups = partnerGroups.Where(p => p.ParentCode == parentCode)
                                .OrderBy(p => p.AccCode).ToList();
            if (groups.Count == 0) return;

            foreach (var item in groups)
            {
                var child = new AccountSystemTreeItemDto()
                {
                    Id = item.AccCode,
                    Value = item.AccCode,
                    Open = true,
                    Code = item.AccCode,
                    Name = item.AccName,
                    ParentCode = item.ParentCode
                };

                child.Data = new List<AccountSystemTreeItemDto>();
                BuildTreeView(partnerGroups, item.AccCode, child.Data);
                if (child.Data.Count == 0)
                {
                    child.Data = null;
                    child.Open = null;
                }

                tree.Add(child);
            }
        }
        private async Task<int?> GetTenantType()
        {
            var tenantExtendInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantExtendInfo == null) return null;
            return tenantExtendInfo.TenantType;
        }
        private async Task<YearCategoryDto> GetYearCategoryAsync()
        {
            int year = _webHelper.GetCurrentYear();
            return await _accountingCacheManager.GetYearCategoryByYearAsync(year);
        }
        private List<CrudDefaultAccountSystemDto> GetNewTreeId(List<CrudDefaultAccountSystemDto> dtos)
        {
            foreach (var item in dtos)
            {
                string oldId = item.Id;
                item.Id = this.GetNewObjectId();
                var lstChilds = dtos.Where(p => p.ParentAccId == oldId);
                foreach (var child in lstChilds)
                {
                    child.ParentAccId = item.Id;
                }
            }
            return dtos;
        }
        private async Task RemoveAllCache()
        {
            await _accountingCacheManager.RemoveClassCache<AccountSystemDto>();
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            bool isExists = await _accountSystemService.IsExistAsync(orgCode, year);
            if (isExists) return;

            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return;
            if (yearCategory.UsingDecision == null) return;

            await this.CreateDefaultAsync(yearCategory.UsingDecision.Value);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        #endregion
    }

}
