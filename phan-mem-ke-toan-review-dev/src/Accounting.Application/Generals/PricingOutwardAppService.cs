using Accounting.Caching;
using Accounting.Categories.CostOfGoods;
using Accounting.Catgories.Others.CostOfGoods;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Generals;
using Accounting.DomainServices.Users;
using Accounting.Generals.PriceStockOuts;
using Accounting.Helpers;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accounting.Generals
{
    public class PricingOutwardAppService : AccountingAppService
    {
        #region Fields
        private readonly InfoCalcPriceStockOutService _infoCalcPriceStockOutService;
        private readonly InfoCalcPriceStockOutDetailService _infoCalcPriceStockOutDetailService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly InfoCalcPriceStockOutAppService _infoCalcPriceStockOutAppService;
        private readonly CostOfGoodsAppService _costOfGoodsAppService;
        private readonly VoucherCategoryService _voucherCategoryService;

        #endregion
        #region Ctor
        public PricingOutwardAppService(WarehouseBookService warehouseBookService,
            WebHelper webHelper,
            AccountingCacheManager accountingCacheManager,
            InfoCalcPriceStockOutAppService infoCalcPriceStockOutAppService,
            CostOfGoodsAppService costOfGoodsAppService,
            InfoCalcPriceStockOutService infoCalcPriceStockOutService,
            InfoCalcPriceStockOutDetailService infoCalcPriceStockOutDetailService,
            VoucherCategoryService voucherCategoryService)
        {
            _warehouseBookService = warehouseBookService;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
            _infoCalcPriceStockOutAppService = infoCalcPriceStockOutAppService;
            _costOfGoodsAppService = costOfGoodsAppService;
            _infoCalcPriceStockOutService = infoCalcPriceStockOutService;
            _infoCalcPriceStockOutDetailService = infoCalcPriceStockOutDetailService;
            _voucherCategoryService = voucherCategoryService;
        }
        #endregion
        public async Task CreatePricingOutwardAsync(string id)
        {
            var test = await _infoCalcPriceStockOutService.GetQueryableAsync();
            List<InfoCalcPriceStockOut> infoCalcPriceStockOut = new List<InfoCalcPriceStockOut>();

            infoCalcPriceStockOut = await _infoCalcPriceStockOutService.GetByInfoCalcPriceStockOutIdAsync(id);
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();

            if (infoCalcPriceStockOut != null)

            {
                for (int i = 0; i < infoCalcPriceStockOut.Count; i++)
                {
                    var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == infoCalcPriceStockOut[i].OrgCode).ToList();
                    List<InfoCalcPriceStockOutDetail> infoCalcPriceStockOutDetailDto = await _infoCalcPriceStockOutDetailService.GetByInfoCalcPriceStockOutAsync(infoCalcPriceStockOut[i].Id);
                    for (int j = 0; j < infoCalcPriceStockOutDetailDto.Count; j++)
                    {
                        //  dto.ProductCode = infoCalcPriceStockOutDetailDto[j].ProductCode;
                        infoCalcPriceStockOutDetailDto[j].Status = ExcutionStatus.Excuting;
                        infoCalcPriceStockOutDetailDto[j].BeginDate = DateTime.Now;
                        var lstdiscountPrice = infoCalcPriceStockOutDetailDto.Select(p => ObjectMapper.Map<InfoCalcPriceStockOutDetail, CrudInfoCalcPriceStockOutDetailDto>(p)).ToList();
                        //await _infoCalcPriceStockOutAppService.UpdateAsync(infoCalcPriceStockOutDetailDto[j].Id, lstdiscountPrice[j]);

                        CostOfGoodsDto crs = new CostOfGoodsDto();
                        crs.ProductCode = infoCalcPriceStockOutDetailDto[j].ProductCode;
                        crs.FromDate = (DateTime)infoCalcPriceStockOut[i].FromDate;
                        crs.ToDate = (DateTime)infoCalcPriceStockOut[i].ToDate;
                        crs.WareHouseCode = infoCalcPriceStockOut[i].WarehouseCose;
                        crs.ProductionPeriodCode = infoCalcPriceStockOut[i].ProductionPeriodCode;
                        crs.ProductLotCode = infoCalcPriceStockOut[i].ProductLotCode;
                        crs.Type = infoCalcPriceStockOut[i].CalculatingMethod;
                        crs.Year = infoCalcPriceStockOut[i].Year;
                        crs.OrdCode = infoCalcPriceStockOut[i].OrgCode;
                        try
                        {
                            await _costOfGoodsAppService.CreateCostOfGoodsAsync(crs);
                            infoCalcPriceStockOutDetailDto[j].IsError = true;
                            infoCalcPriceStockOutDetailDto[j].Status = ExcutionStatus.Ended;
                        }
                        catch (Exception ex)
                        {
                            infoCalcPriceStockOutDetailDto[j].ErrorMsg = ex.ToString();
                            infoCalcPriceStockOutDetailDto[j].IsError = false;
                            infoCalcPriceStockOutDetailDto[j].Status = ExcutionStatus.Error;

                        }

                        infoCalcPriceStockOutDetailDto[j].EndDate = DateTime.Now;
                        var lstdiscountPrices = infoCalcPriceStockOutDetailDto.Select(p => ObjectMapper.Map<InfoCalcPriceStockOutDetail, CrudInfoCalcPriceStockOutDetailDto>(p)).ToList();
                        //await _infoCalcPriceStockOutAppService.UpdateAsync(infoCalcPriceStockOutDetailDto[j].Id, lstdiscountPrices[j]);
                    }
                    //if (lstVoucherCategory.Count > 0)
                    //{
                    //    // Update DmCt Set Tinh_gia = Iif(@p_Type = 3, 'F', 'B') Where Dvcs_id = @p_Dvcs_id And CharIndex(Tinh_Gia,'B,F')> 0
                    //    var lstVoucherCategorys = lstVoucherCategory.Where(p => p.PriceCalculatingMethod.Contains("B,F") == true);
                    //    foreach (var item in lstVoucherCategorys)
                    //    {
                    //        if (infoCalcPriceStockOut[i].CalculatingMethod == "3")
                    //        {
                    //            item.PriceCalculatingMethod = "F";
                    //        }
                    //        else
                    //        {
                    //            item.PriceCalculatingMethod = "B";
                    //        }
                    //        await _voucherCategoryService.UpdateAsync(item);
                    //    }
                    //}
                    CostOfGoodsDto cost = new CostOfGoodsDto();
                    cost.FromDate = (DateTime)infoCalcPriceStockOut[i].FromDate;
                    cost.ToDate = (DateTime)infoCalcPriceStockOut[i].ToDate;
                    cost.OrdCode = infoCalcPriceStockOut[i].OrgCode;
                    await _costOfGoodsAppService.UpadateProductNorms(cost);
                }

            }


        }
    }

}