using Accounting.BaseDtos.Customines;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Others.BusinessCategories;
using Accounting.Catgories.VoucherCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Vouchers;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.Business
{
    public class VoucherNumberBusiness : BaseBusiness, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly VoucherNumberService _voucherNumberService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IObjectMapper _objectMapper;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public VoucherNumberBusiness(VoucherNumberService voucherNumberService,
                            UserService userService,
                            VoucherCategoryService voucherCategoryService,
                            IUnitOfWorkManager unitOfWorkManager,
                            WebHelper webHelper,
                            IObjectMapper objectMapper,
                            AccountingCacheManager accountingCacheManager,
                            IStringLocalizer<AccountingResource> localizer
                            ) : base(localizer)
        {
            _voucherNumberService = voucherNumberService;
            _userService = userService;
            _voucherCategoryService = voucherCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _objectMapper = objectMapper;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        public async Task<VoucherNumberCustomineDto> AutoVoucherNumberAsync(string voucherCode, DateTime voucherDate)
        {
            var res = await GetVoucherNumberAsync(voucherCode, voucherDate);
            await IncreaseTotalNumberAsync(res.Id);
            return res;
        }
        public async Task<VoucherNumberCustomineDto> GetVoucherNumberAsync(string voucherCode, DateTime voucherDate)
        {
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(voucherCode);            
            if (voucherCategory == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:VoucherCategoryCodeIsNotFound"]);
            }
            var voucherNumberCustomer = voucherCategory.IncreaseNumberMethod switch
            {
                "D" => await this.GetVoucherNumberByDayAsync(voucherCode, voucherDate, voucherCategory),
                "M" => await this.GetVoucherNumberByMonthAsync(voucherCode, voucherDate, voucherCategory),
                _  => await this.GetVoucherNumberByYearAsync(voucherCode, voucherDate, voucherCategory)
            };

            return voucherNumberCustomer;
        }
        public async Task UpdateVoucherNumberAsync(string voucherCode, string voucherNumber, DateTime voucherDate)
        {
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(voucherCode);
            if (voucherCategory == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:VoucherCategoryCodeIsNotFound"]);
            }
            var voucherNumberSystemDto = await GetVoucherNumberAsync(voucherCode, voucherDate);
            var voucherNumberSystem = voucherNumberSystemDto.VoucherNumber;
            string formDto = "";
            string formSystem = "";
            int numberDto = 0;
            int numberSystem = 0;
            if (voucherNumber.Length < voucherCategory.Suffix.Length || !voucherNumber.StartsWith(voucherCategory.Prefix + voucherCategory.SeparatorCharacter))
            {
                return;
            }
            if ("M,D".Contains(voucherCategory.IncreaseNumberMethod))
            {
                var indexOfDto = voucherNumber.IndexOf(voucherCategory.SeparatorCharacter);
                indexOfDto = indexOfDto < 0 ? 0 : indexOfDto;
                formDto = voucherNumber.Substring(0, indexOfDto);
                formSystem = voucherNumberSystem.Substring(0, voucherNumberSystem.IndexOf(voucherCategory.SeparatorCharacter));
                numberDto = int.Parse(voucherNumber.Substring(voucherNumber.IndexOf(voucherCategory.SeparatorCharacter) + 1));
                numberSystem = int.Parse(voucherNumberSystem.Substring(voucherNumberSystem.IndexOf(voucherCategory.SeparatorCharacter) + 1));
            }
            else
            {
                var indexOfDto = voucherNumber.IndexOf(voucherCategory.Prefix);
                indexOfDto = indexOfDto < 0 ? 0 : indexOfDto + voucherCategory.Prefix.Length + 1;
                numberDto = indexOfDto == 0 ? 0 : int.Parse(voucherNumber.Substring(indexOfDto));
                var indexOfSystem = voucherNumberSystem.IndexOf(voucherCategory.Prefix);
                indexOfSystem = indexOfSystem < 0 ? 0 : indexOfSystem + voucherCategory.Prefix.Length + 1;
                numberSystem = int.Parse(voucherNumberSystem.Substring(indexOfSystem));
            }

            if (formDto == formSystem && voucherNumber.Length == voucherNumberSystem.Length && numberDto > numberSystem)
            {
                var entity = await _voucherNumberService.GetAsync(voucherNumberSystemDto.Id);
                entity.TotalNumberRecord = numberDto;
            }
        }
        
        public async Task<VoucherNumberCustomineDto> AutoBusinessVoucherNumberAsync(string voucherCode, string businessCode, DateTime voucherDate)
        {
            var res = await GetBusinessVoucherNumberAsync(voucherCode, businessCode, voucherDate);
            await IncreaseTotalNumberAsync(res.Id);
            return res;
        }
        public async Task<VoucherNumberCustomineDto> GetBusinessVoucherNumberAsync(string voucherCode, string businessCode, DateTime voucherDate)
        {
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(voucherCode);
            var businessCategory = await _accountingCacheManager.GetBusinessCategoryByCodeAsync(businessCode);
            if (voucherCategory == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:VoucherCategoryCodeIsNotFound"]);
            }
            if (businessCategory == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:BusinessCodeNotExist"]);
            }
            var voucherNumberCustomer = voucherCategory.IncreaseNumberMethod switch
            {
                "D" => await this.GetBusinessVoucherNumberByDayAsync(businessCode, voucherDate, businessCategory),
                "M" => await this.GetBusinessVoucherNumberByMonthAsync(businessCode, voucherDate, businessCategory),
                _ => await this.GetBusinessVoucherNumberByYearAsync(businessCode, voucherDate, businessCategory)
            };

            return voucherNumberCustomer;
        }
        public async Task UpdateBusinessVoucherNumberAsync(string voucherCode, string businessCode, string voucherNumber, DateTime voucherDate)
        {
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(voucherCode);
            var businessCategory = await _accountingCacheManager.GetBusinessCategoryByCodeAsync(businessCode);
            if (voucherCategory == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:VoucherCategoryCodeIsNotFound"]);
            }
            if (businessCategory == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:BusinessCodeNotExist", businessCode]);
            }
            voucherCategory.Prefix = businessCategory.Prefix;
            voucherCategory.Suffix = businessCategory.Suffix;
            voucherCategory.SeparatorCharacter = businessCategory.Separator;
            var voucherNumberSystemDto = await GetBusinessVoucherNumberAsync(voucherCode, businessCode, voucherDate);
            var voucherNumberSystem = voucherNumberSystemDto.VoucherNumber;
            string formDto = "";
            string formSystem = "";
            int numberDto = 0;
            int numberSystem = 0;
            if ("M,D".Contains(voucherCategory.IncreaseNumberMethod))
            {
                var indexOfDto = voucherNumber.IndexOf(voucherCategory.SeparatorCharacter);
                indexOfDto = indexOfDto < 0 ? 0 : indexOfDto;
                if (indexOfDto == 0)
                {
                    return;
                }
                formDto = voucherNumber.Substring(0, indexOfDto);
                formSystem = voucherNumberSystem.Substring(0, voucherNumberSystem.IndexOf(voucherCategory.SeparatorCharacter));
                numberDto = int.Parse(voucherNumber.Substring(voucherNumber.IndexOf(voucherCategory.SeparatorCharacter) + 1));
                numberSystem = int.Parse(voucherNumberSystem.Substring(voucherNumberSystem.IndexOf(voucherCategory.SeparatorCharacter) + 1));
            }
            else
            {
                var indexOfDto = voucherNumber.IndexOf(voucherCategory.Prefix);
                if (indexOfDto == 0)
                {
                    return;
                }
                indexOfDto = indexOfDto < 0 ? 0 : indexOfDto + voucherCategory.Prefix.Length;
                numberDto = int.Parse(voucherNumber.Substring(indexOfDto));
                var indexOfSystem = voucherNumberSystem.IndexOf(voucherCategory.Prefix);
                indexOfSystem = indexOfSystem < 0 ? 0 : indexOfSystem + voucherCategory.Prefix.Length;
                numberSystem = int.Parse(voucherNumberSystem.Substring(indexOfSystem));
            }

            if (formDto == formSystem && voucherNumber.Length == voucherNumberSystem.Length && numberDto > numberSystem)
            {
                var entity = await _voucherNumberService.GetAsync(voucherNumberSystemDto.Id);
                entity.TotalNumberRecord = numberDto;
            }
        }

        #endregion
        #region Privates
        private async Task IncreaseTotalNumberAsync(string id)
        {
            if (id == null) return;
            var entity = await _voucherNumberService.GetAsync(id);
            entity.TotalNumberRecord++;
            await _voucherNumberService.UpdateAsync(entity, true);
        }
        private async Task<VoucherNumberCustomineDto> GetVoucherNumberByMonthAsync(string voucherCode, DateTime voucherDate,
                                VoucherCategoryDto voucherCategory)
        {
            var voucherNumberCustomer = new VoucherNumberCustomineDto();

            if (await _voucherNumberService.IsExistVoucherNumber(voucherCode, voucherDate.Day, voucherDate.Month, voucherDate.Year))
            {
                var voucherNumber = _voucherNumberService.GetDataVoucherNumber(voucherCode, voucherDate.Day, voucherDate.Month, voucherDate.Year);
                voucherNumberCustomer.Id = voucherNumber.Result.Id;
                decimal totalNumberRecord = voucherNumber.Result.TotalNumberRecord;
                voucherNumberCustomer.VoucherNumber = voucherCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2) 
                                + voucherDate.Month.ToString().PadLeft(2, '0') + voucherDate.Day.ToString().PadLeft(2, '0') 
                                + voucherCategory.SeparatorCharacter + (totalNumberRecord + 1).ToString().PadLeft((voucherCategory.Suffix ?? "0000").Length, '0');
                return voucherNumberCustomer;
            }
            
            CrudVoucherNumberDto dto = new CrudVoucherNumberDto();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Day = voucherDate.Day;
            dto.Month = voucherDate.Month;
            dto.Year = voucherDate.Year;
            dto.VoucherCode = voucherCode;
            dto.TotalNumberRecord = 0;
            var entity = _objectMapper.Map<CrudVoucherNumberDto, VoucherNumber>(dto);
            var result = await _voucherNumberService.CreateAsync(entity, true);
            voucherNumberCustomer.Id = result.Id;
            voucherNumberCustomer.VoucherNumber = voucherCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2) + voucherDate.Month.ToString().PadLeft(2, '0') + voucherDate.Day.ToString().PadLeft(2, '0') + voucherCategory.SeparatorCharacter + (dto.TotalNumberRecord + 1).ToString().PadLeft((voucherCategory.Suffix ?? "0000").Length, '0');
            return voucherNumberCustomer;            
        }
        private async Task<VoucherNumberCustomineDto> GetVoucherNumberByDayAsync(string voucherCode, DateTime voucherDate,
                                VoucherCategoryDto voucherCategory)
        {
            var voucherNumberCustomer = new VoucherNumberCustomineDto();

            if (await _voucherNumberService.IsExistVoucherNumber(voucherCode, voucherDate.Day, voucherDate.Month, voucherDate.Year))
            {
                var voucherNumber = _voucherNumberService.GetDataVoucherNumber(voucherCode, voucherDate.Day, voucherDate.Month, voucherDate.Year);
                voucherNumberCustomer.Id = voucherNumber.Result.Id;
                decimal totalNumberRecord = voucherNumber.Result.TotalNumberRecord;
                voucherNumberCustomer.VoucherNumber = voucherCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2) + voucherDate.Month.ToString().PadLeft(2, '0') + voucherDate.Day.ToString().PadLeft(2, '0') + voucherCategory.SeparatorCharacter + (totalNumberRecord + 1).ToString().PadLeft((voucherCategory.Suffix ?? "0000").Length, '0');
                return voucherNumberCustomer;
            }
            
            CrudVoucherNumberDto dto = new CrudVoucherNumberDto();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Day = voucherDate.Day;
            dto.Month = voucherDate.Month;
            dto.Year = voucherDate.Year;
            dto.VoucherCode = voucherCode;
            dto.TotalNumberRecord = 0;
            var entity = _objectMapper.Map<CrudVoucherNumberDto, VoucherNumber>(dto);
            var result = await _voucherNumberService.CreateAsync(entity, true);
            voucherNumberCustomer.Id = result.Id;
            voucherNumberCustomer.VoucherNumber = voucherCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2) + voucherDate.Month.ToString().PadLeft(2, '0') 
                                    + voucherDate.Day.ToString().PadLeft(2, '0') 
                                    + voucherCategory.SeparatorCharacter 
                                    + (dto.TotalNumberRecord + 1).ToString().PadLeft((voucherCategory.Suffix ?? "0000").Length, '0');
            return voucherNumberCustomer;            
        }
        private async Task<VoucherNumberCustomineDto> GetVoucherNumberByYearAsync(string voucherCode, DateTime voucherDate,
                                VoucherCategoryDto voucherCategory)
        {
            var voucherNumberCustomer = new VoucherNumberCustomineDto();

            if (await _voucherNumberService.IsExistVoucherNumber(voucherCode, 0, 0, voucherDate.Year))
            {
                var voucherNumber = _voucherNumberService.GetDataVoucherNumber(voucherCode, 0, 0, voucherDate.Year);
                voucherNumberCustomer.Id = voucherNumber.Result.Id;
                decimal totalNumberRecord = voucherNumber.Result.TotalNumberRecord;
                voucherNumberCustomer.VoucherNumber = voucherCategory.Prefix + (totalNumberRecord + 1).ToString().PadLeft(voucherCategory.Suffix.Length, '0');
                return voucherNumberCustomer;
            }
            
            CrudVoucherNumberDto dto = new CrudVoucherNumberDto();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Day = 0;
            dto.Month = 0;
            dto.Year = voucherDate.Year;
            dto.VoucherCode = voucherCode;
            dto.TotalNumberRecord = 0;
            var entity = _objectMapper.Map<CrudVoucherNumberDto, VoucherNumber>(dto);
            var result = await _voucherNumberService.CreateAsync(entity, true);
            voucherNumberCustomer.Id = result.Id;
            voucherNumberCustomer.VoucherNumber = voucherCategory.Prefix + (dto.TotalNumberRecord + 1).ToString().PadLeft(voucherCategory.Suffix.Length, '0');
            return voucherNumberCustomer;            
        }

        private async Task<VoucherNumberCustomineDto> GetBusinessVoucherNumberByMonthAsync(string businessCode, DateTime voucherDate,
                                BusinessCategoryDto businessCategory)
        {
            var voucherNumberCustomer = new VoucherNumberCustomineDto();

            if (await _voucherNumberService.IsExistBusinessVoucherNumber(businessCode, voucherDate.Day, voucherDate.Month, voucherDate.Year))
            {
                var voucherNumber = _voucherNumberService.GetDataBusinessVoucherNumber(businessCode, voucherDate.Day, voucherDate.Month, voucherDate.Year);
                voucherNumberCustomer.Id = voucherNumber.Result.Id;
                decimal totalNumberRecord = voucherNumber.Result.TotalNumberRecord;
                voucherNumberCustomer.VoucherNumber = businessCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2)
                                + voucherDate.Month.ToString().PadLeft(2, '0') + voucherDate.Day.ToString().PadLeft(2, '0')
                                + businessCategory.Separator + (totalNumberRecord + 1).ToString().PadLeft((businessCategory.Suffix ?? "0000").Length, '0');
                return voucherNumberCustomer;
            }

            CrudVoucherNumberDto dto = new CrudVoucherNumberDto();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Day = voucherDate.Day;
            dto.Month = voucherDate.Month;
            dto.Year = voucherDate.Year;
            dto.BusinessCode = businessCode;
            dto.TotalNumberRecord = 0;
            var entity = _objectMapper.Map<CrudVoucherNumberDto, VoucherNumber>(dto);
            var result = await _voucherNumberService.CreateAsync(entity, true);
            voucherNumberCustomer.Id = result.Id;
            voucherNumberCustomer.VoucherNumber = businessCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2) + voucherDate.Month.ToString().PadLeft(2, '0') + voucherDate.Day.ToString().PadLeft(2, '0') + businessCategory.Separator + (dto.TotalNumberRecord + 1).ToString().PadLeft((businessCategory.Suffix ?? "0000").Length, '0');
            return voucherNumberCustomer;
        }
        private async Task<VoucherNumberCustomineDto> GetBusinessVoucherNumberByDayAsync(string businessCode, DateTime voucherDate,
                                BusinessCategoryDto businessCategory)
        {
            var voucherNumberCustomer = new VoucherNumberCustomineDto();

            if (await _voucherNumberService.IsExistBusinessVoucherNumber(businessCode, voucherDate.Day, voucherDate.Month, voucherDate.Year))
            {
                var voucherNumber = _voucherNumberService.GetDataBusinessVoucherNumber(businessCode, voucherDate.Day, voucherDate.Month, voucherDate.Year);
                voucherNumberCustomer.Id = voucherNumber.Result.Id;
                decimal totalNumberRecord = voucherNumber.Result.TotalNumberRecord;
                voucherNumberCustomer.VoucherNumber = businessCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2) + voucherDate.Month.ToString().PadLeft(2, '0') + voucherDate.Day.ToString().PadLeft(2, '0') + businessCategory.Separator + (totalNumberRecord + 1).ToString().PadLeft((businessCategory.Suffix ?? "0000").Length, '0');
                return voucherNumberCustomer;
            }

            CrudVoucherNumberDto dto = new CrudVoucherNumberDto();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Day = voucherDate.Day;
            dto.Month = voucherDate.Month;
            dto.Year = voucherDate.Year;
            dto.BusinessCode = businessCode;
            dto.TotalNumberRecord = 0;
            var entity = _objectMapper.Map<CrudVoucherNumberDto, VoucherNumber>(dto);
            var result = await _voucherNumberService.CreateAsync(entity, true);
            voucherNumberCustomer.Id = result.Id;
            voucherNumberCustomer.VoucherNumber = businessCategory.Prefix + voucherDate.Year.ToString().Substring(2, 2) + voucherDate.Month.ToString().PadLeft(2, '0')
                                    + voucherDate.Day.ToString().PadLeft(2, '0')
                                    + businessCategory.Separator
                                    + (dto.TotalNumberRecord + 1).ToString().PadLeft((businessCategory.Suffix ?? "0000").Length, '0');
            return voucherNumberCustomer;
        }
        private async Task<VoucherNumberCustomineDto> GetBusinessVoucherNumberByYearAsync(string businessCode, DateTime voucherDate,
                                BusinessCategoryDto businessCategory)
        {
            var voucherNumberCustomer = new VoucherNumberCustomineDto();

            if (await _voucherNumberService.IsExistBusinessVoucherNumber(businessCode, 0, 0, voucherDate.Year))
            {
                var voucherNumber = _voucherNumberService.GetDataBusinessVoucherNumber(businessCode, 0, 0, voucherDate.Year);
                voucherNumberCustomer.Id = voucherNumber.Result.Id;
                decimal totalNumberRecord = voucherNumber.Result.TotalNumberRecord;
                voucherNumberCustomer.VoucherNumber = businessCategory.Prefix + (totalNumberRecord + 1).ToString().PadLeft(businessCategory.Suffix.Length, '0');
                return voucherNumberCustomer;
            }

            CrudVoucherNumberDto dto = new CrudVoucherNumberDto();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Day = 0;
            dto.Month = 0;
            dto.Year = voucherDate.Year;
            dto.BusinessCode = businessCode;
            dto.TotalNumberRecord = 0;
            var entity = _objectMapper.Map<CrudVoucherNumberDto, VoucherNumber>(dto);
            var result = await _voucherNumberService.CreateAsync(entity, true);
            voucherNumberCustomer.Id = result.Id;
            voucherNumberCustomer.VoucherNumber = businessCategory.Prefix + (dto.TotalNumberRecord + 1).ToString().PadLeft(businessCategory.Suffix.Length, '0');
            return voucherNumberCustomer;
        }

        #endregion
    }
}
