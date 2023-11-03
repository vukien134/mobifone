using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Catgories.Contracts;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
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
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.Contracts
{
    public class ContractAppService : AccountingAppService, IContractAppService
    {
        #region Field
        private readonly ContractService _contractService;
        private readonly ContractDetailService _contractDetailService;
        private readonly UserService _userService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly ProductService _productService;
        #endregion
        #region Ctor
        public ContractAppService(ContractService contractService,
                                ContractDetailService contractDetailService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ExcelService excelService,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer,
                                ProductService productService
            )
        {
            _contractService = contractService;
            _contractDetailService = contractDetailService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _productService = productService;
        }
        #endregion

        [Authorize(AccountingPermissions.ContractManagerCreate)]
        public async Task<ContractDto> CreateAsync(CrudContractDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudContractDto, Contract>(dto);
            var result = await _contractService.CreateAsync(entity);
            return ObjectMapper.Map<Contract, ContractDto>(result);
        }

        [Authorize(AccountingPermissions.ContractManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _contractService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ContractCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Contract, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _contractService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.ContractManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                var entity = await _contractService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ContractCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Contract, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _contractService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.ContractManagerView)]
        public Task<PageResultDto<ContractDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.ContractManagerView)]
        public async Task<PageResultDto<ContractDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ContractDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Contract, ContractDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<ContractDetailDto>> GetListContractDetailAsync(string contractId)
        {
            var contractDetails = await _contractDetailService.GetByContractIdAsync(contractId);
            var dtos = contractDetails.Select(p => ObjectMapper.Map<ContractDetail, ContractDetailDto>(p)).ToList();
            return dtos;
        }

        [Authorize(AccountingPermissions.ContractManagerUpdate)]
        public async Task UpdateAsync(string id, CrudContractDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _contractService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            try
            {
                var contractDetails = await _contractDetailService.GetByContractIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (contractDetails != null)
                {
                    await _contractDetailService.DeleteManyAsync(contractDetails);
                }
                await _contractService.UpdateAsync(entity);
                if (isChangeCode)
                {
                    await _linkCodeBusiness.UpdateCode(LinkCodeConst.ContractCode, dto.Code, oldCode, entity.OrgCode);
                }
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
            string filterValue = $"%{dto.FilterValue}%";
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var contracts = await _contractService.GetDataReference(orgCode, filterValue);
            return contracts.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        public async Task<ContractDto> GetByIdAsync(string partnerId)
        {
            var partner = await _contractService.GetAsync(partnerId);
            return ObjectMapper.Map<Contract, ContractDto>(partner);
        }
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstContracts = await _excelService.ImportFileToList<CrudContractDto>(bytes, dto.WindowId);
            var lstContract = (from a in lstContracts
                               where string.IsNullOrEmpty(a.Code) == false
                               group new { a } by new
                               {
                                   a.Code,
                                   a.Name,
                                   a.BeginDate,
                                   a.EndDate,
                                   a.Note,
                                   a.ContractType,
                                   a.SignedDate,
                                   a.InvoiceDate,
                                   a.PartnerCode
                               } into gr
                               select new CrudContractDto
                               {
                                   Code = gr.Key.Code,
                                   Name = gr.Key.Name,
                                   BeginDate = gr.Key.BeginDate,
                                   EndDate = gr.Key.EndDate,
                                   Note = gr.Key.Note,
                                   ContractType = gr.Key.ContractType,
                                   SignedDate = gr.Key.SignedDate,
                                   InvoiceDate = gr.Key.InvoiceDate,
                                   Id = this.GetNewObjectId(),
                                   OrgCode = _webHelper.GetCurrentOrgUnit(),
                                   PartnerCode = gr.Key.PartnerCode
                               }).ToList();
            var lstProduct = await _productService.GetQueryableAsync();
            var product = lstProduct.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstContrac = await _contractService.GetListAsync(_webHelper.GetCurrentOrgUnit());
            foreach (var items in lstContract)
            {

                items.CreatorName = await _userService.GetCurrentUserNameAsync();
                var discountPriceDetal = from a in lstContracts
                                         join b in product on a.ProductCode equals b.Code into c
                                         from pro in c.DefaultIfEmpty()
                                         where a.Code == items.Code// && string.IsNullOrEmpty(a.ProductCode) == false
                                         select new CrudContractDetailDto
                                         {
                                             Id = this.GetNewObjectId(),
                                             ContractId = items.Id,
                                             ProductCode = a.ProductCode,
                                             ProductName = pro != null ? pro.Name : null,
                                             Price = a.Price,
                                             Quantity = a.Quantity,
                                             Amount = a.Amount,
                                             TrxQuantity = a.TrxQuantity,
                                             TrxPrice = a.TrxPrice,
                                             TrxAmount = a.TrxAmount,
                                             TrxPriceCur = a.TrxPriceCur,
                                             PriceCur = a.PriceCur,
                                             AmountCur = a.AmountCur,
                                             TrxAmountCur = a.TrxAmountCur,
                                             Note = a.NoteDetal,
                                         };


                items.ContractDetails = discountPriceDetal.ToList();

            }

            var lstdiscountPrice = lstContract.Select(p => ObjectMapper.Map<CrudContractDto, Contract>(p))
                                .ToList();
            await _contractService.CreateManyAsync(lstdiscountPrice);
            return new UploadFileResponseDto() { Ok = true };
        }

        #region Private
        private async Task<IQueryable<Contract>> Filter(PageRequestDto dto)
        {
            var queryable = await _contractService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }


        #endregion
    }
}
