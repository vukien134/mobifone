using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Products;
using Accounting.Catgories.FProductWorks;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
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

namespace Accounting.Categories.FProductWorks
{
    public class FProductWorkAppService : AccountingAppService, IFProductWorkAppService
    {
        #region Fields
        private readonly FProductWorkService _fProductWorkService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public FProductWorkAppService(FProductWorkService fProductWorkService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
            )
        {
            _fProductWorkService = fProductWorkService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion

        [Authorize(AccountingPermissions.FProductWorkManagerCreate)]
        public async Task<FProductWorkDto> CreateAsync(CrudFProductWorkDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.FProductOrWork = FProductWorkTypeConst.Work;
            var entity = ObjectMapper.Map<CrudFProductWorkDto, FProductWork>(dto);
            var result = await _fProductWorkService.CreateAsync(entity);
            return ObjectMapper.Map<FProductWork, FProductWorkDto>(result);
        }

        [Authorize(AccountingPermissions.FProductWorkManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _fProductWorkService.GetAsync(id);
            if (!entity.FProductOrWork.Equals(FProductWorkTypeConst.Work))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.Other),
                            _localizer["Err:NotProductWork", entity.Code]);
            }
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.FProductWorkCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _fProductWorkService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.FProductWorkManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _fProductWorkService.GetAsync(item);
                if (!entity.FProductOrWork.Equals(FProductWorkTypeConst.Work))
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.Other),
                                _localizer["Err:NotProductWork", entity.Code]);
                }
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.FProductWorkCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _fProductWorkService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.FProductWorkManagerView)]
        public Task<PageResultDto<FProductWorkDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.FProductWorkManagerView)]
        public async Task<PageResultDto<FProductWorkDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<FProductWorkDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<FProductWork, FProductWorkDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.FProductWorkManagerUpdate)]
        public async Task UpdateAsync(string id, CrudFProductWorkDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _fProductWorkService.GetAsync(id);
            if (!entity.FProductOrWork.Equals(FProductWorkTypeConst.Work))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.Other),
                            _localizer["Err:NotProductWork", entity.Code]);
            }

            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();

            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.FProductWorkCode, dto.Code, oldCode, entity.OrgCode);
            }
            await _fProductWorkService.UpdateAsync(entity);
        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var products = await _fProductWorkService.GetDataReference(orgCode, filterValue);
            return products.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudFProductWorkDto>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {
                var checkFfroductcode = await _fProductWorkService.GetByFProductWorkAsync(item.Code, _webHelper.GetCurrentOrgUnit());
                if (checkFfroductcode == null)
                {
                    item.Id = this.GetNewObjectId();
                    item.OrgCode = _webHelper.GetCurrentOrgUnit();
                    item.CreatorName = await _userService.GetCurrentUserNameAsync();
                    item.FProductOrWork = "C";
                }
                else
                {
                    throw new Exception("Mã  công trình sản phẩm này   " + item.Code + " đã tồn tại");
                }

            }

            var lstfProductWork = lstImport.Select(p => ObjectMapper.Map<CrudFProductWorkDto, FProductWork>(p))
                                .ToList();
            await _fProductWorkService.CreateManyAsync(lstfProductWork);
            return new UploadFileResponseDto() { Ok = true };
        }
        public async Task<FProductWorkDto> GetByIdAsync(string fProductWorkId)
        {
            var entity = await _fProductWorkService.GetAsync(fProductWorkId);
            return ObjectMapper.Map<FProductWork, FProductWorkDto>(entity);
        }
        #region Private
        private async Task<IQueryable<FProductWork>> Filter(PageRequestDto dto)
        {
            var queryable = await _fProductWorkService.GetQueryableAsync();
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
