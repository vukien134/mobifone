using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines.AssetTool;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.AssetTools;
using Accounting.Constants;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Validation;

namespace Accounting.Categories.AssetTools
{
    public class AssetToolAccountAppService : AccountingAppService, IAssetToolAccountAppService
    {
        #region Field
        private readonly AssetToolService _assetToolService;
        private readonly AssetToolAccountService _assetToolAccountService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly AssetToolStoppingDepreciationService _assetToolStoppingDepreciationService;
        private readonly AssetToolAccessoryService _assetToolAccessoryService;
        private readonly ExcelService _excelService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        private readonly AccountSystemService _accountSystemService;
        private readonly AccPartnerService _accPartnerService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccSectionService _accSectionService;
        private readonly AccCaseService _accCaseService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly CacheManager _cacheManager;
        private readonly IObjectMapper _objectMapper;
        private readonly GeneralDomainService _generalDomainService;
        #endregion
        #region Ctor
        public AssetToolAccountAppService(AssetToolService assetToolService,
                                AssetToolAccountService assetToolAccountService,
                                ExcelService excelService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                WebHelper webHelper,
                                AccountSystemService accountSystemService,
                                AccPartnerService accPartnerService,
                                WorkPlaceSevice workPlaceSevice,
                                FProductWorkService fProductWorkService,
                                AccSectionService accSectionService,
                                AccCaseService accCaseService,
                                LicenseBusiness licenseBusiness,
                                IStringLocalizer<AccountingResource> localizer,
                                CacheManager cacheManager,
                                IObjectMapper objectMapper,
                                GeneralDomainService generalDomainService
            )
        {
            _assetToolService = assetToolService;
            _assetToolAccountService = assetToolAccountService;
            _excelService = excelService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _accountSystemService = accountSystemService;
            _accPartnerService = accPartnerService;
            _workPlaceSevice = workPlaceSevice;
            _fProductWorkService = fProductWorkService;
            _accSectionService = accSectionService;
            _accCaseService = accCaseService;
            _licenseBusiness = licenseBusiness;
            _localizer = localizer;
            _cacheManager = cacheManager;
            _objectMapper = objectMapper;
            _generalDomainService = generalDomainService;
        }
        #endregion
        public async Task<AssetToolAccountDto> CreateAsync(CrudAssetToolAccountDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Asset;        
            var entity = ObjectMapper.Map<CrudAssetToolAccountDto, AssetToolAccount>(dto);
            try
            {
                var assetToolDetails = await _assetToolAccountService.GetByAssetToolIdAsync(dto.AssetToolId);
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolAccountService.DeleteManyAsync(assetToolDetails);
                var result = await _assetToolAccountService.CreateAsync(entity);
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(result);
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }



        public async Task<List<AssetToolAccountDto>> CreateListAsync(ObjectListAssetToolAccountDto listDto)
        {
            await _licenseBusiness.CheckExpired();
            List<AssetToolAccountDto> listResult = new List<AssetToolAccountDto>();
            var listAssetToolAccount = from a in listDto.Data
                                       group a.AssetToolId by a.AssetToolId into g
                                       select new { AssetToolId = g.Key };
            var assetToolAccounts = await _assetToolAccountService.GetByAssetToolIdAsync(listDto.AssetToolId);
            assetToolAccounts = assetToolAccounts.Where(p => p.Year == _webHelper.GetCurrentYear()).ToList();
            if (assetToolAccounts != null)
            {
                await _assetToolAccountService.DeleteManyAsync(assetToolAccounts);
            }
            int ord = 1;
            var org = _webHelper.GetCurrentOrgUnit();
            var year = _webHelper.GetCurrentYear();

            foreach (var dto in listDto.Data)
            {
                var status = await this.CheckExistedAccCode(dto.DebitAcc, dto.CreditAcc);               
                var dataDebit = await _accountSystemService.GetAccountByAccCodeAsync(dto.DebitAcc,org,year);
                var dataCredit = await _accountSystemService.GetAccountByAccCodeAsync(dto.CreditAcc, org, year);
                await this.CheckRequiredAttachAcc(dataDebit, dataCredit, dto);
                dto.CreatorName = await _userService.GetCurrentUserNameAsync();
                dto.Id = GetNewObjectId();
                dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                dto.Year = _webHelper.GetCurrentYear();
                dto.Ord0 = "B" + ord.ToString().PadLeft(9, '0');
                var entity = ObjectMapper.Map<CrudAssetToolAccountDto, AssetToolAccount>(dto);
                var result = await _assetToolAccountService.CreateAsync(entity);
                listResult.Add(ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(result));
                ord++;               
            }
            return listResult;
        }
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolAccountService.DeleteAsync(id);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public Task<PageResultDto<AssetToolAccountDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<AssetToolAccountDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AssetToolAccountDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.AssetToolId).OrderBy(p => p.Year).OrderBy(p => p.Month).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudAssetToolAccountDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.AssetOrTool = AssetToolConst.Asset;
            dto.Id = id;
            var entity = await _assetToolAccountService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                await _assetToolAccountService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<AssetToolAccountDto> GetByIdAsync(string assetToolAccountId)
        {
            var assetToolAccount = await _assetToolAccountService.GetAsync(assetToolAccountId);
            return ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(assetToolAccount);
        }
        public async Task<List<AssetToolAccountDto>> GetListByAssetToolIdAsync(string assetToolId)
        {
            var result = new List<AssetToolAccountDto>();
            var query = await _assetToolAccountService.GetQueryableAsync();
            query = query.Where(p => p.AssetToolId == assetToolId && p.Year == _webHelper.GetCurrentYear());
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(p)).ToList();
            return result;
        }
        #region Private
        private async Task<IQueryable<AssetToolAccount>> Filter(PageRequestDto dto)
        {
            var queryable = await _assetToolAccountService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                          && p.Year == _webHelper.GetCurrentYear()
                                          && p.AssetOrTool == AssetToolConst.Asset);
            return queryable;
        }
        private async Task<List<AccountSystemDto>> GetListAccount()
        {
            var org = _webHelper.GetCurrentOrgUnit();
            var year = _webHelper.GetCurrentYear();
            string key = string.Format(CacheKeyManager.AccountSystemByYear, org, year);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<AccountSystemDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _accountSystemService.GetAccounts(org, year);
                    return partnes.Select(p => _objectMapper.Map<AccountSystem, AccountSystemDto>(p)).ToList();
                }
            );
        }

        private async Task<bool> CheckExistedPartnerCode(string partnerCode)
        {
            var org = _webHelper.GetCurrentOrgUnit();
            var dataCheck = await _accPartnerService.GetAllByOrgCode(org);

            var data = dataCheck.Any(x => x.Code == partnerCode);
            if (data == false)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.NotFoundEntity),
                         _localizer["Err:PartnerCodeNotExist"]);
            }
            return true;
        }
        private async Task<bool> CheckExistedAccCode(string debitAcc, string creditAcc) 
        {            
            List<ValidationResult> errors = new List<ValidationResult>();
            var dataCheck = await this.GetListAccount();

            var dataCredit = dataCheck.Any(x => x.AccCode == creditAcc);
            if (dataCredit == false)
            {
                errors.Add(new ValidationResult(_localizer["Err:CreditAccCodeNotExist", creditAcc]));
            }

            var dataDebit = dataCheck.Any(x => x.AccCode == debitAcc);
            if (dataDebit == false)
            {
                errors.Add(new ValidationResult(_localizer["Err:DebitAccCodeNotExist", debitAcc]));
            }

            if (errors.Count > 0)
            {
                throw new AbpValidationException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.NotFoundEntity),
                    errors);
            }
            return true;
        }
        private async Task<bool> CheckExistedWorkPlaceCode(string workPlaceCode)
        {
            var org = _webHelper.GetCurrentOrgUnit();
            var dataCheck = await _workPlaceSevice.GetListAsync(org);

            var data = dataCheck.Any(x => x.Code == workPlaceCode);
            if (data == false)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.WorkPlace, ErrorCode.NotFoundEntity),
                         _localizer["Err:WorkPlaceCodeNotExist"]);
            }
            return true;
        }
        private async Task<bool> CheckExistedFProductWorkCode(string fProductWorkCode)
        {
            var org = _webHelper.GetCurrentOrgUnit();
            var dataCheck = await _fProductWorkService.GetListAsync(org);

            var data = dataCheck.Any(x => x.Code == fProductWorkCode);
            if (data == false)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.NotFoundEntity),
                         _localizer["Err:FProductCodeNotExist"]);
            }
            return true;
        }
        private async Task<bool> CheckExistedCaseCode(string caseCode)
        {
            var org = _webHelper.GetCurrentOrgUnit();
            var dataCheck = await _accCaseService.GetAllByOrgCode(org);

            var data = dataCheck.Any(x => x.Code == caseCode);
            if (data == false)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccCase, ErrorCode.NotFoundEntity),
                         _localizer["Err:CaseCodeNotExist"]);
            }
            return true;
        }
        private async Task<bool> CheckExistedFProductCode(string fProductCode)
        {
            var org = _webHelper.GetCurrentOrgUnit();
            var dataCheck = await _fProductWorkService.GetListAsync(org);

            var data = dataCheck.Any(x => x.Code == fProductCode);
            if (data == false)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.NotFoundEntity),
                         _localizer["Err:FProductCodeNotExist"]);
            }
            return true;
        }
        private async Task<bool> CheckExistedSectionCode(string sectionCode)
        {
            var org = _webHelper.GetCurrentOrgUnit();
            var dataCheck = await _accSectionService.GetAll(org);

            var data = dataCheck.Any(x => x.Code == sectionCode);
            if (data == false)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccSection, ErrorCode.NotFoundEntity),
                         _localizer["Err:SectionCodeNotExist"]);
            }
            return true;
        }       

        private async Task CheckValidateData(string partnerCode,string workPlaceCode, string fProductCode, string sectionCode, string caseCode)
        {
            var org =  _webHelper.GetCurrentOrgUnit();
            Dictionary<Type, object[]> dict = new();
            dict.Add(typeof(AccPartnerService), new object[] { org, partnerCode });
            dict.Add(typeof(WorkPlaceSevice), new object[] { org, workPlaceCode });
            dict.Add(typeof(FProductWorkService), new object[] { org, fProductCode });
            dict.Add(typeof(AccSectionService), new object[] { org, sectionCode });
            dict.Add(typeof(AccCaseService), new object[] { org, caseCode });
            var result = await _generalDomainService.IsExistCode(dict);

            var list = result.Where(p => p.Value == false)
                                .Select(p => p.Key)
                                .ToList();
            if (list.Count == 0) {  return ; }
            foreach (var item in list)
            {
                var ex = _generalDomainService.GetExistException(item.Name, ErrorCode.NotFoundEntity);
                throw ex;  
            }          
           
        }
        private async Task CheckRequiredAttachAcc(AccountSystem debitAcc,AccountSystem creditAcc,CrudAssetToolAccountDto dto)
        {
            List<ValidationResult> errors = new List<ValidationResult>();        
            if ((creditAcc.AttachPartner == "C" || debitAcc.AttachPartner =="C" )&& string.IsNullOrWhiteSpace(dto.PartnerCode))
            {
                errors.Add(new ValidationResult(_localizer["PartnerCodeNotNull"]));
            }
            if ((creditAcc.AttachAccSection == "C" || debitAcc.AttachAccSection =="C")&& string.IsNullOrWhiteSpace(dto.SectionCode))
            {
                errors.Add(new ValidationResult(_localizer["SectionCodeNotNull"]));
            }
            if ((creditAcc.AttachWorkPlace == "C" ||debitAcc.AttachWorkPlace =="C")&& string.IsNullOrWhiteSpace(dto.WorkPlaceCode))
            {
                errors.Add(new ValidationResult(_localizer["WorkPlaceCodeNotNull"]));
            }
            if((creditAcc.AttachProductCost == "C" || debitAcc.AttachProductCost == "C")&& string.IsNullOrWhiteSpace(dto.FProductWorkCode))
            {
                errors.Add(new ValidationResult(_localizer["FProductWorkCodeNotNull"]));
            }
            if (string.IsNullOrWhiteSpace(dto.PartnerCode) == false && (await this.CheckExistedPartnerCode(dto.PartnerCode)) == false )
            {
                errors.Add(new ValidationResult(_localizer["Err:PartnerCodeNotExist"]));
            }
            if (string.IsNullOrWhiteSpace(dto.SectionCode) == false && (await this.CheckExistedSectionCode(dto.SectionCode)) == false )
            {
                errors.Add(new ValidationResult(_localizer["Err:WorkPlaceCodeNotExist"]));
            }
            if (string.IsNullOrWhiteSpace(dto.WorkPlaceCode) == false && await this.CheckExistedWorkPlaceCode(dto.WorkPlaceCode) == false )
            {
                errors.Add(new ValidationResult(_localizer["Err:WorkPlaceCodeNotExist"]));
            }
            if (string.IsNullOrWhiteSpace(dto.FProductWorkCode) == false && await this.CheckExistedFProductWorkCode(dto.FProductWorkCode) == false)
            {
                errors.Add(new ValidationResult(_localizer["Err:FProductCodeNotExist"]));
            }
            if (string.IsNullOrWhiteSpace(dto.CaseCode) == false && await this.CheckExistedCaseCode(dto.CaseCode) == false)
            {
                errors.Add(new ValidationResult(_localizer["Err:CaseCodeNotExist"]));
            }

            if (errors.Count > 0)
            {
                throw new AbpValidationException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.NotFoundEntity),
                    errors);
            }

        }

        #endregion
    }
}
