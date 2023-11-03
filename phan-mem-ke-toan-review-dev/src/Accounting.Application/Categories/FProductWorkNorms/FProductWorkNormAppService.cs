using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Products;
using Accounting.Catgories.AccCases;
using Accounting.Catgories.AdjustDepreciations;
using Accounting.Catgories.AssetTools;
using Accounting.Catgories.Customines;
using Accounting.Catgories.FProductWorkNorms;
using Accounting.Catgories.FProductWorks;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accounting.Categories.FProductWorkNorms
{
    public class FProductWorkNormAppService : AccountingAppService, IFProductWorkNormAppService
    {
        #region Field
        private readonly FProductWorkNormService _fProductWorkNormService;
        private readonly AssetToolGroupService _assetToolGroupService;
        private readonly FProductWorkNormDetailService _fProductWorkNormDetailService;
        private readonly AssetToolStoppingDepreciationService _assetToolStoppingDepreciationService;
        private readonly AssetToolAccessoryService _assetToolAccessoryService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly ExcelService _excelService;
        private readonly ProductService _productService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        private readonly CacheManager _cacheManager;

        #endregion
        #region Ctor
        public FProductWorkNormAppService(FProductWorkNormService fProductWorkNormService,
                                AssetToolGroupService assetToolGroupService,
                                FProductWorkNormDetailService fProductWorkNormDetailService,
                                AssetToolStoppingDepreciationService assetToolStoppingDepreciationService,
                                AssetToolAccessoryService assetToolAccessoryService,
                                AssetToolDetailService assetToolDetailService,
                                ExcelService excelService,
                                ProductService productService,
                                FProductWorkService fProductWorkService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                CacheManager cacheManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper)
        {
            _fProductWorkNormService = fProductWorkNormService;
            _assetToolGroupService = assetToolGroupService;
            _fProductWorkNormDetailService = fProductWorkNormDetailService;
            _assetToolStoppingDepreciationService = assetToolStoppingDepreciationService;
            _assetToolAccessoryService = assetToolAccessoryService;
            _assetToolDetailService = assetToolDetailService;
            _excelService = excelService;
            _productService = productService;
            _fProductWorkService = fProductWorkService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _cacheManager = cacheManager;
        }
        #endregion
        public async Task<FProductWorkNormDto> CreateAsync(CrudFProductWorkNormDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto = this.StandardDto(dto);
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            foreach (var item in dto.FProductWorkNormDetails)
            {
                item.ProductOrigin = item.ProductOriginCode == "" ? null : item.ProductOriginCode;
                item.ProductLotCode = item.ProductLotCode == "" ? null : item.ProductLotCode;
            }
            var entity = ObjectMapper.Map<CrudFProductWorkNormDto, FProductWorkNorm>(dto);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _fProductWorkNormService.CreateAsync(entity);
                await RemoveAllCache();
                await unitOfWork.CompleteAsync();

                var lstproductService = await _productService.GetQueryableAsync();
                var products = lstproductService.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.FProductWorkCode == entity.FProductWorkCode).FirstOrDefault();
                if (products != null)
                {
                    dto.FProductWorkName = products.Name;
                }

                return ObjectMapper.Map<CrudFProductWorkNormDto, FProductWorkNormDto>(dto);



            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _fProductWorkNormService.DeleteAsync(id);
            try
            {
                var fProductWorkNormDetails = await _fProductWorkNormDetailService.GetByFProductWorkNormIdAsync(id);

                using var unitOfWork = _unitOfWorkManager.Begin();
                if (fProductWorkNormDetails != null)
                {
                    await _fProductWorkNormDetailService.DeleteManyAsync(fProductWorkNormDetails);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public Task<PageResultDto<FProductWorkNormCustomineDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<FProductWorkNormCustomineDto>> GetListAsync(PageRequestDto dto)
        {
            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            fProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var result = new PageResultDto<FProductWorkNormCustomineDto>();
            var query = await Filter(dto);
            var queryData = from a in query
                            join b in fProductWork on a.FProductWorkCode equals b.Code into ajb
                            from b in ajb.DefaultIfEmpty()
                            select new FProductWorkNormCustomineDto
                            {
                                Year = a.Year,
                                FProductWorkCode = a.FProductWorkCode,
                                Quantity = a.Quantity,
                                Note = a.Note,
                                FProductWorkName = (b != null) ? b.Name : "",
                                Id = a.Id
                            };
            var querysort = queryData.OrderBy(p => p.FProductWorkCode).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudFProductWorkNormDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto = this.StandardDto(dto);
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Id = id;
            var entity = await _fProductWorkNormService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                var fProductWorkNormDetails = await _fProductWorkNormDetailService.GetByFProductWorkNormIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (fProductWorkNormDetails != null)
                {
                    await _fProductWorkNormDetailService.DeleteManyAsync(fProductWorkNormDetails);
                }

                await _fProductWorkNormService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }
        public async Task<FProductWorkNormCustomineDto> GetByIdAsync(string fProductWorkNormId)
        {
            var fProductWorkNorm = await _fProductWorkNormService.GetAsync(fProductWorkNormId);
            var fProductWork = await _fProductWorkService.GetByFProductWorkAsync(fProductWorkNorm.FProductWorkCode, _webHelper.GetCurrentOrgUnit());
            var res = new FProductWorkNormCustomineDto
            {
                Year = fProductWorkNorm.Year,
                FProductWorkCode = fProductWorkNorm.FProductWorkCode,
                Quantity = fProductWorkNorm.Quantity,
                Note = fProductWorkNorm.Note,
                FProductWorkName = fProductWork != null ? fProductWork.Name : ""
            };
            return res;
        }

        public async Task<FProductWorkNormDto> GetByCodeAsync(string fProductWorkNormCode)
        {
            var fProductWorkNorm = await _fProductWorkNormService.GetQueryableAsync();
            fProductWorkNorm = fProductWorkNorm.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                        && p.Year == _webHelper.GetCurrentYear()
                                                        && p.FProductWorkCode == fProductWorkNormCode);
            var data = fProductWorkNorm.FirstOrDefault();
            return ObjectMapper.Map<FProductWorkNorm, FProductWorkNormDto>(data);
        }

        public async Task<List<FProductWorkNormDetailCustomineDto>> GetFProductWorkNormDetailAsync(string fProductWorkNormId)
        {
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var fProductWorkNormDetails = await _fProductWorkNormDetailService.GetByFProductWorkNormIdAsync(fProductWorkNormId);
            var iQFProductWorkNormDetailCustomine = from a in fProductWorkNormDetails
                                                    join b in lstProduct on a.ProductCode equals b.Code into ajb
                                                    from b in ajb.DefaultIfEmpty()
                                                    select new FProductWorkNormDetailCustomineDto
                                                    {
                                                        Id = a.Id,
                                                        OrgCode = a.OrgCode,
                                                        FProductWorkNormId = a.FProductWorkNormId,
                                                        Year = a.Year,
                                                        Month = a.Month,
                                                        BeginDate = a.BeginDate,
                                                        EndDate = a.EndDate,
                                                        Ord0 = a.Ord0,
                                                        AccCode = a.AccCode,
                                                        WorkPlaceCode = a.WorkPlaceCode,
                                                        SectionCode = a.SectionCode,
                                                        WarehouseCode = a.WarehouseCode,
                                                        ProductCode = a.ProductCode,
                                                        ProductLotCode = a.ProductLotCode,
                                                        ProductOrigin = a.ProductOrigin,
                                                        ProductOriginCode = a.ProductOrigin,
                                                        UnitCode = a.UnitCode,
                                                        Quantity = a.Quantity,
                                                        QuantityLoss = a.QuantityLoss,
                                                        PercentLoss = a.PercentLoss,
                                                        PriceCur = a.PriceCur,
                                                        Price = a.Price,
                                                        AmountCur = a.AmountCur,
                                                        Amount = a.Amount,
                                                        ApplicableDate1 = a.ApplicableDate1,
                                                        ApplicableDate2 = a.ApplicableDate2,
                                                        ProductName = (b != null) ? b.Name : ""
                                                    };
            var dtos = iQFProductWorkNormDetailCustomine.ToList();
            return dtos;
        }

        #region Private
        private async Task<IQueryable<FProductWorkNormCustomineDto>> Filter(PageRequestDto dto)
        {
            var queryable = await _fProductWorkNormService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                    && p.Year == _webHelper.GetCurrentYear());
            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            fProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var queryData = from a in queryable
                            join b in fProductWork on a.FProductWorkCode equals b.Code into ajb
                            from b in ajb.DefaultIfEmpty()
                            select new FProductWorkNormCustomineDto
                            {
                                Year = a.Year,
                                FProductWorkCode = a.FProductWorkCode,
                                Quantity = a.Quantity,
                                Note = a.Note,
                                FProductWorkName = (b != null) ? b.Name : "",
                                Id = a.Id
                            };
            if (dto.FilterRows == null) return queryData;


            foreach (var item in dto.FilterRows)
            {
                try
                {
                    if (item.ColumnName == "fProductWorkCode")
                    {
                        var checkCode = item.Value.ToString().ToUpper();
                        queryData = queryData.Where(item.ColumnName, checkCode, FilterOperator.Contains);
                    }
                    if (item.ColumnName == "fProductWorkName")
                    {
                        var checkName = item.Value.ToString().ToLower();
                        queryData = queryData.Where(x => x.FProductWorkName.ToLower().Contains(checkName));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return queryData;
        }
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();
            var lstProduct = await _productService.GetByProductAsync(_webHelper.GetCurrentOrgUnit());
            var lstFFProductWorkNorms = await _excelService.ImportFileToList<CrudFProductWorkNormDto>(bytes, dto.WindowId);
            lstFFProductWorkNorms = lstFFProductWorkNorms.Where(p => !p.FProductWorkCode.IsNullOrEmpty()).ToList();
            var lstFFProductWorkNorm = (from a in lstFFProductWorkNorms
                                        where string.IsNullOrEmpty(a.FProductWorkCode) == false
                                        group new { a } by new
                                        {
                                            a.FProductWorkCode,
                                            a.Quantity,
                                            a.BeginDate,
                                            a.EndDate,
                                            a.Note,

                                        } into gr
                                        select new CrudFProductWorkNormDto
                                        {
                                            FProductWorkCode = gr.Key.FProductWorkCode,
                                            Quantity = gr.Key.Quantity,
                                            Note = gr.Key.Note,


                                        }).ToList();

            foreach (var item in lstFFProductWorkNorm)
            {
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
                item.Year = _webHelper.GetCurrentYear();
                var lstFFProductWorkNormDetal = (from a in lstFFProductWorkNorms
                                                 join b in lstProduct on a.ProductCode equals b.Code into ajb
                                                 from b in ajb.DefaultIfEmpty()
                                                 where a.FProductWorkCode == item.FProductWorkCode// && string.IsNullOrEmpty(a.ProductCode) == false
                                                 select new CrudFProductWorkNormDetailDto
                                                 {
                                                     Id = this.GetNewObjectId(),
                                                     Ord0 = a.Ord0,
                                                     FProductWorkNormId = a.Id,
                                                     ProductCode = a.ProductCode,
                                                     WarehouseCode = a.WarehouseCode,
                                                     UnitCode = b?.UnitCode ?? "",
                                                     Price = a.Price,
                                                     Quantity = a.QuantityDetail,
                                                     Amount = a.Amount,
                                                     QuantityLoss = a.QuantityLoss,
                                                     PercentLoss = a.PercentLoss,
                                                     AccCode = a.AccCode,
                                                     SectionCode = a.SectionCode,
                                                     WorkPlaceCode = a.WorkPlaceCode,
                                                     Year = _webHelper.GetCurrentYear(),
                                                     Month = a.Month
                                                 }).ToList();


                item.FProductWorkNormDetails = lstFFProductWorkNormDetal;

            }

            var lstFFProductWorkNormDetale = lstFFProductWorkNorm.Select(p => ObjectMapper.Map<CrudFProductWorkNormDto, FProductWorkNorm>(p))
                                .ToList();
            var deleData = (await _fProductWorkNormService.GetQueryableAsync())
                           .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                           && p.Year == _webHelper.GetCurrentYear()
                           && lstFFProductWorkNorm.Select(p => p.FProductWorkCode).ToList().Contains(p.FProductWorkCode));
            await _fProductWorkNormService.DeleteManyAsync(deleData, true);
            await _fProductWorkNormService.CreateManyAsync(lstFFProductWorkNormDetale);
            return new UploadFileResponseDto() { Ok = true };
        }
        private CrudFProductWorkNormDto StandardDto(CrudFProductWorkNormDto dto)
        {
            if (dto.FProductWorkNormDetails == null) return dto;
            foreach (var item in dto.FProductWorkNormDetails)
            {
                string fromDateValue = $"{item.Year}-{item.Month.ToString().PadLeft(2, '0')}-01T00:00:00";
                DateTime beginDate = DateTime.Parse(fromDateValue);
                DateTime endDate = beginDate.AddMonths(1).AddDays(-1);

                item.BeginDate = beginDate;
                item.EndDate = endDate;
                item.ApplicableDate1 = beginDate;
                item.ApplicableDate2 = endDate;
            }
            return dto;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<AccCaseDto>();
        }
        #endregion
    }
}
