using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.ImportExports.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;

namespace Accounting.Reports.ImportExports
{
    public class InventorySummaryBookAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly ProductGroupAppService _productGroupAppService;
        private readonly ProductGroupService _productGroupService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _productService;
        private readonly ProductLotService _productLotService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public InventorySummaryBookAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        ProductGroupAppService productGroupAppService,
                        ProductGroupService productGroupService,
                        WarehouseService warehouseService,
                        ProductService productService,
                        ProductLotService productLotService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager,
                        IStringLocalizer<AccountingResource> localizer
                        )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _productGroupAppService = productGroupAppService;
            _productGroupService = productGroupService;
            _warehouseService = warehouseService;
            _productService = productService;
            _productLotService = productLotService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
            _localizer = localizer;
        }
        #endregion
        #region Methods
        
        public async Task<ReportResponseDto<InventorySummaryBookDto>> CreateDataAsync(ReportRequestDto<InventorySummaryBookParameterDto> dto)
        {
            await this.CheckPermission(dto.ReportTemplateCode, ReportPermissions.ActionView);
            string orgCode = _webHelper.GetCurrentOrgUnit();
            if (dto.Parameters.Type == "DK")
            {
                dto.Parameters.FromDate = dto.Parameters.ToDate;
            }
            var dic = GetWarehouseBookParameter(dto.Parameters);
            var lst = new List<InventorySummaryBookDto>();
            var iEInventoryData = await GetIEInventoryData(dic);
            var lstRankProduct = await _productGroupAppService.GetRankGroup(dto.Parameters.ProductGroupCode ?? "");
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && lstRankProduct.Select(a => a.Id).Contains(p.ProductGroupId)).ToList();
            var productLot = await _productLotService.GetQueryableAsync();
            var lstProductLot = productLot.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var accSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccSystem = accSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            var warehouse = await _warehouseService.GetQueryableAsync();
            var lstWarehouse = warehouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            foreach (var itemIEInventory in iEInventoryData)
            {
                if (dto.Parameters.CheckWarehouse == false) itemIEInventory.WarehouseCode = "";
                if (dto.Parameters.CheckAcc == false) itemIEInventory.AccCode = "";
            }
            var inventorySummaryBookData0 = (from a in iEInventoryData
                                             join b in lstProduct on a.ProductCode equals b.Code
                                             join c in lstRankProduct on new { ProductGroupId = b?.ProductGroupId ?? "" } equals new { ProductGroupId = c.Id } into bjc
                                             from c in bjc.DefaultIfEmpty()
                                             group new { a, b, c } by new
                                             {
                                                 a.OrgCode,
                                                 a.AccCode,
                                                 a.WarehouseCode,
                                                 Rank = c?.Rank ?? 0,
                                                 OrdGroup = c?.OrdGroup ?? "",
                                                 a.ProductCode,
                                                 ProductName = b?.Name ?? "",
                                                 UnitCode = b?.UnitCode ?? "",
                                                 a.ProductOriginCode,
                                                 a.ProductLotCode,
                                                 Specification = b?.Specification ?? ""

                                             } into gr
                                             select new InventorySummaryBookDto
                                             {
                                                 OrgCode = gr.Key.OrgCode,
                                                 Acc = gr.Key.AccCode,
                                                 WarehouseCode = gr.Key.WarehouseCode,
                                                 RankProductGroup = gr.Key.Rank + 2,
                                                 OrdGroup = gr.Key.OrdGroup,
                                                 ProductCode = gr.Key.ProductCode,
                                                 ProductOriginCode = gr.Key.ProductOriginCode,
                                                 ProductLotCode = gr.Key.ProductLotCode,
                                                 ProductName = gr.Key.ProductName,
                                                 UnitCode = gr.Key.UnitCode,
                                                 Bold = "K",
                                                 ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1),
                                                 ImportAmount1 = gr.Sum(p => p.a.ImportAmount1),
                                                 ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1),
                                                 ImportPrice1 = (gr.Sum(p => p.a.ImportQuantity1) != 0) ? gr.Sum(p => p.a.ImportAmount1) / gr.Sum(p => p.a.ImportQuantity1) : null,
                                                 ImportQuantity = gr.Sum(p => p.a.ImportQuantity),
                                                 ImportAmount = gr.Sum(p => p.a.ImportAmount),
                                                 ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur),
                                                 ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                                 ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                                 ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur),
                                                 ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2),
                                                 ImportAmount2 = gr.Sum(p => p.a.ImportAmount2),
                                                 ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2),
                                                 ImportPrice2 = (gr.Sum(p => p.a.ImportQuantity2) != 0) ? gr.Sum(p => p.a.ImportAmount2) / gr.Sum(p => p.a.ImportQuantity2) : null,
                                                 Amount2 = gr.Sum(p => p.a.Amount2),
                                                 AmountCur2 = gr.Sum(p => p.a.AmountCur2),
                                                 Specification = gr.Key.Specification
                                             }).ToList();
            var inventorySummaryBookData = (from a in inventorySummaryBookData0
                                            group new { a } by new
                                            {
                                                a.OrgCode,
                                                a.Acc,
                                                a.WarehouseCode,
                                                a.RankProductGroup,
                                                a.OrdGroup,
                                                a.ProductCode,
                                                a.ProductName,
                                                a.UnitCode,
                                                a.Specification

                                            } into gr
                                            select new InventorySummaryBookDto
                                            {
                                                OrgCode = gr.Key.OrgCode,
                                                Acc = gr.Key.Acc,
                                                WarehouseCode = gr.Key.WarehouseCode,
                                                RankProductGroup = gr.Key.RankProductGroup - 1,
                                                OrdGroup = gr.Key.OrdGroup + @"00\",
                                                ProductCode = gr.Key.ProductCode,
                                                ProductOriginCode = "",
                                                ProductLotCode = "",
                                                ProductName = gr.Key.ProductName,
                                                UnitCode = gr.Key.UnitCode,
                                                Bold = "K",
                                                ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1),
                                                ImportAmount1 = gr.Sum(p => p.a.ImportAmount1),
                                                ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1),
                                                ImportPrice1 = gr.Max(p => p.a.ImportPrice1),
                                                ImportQuantity = gr.Sum(p => p.a.ImportQuantity),
                                                ImportAmount = gr.Sum(p => p.a.ImportAmount),
                                                ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur),
                                                ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                                ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                                ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur),
                                                ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2),
                                                ImportAmount2 = gr.Sum(p => p.a.ImportAmount2),
                                                ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2),
                                                ImportPrice2 = gr.Max(p => p.a.ImportPrice2),
                                                Amount2 = gr.Sum(p => p.a.Amount2),
                                                AmountCur2 = gr.Sum(p => p.a.AmountCur2),
                                                Specification = gr.Key.Specification
                                            }).ToList();

            foreach (var itemInventorySummaryBook in inventorySummaryBookData0)
            {
                if (dto.Parameters.CheckProductOrigin == false) itemInventorySummaryBook.ProductOriginCode = "";
                if (dto.Parameters.CheckProductLot == false) itemInventorySummaryBook.ProductLotCode = "";
            }

            if (dto.Parameters.CheckProductOrigin == true || dto.Parameters.CheckProductLot == true)
            {
                inventorySummaryBookData.AddRange((from a in inventorySummaryBookData0
                                                   where (a.ProductOriginCode ?? "") != "" || (a.ProductLotCode ?? "") != ""
                                                   group new { a } by new
                                                   {
                                                       a.OrgCode,
                                                       a.Acc,
                                                       a.WarehouseCode,
                                                       a.RankProductGroup,
                                                       a.OrdGroup,
                                                       a.ProductCode,
                                                       a.ProductName,
                                                       a.UnitCode,
                                                       a.ProductOriginCode,
                                                       a.ProductLotCode,
                                                       a.Specification

                                                   } into gr
                                                   select new InventorySummaryBookDto
                                                   {
                                                       OrgCode = gr.Key.OrgCode,
                                                       Acc = gr.Key.Acc,
                                                       WarehouseCode = gr.Key.WarehouseCode,
                                                       RankProductGroup = gr.Key.RankProductGroup,
                                                       OrdGroup = gr.Key.OrdGroup + @"00\",
                                                       ProductCode = gr.Key.ProductCode,
                                                       ProductOriginCode = gr.Key.ProductOriginCode,
                                                       ProductLotCode = gr.Key.ProductLotCode,
                                                       ProductName = gr.Key.ProductName,
                                                       UnitCode = gr.Key.UnitCode,
                                                       Bold = "K",
                                                       ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1),
                                                       ImportAmount1 = gr.Sum(p => p.a.ImportAmount1),
                                                       ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1),
                                                       ImportPrice1 = gr.Max(p => p.a.ImportPrice1),
                                                       ImportQuantity = gr.Sum(p => p.a.ImportQuantity),
                                                       ImportAmount = gr.Sum(p => p.a.ImportAmount),
                                                       ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur),
                                                       ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                                       ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                                       ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur),
                                                       ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2),
                                                       ImportAmount2 = gr.Sum(p => p.a.ImportAmount2),
                                                       ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2),
                                                       ImportPrice2 = gr.Max(p => p.a.ImportPrice2),
                                                       Amount2 = gr.Sum(p => p.a.Amount2),
                                                       AmountCur2 = gr.Sum(p => p.a.AmountCur2),
                                                       Specification = gr.Key.Specification
                                                   }).ToList());
            }

            //data_NXT
            var dataIEI = (from a in inventorySummaryBookData0
                           join b in lstProduct on a.ProductCode equals b.Code
                           join c in lstRankProduct on b.ProductGroupId equals c.Id
                           where (a.ProductOriginCode ?? "") == "" && (a.ProductLotCode ?? "") == ""
                           group new { a, b, c } by new
                           {
                               a.OrgCode,
                               a.Acc,
                               a.WarehouseCode,
                               RankProductGroup = c.Rank,
                               a.OrdGroup,
                               ProductGroupCode = c.Code,
                               ProductGroupName = c.Name
                           } into gr
                           select new InventorySummaryBookDto
                           {
                               OrgCode = gr.Key.OrgCode,
                               Acc = gr.Key.Acc,
                               WarehouseCode = gr.Key.WarehouseCode,
                               RankProductGroup = gr.Key.RankProductGroup,
                               OrdGroup = gr.Key.OrdGroup,
                               ProductCode = gr.Key.ProductGroupCode,
                               ProductGroupCode = gr.Key.ProductGroupCode,
                               ProductOriginCode = "",
                               ProductLotCode = "",
                               ProductName = gr.Key.ProductGroupName,
                               UnitCode = "",
                               Bold = "C",
                               ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1 ?? 0),
                               ImportAmount1 = gr.Sum(p => p.a.ImportAmount1 ?? 0),
                               ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1 ?? 0),
                               ImportPrice1 = gr.Max(p => p.a.ImportPrice1 ?? 0),
                               ImportQuantity = gr.Sum(p => p.a.ImportQuantity ?? 0),
                               ImportAmount = gr.Sum(p => p.a.ImportAmount ?? 0),
                               ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur ?? 0),
                               ExportQuantity = gr.Sum(p => p.a.ExportQuantity ?? 0),
                               ExportAmount = gr.Sum(p => p.a.ExportAmount ?? 0),
                               ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur ?? 0),
                               ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2 ?? 0),
                               ImportAmount2 = gr.Sum(p => p.a.ImportAmount2 ?? 0),
                               ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2 ?? 0),
                               ImportPrice2 = gr.Max(p => p.a.ImportPrice2 ?? 0),
                               Amount2 = gr.Sum(p => p.a.Amount2 ?? 0),
                               AmountCur2 = gr.Sum(p => p.a.AmountCur2 ?? 0)
                           }).ToList();
            // theo kho
            if (dto.Parameters.CheckWarehouse == true)
            {
                inventorySummaryBookData.AddRange((from a in inventorySummaryBookData
                                                   join b in lstWarehouse on a.WarehouseCode equals b.Code into ajb
                                                   from b in ajb.DefaultIfEmpty()
                                                   group new { a, b } by new
                                                   {
                                                       a.OrgCode,
                                                       a.Acc,
                                                       a.WarehouseCode,
                                                       WarehouseName = b?.Name ?? ""

                                                   } into gr
                                                   select new InventorySummaryBookDto
                                                   {
                                                       OrgCode = gr.Key.OrgCode,
                                                       Acc = gr.Key.Acc,
                                                       WarehouseCode = gr.Key.WarehouseCode,
                                                       RankProductGroup = 0,
                                                       OrdGroup = "",
                                                       ProductCode = gr.Key.WarehouseCode,
                                                       ProductOriginCode = "",
                                                       ProductLotCode = "",
                                                       ProductName = gr.Key.WarehouseName,
                                                       UnitCode = "",
                                                       Bold = "C",
                                                       ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1),
                                                       ImportAmount1 = gr.Sum(p => p.a.ImportAmount1),
                                                       ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1),
                                                       ImportQuantity = gr.Sum(p => p.a.ImportQuantity),
                                                       ImportAmount = gr.Sum(p => p.a.ImportAmount),
                                                       ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur),
                                                       ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                                       ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                                       ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur),
                                                       ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2),
                                                       ImportAmount2 = gr.Sum(p => p.a.ImportAmount2),
                                                       ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2),
                                                       Amount2 = gr.Sum(p => p.a.Amount2),
                                                       AmountCur2 = gr.Sum(p => p.a.AmountCur2)
                                                   }).ToList());
            }
            // Theo tài khoản
            if (dto.Parameters.CheckAcc == true)
            {
                inventorySummaryBookData.AddRange((from a in inventorySummaryBookData
                                                   join b in lstAccSystem on a.Acc equals b.AccCode into ajb
                                                   from b in ajb.DefaultIfEmpty()
                                                   group new { a, b } by new
                                                   {
                                                       a.OrgCode,
                                                       a.Acc,
                                                       AccName = b?.AccName ?? ""

                                                   } into gr
                                                   select new InventorySummaryBookDto
                                                   {
                                                       OrgCode = gr.Key.OrgCode,
                                                       Acc = gr.Key.Acc,
                                                       WarehouseCode = "",
                                                       RankProductGroup = 0,
                                                       OrdGroup = "",
                                                       ProductCode = gr.Key.Acc,
                                                       ProductOriginCode = "",
                                                       ProductLotCode = "",
                                                       ProductName = gr.Key.AccName,
                                                       UnitCode = "",
                                                       Bold = "C",
                                                       ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1),
                                                       ImportAmount1 = gr.Sum(p => p.a.ImportAmount1),
                                                       ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1),
                                                       ImportQuantity = gr.Sum(p => p.a.ImportQuantity),
                                                       ImportAmount = gr.Sum(p => p.a.ImportAmount),
                                                       ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur),
                                                       ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                                       ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                                       ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur),
                                                       ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2),
                                                       ImportAmount2 = gr.Sum(p => p.a.ImportAmount2),
                                                       ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2),
                                                       Amount2 = gr.Sum(p => p.a.Amount2),
                                                       AmountCur2 = gr.Sum(p => p.a.AmountCur2)
                                                   }).ToList());
            }
            if (dto.Parameters.Type == "DK")
            {
                inventorySummaryBookData = inventorySummaryBookData.Where(p => (p.ImportQuantity1 ?? 0) != 0 && (p.ImportAmount1 ?? 0) != 0).ToList();
            }
            else if (dto.Parameters.Type == "CK")
            {
                inventorySummaryBookData = inventorySummaryBookData.Where(p => (p.ImportQuantity2 ?? 0) != 0 && (p.ImportAmount2 ?? 0) != 0).ToList();
            }
            // tạo cây
            int rankMin = 1;
            int rankMax = (dataIEI.Count > 0) ? dataIEI.Max(p => p.RankProductGroup + 1) : 0;
            string warehouseCodeMax = dataIEI.Max(p => p.WarehouseCode);
            string accMax = dataIEI.Max(p => p.Acc);
            while (rankMax > rankMin)
            {
                foreach (var itemIEI in dataIEI)
                {
                    if (itemIEI.RankProductGroup == rankMax)
                    {
                        var productGroupId = lstRankProduct.Where(p => p.Code == itemIEI.ProductGroupCode)
                                                                 .Select(p => p.ParentId).FirstOrDefault();
                        itemIEI.ProductGroupCode = lstRankProduct.Where(p => p.Id == productGroupId).Select(p => p.Code).FirstOrDefault();
                        itemIEI.RankProductGroup--;
                    }
                }
                var itemIEIAdd = (from a in dataIEI
                                  join b in lstRankProduct on new { ProductGroupCode = a?.ProductGroupCode ?? "" } equals new { ProductGroupCode = b.Code }
                                  where a.RankProductGroup == rankMax - 1
                                  group new { a, b } by new
                                  {
                                      OrgCode = a.OrgCode ?? "",
                                      Acc = a.Acc ?? "",
                                      WarehouseCode = a.WarehouseCode ?? "",
                                      a.RankProductGroup,
                                      a.OrdGroup,
                                      ProductGroupCode = b.Code ?? "",
                                      ProductGroupName = b.Name ?? "",
                                  } into gr
                                  select new InventorySummaryBookDto
                                  {
                                      Bold = "C",
                                      OrgCode = gr.Key.OrgCode,
                                      Acc = gr.Key.Acc,
                                      ProductCode = gr.Key.ProductGroupCode,
                                      OrdGroup = gr.Key.OrdGroup,
                                      RankProductGroup = gr.Key.RankProductGroup,
                                      ProductGroupCode = gr.Key.ProductGroupCode,
                                      ProductName = gr.Key.ProductGroupName,
                                      ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1 ?? 0),
                                      ImportAmount1 = gr.Sum(p => p.a.ImportAmount1 ?? 0),
                                      ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1 ?? 0),
                                      ImportQuantity = gr.Sum(p => p.a.ImportQuantity ?? 0),
                                      ImportAmount = gr.Sum(p => p.a.ImportAmount ?? 0),
                                      ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur ?? 0),
                                      ExportQuantity = gr.Sum(p => p.a.ExportQuantity ?? 0),
                                      ExportAmount = gr.Sum(p => p.a.ExportAmount ?? 0),
                                      ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur ?? 0),
                                      ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2 ?? 0),
                                      ImportAmount2 = gr.Sum(p => p.a.ImportAmount2 ?? 0),
                                      ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2 ?? 0),
                                      Amount2 = gr.Sum(p => p.a.Amount2 ?? 0),
                                      AmountCur2 = gr.Sum(p => p.a.AmountCur2 ?? 0),
                                      WarehouseCode = gr.Key.WarehouseCode,
                                  }).ToList();
                inventorySummaryBookData = Enumerable.Concat(inventorySummaryBookData, itemIEIAdd).ToList();
                rankMax--;
            }
            inventorySummaryBookData = inventorySummaryBookData.Where(p => Math.Abs(p.ImportQuantity1 ?? 0) + Math.Abs(p.ImportAmount1 ?? 0) + Math.Abs(p.ImportQuantity ?? 0) + Math.Abs(p.ImportAmount ?? 0) + Math.Abs(p.ExportQuantity ?? 0) + Math.Abs(p.ExportAmount ?? 0) != 0).ToList();
            foreach (var item in inventorySummaryBookData)
            {
                item.RankProductGroup = item.RankProductGroup + 1;
                var space = "";
                for (var i = 1; i < item.RankProductGroup; i++)
                {
                    space += "  ";
                }
                item.ProductName = space + item.ProductName;

                item.ImportQuantity1 = (item.ImportQuantity1 == 0) ? null : item.ImportQuantity1;
                item.ImportAmount1 = (item.ImportAmount1 == 0) ? null : item.ImportAmount1;
                item.ImportAmountCur1 = (item.ImportAmountCur1 == 0) ? null : item.ImportAmountCur1;
                item.ImportQuantity = (item.ImportQuantity == 0) ? null : item.ImportQuantity;
                item.ImportAmount = (item.ImportAmount == 0) ? null : item.ImportAmount;
                item.ImportAmountCur = (item.ImportAmountCur == 0) ? null : item.ImportAmountCur;
                item.ExportQuantity = (item.ExportQuantity == 0) ? null : item.ExportQuantity;
                item.ExportAmount = (item.ExportAmount == 0) ? null : item.ExportAmount;
                item.ExportAmountCur = (item.ExportAmountCur == 0) ? null : item.ExportAmountCur;
                item.ImportQuantity2 = (item.ImportQuantity2 == 0) ? null : item.ImportQuantity2;
                item.ImportAmount2 = (item.ImportAmount2 == 0) ? null : item.ImportAmount2;
                item.Amount2 = (item.Amount2 == 0) ? null : item.Amount2;
                item.AmountCur2 = (item.AmountCur2 == 0) ? null : item.AmountCur2;
            }

            inventorySummaryBookData.AddRange((from a in dataIEI
                                               group new { a } by new
                                               {
                                                   a.OrgCode

                                               } into gr
                                               select new InventorySummaryBookDto
                                               {
                                                   OrgCode = gr.Key.OrgCode,
                                                   Acc = accMax,
                                                   WarehouseCode = warehouseCodeMax,
                                                   RankProductGroup = 99,
                                                   OrdGroup = "Z",
                                                   ProductCode = "",
                                                   ProductName = "TỔNG CỘNG",
                                                   ProductOriginCode = "",
                                                   ProductLotCode = "",
                                                   UnitCode = "",
                                                   Bold = "C",
                                                   ImportQuantity1 = gr.Sum(p => p.a.ImportQuantity1 ?? 0),
                                                   ImportAmount1 = gr.Sum(p => p.a.ImportAmount1 ?? 0),
                                                   ImportAmountCur1 = gr.Sum(p => p.a.ImportAmountCur1 ?? 0),
                                                   ImportPrice1 = gr.Max(p => p.a.ImportPrice1 ?? 0),
                                                   ImportQuantity = gr.Sum(p => p.a.ImportQuantity ?? 0),
                                                   ImportAmount = gr.Sum(p => p.a.ImportAmount ?? 0),
                                                   ImportAmountCur = gr.Sum(p => p.a.ImportAmountCur ?? 0),
                                                   ExportQuantity = gr.Sum(p => p.a.ExportQuantity ?? 0),
                                                   ExportAmount = gr.Sum(p => p.a.ExportAmount ?? 0),
                                                   ExportAmountCur = gr.Sum(p => p.a.ExportAmountCur ?? 0),
                                                   ImportQuantity2 = gr.Sum(p => p.a.ImportQuantity2 ?? 0),
                                                   ImportAmount2 = gr.Sum(p => p.a.ImportAmount2 ?? 0),
                                                   ImportAmountCur2 = gr.Sum(p => p.a.ImportAmountCur2 ?? 0),
                                                   ImportPrice2 = gr.Max(p => p.a.ImportPrice2 ?? 0),
                                                   Amount2 = gr.Sum(p => p.a.Amount2),
                                                   AmountCur2 = gr.Sum(p => p.a.AmountCur2)
                                               }).ToList());
            lst = (from a in inventorySummaryBookData
                   join b in lstProductLot on a.ProductLotCode equals b.Code into ajb
                   from b in ajb.DefaultIfEmpty()
                   join c in lstProduct on a.ProductCode equals c.Code into bjc
                   from c in bjc.DefaultIfEmpty()
                   orderby a.Acc, a.WarehouseCode, a.OrdGroup, a.ProductCode, a.RankProductGroup, a.ProductLotCode, a.ProductOriginCode
                   select new InventorySummaryBookDto
                   {
                       OrgCode = a.OrgCode,
                       Acc = a.Acc,
                       WarehouseCode = a.WarehouseCode,
                       RankProductGroup = a.RankProductGroup,
                       OrdGroup = a.OrdGroup,
                       ProductCode = a.ProductCode,
                       ProductOriginCode = a.ProductOriginCode,
                       ProductLotCode = a.ProductLotCode,
                       ProductName = a.ProductName,
                       UnitCode = a.UnitCode,
                       Bold = a.Bold,
                       ImportQuantity1 = a.ImportQuantity1,
                       ImportAmount1 = a.ImportAmount1,
                       ImportAmountCur1 = a.ImportAmountCur1,
                       ImportPrice1 = a.ImportPrice1,
                       ImportQuantity = a.ImportQuantity,
                       ImportAmount = a.ImportAmount,
                       ImportAmountCur = a.ImportAmountCur,
                       ExportQuantity = a.ExportQuantity,
                       ExportAmount = a.ExportAmount,
                       ExportAmountCur = a.ExportAmountCur,
                       ImportQuantity2 = a.ImportQuantity2,
                       ImportAmount2 = a.ImportAmount2,
                       ImportAmountCur2 = a.ImportAmountCur2,
                       ImportPrice2 = a.ImportPrice2,
                       Amount2 = a.Amount2,
                       AmountCur2 = a.AmountCur2,
                       AvgBInventoryPrice = (a.ImportQuantity1 != 0) ? a.ImportAmount1 / a.ImportQuantity1 : 0,
                       AvgImportPrice = (a.ImportQuantity != 0) ? a.ImportAmount / a.ImportQuantity : 0,
                       AvgExportPrice = (a.ExportQuantity != 0) ? a.ExportAmount / a.ExportQuantity : 0,
                       AvgEInventoryPrice = (a.ImportQuantity2 != 0) ? a.ImportAmount2 / a.ImportQuantity2 : 0,
                       FromDate = dto.Parameters.FromDate,
                       ToDate = dto.Parameters.ToDate,
                       ExpiryDate = b?.ExpireDate ?? null,
                       QuantityMin = c?.MinQuantity ?? null,
                       QuantityMax = c?.MaxQuantity ?? null,
                       OverdueDate = (DateTime.Today > (b?.ExpireDate ?? DateTime.Today)) ? (int)((DateTime.Today - (b?.ExpireDate ?? DateTime.Today)).TotalDays) : 0,
                       ComingDate = (DateTime.Today < (b?.ExpireDate ?? DateTime.Today)) ? (int)(((b?.ExpireDate ?? DateTime.Today) - DateTime.Today).TotalDays) : 0,
                   }).ToList();
            var reportResponse = new ReportResponseDto<InventorySummaryBookDto>();
            reportResponse.Data = lst.OrderBy(p => p.Acc).ThenBy(p => p.WarehouseCode)
                                                         .ThenBy(p => p.OrdGroup).ThenBy(p => p.RankProductGroup)
                                                         .ThenBy(p => p.ProductLotCode).ThenBy(p => p.ProductOriginCode).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<InventorySummaryBookParameterDto> dto)
        {
            await this.CheckPermission(dto.ReportTemplateCode, ReportPermissions.ActionView);
            var dataSource = await CreateDataAsync(dto);
            var currencyFormats = await _accountingCacheManager.GetCurrencyFormats(_webHelper.GetCurrentOrgUnit());
            var reportTemplate = await _reportTemplateService.GetByCodeAsync(dto.ReportTemplateCode);
            string fileTemplate = reportTemplate.FileTemplate.Replace(".xml", "");
            fileTemplate = fileTemplate + "_" + dto.VndNt;
            if (!File.Exists(GetFileTemplatePath(fileTemplate + ".xml")))
            {
                throw new Exception("Không tìm thấy mẫu in!");
            }
            var renderOption = new RenderOption()
            {
                DataSource = dataSource,
                TypePrint = dto.Type,
                TemplateFile = GetFileTemplatePath(fileTemplate + ".xml"),
                CurrencyFormats = currencyFormats
            };
            var render = new ReportRender(renderOption);
            var result = render.Execute();

            return new FileContentResult(result, MIMETYPE.GetContentType(dto.Type.ToLower()))
            {
                FileDownloadName = $"{fileTemplate}.{dto.Type}"
            };
        }
        #endregion
        #region Private
        protected async Task CheckPermission(string reportCode, string action)
        {
            bool isGrant = await this.IsGrantPermission(reportCode, action);
            if (!isGrant)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
        }
        private async Task<bool> IsGrantPermission(string reportCode, string action)
        {
            string permissionName = $"{reportCode}_{action}";
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            return result.Succeeded;
        }
        private async Task<List<IEInventoryDto>> GetIEInventoryData(Dictionary<string, object> dic)
        {
            var incurredData = await _reportDataService.GetIEInventoryAsync(dic);
            return incurredData;
        }

        private Dictionary<string, object> GetWarehouseBookParameter(InventorySummaryBookParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            dic.Add("isTransfer", dto.CheckTransfer);
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductOriginCode, dto.ProductOriginCode);
            }
            dic.Add(WarehouseBookParameterConst.Year, _webHelper.GetCurrentYear());
            return dic;
        }

        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return ObjectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
        }
        private async Task<dynamic> GetTenantSetting(string orgCode)
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            var tenantSettings = await _tenantSettingService.GetBySettingTypeAsync(orgCode, TenantSettingType.Report);
            foreach (var setting in tenantSettings)
            {
                ((IDictionary<String, Object>)exo).Add(setting.Key, setting.Value);
            }
            return exo;
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return ObjectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        private async Task<string> GetPartnerName(string orgCode, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var partner = await _accPartnerService.GetAccPartnerByCodeAsync(code, orgCode);
            if (partner == null) return null;

            return partner.Name;
        }
        #endregion
    }
}
