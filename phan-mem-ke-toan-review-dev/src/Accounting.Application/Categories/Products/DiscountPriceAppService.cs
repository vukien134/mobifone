using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Products;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Helpers;
using Accounting.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.Products
{
    public class DiscountPriceAppService : AccountingAppService, IDiscountPriceAppService
    {
        #region Field
        private readonly DiscountPriceService _discountPriceService;
        private readonly DiscountPriceDetailService _discountPriceDetailService;
        private readonly DiscountPricePartnerService _discountPricePartnerService;
        private readonly UserService _userService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly CacheManager _cacheManager;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly ProductUnitService _productUnitService;
        #endregion
        #region Ctor
        public DiscountPriceAppService(DiscountPriceService discountPriceService,
                                DiscountPriceDetailService discountPriceDetailService,
                                DiscountPricePartnerService discountPricePartnerService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ExcelService excelService,
                                CacheManager cacheManager,
                                IStringLocalizer<AccountingResource> localizer,
                                ProductUnitService productUnitService)
        {
            _discountPriceService = discountPriceService;
            _discountPriceDetailService = discountPriceDetailService;
            _discountPricePartnerService = discountPricePartnerService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cacheManager = cacheManager;
            _localizer = localizer;
            _productUnitService = productUnitService;
        }
        #endregion
        public async Task<DiscountPriceDto> CreateAsync(CrudDiscountPriceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudDiscountPriceDto, DiscountPrice>(dto);
            var result = await _discountPriceService.CreateAsync(entity);
            return ObjectMapper.Map<DiscountPrice, DiscountPriceDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _discountPriceService.DeleteAsync(id);
        }
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string[] idsDelete = dto.ListId.ToArray();
            await _discountPriceService.DeleteManyAsync(idsDelete);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }
        public Task<PageResultDto<DiscountPriceDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        public async Task<PageResultDto<DiscountPriceDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<DiscountPriceDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<DiscountPrice, DiscountPriceDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<DiscountPricePartnerDto>> GetListDiscountPricePartnerAsync(string discountPriceId)
        {
            var details = await _discountPricePartnerService.GetByDiscountPriceIdAsync(discountPriceId);
            var dtos = details.Select(p => ObjectMapper.Map<DiscountPricePartner, DiscountPricePartnerDto>(p)).ToList();
            return dtos;
        }
        public async Task<List<DiscountPriceDetailDto>> GetListDiscountPriceDetailAsync(string discountPriceId)
        {
            var details = await _discountPriceDetailService.GetByDiscountPriceIdAsync(discountPriceId);
            var dtos = details.Select(p => ObjectMapper.Map<DiscountPriceDetail, DiscountPriceDetailDto>(p)).ToList();
            return dtos;
        }
        public async Task UpdateAsync(string id, CrudDiscountPriceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _discountPriceService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                var partners = await _discountPricePartnerService.GetByDiscountPriceIdAsync(id);
                var details = await _discountPriceDetailService.GetByDiscountPriceIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (partners != null)
                {
                    await _discountPricePartnerService.DeleteManyAsync(partners);
                }
                if (details != null)
                {
                    await _discountPriceDetailService.DeleteManyAsync(details);
                }
                await _discountPriceService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        #region Private
        private async Task<IQueryable<DiscountPrice>> Filter(PageRequestDto dto)
        {
            var queryable = await _discountPriceService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }

        public async Task<DiscountPriceDto> GetByIdAsync(string discountPriceId)
        {
            var discountPrices = await _discountPriceService.GetAsync(discountPriceId);
            return ObjectMapper.Map<DiscountPrice, DiscountPriceDto>(discountPrices);
        }

        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstCrudDiscountPrice = await _excelService.ImportFileToList<CrudDiscountPriceDto>(bytes, dto.WindowId);
            var listDiscountPrice = (from a in lstCrudDiscountPrice
                                     where string.IsNullOrEmpty(a.Code) == false
                                     group new { a } by new
                                     {
                                         a.Code,
                                         a.Name,
                                         a.BeginDate,
                                         a.EndDate,
                                         a.Note
                                     } into gr
                                     select new CrudDiscountPriceDto
                                     {
                                         Code = gr.Key.Code,
                                         Name = gr.Key.Name,
                                         BeginDate = gr.Key.BeginDate,
                                         EndDate = gr.Key.EndDate,
                                         Note = gr.Key.Note
                                     }).ToList();
            var discountPrice = await _discountPriceService.GetQueryableAsync();
            var product = await _productUnitService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.IsBasicUnit == true).ToList();
            foreach (var item in listDiscountPrice)
            {
                var lstDiscout = discountPrice.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.Code).ToList();
                if (lstDiscout.Count() != 0)
                {
                    throw new Exception("Mã " + item.Code + " đã tồn tại vui lòng thử lại!");

                }
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
                var discountPriceDetal = (from a in lstCrudDiscountPrice
                                          join b in lstProduct on a.ProductCode equals b.ProductCode into d
                                          from c in d.DefaultIfEmpty()
                                          where a.Code == item.Code
                                          select new CrudDiscountPriceDetailDto
                                          {
                                              Id = this.GetNewObjectId(),
                                              DiscountPriceId = item.Id,
                                              ProductCode = a.ProductCode,
                                              UnitCode = string.IsNullOrEmpty(a.UnitCode) == true ? (c != null ? c.UnitCode : a.UnitCode) : a.UnitCode,
                                              Price2 = string.IsNullOrEmpty((a.Price ?? "").ToString()) == true ? (c != null ? c.SalePrice : 0) : decimal.Parse(a.Price.ToString()),
                                              DiscountPercentage = (decimal?)decimal.Parse(a.DiscountPercentage.ToString()),
                                              Note = a.NoteDetail
                                          }).ToList();
                var discountPricePartner = (from a in lstCrudDiscountPrice
                                            where a.Code == item.Code
                                            select new CrudDiscountPricePartnerDto
                                            {
                                                Id = this.GetNewObjectId(),
                                                DiscountPriceId = item.Id,
                                                PartnerCode = a.PartnerCode,
                                                Note = a.NotePartner
                                            }).ToList();

                item.DiscountPriceDetails = discountPriceDetal;
                item.DiscountPricePartners = discountPricePartner;
            }

            var lstdiscountPrice = listDiscountPrice.Select(p => ObjectMapper.Map<CrudDiscountPriceDto, DiscountPrice>(p))
                                .ToList();
            await _discountPriceService.CreateManyAsync(lstdiscountPrice);
            await RemoveAllCache();
            return new UploadFileResponseDto() { Ok = true };
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<DiscountPriceDto>();
        }
        #endregion
    }
}
