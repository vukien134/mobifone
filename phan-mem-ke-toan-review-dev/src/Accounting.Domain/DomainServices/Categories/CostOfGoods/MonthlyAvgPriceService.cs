using Accounting.Categories.Products;
using Accounting.Catgories.Others.CostOfGoods;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.Categories.CostOfGoods
{

    public class MonthlyAvgPriceService : DomainService
    {
        #region Fields
        private readonly ProductLotService _productLotService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly ProductGroupService _productGroupService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly ProductService _productService;

        #endregion
        #region Ctor
        public MonthlyAvgPriceService(ProductLotService productLotService,
                            UserService userService,
                            WebHelper webHelper,
                            VoucherCategoryService voucherCategoryService,
                            TenantSettingService tenantSettingService,
                            ProductGroupService productGroupService,
                            WarehouseBookService warehouseBookService,
                            ProductService productService) : base()
        {
            _productLotService = productLotService;
            _userService = userService;
            _webHelper = webHelper;
            _voucherCategoryService = voucherCategoryService;
            _tenantSettingService = tenantSettingService;
            _productGroupService = productGroupService;
            _warehouseBookService = warehouseBookService;
            _productService = productService;
        }
        #endregion
        //private async Task<CostOfGoodsDto> MonthlyAvgPrice(CostOfGoodsDto dto, string AmountTl)
        //{

        //    var product = "";
        //    List<ProductGroup> productGroups = new List<ProductGroup>();
        //    if (string.IsNullOrEmpty(dto.ProductGroup) == true)
        //    {


        //        var productGroup = _productGroupService.GetByProductGroupParnerAsync(dto.ProductGroup.ToString(), _webHelper.GetCurrentOrgUnit());
        //        try
        //        {
        //            productGroups = await productGroup;
        //        }
        //        catch (System.Exception ex)
        //        {

        //            throw;
        //        }
        //        var idProductGroup = productGroups[0].Id;
        //        var numberProductGroup = productGroups[0].Id + "\\";
        //        if (productGroups.Count() > 0)
        //        {

        //            var ProductGroupId = _productGroupService.GetByProductGroupAsync(idProductGroup);
        //            List<ProductGroup> productGroups2 = await ProductGroupId;
        //            for (int i = 0; i < productGroups2.Count(); i++)
        //            {
        //                idProductGroup = productGroups2[i].Id;
        //                if (product == "")
        //                {
        //                    product = numberProductGroup + productGroups2[i].Id + "\\";
        //                }
        //                else
        //                {
        //                    product = productGroups2[i].Id + "\\" + product;
        //                }
        //                productGroups.Add(productGroups2[i]);
        //                var stt = "";
        //                while (stt != "0")
        //                {

        //                    var parnerGroupIds = _productGroupService.GetByProductGroupAsync(idProductGroup);
        //                    List<ProductGroup> parnerGroups2s = await parnerGroupIds;
        //                    stt = parnerGroups2s.Count().ToString();
        //                    if (parnerGroups2s.Count > 0)
        //                    {
        //                        for (int j = 0; j < parnerGroups2s.Count(); j++)
        //                        {
        //                            idProductGroup = parnerGroups2s[j].Id;
        //                            product = parnerGroups2s[j].Id + "\\" + product;
        //                            productGroups.Add(parnerGroups2s[j]);
        //                        }

        //                    }

        //                }


        //            }


        //        }

        //    }



        //    var wareHoseBook = await _warehouseBookService.GetQueryableAsync();
        //    wareHoseBook = wareHoseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
        //                                           && p.WarehouseCode == dto.WareHouse || p.WarehouseCode != ""
        //                                           && p.ProductLotCode == dto.ProductLotCode || p.ProductLotCode != ""
        //                                           && p.ProductCode == dto.ProductCode || p.ProductCode != ""
        //                                           && p.Year == _webHelper.GetCurrentYear()
        //    );
        //    var products = await _productService.GetQueryableAsync();

        //    return dto;
        //}

        public async Task MonthlyAvgPrices(CostOfGoodsDto dto, string v)
        {
            var product = "";
            List<ProductGroup> productGroups = new List<ProductGroup>();
            if (string.IsNullOrEmpty(dto.ProductGroup) == false)
            {


                var productGroup = _productGroupService.GetByProductGroupParnerAsync(dto.ProductGroup.ToString(), "VP");

                productGroups = await productGroup;

                var idProductGroup = productGroups[0].Id;
                var numberProductGroup = productGroups[0].Id + "\\";
                if (productGroups.Count() > 0)
                {

                    var ProductGroupId = _productGroupService.GetByProductGroupAsync(idProductGroup);
                    List<ProductGroup> productGroups2 = await ProductGroupId;
                    for (int i = 0; i < productGroups2.Count(); i++)
                    {
                        idProductGroup = productGroups2[i].Id;
                        if (product == "")
                        {
                            product = numberProductGroup + productGroups2[i].Id + "\\";
                        }
                        else
                        {
                            product = productGroups2[i].Id + "\\" + product;
                        }
                        productGroups.Add(productGroups2[i]);
                        var stt = "";
                        while (stt != "0")
                        {

                            var parnerGroupIds = _productGroupService.GetByProductGroupAsync(idProductGroup);
                            List<ProductGroup> parnerGroups2s = await parnerGroupIds;
                            stt = parnerGroups2s.Count().ToString();
                            if (parnerGroups2s.Count > 0)
                            {
                                for (int j = 0; j < parnerGroups2s.Count(); j++)
                                {
                                    idProductGroup = parnerGroups2s[j].Id;
                                    product = parnerGroups2s[j].Id + "\\" + product;
                                    productGroups.Add(parnerGroups2s[j]);
                                }

                            }

                        }


                    }


                }



            }


            try
            {
                var wareHoseBook = await _warehouseBookService.GetQueryableAsync();
                wareHoseBook = wareHoseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                       && p.WarehouseCode == dto.WareHouseCode || p.WarehouseCode != ""
                                                       && p.ProductLotCode == dto.ProductLotCode || p.ProductLotCode != ""
                                                       && p.ProductCode == dto.ProductCode || p.ProductCode != ""
                                                       && p.Year == _webHelper.GetCurrentYear()
                );
                var Products = _productService.GetByProductAsync(_webHelper.GetCurrentOrgUnit());
                List<Product> productList = await Products;

                var productgroup = from p in productList
                                   join c in productGroups on p.ProductGroupId equals c.Id
                                   select p;
                var productgroups = productgroup.AsQueryable();
                var resul = from w in wareHoseBook
                            join p in productgroups on w.ProductCode equals p.Code into aob
                            from p in aob.DefaultIfEmpty()
                            group new { w, p } by new
                            {
                                w.WarehouseCode,
                                w.ProductLotCode,
                                w.ProductCode,
                                w.ProductOriginCode
                            } into s
                            select new
                            {
                                WarehouseCode = s.Key.WarehouseCode,
                                ProductLotCode = s.Key.ProductLotCode,
                                ProductCode = s.Key.ProductCode,
                                ProductOriginCode = s.Key.ProductOriginCode,
                                Quantity = s.Sum(p => p.w.ImportQuantity - p.w.ExportQuantity),
                                AmountCur = s.Sum(p => p.w.ImportAmountCur - p.w.ExportAmountCur),
                                Amount = s.Sum(p => p.w.ImportAmount - p.w.ExportAmount)
                            };
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }
    }
}
