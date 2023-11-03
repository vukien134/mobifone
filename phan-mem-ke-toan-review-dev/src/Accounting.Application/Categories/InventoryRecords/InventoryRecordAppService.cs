using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Products;
using Accounting.Categories.ProductVouchers;
using Accounting.Catgories.InventoryRecords;
using Accounting.Catgories.ProductVouchers;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Excels;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.RefVouchers;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Categories.InventoryRecords
{
    public class InventoryRecordAppService : AccountingAppService, IInventoryRecordAppService
    {
        #region Field
        private readonly InventoryRecordService _inventoryRecordService;
        private readonly InventoryRecordDetailService _inventoryRecordDetailService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly ProductService _productService;
        private readonly UserService _userService;
        private readonly CreateAccVoucherBusiness _createAccVoucherBusiness;
        private readonly CreateProductVoucherBusiness _createProductVoucherBusiness;
        private readonly TenantSettingService _tenantSettingService;
        private readonly AccPartnerService _accPartnerService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly AccVoucherService _accVoucherService;
        private readonly RefVoucherService _refVoucherService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        #endregion
        #region Ctor
        public InventoryRecordAppService(InventoryRecordService inventoryRecordService,
                                InventoryRecordDetailService inventoryRecordDetailService,
                                VoucherNumberBusiness voucherNumberBusiness,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                WarehouseBookService warehouseBookService,
                                ProductService productService,
                                UserService userService,
                                CreateAccVoucherBusiness createAccVoucherBusiness,
                                CreateProductVoucherBusiness createProductVoucherBusiness,
                                TenantSettingService tenantSettingService,
                                AccPartnerService accPartnerService,
                                ProductVoucherService productVoucherService,
                                AccVoucherService accVoucherService,
                                RefVoucherService refVoucherService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ExcelService excelService)
        {
            _inventoryRecordService = inventoryRecordService;
            _inventoryRecordDetailService = inventoryRecordDetailService;
            _voucherNumberBusiness = voucherNumberBusiness;
            _productOpeningBalanceService = productOpeningBalanceService;
            _warehouseBookService = warehouseBookService;
            _productService = productService;
            _userService = userService;
            _createAccVoucherBusiness = createAccVoucherBusiness;
            _createProductVoucherBusiness = createProductVoucherBusiness;
            _tenantSettingService = tenantSettingService;
            _accPartnerService = accPartnerService;
            _productVoucherService = productVoucherService;
            _accVoucherService = accVoucherService;
            _refVoucherService = refVoucherService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
        }
        #endregion

        public async Task<InventoryRecordDto> CreateAsync(CrudInventoryRecordDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            if ((dto.VoucherNumber ?? "") == "")
            {
                var voucherNumber = await _voucherNumberBusiness.AutoVoucherNumberAsync(dto.VoucherCode, dto.VoucherDate);
                dto.VoucherNumber = voucherNumber.VoucherNumber;
            }
            else
            {
                await _voucherNumberBusiness.UpdateVoucherNumberAsync(dto.VoucherCode, dto.VoucherNumber, dto.VoucherDate);
            }
            var entity = ObjectMapper.Map<CrudInventoryRecordDto, InventoryRecord>(dto);
            var result = await _inventoryRecordService.CreateAsync(entity);
            return ObjectMapper.Map<InventoryRecord, InventoryRecordDto>(result);
        }

        public async Task<ResultDto> PostCreateVoucherAsync(IRCreateVoucherDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var curencyCode = await _tenantSettingService.GetTenantSettingByKeyAsync("M_MA_NT0", _webHelper.GetCurrentOrgUnit());
            var refVoucher = await _refVoucherService.GetQueryableAsync();
            var inventoryRecord = await _inventoryRecordService.GetQueryableAsync();
            inventoryRecord = inventoryRecord.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var inventoryRecordDetail = await _inventoryRecordDetailService.GetQueryableAsync();
            inventoryRecordDetail = inventoryRecordDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var product = await _productService.GetQueryableAsync();
            product = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstRefVoucherDel = refVoucher.Where(p => p.SrcId == dto.Id).ToList();
            // xóa chứng từ trước khi thêm vào
            foreach (var itemDel in lstRefVoucherDel)
            {
                if (await _accVoucherService.IsExistId(itemDel.DestId)) await _createAccVoucherBusiness.DeleteAccVoucherAsync(itemDel.DestId);
                if (await _productVoucherService.IsExistId(itemDel.DestId)) await _createProductVoucherBusiness.DeleteProductVoucherAsync(itemDel.DestId);
                var refVoucherDel = (await _refVoucherService.GetQueryableAsync()).Where(p => p.SrcId == dto.Id || p.DestId == dto.Id);
                await _refVoucherService.DeleteManyAsync(refVoucherDel);
            }
            var dataCreate = new CrudProductVoucherDto();
            // Xuất
            var dataExport = (from a in inventoryRecord
                              join b in inventoryRecordDetail on a.Id equals b.InventoryRecordId
                              join c in product on b.ProductCode equals c.Code into bjc
                              from c in bjc.DefaultIfEmpty()
                              where a.Id == dto.Id && b.ShortQuantity > 0
                              orderby b.Ord0
                              select new
                              {
                                  Id = GetNewObjectId(),
                                  Year = b.Year,
                                  ProductCode = b.ProductCode,
                                  UnitCode = b.UnitCode,
                                  WarehouseCode = b.WarehouseCode,
                                  ProductLotCode = b.ProductLotCode,
                                  ProductOriginCode = b.ProductOriginCode,
                                  Price = b.Price,
                                  AuditQuantity = b.AuditQuantity,
                                  AuditAmount = b.AuditAmount,
                                  InventoryQuantity = b.InventoryQuantity,
                                  InventoryAmount = b.InventoryAmount,
                                  OverQuantity = b.OverQuantity,
                                  OverAmount = b.OverAmount,
                                  ShortQuantity = b.ShortQuantity,
                                  ShortAmount = b.ShortAmount,
                                  Quality1 = b.Quality1,
                                  Quality2 = b.Quality2,
                                  Quality3 = b.Quality3,
                                  Note = b.Note,
                                  Acc = b.Acc,
                                  FProductWorkCode = b.FProductWorkCode,
                                  WorkPlaceCode = b.WorkPlaceCode,
                                  SectionCode = b.SectionCode,
                                  ProductName = (c == null || c.Name == null) ? "" : c.Name,
                                  ProductAcc = (c == null || c.ProductAcc == null) ? "" : c.ProductAcc,
                              }).ToList();
            if (dataExport.Count > 0)
            {
                var partner = await _accPartnerService.GetAccPartnerByCodeAsync(dto.PartnerCodeExport, _webHelper.GetCurrentOrgUnit());
                dataCreate.VoucherCode = dto.VoucherCodeExport;
                dataCreate.VoucherDate = dto.DateExport;
                dataCreate.VoucherGroup = 2;
                dataCreate.CurrencyCode = curencyCode.Value;
                dataCreate.ExchangeRate = 1;
                dataCreate.Description = dto.DescriptionExport;
                dataCreate.TotalAmountWithoutVat = dataExport.Sum(p => p.ShortAmount);
                dataCreate.TotalAmount = dataExport.Sum(p => p.ShortAmount);
                dataCreate.TotalProductAmount = dataExport.Sum(p => p.ShortAmount);
                dataCreate.Status = "1";
                dataCreate.Locked = false;
                dataCreate.PartnerCode0 = dto.PartnerCodeExport;
                dataCreate.Address = partner?.Address ?? "";
                dataCreate.PartnerName0 = partner?.Name ?? "";
                dataCreate.Representative = partner?.Representative ?? "";
                dataCreate.RefVoucher = dto.Id;

                var productVoucherReceipt = new CrudProductVoucherReceiptDto();
                productVoucherReceipt.OrgCode = _webHelper.GetCurrentOrgUnit();
                productVoucherReceipt.Year = _webHelper.GetCurrentYear();
                var lstProductVoucherReceipt = new List<CrudProductVoucherReceiptDto>();
                lstProductVoucherReceipt.Add(productVoucherReceipt);
                dataCreate.ProductVoucherReceipts = lstProductVoucherReceipt;

                dataCreate.ProductVoucherDetails = (from a in dataExport
                                                    select new CrudProductVoucherDetailDto
                                                    {
                                                        ProductCode = a.ProductCode,
                                                        ProductName = a.ProductName,
                                                        UnitCode = a.UnitCode,
                                                        WarehouseCode = a.WarehouseCode,
                                                        ProductLotCode = a.ProductLotCode,
                                                        ProductOriginCode = a.ProductOriginCode,
                                                        Quantity = a.ShortQuantity,
                                                        Price = a.Price,
                                                        Amount = a.ShortAmount,
                                                        DebitAcc = (a.Acc ?? "") == "" ? dto.AdjustAcc : a.Acc,
                                                        CreditAcc = a.ProductAcc,
                                                        FProductWorkCode = a.FProductWorkCode,
                                                        WorkPlaceCode = a.WorkPlaceCode,
                                                        SectionCode = a.SectionCode,

                                                    }).ToList();
                var productVoucherDetailReceipt = new CrudProductVoucherDetailReceiptDto();
                productVoucherReceipt.OrgCode = _webHelper.GetCurrentOrgUnit();
                productVoucherReceipt.Year = _webHelper.GetCurrentYear();
                var lstProductVoucherDetailReceipt = new List<CrudProductVoucherDetailReceiptDto>();
                foreach (var item in dataCreate.ProductVoucherDetails)
                {
                    lstProductVoucherDetailReceipt.Add(productVoucherDetailReceipt);
                    item.ProductVoucherDetailReceipts = lstProductVoucherDetailReceipt;
                }
                await _createProductVoucherBusiness.CreateProductVoucherAsync(dataCreate);
            }

            // Nhập
            var dataImport = (from a in inventoryRecord
                              join b in inventoryRecordDetail on a.Id equals b.InventoryRecordId
                              join c in product on b.ProductCode equals c.Code into bjc
                              from c in bjc.DefaultIfEmpty()
                              where a.Id == dto.Id && b.OverQuantity > 0
                              orderby b.Ord0
                              select new
                              {
                                  Id = GetNewObjectId(),
                                  Year = b.Year,
                                  ProductCode = b.ProductCode,
                                  UnitCode = b.UnitCode,
                                  WarehouseCode = b.WarehouseCode,
                                  ProductLotCode = b.ProductLotCode,
                                  ProductOriginCode = b.ProductOriginCode,
                                  Price = b.Price,
                                  AuditQuantity = b.AuditQuantity,
                                  AuditAmount = b.AuditAmount,
                                  InventoryQuantity = b.InventoryQuantity,
                                  InventoryAmount = b.InventoryAmount,
                                  OverQuantity = b.OverQuantity,
                                  OverAmount = b.OverAmount,
                                  ShortQuantity = b.ShortQuantity,
                                  ShortAmount = b.ShortAmount,
                                  Quality1 = b.Quality1,
                                  Quality2 = b.Quality2,
                                  Quality3 = b.Quality3,
                                  Note = b.Note,
                                  Acc = b.Acc,
                                  FProductWorkCode = b.FProductWorkCode,
                                  WorkPlaceCode = b.WorkPlaceCode,
                                  SectionCode = b.SectionCode,
                                  ProductName = (c == null || c.Name == null) ? "" : c.Name,
                                  ProductAcc = (c == null || c.ProductAcc == null) ? "" : c.ProductAcc,
                              }).ToList();
            if (dataImport.Count > 0)
            {
                dataCreate = new CrudProductVoucherDto();
                var partner = await _accPartnerService.GetAccPartnerByCodeAsync(dto.PartnerCodeImport, _webHelper.GetCurrentOrgUnit());
                dataCreate.VoucherCode = dto.VoucherCodeImport;
                dataCreate.VoucherDate = dto.DateImport;
                dataCreate.VoucherGroup = 1;
                dataCreate.CurrencyCode = curencyCode.Value;
                dataCreate.ExchangeRate = 1;
                dataCreate.Description = dto.DescriptionImport;
                dataCreate.TotalAmountWithoutVat = dataImport.Sum(p => p.OverAmount);
                dataCreate.TotalAmount = dataImport.Sum(p => p.OverAmount);
                dataCreate.TotalProductAmount = dataImport.Sum(p => p.OverAmount);
                dataCreate.Status = "1";
                dataCreate.Locked = false;
                dataCreate.PartnerCode0 = dto.PartnerCodeImport;
                dataCreate.Address = partner?.Address ?? "";
                dataCreate.PartnerName0 = partner?.Name ?? "";
                dataCreate.Representative = partner?.Representative ?? "";
                dataCreate.RefVoucher = dto.Id;

                var productVoucherReceipt = new CrudProductVoucherReceiptDto();
                productVoucherReceipt.OrgCode = _webHelper.GetCurrentOrgUnit();
                productVoucherReceipt.Year = _webHelper.GetCurrentYear();
                var lstProductVoucherReceipt = new List<CrudProductVoucherReceiptDto>();
                lstProductVoucherReceipt.Add(productVoucherReceipt);
                dataCreate.ProductVoucherReceipts = lstProductVoucherReceipt;

                dataCreate.ProductVoucherDetails = (from a in dataImport
                                                    select new CrudProductVoucherDetailDto
                                                    {
                                                        ProductCode = a.ProductCode,
                                                        ProductName = a.ProductName,
                                                        UnitCode = a.UnitCode,
                                                        WarehouseCode = a.WarehouseCode,
                                                        ProductLotCode = a.ProductLotCode,
                                                        ProductOriginCode = a.ProductOriginCode,
                                                        Quantity = a.OverQuantity,
                                                        Price = a.Price,
                                                        Amount = a.OverAmount,
                                                        DebitAcc = a.ProductAcc,
                                                        CreditAcc = (a.Acc ?? "") == "" ? dto.AdjustAcc : a.Acc,
                                                        FProductWorkCode = a.FProductWorkCode,
                                                        WorkPlaceCode = a.WorkPlaceCode,
                                                        SectionCode = a.SectionCode,

                                                    }).ToList();
                var productVoucherDetailReceipt = new CrudProductVoucherDetailReceiptDto();
                productVoucherReceipt.OrgCode = _webHelper.GetCurrentOrgUnit();
                productVoucherReceipt.Year = _webHelper.GetCurrentYear();
                var lstProductVoucherDetailReceipt = new List<CrudProductVoucherDetailReceiptDto>();
                foreach (var item in dataCreate.ProductVoucherDetails)
                {
                    lstProductVoucherDetailReceipt.Add(productVoucherDetailReceipt);
                    item.ProductVoucherDetailReceipts = lstProductVoucherDetailReceipt;
                }
                await _createProductVoucherBusiness.CreateProductVoucherAsync(dataCreate);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Tạo chứng từ thành công";
            return res;
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            try
            {
                var inventoryRecordDetails = await _inventoryRecordDetailService.GetByInventoryRecordIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (inventoryRecordDetails != null)
                {
                    await _inventoryRecordDetailService.DeleteManyAsync(inventoryRecordDetails);
                }
                await _inventoryRecordService.DeleteAsync(id);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

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

        public Task<PageResultDto<InventoryRecordDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<InventoryRecordDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<InventoryRecordDto>();
            var query = await Filter(dto);
            var querysort = query.OrderByDescending(p => p.VoucherDate).OrderBy(p => p.VoucherNumber).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<InventoryRecord, InventoryRecordDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<InventoryRecordDetailResDto>> GetListInventoryRecordDetailAsync(string inventoryRecordId)
        {
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var inventoryRecordDetails = await _inventoryRecordDetailService.GetByInventoryRecordIdAsync(inventoryRecordId);
            var data = (from a in inventoryRecordDetails
                        join b in lstProduct on a.ProductCode equals b.Code into ajb
                        from b in ajb.DefaultIfEmpty()
                        select new InventoryRecordDetailResDto
                        {
                            Id = a.Id,
                            InventoryRecordId = a.InventoryRecordId,
                            Ord0 = a.Ord0,
                            Year = a.Year,
                            ProductCode = a.ProductCode,
                            UnitCode = a.UnitCode,
                            WarehouseCode = a.WarehouseCode,
                            ProductLotCode = a.ProductLotCode,
                            ProductOriginCode = a.ProductOriginCode,
                            Price = a.Price,
                            AuditQuantity = a.AuditQuantity,
                            AuditAmount = a.AuditAmount,
                            InventoryQuantity = a.InventoryQuantity,
                            InventoryAmount = a.InventoryAmount,
                            OverQuantity = a.OverQuantity,
                            OverAmount = a.OverAmount,
                            ShortAmount = a.ShortAmount,
                            ShortQuantity = a.ShortQuantity,
                            Quality1 = a.Quality1,
                            Quality2 = a.Quality2,
                            Quality3 = a.Quality3,
                            Note = a.Note,
                            Acc = a.Acc,
                            FProductWorkCode = a.FProductWorkCode,
                            WorkPlaceCode = a.WorkPlaceCode,
                            SectionCode = a.SectionCode,
                            ProductName = b?.Name ?? "",
                            AttachProductLot = b?.AttachProductLot ?? "",
                            AttachProductOrigin = b?.AttachProductOrigin ?? "",
                        }).ToList();
            return data;
        }

        public async Task UpdateAsync(string id, CrudInventoryRecordDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            var entity = await _inventoryRecordService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                var inventoryRecordDetails = await _inventoryRecordDetailService.GetByInventoryRecordIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (inventoryRecordDetails != null)
                {
                    await _inventoryRecordDetailService.DeleteManyAsync(inventoryRecordDetails);
                }
                await _inventoryRecordService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }

        public async Task<List<ConfirmInventoryDto>> ConfirmInventoryAsync(ConfirmInventoryParameterDto dto)
        {
            var data = dto.Data;
            var productOpeningBalance = await _productOpeningBalanceService.GetQueryableAsync();
            var lstProductOpeningBalance = productOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            var lstWarehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var id0 = data.Select(p => p.Id).FirstOrDefault();
            var voucherDate = data.Select(p => p.VoucherDate).FirstOrDefault();
            var year = voucherDate.Year;
            foreach (var item in data)
            {
                item.Year = year;
                item.ProductLotCode = item.ProductLotCode ?? "";
                item.ProductOriginCode = item.ProductOriginCode ?? "";
            }
            var quantityInventory = (from a in data
                                     join b in lstProductOpeningBalance
                                     on new { WarehouseCode = a.WarehouseCode ?? "", ProductCode = a.ProductCode ?? "", ProductLotCode = a.ProductLotCode ?? "", ProductOriginCode = a.ProductOriginCode ?? "", Year = a.VoucherDate.Year }
                                     equals new { WarehouseCode = b?.WarehouseCode ?? "", ProductCode = b?.ProductCode ?? "", ProductLotCode = b?.ProductLotCode ?? "", ProductOriginCode = b?.ProductOriginCode ?? "", Year = b?.Year ?? 0 } into ajb
                                     from b in ajb.DefaultIfEmpty()
                                     group new { a, b } by new { a.Id } into gr
                                     select new ConfirmInventoryDto
                                     {
                                         Id = gr.Key.Id,
                                         Quantity = gr.Sum(p => p.b?.Quantity ?? 0),
                                         Amount = gr.Sum(p => p.b?.Amount ?? 0),
                                     }).ToList();
            quantityInventory.AddRange((from a in data
                                        join b in lstWarehouseBook
                                        on new { WarehouseCode = a.WarehouseCode ?? "", ProductCode = a.ProductCode ?? "", ProductLotCode = a.ProductLotCode ?? "", ProductOriginCode = a.ProductOriginCode ?? "" }
                                        equals new { WarehouseCode = b?.WarehouseCode ?? "", ProductCode = b?.ProductCode ?? "", ProductLotCode = b?.ProductLotCode ?? "", ProductOriginCode = b?.ProductOriginCode ?? "" }
                                        where b.Year == year && String.Compare(b.Status, "2") < 0 && b.VoucherDate <= voucherDate
                                        group new { a, b } by new { a.Id } into gr
                                        select new ConfirmInventoryDto
                                        {
                                            Id = gr.Key.Id,
                                            Quantity = gr.Sum(p => (p.b.ImportQuantity ?? 0) - (p.b.ExportQuantity ?? 0)),
                                            Amount = gr.Sum(p => (p.b.ImportAmount ?? 0) - (p.b.ExportAmount ?? 0)),
                                        }).ToList());
            quantityInventory = (from a in quantityInventory
                                 group new { a } by new { a.Id } into gr
                                 select new ConfirmInventoryDto
                                 {
                                     Id = gr.Key.Id,
                                     Quantity = gr.Sum(p => p.a.Quantity),
                                     Price = (gr.Sum(p => p.a.Quantity ?? 0) != 0) ? Math.Round(gr.Sum(p => p.a.Amount ?? 0) / gr.Sum(p => p.a.Quantity ?? 0), 2) : 0,
                                     Amount = gr.Sum(p => (p.a.Amount ?? 0))
                                 }).ToList();
            return quantityInventory;
        }

        public async Task<InventoryRecordDto> GetByIdAsync(string partnerId)
        {
            var partner = await _inventoryRecordService.GetAsync(partnerId);
            return ObjectMapper.Map<InventoryRecord, InventoryRecordDto>(partner);
        }

        #region Private
        private async Task<IQueryable<InventoryRecord>> Filter(PageRequestDto dto)
        {
            var queryable = await _inventoryRecordService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }


        #endregion
    }
}
