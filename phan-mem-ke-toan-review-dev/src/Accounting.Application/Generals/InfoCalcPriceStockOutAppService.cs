using Accounting.BaseDtos;
using Accounting.Categories.CostProductions;
using Accounting.Catgories.CostProductions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Generals;
using Accounting.DomainServices.Users;
using Accounting.Generals.PriceStockOuts;
using Accounting.Helpers;
using Accounting.Jobs.CalcPrices;
using Accounting.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Generals
{
    public class InfoCalcPriceStockOutAppService : AccountingAppService, IInfoCalcPriceStockOutAppService
    {
        #region Fields
        private readonly InfoCalcPriceStockOutService _infoCalcPriceStockOutService;
        private readonly InfoCalcPriceStockOutDetailService _infoCalcPriceStockOutDetailService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ICurrentTenant _currentTenant;
        private readonly ConfigCostPriceService _configCostPriceService;
        #endregion
        #region Ctor
        public InfoCalcPriceStockOutAppService(InfoCalcPriceStockOutService infoCalcPriceStockOutService,
                    InfoCalcPriceStockOutDetailService infoCalcPriceStockOutDetailService,
                    UserService userService,
                    WebHelper webHelper,
                    IUnitOfWorkManager unitOfWorkManager,
                    WarehouseBookService warehouseBookService,
                    IBackgroundJobManager backgroundJobManager,
                    ICurrentTenant currentTenant,
                    ConfigCostPriceService configCostPriceService
            )
        {
            _infoCalcPriceStockOutService = infoCalcPriceStockOutService;
            _infoCalcPriceStockOutDetailService = infoCalcPriceStockOutDetailService;
            _webHelper = webHelper;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _warehouseBookService = warehouseBookService;
            _backgroundJobManager = backgroundJobManager;
            _currentTenant = currentTenant;
            _configCostPriceService = configCostPriceService;
        }
        #endregion
        public async Task<InfoCalcPriceStockOutDto> CreateAsync(CrudInfoCalcPriceStockOutDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto.Status = ExcutionStatus.Waiting;

            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var infoCalcPriceStockOut = await _infoCalcPriceStockOutService.GetByInfoCalcPriceStockOutAsync(_webHelper.GetCurrentOrgUnit());
                if (infoCalcPriceStockOut.Count == 0)
                {
                    var entity = ObjectMapper.Map<CrudInfoCalcPriceStockOutDto, InfoCalcPriceStockOut>(dto);
                    entity.WarehouseCose = dto.WarehouseCode;
                    var result = await _infoCalcPriceStockOutService.CreateAsync(entity,true);
                    var configCostPrice = await _configCostPriceService.GetQueryableAsync();
                    configCostPrice = configCostPrice.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                    if (configCostPrice == null)
                    {
                        CrudConfigCostPriceDto crudConfigCostPriceDto = new CrudConfigCostPriceDto();
                        crudConfigCostPriceDto.Id = this.GetNewObjectId();
                        crudConfigCostPriceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        crudConfigCostPriceDto.Type = int.Parse(dto.CalculatingMethod);
                        crudConfigCostPriceDto.ConsecutiveMonth = dto.Continuous == true ? 1 : 0;
                        var entitys = ObjectMapper.Map<CrudConfigCostPriceDto, ConfigCostPrice>(crudConfigCostPriceDto);
                        await _configCostPriceService.CreateAsync(entitys,true);
                    }
                    else
                    {
                        foreach (var item in configCostPrice)
                        {


                            item.Type = int.Parse(dto.CalculatingMethod);
                            item.ConsecutiveMonth = dto.Continuous == true ? 1 : 0;
                            // var entitys = ObjectMapper.Map<CrudConfigCostPriceDto, ConfigCostPrice>(crudConfigCostPriceDto);
                            await _configCostPriceService.UpdateAsync(item, true);
                        }

                    }
                    await SaveListProduct(result);
                    await unitOfWork.CompleteAsync();
                    return ObjectMapper.Map<InfoCalcPriceStockOut, InfoCalcPriceStockOutDto>(result);
                }
                else
                {
                    throw new Exception("Đang có mã hàng chờ thực hiện vui lòng thử lại sau ");
                }

            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(string id, CrudInfoCalcPriceStockOutDetailDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            if (string.IsNullOrEmpty(dto.OrgCode))
            {
                dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            }
            // dto.Id = this.GetNewObjectId();
            var entity = await _infoCalcPriceStockOutDetailService.GetByInfoCalcPriceStockOutAsync(dto.Id);
            await _infoCalcPriceStockOutDetailService.DeleteAsync(dto.Id, true);

            entity.Select(p => ObjectMapper.Map<InfoCalcPriceStockOutDetail, CrudInfoCalcPriceStockOutDetailDto>(p)).ToList();
            await _infoCalcPriceStockOutDetailService.CreateManyAsync(entity, true);
        }
        public async Task DeleteAsync(string id)
        {
            await _infoCalcPriceStockOutService.DeleteAsync(id, true);
        }
        public async Task UpdatePartAsync(string id, CrudInfoCalcPriceStockOutDto dto)
        {
            var entity = await _infoCalcPriceStockOutService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _infoCalcPriceStockOutService.UpdateAsync(entity, true);

        }
        public async Task<InfoCalcPriceStockOutDto> GetByIdAsync(string infoId)
        {
            var info = await _infoCalcPriceStockOutService.GetAsync(infoId);
            return ObjectMapper.Map<InfoCalcPriceStockOut, InfoCalcPriceStockOutDto>(info);
        }

        public async Task<PageResultDto<InfoCalcPriceStockOutDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<InfoCalcPriceStockOutDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Status).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<InfoCalcPriceStockOut, InfoCalcPriceStockOutDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<PageResultDto<InfoCalcPriceStockOutDetailDto>> ProductsAsync(string id, PageRequestDto dto)
        {
            var result = new PageResultDto<InfoCalcPriceStockOutDetailDto>();
            var query = await FilterProducts(id);
            var querysort = query.OrderBy(p => p.Status).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<InfoCalcPriceStockOutDetail, InfoCalcPriceStockOutDetailDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;

        }

        public Task<PageResultDto<InfoCalcPriceStockOutDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        public async Task<InfoCalcPriceStockOutDto> GetStart(string id)
        {
            var infoCalc = await _infoCalcPriceStockOutService.GetAsync(id);
            try
            {

                infoCalc.Status = ExcutionStatus.Excuting;
                await _infoCalcPriceStockOutService.UpdateAsync(infoCalc, true);

                await _backgroundJobManager.EnqueueAsync(
                    new CalcStockOutPriceArg
                    {
                        TenantId = _currentTenant.GetId(),
                        InfoCalcStockOutId = id,
                        Year = _webHelper.GetCurrentYear()

                    }
                );

            }
            catch (Exception ex)
            {

            }
            return ObjectMapper.Map<InfoCalcPriceStockOut, InfoCalcPriceStockOutDto>(infoCalc);
        }
        #region Privates
        private async Task<IQueryable<InfoCalcPriceStockOut>> Filter(PageRequestDto dto)
        {
            var queryable = await _infoCalcPriceStockOutService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                            && p.Year == _webHelper.GetCurrentYear());
            return queryable;
        }
        private async Task<IQueryable<InfoCalcPriceStockOutDetail>> FilterProducts(string id)
        {
            var queryable = await _infoCalcPriceStockOutDetailService.GetQueryableAsync();
            queryable = queryable.Where(p => p.InfoCalcPriceStockOutId.Equals(id));

            return queryable;
        }
        private async Task SaveListProduct(InfoCalcPriceStockOut info)
        {
            var listProduct = await _warehouseBookService.GetQueryableAsync();
            listProduct = listProduct.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (string.IsNullOrEmpty(info.WarehouseCose) == false)
            {
                listProduct = listProduct.Where(p => p.WarehouseCode == info.WarehouseCose);
            }
            if (string.IsNullOrEmpty(info.ProductOriginCode) == false)
            {
                listProduct = listProduct.Where(p => p.ProductOriginCode == info.ProductOriginCode);
            }
            if (string.IsNullOrEmpty(info.ProductLotCode) == false)
            {
                listProduct = listProduct.Where(p => p.ProductLotCode == info.ProductLotCode);
            }
            if (string.IsNullOrEmpty(info.ProductCode) == false)
            {
                listProduct = listProduct.Where(p => p.ProductCode == info.ProductCode);
            }
            var listProducts = (from a in listProduct
                                where a.VoucherDate >= info.FromDate && a.VoucherDate <= info.ToDate

                                group new { a } by new
                                {
                                    a.ProductCode,


                                } into gr

                                select new WarehouseBook
                                {
                                    ProductCode = gr.Key.ProductCode,
                                    ProductName0 = gr.Max(p => p.a.ProductName0)

                                }).ToList();
            List<CrudInfoCalcPriceStockOutDetailDto> CrudInfoCalcPriceStockOutDetailDto = new List<CrudInfoCalcPriceStockOutDetailDto>();
            for (int i = 0; i < listProducts.Count; i++)
            {
                CrudInfoCalcPriceStockOutDetailDto cru = new CrudInfoCalcPriceStockOutDetailDto();
                cru.Id = this.GetNewObjectId();
                cru.InfoCalcPriceStockOutId = info.Id;
                cru.ProductCode = listProducts[i].ProductCode;
                cru.ProductName = listProducts[i].ProductName0;
                cru.Status = ExcutionStatus.Waiting;
                cru.OrgCode = _webHelper.GetCurrentOrgUnit();
                CrudInfoCalcPriceStockOutDetailDto.Add(cru);
            }
            var lstdiscountPrice = CrudInfoCalcPriceStockOutDetailDto.Select(p => ObjectMapper.Map<CrudInfoCalcPriceStockOutDetailDto, InfoCalcPriceStockOutDetail>(p))
                              .ToList();
            await _infoCalcPriceStockOutDetailService.CreateManyAsync(lstdiscountPrice, true);

        }
        #endregion
    }
}
