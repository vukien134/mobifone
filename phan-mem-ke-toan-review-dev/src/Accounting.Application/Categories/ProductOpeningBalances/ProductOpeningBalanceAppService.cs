using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Catgories.ProductOpeningBalances;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Categories.ProductOpeningBalances
{
    public class ProductOpeningBalanceAppService : AccountingAppService, IProductOpeningBalanceAppService
    {
        #region Fields
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly WarehouseService _warehouseService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly ProductService _product;
        #endregion
        #region Ctor
        public ProductOpeningBalanceAppService(ProductOpeningBalanceService productOpeningBalanceService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            WarehouseService warehouse,
                            ExcelService excelService,
                            ProductService productService
                            )
        {
            _productOpeningBalanceService = productOpeningBalanceService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _warehouseService = warehouse;
            _excelService = excelService;
            _product = productService;
        }
        #endregion
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerCreate)]
        public async Task<ProductOpeningBalanceDto> CreateAsync(CrudProductOpeningBalanceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var product = await _product.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.ProductCode).ToList().FirstOrDefault();
            if (lstProduct != null)
            {
                if (lstProduct.AttachProductLot == "C")
                {
                    if (string.IsNullOrEmpty(dto.ProductLotCode))
                    {
                        throw new Exception("Mã hàng " + dto.ProductCode + " bắt theo  lô hàng!");
                    }
                }
                if (lstProduct.AttachProductOrigin == "C")
                {
                    if (string.IsNullOrEmpty(dto.ProductOriginCode))
                    {
                        throw new Exception("Mã hàng " + dto.ProductCode + " bắt theo nguồn hàng!");
                    }
                }

            }
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudProductOpeningBalanceDto, ProductOpeningBalance>(dto);
            var result = await _productOpeningBalanceService.CreateAsync(entity);
            return ObjectMapper.Map<ProductOpeningBalance, ProductOpeningBalanceDto>(result);
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _productOpeningBalanceService.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerView)]
        public async Task<PageResultDto<ProductOpeningBalanceDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerView)]
        public async Task<PageResultDto<ProductOpeningBalanceDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ProductOpeningBalanceDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.WarehouseCode).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ProductOpeningBalance, ProductOpeningBalanceDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerUpdate)]
        public async Task UpdateAsync(string id, CrudProductOpeningBalanceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _productOpeningBalanceService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _productOpeningBalanceService.UpdateAsync(entity);
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudProductOpeningBalanceDto>(bytes, dto.WindowId);

            foreach (var item in lstImport)
            {
                var productOpeningBalances = await _productOpeningBalanceService.GetByProductOpeningBalanceAsync(item.WarehouseCode);
                if (productOpeningBalances != null)
                {
                    await _productOpeningBalanceService.DeleteManyAsync(productOpeningBalances);
                }
                var product = await _product.GetQueryableAsync();
                var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ProductCode).ToList().FirstOrDefault();
                if (lstProduct != null)
                {
                    if (lstProduct.AttachProductLot == "C")
                    {
                        if (string.IsNullOrEmpty(item.ProductLotCode))
                        {
                            throw new Exception("Mã hàng " + item.ProductCode + " bắt theo  lô hàng!");
                        }
                    }
                    if (lstProduct.AttachProductOrigin == "C")
                    {
                        if (string.IsNullOrEmpty(item.ProductOriginCode))
                        {
                            throw new Exception("Mã hàng " + item.ProductCode + " bắt theo nguồn hàng!");
                        }
                    }
                    item.AccCode = lstProduct.ProductAcc;
                }

                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
                item.Year = _webHelper.GetCurrentYear();
            }

            var lstProductOpeningBalance = lstImport.Select(p => ObjectMapper.Map<CrudProductOpeningBalanceDto, ProductOpeningBalance>(p)).ToList();
            await _productOpeningBalanceService.CreateManyAsync(lstProductOpeningBalance);
            return new UploadFileResponseDto() { Ok = true };
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerCreate)]
        public async Task<List<ProductOpeningBalanceDto>> CreateListAsync(ObjectProductOpeningBalanceDto listDto)
        {
            //var productOpeningBalances = await _productOpeningBalanceService.GetByProductOpeningBalanceAsync(item.WarehouseCode);
            await _licenseBusiness.CheckExpired();
            List<ProductOpeningBalanceDto> listResult = new List<ProductOpeningBalanceDto>();
            var listAccCode = from a in listDto.Data
                              group a.WarehouseCode by a.WarehouseCode into g
                              select new { WarehouseCode = g.Key };


            var productOpeningBalances = await _productOpeningBalanceService.GetByProductOpeningBalanceAsync(listDto.WarehouseCode);
            if (productOpeningBalances != null)
            {
                await _productOpeningBalanceService.DeleteManyAsync(productOpeningBalances);
            }

            var product = await _product.GetQueryableAsync();
            product = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var attachProductLot = "";
            var attachProductOrigin = "";

            foreach (var dto in listDto.Data)
            {

                dto.CreatorName = await _userService.GetCurrentUserNameAsync();
                dto.Id = GetNewObjectId();
                dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                dto.Year = _webHelper.GetCurrentYear();
                var products = product.Where(p => p.Code == dto.ProductCode);

                attachProductLot = products.FirstOrDefault().AttachProductLot;
                if (attachProductLot == "C")
                {
                    if (string.IsNullOrEmpty(dto.ProductLotCode))
                    {
                        throw new Exception("Chưa nhập lô hàng cho mã hàng " + dto.ProductCode);
                    }
                }
                attachProductOrigin = products.FirstOrDefault().AttachProductOrigin;
                if (attachProductOrigin == "C")
                {
                    if (string.IsNullOrEmpty(dto.ProductOriginCode))
                    {
                        throw new Exception("Chưa nhập nguồn hàng cho mã hàng " + dto.ProductCode);
                    }
                }
                var entity = ObjectMapper.Map<CrudProductOpeningBalanceDto, ProductOpeningBalance>(dto);
                var result = await _productOpeningBalanceService.CreateAsync(entity);
                listResult.Add(ObjectMapper.Map<ProductOpeningBalance, ProductOpeningBalanceDto>(result));
            }
            return listResult;
        }
        public async Task<List<ProductOpeningBalanceCustommerDto>> GetDetailProductAsync(string WarehouseCode)
        {
            var result = new List<ProductOpeningBalanceCustommerDto>();
            var query = await _productOpeningBalanceService.GetQueryableAsync();
            query = query.Where(p => p.WarehouseCode == WarehouseCode
                                     && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                     && p.Year == _webHelper.GetCurrentYear());
            var pro = await _product.GetQueryableAsync();

            pro = pro.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var resuls = from b in pro
                         join d in query on b.Code equals d.ProductCode
                         select new ProductOpeningBalanceCustommerDto
                         {
                             Ord0 = d.Ord0,
                             Year = d.Year,
                             WarehouseCode = d.WarehouseCode,
                             AccCode = b.ProductAcc,
                             ProductCode = d.ProductCode,
                             ProductLotCode = d.ProductLotCode,
                             ProductOriginCode = d.ProductOriginCode,
                             Quantity = d.Quantity,
                             Price = d.Price,
                             PriceCur = d.PriceCur,
                             Amount = d.Amount,
                             AmountCur = d.AmountCur,
                             ProductName = b.Name,
                             OrgUnit = b.UnitCode,
                             Id = d.Id,
                             CreatorName = d.CreatorName,
                             LastModifierName = b.LastModifierName,
                             OrgCode = b.OrgCode,


                         };
            var sections = await AsyncExecuter.ToListAsync(resuls);

            return sections;
        }
        [Authorize(AccountingPermissions.ProductOpeningBalanceManagerView)]
        public async Task<List<ProductOpeningBalanceCustomerDto>> GetDataAsync()
        {

            var result = new List<ProductOpeningBalanceCustomerDto>();
            var queryable = await _productOpeningBalanceService.GetQueryableAsync();
            var warehouses = await _warehouseService.GetQueryableAsync();
            var ordCode = _webHelper.GetCurrentOrgUnit();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                            && p.Year == _webHelper.GetCurrentYear()
                                                            );

            warehouses = warehouses.Where(c => c.OrgCode == ordCode && c.WarehouseType == "C");
            var datas = from aS in warehouses
                        join aOB in queryable on aS.Code equals aOB.WarehouseCode into aOBDt
                        from aOB in aOBDt.DefaultIfEmpty()
                        group new { aS, aOB } by new
                        {
                            aS.Code,
                            aS.Name,
                            aS.OrgCode
                        } into aSGr
                        select new ProductOpeningBalanceCustomerDto
                        {
                            WarehouseCode = aSGr.Key.Code,
                            WarehouseName = aSGr.Key.Name,
                            OrgCode = aSGr.Key.OrgCode,
                            Quantity = aSGr.Sum(d => d.aOB.Quantity),
                            Amount = aSGr.Sum(d => d.aOB.Amount),
                            AmountCur = aSGr.Sum(d => d.aOB.AmountCur)
                        };
            var sections = await AsyncExecuter.ToListAsync(datas);
            result = sections.Select(p => p).ToList();
            return result;
        }
        #region Private
        private async Task<IQueryable<ProductOpeningBalance>> Filter(PageRequestDto dto)
        {
            var queryable = await _productOpeningBalanceService.GetQueryableAsync();
            if (dto.FilterRows == null) return queryable;

            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
            }
            return queryable;
        }

        #endregion

    }
}
