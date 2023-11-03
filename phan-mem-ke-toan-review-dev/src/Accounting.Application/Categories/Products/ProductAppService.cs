using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Products;
using Accounting.Catgories.FProductWorks;
using Accounting.Catgories.Partners;
using Accounting.Catgories.Products;
using Accounting.Catgories.ProductVouchers;
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
using Accounting.Vouchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Uow;

namespace Accounting.Categories.Partners
{
    public class ProductAppService : AccountingAppService, IProductAppService
    {
        #region Field
        private readonly ProductService _productService;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductPriceService _productPriceService;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly UserService _userService;
        private readonly ExcelService _excelService;
        private readonly ProductGroupService _productGroupService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly FProductWorkService _fProductWorkService;
        private readonly ProductGroupAppService _productGroupAppService;
        private readonly IDistributedCache<ProductDto> _cache;
        private readonly IDistributedCache<PageResultDto<ProductDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly DiscountPriceService _discountPriceService;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public ProductAppService(ProductService productService,
                                ProductUnitService productUnitService,
                                ProductPriceService productPriceService,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                UserService userService,
                                ExcelService excelService,
                                ProductGroupService productGroupService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                FProductWorkService fProductWorkService,
                                ProductGroupAppService productGroupAppService,
                                IDistributedCache<ProductDto> cache,
                                IDistributedCache<PageResultDto<ProductDto>> pageCache,
                                CacheManager cacheManager,
                                DiscountPriceService discountPriceService,
                                LinkCodeBusiness linkCodeBusiness,
                                WarehouseBookService warehouseBookService,
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _productService = productService;
            _productUnitService = productUnitService;
            _productPriceService = productPriceService;
            _productOpeningBalanceService = productOpeningBalanceService;
            _userService = userService;
            _excelService = excelService;
            _productGroupService = productGroupService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _fProductWorkService = fProductWorkService;
            _productGroupAppService = productGroupAppService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _discountPriceService = discountPriceService;
            _linkCodeBusiness = linkCodeBusiness;
            _warehouseBookService = warehouseBookService;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.ProductManagerCreate)]
        public async Task<ProductDto> CreateAsync(CrudProductDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.UnitCode = GetBasicUnit(dto.ProductUnits);
            if (await _productGroupService.IsParentGroup(dto.ProductGroupId))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Product, ErrorCode.Other),
                        _localizer["Err:GroupIsParent"]);
            }
            if (dto.ProductType.Equals(ProductTypeConst.FinishedProduct))
            {
                dto.FProductWorkCode = dto.Code;
            }
            var entity = ObjectMapper.Map<CrudProductDto, Product>(dto);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _productService.CreateAsync(entity);
                await CreateFProductWork(result);
                await unitOfWork.CompleteAsync();
                await RemoveAllCache();
                return ObjectMapper.Map<Product, ProductDto>(result);
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        [Authorize(AccountingPermissions.ProductManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _productService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Product, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.FProductWorkCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _productService.DeleteAsync(id);
            await _fProductWorkService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.ProductManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _productService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Product, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
                isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.FProductWorkCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _productService.DeleteManyAsync(deleteIds);
            await _fProductWorkService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.ProductManagerView)]
        public async Task<PageResultDto<ProductDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<ProductDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }

        [Authorize(AccountingPermissions.ProductManagerView)]
        public async Task<PageResultDto<ProductDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ProductDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Product, ProductDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.ProductManagerUpdate)]
        public async Task UpdateAsync(string id, CrudProductDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.UnitCode = GetBasicUnit(dto.ProductUnits);
            if (await _productGroupService.IsParentGroup(dto.ProductGroupId))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Product, ErrorCode.Other),
                        _localizer["Err:GroupIsParent"]);
            }
            if (dto.ProductType.Equals(ProductTypeConst.FinishedProduct))
            {
                dto.FProductWorkCode = dto.Code;
            }
            var entity = await _productService.GetAsync(id);
            var oldLstProductUnit = await _productUnitService.GetByProductIdAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            var oldExc = oldLstProductUnit.Where(p => p.IsBasicUnit == true).FirstOrDefault()?.ExchangeRate ?? 1;
            var newExc = dto.ProductUnits.Where(p => p.IsBasicUnit == true).FirstOrDefault()?.ExchangeRate ?? 1;
            bool isChangeUnitCode = (dto.UnitCode == entity.UnitCode && oldExc == newExc) ? false : true;
            try
            {
                var productPrices = await _productPriceService.GetByProductIdAsync(id);

                using var unitOfWork = _unitOfWorkManager.Begin();

                await DeleteFProductWork(dto, entity);
                ObjectMapper.Map(dto, entity);
                await _productUnitService.DeleteManyAsync(oldLstProductUnit);
                await _productPriceService.DeleteManyAsync(productPrices);

                await _productService.UpdateAsync(entity, true);
                await UpdateFProductWork(entity);
                if (isChangeCode)
                {
                    await _linkCodeBusiness.UpdateCode(LinkCodeConst.ProductCode, dto.Code, oldCode, entity.OrgCode);
                    await _linkCodeBusiness.UpdateCode(LinkCodeConst.FProductWorkCode, dto.Code, oldCode, entity.OrgCode);
                }
                await unitOfWork.CompleteAsync();
                await RemoveAllCache();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
            if (isChangeUnitCode)
            {
                await ChangeUnitCode(dto.Code, oldLstProductUnit);
            }

        }
        public async Task<ProductDto> GetByIdAsync(string productId)
        {
            return await _cache.GetOrAddAsync(
                productId, //Cache key
                async () =>
                {
                    var product = await _productService.GetAsync(productId);
                    return ObjectMapper.Map<Product, ProductDto>(product);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );
        }
        public async Task<List<ProductComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<ProductDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var products = await _productService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
                    var productUnits = (await _productUnitService.GetQueryableAsync())
                                       .Where(p => p.IsBasicUnit == true && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                    var dataProductCombo = (from a in products
                                            join b in productUnits on a.Id equals b.ProductId into ajb
                                            from b in ajb.DefaultIfEmpty()
                                            select new ProductComboItemDto
                                            {
                                                Id = a.Code,
                                                Value = a.Code,
                                                Code = a.Code,
                                                Name = a.Name,
                                                UnitCode = a.UnitCode,
                                                ProductAcc = a.ProductAcc,
                                                ProductCostAcc = a.ProductCostAcc,
                                                DiscountAcc = a.DiscountAcc,
                                                RevenueAcc = a.RevenueAcc,
                                                SaleReturnsAcc = a.SaleReturnsAcc,
                                                AttachProductLot = a.AttachProductLot,
                                                AttachProductOrigin = a.AttachProductOrigin,
                                                ExciseTaxPercentage = a.ExciseTaxPercentage,
                                                VatPercentage = a.VatPercentage,
                                                PITPercentage = a.PITPercentage,
                                                SalePrice = b?.SalePrice ?? 0,
                                                SalePriceCur = b?.SalePriceCur ?? 0,
                                                PurchasePrice = b?.PurchasePrice ?? 0,
                                                PurchasePriceCur = b?.PurchasePriceCur ?? 0,
                                            }).ToList();
                    return dataProductCombo;
                }
            );
        }
        public async Task<List<ProductUnitDto>> GetProductUnitAsync(string productId)
        {
            var productUnits = await _productUnitService.GetByProductIdAsync(productId);
            var dtos = productUnits.Select(p => ObjectMapper.Map<ProductUnit, ProductUnitDto>(p)).ToList();
            return dtos;
        }
        public async Task<List<BaseComboItemDto>> GetProductUnitByProductCodeAsync(string productCode)
        {
            var product = await _productService.GetByCodeAsync(productCode, _webHelper.GetCurrentOrgUnit());
            if (product == null) return null;
            var productUnits = await _productUnitService.GetByProductIdAsync(product.Id);
            var dtos = productUnits.Select(p =>
            {
                var combo = new BaseComboItemDto()
                {
                    Id = p.UnitCode,
                    Code = p.UnitCode,
                    Name = p.UnitCode,
                    Value = p.UnitCode
                };
                return combo;
            }).ToList();
            return dtos;
        }
        public async Task<List<ProductPriceDto>> GetProductPriceImportByProductCodeAsync(string productCode)
        {

            var product = await _productService.GetByCodeAsync(productCode, _webHelper.GetCurrentOrgUnit());
            if (product == null) return null;
            var productPrice = await _productPriceService.GetByProductIdAsync(product.Id);
            var dtos = productPrice.Select(p =>
            {
                var combo = new ProductPriceDto()
                {
                    PurchasePrice = p.PurchasePrice,
                    ProductCode = p.ProductCode,
                    Id = p.Id
                };
                return combo;
            }).ToList();
            return dtos;
        }
        public async Task<List<ProductPriceDto>> GetProductPriceExportByProductCodeAsync(string productCode)
        {

            var product = await _productService.GetByCodeAsync(productCode, _webHelper.GetCurrentOrgUnit());
            if (product == null) return null;
            var productPrice = await _productPriceService.GetByProductIdAsync(product.Id);
            var dtos = productPrice.Select(p =>
            {
                var combo = new ProductPriceDto()
                {
                    SalePrice = p.SalePrice,
                    ProductCode = p.ProductCode,
                    Id = p.Id
                };
                return combo;
            }).ToList();
            return dtos;
        }
        public async Task<List<ProductPriceDto>> GetProductPriceAsync(string productId)
        {
            var productPrices = await _productPriceService.GetByProductIdAsync(productId);
            var dtos = productPrices.Select(p => ObjectMapper.Map<ProductPrice, ProductPriceDto>(p)).ToList();
            return dtos;
        }
        public async Task<List<Product>> GetListByProductGroupCode(string productGroupCode)
        {
            var lstProductGroup = await _productGroupAppService.GetChildGroup(productGroupCode);
            var iQAccProduct = await _productService.GetQueryableAsync();
            var lstProduct = iQAccProduct.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                    && lstProductGroup.Select(p => p.Id).Contains(p.ProductGroupId)).ToList();
            return lstProduct;
        }
        [Authorize(AccountingPermissions.ProductManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();
            var lstImport = await _excelService.ImportFileToList<ExcelProductDto>(bytes, dto.WindowId);
            lstImport = lstImport.Where(p => p.Code != null).ToList();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var products = from p in lstImport
                           group new { p } by new
                           {
                               p.Code,
                               p.Name
                           } into gr
                           select new CrudProductDto
                           {
                               Id = this.GetNewObjectId(),
                               OrgCode = orgCode,
                               Code = gr.Key.Code,
                               Name = gr.Key.Name,
                               Specification = gr.Max(a => a.p.Specification),
                               Barcode = gr.Max(a => a.p.Barcode),
                               UnitCode = gr.Max(a => a.p.UnitCode),
                               AttachProductLot = gr.Max(a => a.p.AttachProductLot),
                               AttachProductOrigin = gr.Max(a => a.p.AttachProductOrigin),
                               AttachWorkPlace = gr.Max(a => a.p.AttachWorkPlace),
                               ProductType = gr.Max(a => a.p.ProductType),
                               ExciseTaxCode = gr.Max(a => a.p.ExciseTaxCode),
                               ProductAcc = gr.Max(a => a.p.ProductAcc),
                               ProductCostAcc = gr.Max(a => a.p.ProductCostAcc),
                               RevenueAcc = gr.Max(a => a.p.RevenueAcc),
                               DiscountAcc = gr.Max(a => a.p.DiscountAcc),
                               SaleReturnsAcc = gr.Max(a => a.p.SaleReturnsAcc),
                               FProductWorkCode = gr.Max(a => a.p.FProductWorkCode),
                               ProductionPeriodCode = gr.Max(a => a.p.ProductionPeriodCode),
                               ProductGroupCode = gr.Max(a => a.p.ProductGroupCode),
                               MinQuantity = gr.Max(a => a.p.MinQuantity),
                               MaxQuantity = gr.Max(a => a.p.MaxQuantity),
                               VatPercentage = gr.Max(a => a.p.VatPercentage),                               
                               DiscountPercentage = gr.Max(a => a.p.DiscountPercentage),
                               ExciseTaxPercentage = gr.Max(a => a.p.ExciseTaxPercentage),
                               ImportTaxPercentage = gr.Max(a => a.p.ImportTaxPercentage),
                               Note = gr.Max(a => a.p.Note),
                               TaxCategoryCode = gr.Max(a => a.p.TaxCategoryCode),
                               CareerCode = gr.Max(a => a.p.CareerCode),
                               PITPercentage = gr.Max(a => a.p.PitPercentage),
                               ProductGroupId = gr.Max(a => a.p.ProductGroupId)
                           };
            var lstProducts = products.ToList();
            int i = 0;
            foreach (var product in lstProducts)
            {
                i += 1;
                var ProductUnits = from p in lstImport
                                   where p.Code == product.Code
                                   select new CrudProductUnitDto
                                   {
                                       Id = this.GetNewObjectId(),
                                       UnitCode = p.UnitCode,
                                       IsBasicUnit = (product.UnitCode == p.UnitCode) ? true : false,
                                       ExchangeRate = 1,
                                       ProductCode = product.Code,
                                       ProductId = product.Id,
                                       OrgCode = product.OrgCode,
                                       SalePrice = p.SalePrice,
                                       PurchasePrice = p.PurchasePrice
                                   };
                if (string.IsNullOrEmpty(product.DiscountAcc) == true)
                {
                    throw new AccountingException("Tài khoản chiết khấu tại dòng " + i + " đang bỏ trống vui lòng kiểm tra và thử lại!");
                }
                if (string.IsNullOrEmpty(product.ProductAcc) == true)
                {
                    throw new AccountingException("Tài khoản hàng hoá tại dòng  " + i + "đang bỏ trống vui lòng kiểm tra và thử lại!");
                }
                if (string.IsNullOrEmpty(product.ProductCostAcc) == true)
                {
                    throw new AccountingException("Tài khoản hàng hàng bán bị trả lại tại dòng  " + i + "đang bỏ trống vui lòng kiểm tra và thử lại!");
                }
                if (string.IsNullOrEmpty(product.RevenueAcc) == true)
                {
                    throw new AccountingException("Tài khoản doanh thu tại dòng  " + i + "đang bỏ trống vui lòng kiểm tra và thử lại!");
                }
                if (string.IsNullOrEmpty(product.SaleReturnsAcc) == true)
                {
                    throw new AccountingException("Tài khoản giá vốn tại dòng  " + i + "đang bỏ trống vui lòng kiểm tra và thử lại!");
                }
                product.ProductUnits = ProductUnits.ToList();
                var productGroup = _productGroupService.GetByCodeAsync(product.ProductGroupCode, orgCode);
                product.ProductGroupId = productGroup.Result.Id;
                await this.CreateAsync(product);
            }
            await RemoveAllCache();
            return new UploadFileResponseDto() { Ok = true };
        }
        public async Task<DiscountPriceDetailDto> SalePriceAsync(GetSalePriceDto dto)
        {
            if (string.IsNullOrEmpty(dto.ProductCode))
            {
                throw new ArgumentNullException(nameof(dto.ProductCode));
            }
            if (dto.VoucherDate == null)
            {
                throw new ArgumentNullException(nameof(dto.VoucherDate));
            }
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _discountPriceService.GetSalePriceAsync(orgCode, dto.VoucherDate.Value,
                                        dto.ProductCode, dto.PartnerCode);
            return ObjectMapper.Map<DiscountPriceDetail, DiscountPriceDetailDto>(entity);
        }
        #region Private
        private async Task<IQueryable<Product>> Filter(PageRequestDto dto)
        {
            var queryable = await _productService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _productService.GetQueryableQuickSearch(queryable, filterValue);
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private string GetBasicUnit(ICollection<CrudProductUnitDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Where(p => p.IsBasicUnit == true)
                        .Select(p => p.UnitCode).FirstOrDefault<string>();
        }
        private async Task ChangeUnitCode(string productCode, List<ProductUnit> oldLstProductUnit)
        {
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            var dataWarehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductCode == productCode).ToList();
            var dataWarehouseBookDto = dataWarehouseBook.Select(p => ObjectMapper.Map<WarehouseBook, WarehouseBookDto>(p)).ToList();

            var productOpeningBalance = await _productOpeningBalanceService.GetQueryableAsync();
            var dataProductOpeningBalance = productOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductCode == productCode).ToList();
            var lstProductUnit = (await _productUnitService.GetQueryableAsync())
                                 .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductCode == productCode).ToList();
            var productUnitBasic = lstProductUnit.Where(p => p.IsBasicUnit == true).FirstOrDefault();
            foreach (var item in dataWarehouseBookDto)
            {
                var productUnit = oldLstProductUnit.Where(p => p.UnitCode == item.UnitCode).FirstOrDefault();

                item.Quantity = Math.Round((item.TrxQuantity * productUnitBasic.ExchangeRate / (productUnit?.ExchangeRate ?? 1)) ?? 0, 4);
                item.ImportQuantity = (item.VoucherGroup == 1 ? item.Quantity : 0);
                item.ExportQuantity = (item.VoucherGroup == 2 ? item.Quantity : 0);
                item.Id = GetNewObjectId();
                item.UnitCode = productUnitBasic.UnitCode;
            }
            var dataWarehouseBookNew = dataWarehouseBookDto.Select(p => ObjectMapper.Map<WarehouseBookDto, WarehouseBook>(p)).ToList();
            await _warehouseBookService.CreateManyAsync(dataWarehouseBookNew, true);
            await _warehouseBookService.DeleteManyAsync(dataWarehouseBook, true);
        }
        private async Task CreateFProductWork(Product product)
        {
            if (!product.ProductType.Equals(ProductTypeConst.FinishedProduct)) return;

            var productWorkDto = new CrudFProductWorkDto()
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                OrgCode = product.OrgCode,
                FProductOrWork = FProductWorkTypeConst.FinishedProduct
            };
            var entity = ObjectMapper.Map<CrudFProductWorkDto, FProductWork>(productWorkDto);
            await _fProductWorkService.CreateAsync(entity);
        }
        private async Task UpdateFProductWork(Product product)
        {
            if (!product.ProductType.Equals(ProductTypeConst.FinishedProduct)) return;
            var entity = await _fProductWorkService.GetRepository().FindAsync(product.Id);
            if (entity == null)
            {
                await CreateFProductWork(product);
                return;
            }

            entity.Code = product.Code;
            entity.Name = product.Name;
            await _fProductWorkService.UpdateAsync(entity);
        }
        private async Task DeleteFProductWork(CrudProductDto dto, Product product)
        {
            if (!dto.ProductType.Equals(ProductTypeConst.FinishedProduct)
                && product.ProductType.Equals(ProductTypeConst.FinishedProduct))
            {
                await _fProductWorkService.DeleteAsync(product.Id);
            }
        }
        private async Task DeleteFProductWork(string id)
        {
            var entity = await _fProductWorkService.GetRepository().FindAsync(id);
            if (entity == null) return;
            await _fProductWorkService.DeleteAsync(id);
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<ProductDto>();
        }
        #endregion
    }
}