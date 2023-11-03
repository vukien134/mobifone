using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Partners;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
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
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.Partners
{
    public class AccPartnerAppService : AccountingAppService, IAccPartnerAppService
    {
        #region Field
        private readonly AccPartnerService _accPartnerService;
        private readonly BankPartnerService _bankPartnerService;
        private readonly UserService _userService;
        private readonly ExcelService _excelService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly PartnerGroupAppService _partnerGroupAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public AccPartnerAppService(AccPartnerService accPartnerService,
                                BankPartnerService bankPartnerService,
                                UserService userService,
                                ExcelService excelService,
                                PartnerGroupService partnerGroupService,
                                PartnerGroupAppService partnerGroupAppService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _accPartnerService = accPartnerService;
            _bankPartnerService = bankPartnerService;
            _userService = userService;
            _excelService = excelService;
            _partnerGroupService = partnerGroupService;
            _partnerGroupAppService = partnerGroupAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion

        [Authorize(AccountingPermissions.PartnerManagerCreate)]
        public async Task<AccPartnerDto> CreateAsync(CrudAccPartnerDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudAccPartnerDto, AccPartner>(dto);
            await _accPartnerService.CheckPartnerGroup(entity);
            var result = await _accPartnerService.CreateAsync(entity);
            return ObjectMapper.Map<AccPartner, AccPartnerDto>(result);
        }

        [Authorize(AccountingPermissions.PartnerManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _accPartnerService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.PartnerCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _accPartnerService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.PartnerManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _accPartnerService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.PartnerCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }

            string[] deleteIds = dto.ListId.ToArray();
            await _accPartnerService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.PartnerManagerView)]
        public async Task<PageResultDto<AccPartnerDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.PartnerManagerView)]
        public async Task<PageResultDto<AccPartnerDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AccPartnerDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AccPartner, AccPartnerDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<List<PartnerComboItemDto>> DataReference(ComboRequestDto dto)
        {
            var partnes = await _accPartnerService.GetDataReference(_webHelper.GetCurrentOrgUnit(), dto.FilterValue);
            return partnes.Select(p => new PartnerComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name,
                Address = p.Address,
                Email = p.Email,
                Representative = p.Representative,
                OtherContact = p.OtherContact,
                ContactPerson = p.ContactPerson,
                TaxCode = p.TaxCode,
                Tel = p.Tel
            }).ToList();
        }
        public async Task<List<BankPartnerDto>> GetListBankPartnerAsync(string partnerId)
        {
            var banks = await _bankPartnerService.GetByAccPartnerIdAsync(partnerId);
            var dtos = banks.Select(p => ObjectMapper.Map<BankPartner, BankPartnerDto>(p)).ToList();
            return dtos;
        }
        public async Task<List<BaseComboItemDto>> BankByPartnerCodeAsync(string partnerCode)
        {
            var partner = await _accPartnerService.GetAccPartnerByCodeAsync(partnerCode, _webHelper.GetCurrentOrgUnit());
            if (partner == null) return null;
            var banks = await _bankPartnerService.GetByAccPartnerIdAsync(partner.Id);
            var dtos = banks.Select(p =>
            {
                var combo = new BaseComboItemDto()
                {
                    Id = p.BankAccNumber,
                    Code = p.BankAccNumber,
                    Name = p.Name,
                    Value = p.BankAccNumber
                };
                return combo;
            }).ToList();
            return dtos;
        }
        public async Task<List<PartnerComboItemDto>> GetTypePartnerAsync()
        {
            var dtos = PartnerConst.TypePartner.Select(p => new PartnerComboItemDto()
            {
                Id = p.Key,
                Value = p.Value,
                Name = p.Value
            }).ToList();
            return await Task.FromResult(dtos);
        }

        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();
            var partnerGroups = await _partnerGroupService.GetListAsync(_webHelper.GetCurrentOrgUnit());
            var lstImport = await _excelService.ImportFileToList<CrudAccPartnerDto>(bytes, dto.WindowId);
            lstImport = lstImport.Where(p => p.Code != null).ToList();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dtos = new List<CrudAccPartnerDto>();
            int i = 0;
            foreach (var item in lstImport)
            {
                i += 1;
                bool isExistedPartner = await _accPartnerService.IsExistCode(orgCode, item.Code);
                if (isExistedPartner)
                {
                    continue;
                }
                var partnerGroup = partnerGroups.Where(p => p.Code == item.PartnerGroupId).FirstOrDefault();
                if (item.PartnerGroupId != null && item.PartnerGroupId != "" && partnerGroup != null)
                {
                    item.PartnerGroupId = partnerGroup.Id;
                }
                else if (item.PartnerGroupId != null && item.PartnerGroupId != "" && partnerGroup == null)
                {
                    throw new Exception("Mã nhóm đối tượng " + item.PartnerGroupId + " không tồn tại!");
                }
                else
                {
                    item.PartnerGroupId = null;
                }
                if (item.Code.Length < 3)
                {
                    throw new Exception("Mã  đối tượng " + item.Code + " nhỏ hơn 3 ký tự! tại dòng số " + i + " vui lòng kiểm tra lại");

                }
                if (item.Code.Length > 30)
                {
                    throw new Exception("Mã  đối tượng " + item.Code + " lớn hơn 3 ký tự! tại dòng số " + i + " vui lòng kiểm tra lại");
                }
                if (item.Name.Length > 250)
                {
                    throw new Exception("Tên  đối tượng " + item.Name + " lớn hơn 250 ký tự! tại dòng số " + i + " vui lòng kiểm tra lại");
                }
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
                dtos.Add(item);
            }

            var lstaccPartners = dtos.Select(p => ObjectMapper.Map<CrudAccPartnerDto, AccPartner>(p)).ToList();
            await _accPartnerService.CreateManyAsync(lstaccPartners);
            return new UploadFileResponseDto() { Ok = true };
        }

        [Authorize(AccountingPermissions.PartnerManagerUpdate)]
        public async Task UpdateAsync(string id, CrudAccPartnerDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _accPartnerService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _accPartnerService.CheckPartnerGroup(entity);
            try
            {
                var bankPartners = await _bankPartnerService.GetByAccPartnerIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (bankPartners != null)
                {
                    await _bankPartnerService.DeleteManyAsync(bankPartners);
                }
                await _accPartnerService.UpdateAsync(entity);
                if (isChangeCode)
                {
                    await _linkCodeBusiness.UpdateCode(LinkCodeConst.PartnerCode, dto.Code, oldCode, entity.OrgCode);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        public async Task<AccPartnerDto> GetByIdAsync(string partnerId)
        {
            var partner = await _accPartnerService.GetAsync(partnerId);
            return ObjectMapper.Map<AccPartner, AccPartnerDto>(partner);
        }
        public async Task<List<AccPartner>> GetListByPartnerGroupCode(string partnerGroupCode)
        {
            var lstPartnerGroup = await _partnerGroupAppService.GetChildGroup(partnerGroupCode);
            var iQAccPartner = await _accPartnerService.GetQueryableAsync();

            var lstAccPartner = iQAccPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                    && lstPartnerGroup.Select(p => p.Id).Contains(p.PartnerGroupId)).ToList();
            return lstAccPartner;
        }

        #region Private
        private async Task<IQueryable<AccPartner>> Filter(PageRequestDto dto)
        {
            var queryable = await _accPartnerService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnName.Equals("partnerType"))
                {
                    int partnerType = Convert.ToInt32(item.Value.ToString());
                    queryable = queryable.Where(p => p.PartnerType == partnerType);
                    continue;
                }
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }

        #endregion
    }
}
