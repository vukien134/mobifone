using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.CostOfGoods;
using Accounting.Categories.Others;
using Accounting.Catgories.AccCases;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.CostProductions;
using Accounting.Catgories.Others.CostOfGoods;
using Accounting.Catgories.ProductVouchers;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Excels;
using Accounting.Extensions;
using Accounting.Generals;
using Accounting.Generals.PriceStockOuts;
using Accounting.Helpers;
using Accounting.Jobs.CalcPrices;
using Accounting.Permissions;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.RefVouchers;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Accounting.Categories.CostProductions
{
    public class CostProductionAppService : AccountingAppService, ICostProductionAppService
    {
        #region Fields
        private readonly InfoCalcPriceStockOutAppService _infoCalcPriceStockOutAppService;
        private readonly PricingOutwardAppService _pricingOutwardAppService;
        private readonly ConfigCostPriceService _configCostPriceService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly ProductionPeriodService _productionPeriodService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AllotmentForwardCategoryService _allotmentForwardCategoryService;
        private readonly InfoExportAutoService _infoExportAutoService;
        private readonly InfoZService _infoZService;
        private readonly LedgerService _ledgerService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly AccVoucherService _accVoucherService;
        private readonly AccVoucherAppService _accVoucherAppService;
        private readonly AccVoucherDetailService _accVoucherDetailService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly ProductService _productService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly FProductWorkNormService _fProductWorkNormService;
        private readonly FProductWorkNormDetailService _fProductWorkNormDetailService;
        private readonly GroupCoefficientService _groupCoefficientService;
        private readonly GroupCoefficientDetailService _groupCoefficientDetailService;
        private readonly AccPartnerService _accPartnerService;
        private readonly IRepository<AccSection, string> _accSectionRepository;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly ExcelService _excelService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly CostOfGoodsAppService _costOfGoodsAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;

        #endregion
        #region Ctor
        public CostProductionAppService(InfoCalcPriceStockOutAppService infoCalcPriceStockOutAppService,
                            PricingOutwardAppService pricingOutwardAppService,
                            ConfigCostPriceService configCostPriceService,
                            UserService userService,
                            VoucherCategoryService voucherCategoryService,
                            DefaultTenantSettingService defaultTenantSettingService,
                            ProductionPeriodService productionPeriodService,
                            YearCategoryService yearCategoryService,
                            AllotmentForwardCategoryService allotmentForwardCategoryService,
                            InfoExportAutoService infoExportAutoService,
                            InfoZService infoZService,
                            LedgerService ledgerService,
                            WarehouseBookService warehouseBookService,
                            AccVoucherService accVoucherService,
                            AccVoucherAppService accVoucherAppService,
                            AccVoucherDetailService accVoucherDetailService,
                            VoucherTypeService voucherTypeService,
                            ProductVoucherService productVoucherService,
                            ProductVoucherDetailService productVoucherDetailService,
                            ProductService productService,
                            AccountSystemService accountSystemService,
                            TenantSettingService tenantSettingService,
                            AccOpeningBalanceService accOpeningBalanceService,
                            FProductWorkNormService fProductWorkNormService,
                            FProductWorkNormDetailService fProductWorkNormDetailService,
                            GroupCoefficientService groupCoefficientService,
                            GroupCoefficientDetailService groupCoefficientDetailService,
                            AccPartnerService accPartnerService,
                            IRepository<AccSection, string> accSectionRepository,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            AccountingCacheManager accountingCacheManager,
                            ExcelService excelService, 
                            IBackgroundJobManager backgroundJobManager,
                            CostOfGoodsAppService costOfGoodsAppService,
                            IUnitOfWorkManager unitOfWorkManager,
                            VoucherNumberBusiness voucherNumberBusiness
                            )
        {
            _infoCalcPriceStockOutAppService = infoCalcPriceStockOutAppService;
            _pricingOutwardAppService = pricingOutwardAppService;
            _configCostPriceService = configCostPriceService;
            _voucherCategoryService = voucherCategoryService;
            _defaultTenantSettingService = defaultTenantSettingService;
            _productionPeriodService = productionPeriodService;
            _yearCategoryService = yearCategoryService;
            _allotmentForwardCategoryService = allotmentForwardCategoryService;
            _infoExportAutoService = infoExportAutoService;
            _infoZService = infoZService;
            _ledgerService = ledgerService;
            _warehouseBookService = warehouseBookService;
            _accVoucherService = accVoucherService;
            _accVoucherAppService = accVoucherAppService;
            _accVoucherDetailService = accVoucherDetailService;
            _voucherTypeService = voucherTypeService;
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _productService = productService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _accOpeningBalanceService = accOpeningBalanceService;
            _fProductWorkNormService = fProductWorkNormService;
            _fProductWorkNormDetailService = fProductWorkNormDetailService;
            _groupCoefficientService = groupCoefficientService;
            _groupCoefficientDetailService = groupCoefficientDetailService;
            _accPartnerService = accPartnerService;
            _accSectionRepository = accSectionRepository;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
            _excelService = excelService;
            _backgroundJobManager = backgroundJobManager;
            _costOfGoodsAppService = costOfGoodsAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _voucherNumberBusiness = voucherNumberBusiness;
        }
        #endregion

        // Tính giá thành sản suất
        public async Task<ResultDto> ProductionCostCalculationAsync(ProductionCostCalculationFilterDto filterDto)
        {
            await _licenseBusiness.CheckExpired();
            var yearSystem = _webHelper.GetCurrentYear();
            if (yearSystem != filterDto.FromDate.Year || yearSystem != filterDto.ToDate.Year)
            {
                throw new Exception("Ngày bạn chọn không đúng năm làm việc: " + yearSystem);
            }
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var isBookClosing = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.BookClosingDate >= filterDto.FromDate).Any();
            if (isBookClosing)
            {
                var lstDateBookClosing = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.BookClosingDate >= filterDto.FromDate).Select(p => p.BookClosingDate).ToArray();
                throw new Exception("Hệ thống chứng từ đã có thời gian khóa đến ngày: " + string.Join(",", lstDateBookClosing));
            }
            var configCostPrice = await _configCostPriceService.GetQueryableAsync();
            var dataConfigCostPrice = configCostPrice.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).FirstOrDefault();
            if (dataConfigCostPrice == null)
            {
                dataConfigCostPrice = new ConfigCostPrice();
                dataConfigCostPrice.Fifo = 1;
                dataConfigCostPrice.Type = 1;
                dataConfigCostPrice.ConsecutiveMonth = 1;
                // throw new Exception("Không tìm thấy config giá vốn của DVCS " + _webHelper.GetCurrentOrgUnit());
            }
            filterDto.LstProductionPeriodCode = (filterDto.LstProductionPeriodCode == null || filterDto.LstProductionPeriodCode == "") ? "" : filterDto.LstProductionPeriodCode + ",";
            if (filterDto.AttachMonth == 1)
            {
                DateTime fromDate = filterDto.FromDate;
                DateTime toDate = filterDto.ToDate;
                var dataDate = new List<DataDateDto>();
                dataDate.Add(new DataDateDto
                {
                    FromDate = Convert.ToDateTime(fromDate.Year + "-" + ((fromDate.Month < 10) ? "0" + fromDate.Month : fromDate.Month) + "-01"),
                    ToDate = await LastDay(fromDate)
                });
                while (fromDate < toDate)
                {
                    fromDate = await NextMonth(fromDate);
                    if (fromDate < toDate)
                    {
                        dataDate.Add(new DataDateDto
                        {
                            FromDate = fromDate,
                            ToDate = await LastDay(fromDate)
                        });
                    }
                }
                foreach(var itemDate in dataDate)
                {
                    var productionPeriod = await _productionPeriodService.GetQueryableAsync();
                    var dataProductionPeriod = productionPeriod.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                        && (filterDto.LstProductionPeriodCode == "" || filterDto.LstProductionPeriodCode == null || filterDto.LstProductionPeriodCode.Contains(p.Code))).OrderBy(p => p.Code);
                    foreach (var itemProductionPeriod in dataProductionPeriod)
                    {
                        int decideApply = 0;
                        var year = filterDto.FromDate.Year;
                        if (filterDto.DecideApply == null || filterDto.DecideApply == 0)
                        {
                            var yearCategory = await _yearCategoryService.GetQueryableAsync();
                            yearCategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                && p.Year == year);
                            decideApply = yearCategory.Select(p => p.UsingDecision).First() ?? 0;
                        }
                        else
                        {
                            decideApply = filterDto.DecideApply ?? 0;
                        }

                        // Tính giá vốn nếu CostCalculation == 1
                        if (filterDto.CostCalculation == 1)
                        {
                            var param = new CrudInfoCalcPriceStockOutDto();
                            param.OrgCode = _webHelper.GetCurrentOrgUnit();
                            param.FromDate = filterDto.FromDate;
                            param.ToDate = filterDto.ToDate;
                            param.CalculatingMethod = dataConfigCostPrice.Type.ToString();
                            param.Continuous = dataConfigCostPrice.ConsecutiveMonth == 1 ? true : false;
                            param.ProductionPeriodCode = "";
                            param.ProductCode = "";
                            param.ProductLotCode = "";
                            param.ProductOriginCode = "";
                            param.WarehouseCode = "";
                            param.Year = filterDto.FromDate.Year;

                            // Tính giá vốn
                            var resInfo = await _infoCalcPriceStockOutAppService.CreateAsync(param);
                            await _pricingOutwardAppService.CreatePricingOutwardAsync(resInfo.Id);
                            await _infoCalcPriceStockOutAppService.DeleteAsync(resInfo.Id);
                        }

                        var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
                        allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                       && (filterDto.LstOrdGrp == "" || filterDto.LstOrdGrp == null || filterDto.LstOrdGrp.Contains(p.OrdGrp))
                                                                       && p.Year == year
                                                                       && p.DecideApply == decideApply
                                                                       && p.ProductionPeriodCode == itemProductionPeriod.Code).ToList();
                        var iQAallotmentForwardCategory = from a in allotmentForwardCategory
                                                            orderby a.OrdGrp, a.Type
                                                            group new { a } by new
                                                            {
                                                                a.OrdGrp,
                                                                a.Type
                                                            } into gr
                                                            select new
                                                            {
                                                                OrdGrp = gr.Key.OrdGrp,
                                                                Type = gr.Key.Type,
                                                            };
                        var lstAllotmentForwardCategory = iQAallotmentForwardCategory.ToList();
                        foreach (var itemAallotmentForwardCategory in lstAllotmentForwardCategory)
                        {
                            var autoAllotmentForwardGrpFilterDto = new AutoAllotmentForwardGrpFilterDto
                            {
                                Year = year,
                                DecideApply = decideApply,
                                FromDate = itemDate.FromDate,
                                ToDate = itemDate.ToDate,
                                FProductWork = filterDto.FProductWork,
                                Type = itemAallotmentForwardCategory.Type,
                                OrdGrp = itemAallotmentForwardCategory.OrdGrp,
                                ProductionPeriodCode = itemProductionPeriod.Code
                            };
                            // excute PB_KC_TD_GRP
                            await AutoAllotmentForwardGrp(autoAllotmentForwardGrpFilterDto);
                        }

                        // update giá thành phẩm
                        var updatePriceFProductFilterDto = new UpdatePriceFProductFilterDto { 
                            FromDate = itemDate.FromDate,
                            ToDate = itemDate.ToDate,
                            FProductWork = filterDto.FProductWork,
                            ProductionPeriodCode = itemProductionPeriod.Code
                        };
                        await UpdatePriceFProduct(updatePriceFProductFilterDto);
                    }
                    // Chuyển số dư
                    await BalanceTransfer(itemDate.ToDate);
                }
            }
            else
            {
                DateTime fromDate = filterDto.FromDate;
                DateTime toDate = filterDto.ToDate;
                var year = fromDate.Year;
                int decideApply = 0;
                if (filterDto.DecideApply == null || filterDto.DecideApply == 0)
                {
                    var yearCategory = await _yearCategoryService.GetQueryableAsync();
                    yearCategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                        && p.Year == year);
                    decideApply = yearCategory.Select(p => p.UsingDecision).First() ?? 0;
                }
                else
                {
                    decideApply = filterDto.DecideApply ?? 0;
                }
                var productionPeriod = await _productionPeriodService.GetQueryableAsync();
                var dataProductionPeriod = productionPeriod.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && (filterDto.LstProductionPeriodCode == "" || filterDto.LstProductionPeriodCode == null || filterDto.LstProductionPeriodCode.Contains(p.Code)));
                foreach (var itemProductionPeriod in dataProductionPeriod)
                {
                    // Tính giá vốn nếu CostCalculation == 1
                    if (filterDto.CostCalculation == 1)
                    {
                        var param = new CrudInfoCalcPriceStockOutDto();
                        param.OrgCode = _webHelper.GetCurrentOrgUnit();
                        param.FromDate = filterDto.FromDate;
                        param.ToDate = filterDto.ToDate;
                        param.CalculatingMethod = dataConfigCostPrice.Type.ToString();
                        param.Continuous = dataConfigCostPrice.ConsecutiveMonth == 1 ? true : false;
                        param.ProductionPeriodCode = "";
                        param.ProductCode = "";
                        param.ProductLotCode = "";
                        param.ProductOriginCode = "";
                        param.WarehouseCode = "";
                        param.Year = filterDto.FromDate.Year;

                        // Tính giá vốn
                        var resInfo = await _infoCalcPriceStockOutAppService.CreateAsync(param);
                        await _pricingOutwardAppService.CreatePricingOutwardAsync(resInfo.Id);
                        await _infoCalcPriceStockOutAppService.DeleteAsync(resInfo.Id);
                    }

                    var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
                    allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && (filterDto.LstOrdGrp == "" || filterDto.LstOrdGrp == null || filterDto.LstOrdGrp.Contains(p.OrdGrp))
                                                                   && p.Year == year
                                                                   && p.DecideApply == decideApply
                                                                   && p.ProductionPeriodCode == itemProductionPeriod.Code).ToList();
                    var iQAallotmentForwardCategory = from a in allotmentForwardCategory
                                                      orderby a.OrdGrp, a.Type
                                                      group new { a } by new
                                                      {
                                                          a.OrdGrp,
                                                          a.Type
                                                      } into gr
                                                      select new
                                                      {
                                                          OrdGrp = gr.Key.OrdGrp,
                                                          Type = gr.Key.Type,
                                                      };
                    var lstAllotmentForwardCategory = iQAallotmentForwardCategory.ToList();
                    foreach (var itemAallotmentForwardCategory in lstAllotmentForwardCategory)
                    {
                        var autoAllotmentForwardGrpFilterDto = new AutoAllotmentForwardGrpFilterDto
                        {
                            Year = year,
                            DecideApply = decideApply,
                            FromDate = fromDate,
                            ToDate = toDate,
                            FProductWork = filterDto.FProductWork,
                            Type = itemAallotmentForwardCategory.Type,
                            OrdGrp = itemAallotmentForwardCategory.OrdGrp,
                            ProductionPeriodCode = itemProductionPeriod.Code
                        };
                        // excute PB_KC_TD_GRP
                        await AutoAllotmentForwardGrp(autoAllotmentForwardGrpFilterDto);
                    }

                    // update giá thành phẩm
                    var updatePriceFProductFilterDto = new UpdatePriceFProductFilterDto
                    {
                        FromDate = fromDate,
                        ToDate = toDate,
                        FProductWork = filterDto.FProductWork,
                        ProductionPeriodCode = itemProductionPeriod.Code
                    };
                    await UpdatePriceFProduct(updatePriceFProductFilterDto);
                }
                // Chuyển số dư
                await BalanceTransfer(toDate);
            }
            if (filterDto.CostCalculation == 1)
            {
                var param = new CrudInfoCalcPriceStockOutDto();
                param.OrgCode = _webHelper.GetCurrentOrgUnit();
                param.FromDate = filterDto.FromDate;
                param.ToDate = filterDto.ToDate;
                param.CalculatingMethod = dataConfigCostPrice.Type.ToString();
                param.Continuous = dataConfigCostPrice.ConsecutiveMonth == 1 ? true : false;
                param.ProductionPeriodCode = "";
                param.ProductCode = "";
                param.ProductLotCode = "";
                param.ProductOriginCode = "";
                param.WarehouseCode = "";
                param.Year = filterDto.FromDate.Year;

                // Tính giá vốn
                var resInfo = await _infoCalcPriceStockOutAppService.CreateAsync(param);
                await _pricingOutwardAppService.CreatePricingOutwardAsync(resInfo.Id);
                await _infoCalcPriceStockOutAppService.DeleteAsync(resInfo.Id);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Tính giá thành sản xuất hoàn thành";
            return res;
        }

        // Tính giá thành công trình
        public async Task<ResultDto> ProjectCostCalculationAsync(ProjectCostCalculationFilterDto filterDto)
        {
            await _licenseBusiness.CheckExpired();
            var yearSystem = _webHelper.GetCurrentYear();
            if (yearSystem != filterDto.FromDate.Year || yearSystem != filterDto.ToDate.Year)
            {
                throw new Exception("Ngày bạn chọn không đúng năm làm việc: " + yearSystem);
            }
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var isBookClosing = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.BookClosingDate >= filterDto.FromDate).Any();
            if (isBookClosing)
            {
                var lstDateBookClosing = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.BookClosingDate >= filterDto.FromDate).Select(p => p.BookClosingDate).ToArray();
                throw new Exception("Hệ thống chứng từ đã có thời gian khóa đến ngày: " + string.Join(",", lstDateBookClosing));
            }
            var configCostPrice = await _configCostPriceService.GetQueryableAsync();
            var dataConfigCostPrice = configCostPrice.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).FirstOrDefault();
            if (dataConfigCostPrice == null)
            {
                if (dataConfigCostPrice == null)
                {
                    dataConfigCostPrice = new ConfigCostPrice();
                    dataConfigCostPrice.Fifo = 1;
                    dataConfigCostPrice.Type = 1;
                    dataConfigCostPrice.ConsecutiveMonth = 1;
                    // throw new Exception("Không tìm thấy config giá vốn của DVCS " + _webHelper.GetCurrentOrgUnit());
                }
            }

            if (filterDto.AttachMonth == 1)
            {
                DateTime fromDate = filterDto.FromDate;
                DateTime toDate = filterDto.ToDate;
                var dataDate = new List<DataDateDto>();
                dataDate.Add(new DataDateDto
                {
                    FromDate = Convert.ToDateTime(fromDate.Year + "-" + ((fromDate.Month < 10) ? "0" + fromDate.Month : fromDate.Month) + "-01"),
                    ToDate = await LastDay(fromDate)
                });
                while (fromDate < toDate)
                {
                    fromDate = await NextMonth(fromDate);
                    if (fromDate < toDate)
                    {
                        dataDate.Add(new DataDateDto
                        {
                            FromDate = fromDate,
                            ToDate = await LastDay(fromDate)
                        });
                    }
                }

                foreach (var itemDate in dataDate)
                {
                    int decideApply = 0;
                    var year = filterDto.FromDate.Year;
                    if (filterDto.DecideApply == null || filterDto.DecideApply == 0)
                    {
                        var yearCategory = await _yearCategoryService.GetQueryableAsync();
                        yearCategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                            && p.Year == year);
                        decideApply = yearCategory.Select(p => p.UsingDecision).First() ?? 0;
                    }
                    else
                    {
                        decideApply = filterDto.DecideApply ?? 0;
                    }
                    // Tính giá vốn nếu CostCalculation == 1
                    if (filterDto.CostCalculation == 1)
                    {
                        var param = new CrudInfoCalcPriceStockOutDto();
                        param.OrgCode = _webHelper.GetCurrentOrgUnit();
                        param.FromDate = filterDto.FromDate;
                        param.ToDate = filterDto.ToDate;
                        param.CalculatingMethod = dataConfigCostPrice.Type.ToString();
                        param.Continuous = dataConfigCostPrice.ConsecutiveMonth == 1 ? true : false;
                        param.ProductionPeriodCode = "";
                        param.ProductCode = "";
                        param.ProductLotCode = "";
                        param.ProductOriginCode = "";
                        param.WarehouseCode = "";
                        param.Year = filterDto.FromDate.Year;

                        // Tính giá vốn
                        var resInfo = await _infoCalcPriceStockOutAppService.CreateAsync(param);
                        await _pricingOutwardAppService.CreatePricingOutwardAsync(resInfo.Id);
                        await _infoCalcPriceStockOutAppService.DeleteAsync(resInfo.Id);
                    }
                    var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
                    allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && (filterDto.LstOrdGrp == "" || filterDto.LstOrdGrp == null || filterDto.LstOrdGrp.Contains(p.OrdGrp))
                                                                   && p.Year == year
                                                                   && p.DecideApply == decideApply).ToList();
                    var iQAallotmentForwardCategory = from a in allotmentForwardCategory
                                                      orderby a.OrdGrp, a.Type
                                                      group new { a } by new
                                                      {
                                                          a.OrdGrp,
                                                          a.Type
                                                      } into gr
                                                      select new
                                                      {
                                                          OrdGrp = gr.Key.OrdGrp,
                                                          Type = gr.Key.Type,
                                                      };
                    var lstAllotmentForwardCategory = iQAallotmentForwardCategory.ToList();
                    foreach (var itemAallotmentForwardCategory in lstAllotmentForwardCategory)
                    {
                        var autoAllotmentForwardGrpFilterDto = new AutoAllotmentForwardGrpFilterDto
                        {
                            Year = year,
                            DecideApply = decideApply,
                            FromDate = itemDate.FromDate,
                            ToDate = itemDate.ToDate,
                            FProductWork = filterDto.FProductWork,
                            Type = itemAallotmentForwardCategory.Type,
                            OrdGrp = itemAallotmentForwardCategory.OrdGrp,
                            ProductionPeriodCode = ""
                        };
                        // excute PB_KC_TD_GRP
                        await AutoAllotmentForwardGrp(autoAllotmentForwardGrpFilterDto);
                    }
                    // Chuyển số dư
                    await BalanceTransfer(itemDate.ToDate);
                    // Tạo bút toán kết chuyển vốn
                    if (filterDto.ProductCostAcc != null && filterDto.ProductCostAcc != "")
                    {
                        var createCapitalTransferFilterDto = new CreateCapitalTransferFilterDto
                        {
                            FProductWork = filterDto.FProductWork,
                            FromDate = itemDate.FromDate,
                            ToDate = itemDate.ToDate,
                            ProductCostAcc = filterDto.ProductCostAcc
                        };
                        await CreateCapitalTransfer(createCapitalTransferFilterDto);
                    }
                }
            }
            else
            {
                DateTime fromDate = filterDto.FromDate;
                DateTime toDate = filterDto.ToDate;
                var year = fromDate.Year;
                int decideApply = 0;
                if (filterDto.DecideApply == null || filterDto.DecideApply == 0)
                {
                    var yearCategory = await _yearCategoryService.GetQueryableAsync();
                    yearCategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                        && p.Year == year);
                    decideApply = yearCategory.Select(p => p.UsingDecision).First() ?? 0;
                }
                else
                {
                    decideApply = filterDto.DecideApply ?? 0;
                }
                // Tính giá vốn nếu CostCalculation == 1
                if (filterDto.CostCalculation == 1)
                {
                    var param = new CrudInfoCalcPriceStockOutDto();
                    param.OrgCode = _webHelper.GetCurrentOrgUnit();
                    param.FromDate = filterDto.FromDate;
                    param.ToDate = filterDto.ToDate;
                    param.CalculatingMethod = dataConfigCostPrice.Type.ToString();
                    param.Continuous = dataConfigCostPrice.ConsecutiveMonth == 1 ? true : false;
                    param.ProductionPeriodCode = "";
                    param.ProductCode = "";
                    param.ProductLotCode = "";
                    param.ProductOriginCode = "";
                    param.WarehouseCode = "";
                    param.Year = filterDto.FromDate.Year;

                    // Tính giá vốn
                    var resInfo = await _infoCalcPriceStockOutAppService.CreateAsync(param);
                    await _pricingOutwardAppService.CreatePricingOutwardAsync(resInfo.Id);
                    await _infoCalcPriceStockOutAppService.DeleteAsync(resInfo.Id);
                }

                var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
                allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                               && (filterDto.LstOrdGrp == "" || filterDto.LstOrdGrp == null || filterDto.LstOrdGrp.Contains(p.OrdGrp))
                                                               && p.Year == year
                                                               && p.DecideApply == decideApply).ToList();
                var iQAallotmentForwardCategory = from a in allotmentForwardCategory
                                                  orderby a.OrdGrp, a.Type
                                                  group new { a } by new
                                                  {
                                                      a.OrdGrp,
                                                      a.Type
                                                  } into gr
                                                  select new
                                                  {
                                                      OrdGrp = gr.Key.OrdGrp,
                                                      Type = gr.Key.Type,
                                                  };
                var lstAllotmentForwardCategory = iQAallotmentForwardCategory.ToList();
                foreach (var itemAallotmentForwardCategory in lstAllotmentForwardCategory)
                {
                    var autoAllotmentForwardGrpFilterDto = new AutoAllotmentForwardGrpFilterDto
                    {
                        Year = year,
                        DecideApply = decideApply,
                        FromDate = fromDate,
                        ToDate = toDate,
                        FProductWork = filterDto.FProductWork,
                        Type = itemAallotmentForwardCategory.Type,
                        OrdGrp = itemAallotmentForwardCategory.OrdGrp,
                        ProductionPeriodCode = ""
                    };
                    // excute PB_KC_TD_GRP
                    await AutoAllotmentForwardGrp(autoAllotmentForwardGrpFilterDto);
                }
                // Chuyển số dư
                await BalanceTransfer(toDate);
                // Tạo bút toán kết chuyển vốn
                if (filterDto.ProductCostAcc != null && filterDto.ProductCostAcc != "")
                {
                    var createCapitalTransferFilterDto = new CreateCapitalTransferFilterDto
                    {
                        FProductWork = filterDto.FProductWork,
                        FromDate = fromDate,
                        ToDate = toDate,
                        ProductCostAcc = filterDto.ProductCostAcc
                    };
                    await CreateCapitalTransfer(createCapitalTransferFilterDto);
                }
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Tính giá thành công trình hoàn thành";
            return res;
        }

        // Kết chuyển tự động cuối kỳ lên lãi lỗ
        public async Task<ResultDto> ForwardAutoAsync(ForwardAutoFilterDto filterDto)
        {
            await _licenseBusiness.CheckExpired();
            var yearSystem = _webHelper.GetCurrentYear();
            if (yearSystem != filterDto.FromDate.Year || yearSystem != filterDto.ToDate.Year)
            {
                throw new Exception("Ngày bạn chọn không đúng năm làm việc: " + yearSystem);
            }
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var isBookClosing = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.BookClosingDate >= filterDto.FromDate).Any();
            if (isBookClosing)
            {
                var lstDateBookClosing = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                      && p.BookClosingDate >= filterDto.FromDate).Select(p => p.BookClosingDate).ToArray();
                throw new Exception("Hệ thống chứng từ đã có thời gian khóa đến ngày: " + string.Join(",", lstDateBookClosing));
            }

            if (filterDto.AttachMonth == 1)
            {
                DateTime fromDate = filterDto.FromDate;
                DateTime toDate = filterDto.ToDate;
                var dataDate = new List<DataDateDto>();
                dataDate.Add(new DataDateDto
                {
                    FromDate = Convert.ToDateTime(fromDate.Year + "-" + ((fromDate.Month < 10) ? "0" + fromDate.Month : fromDate.Month) + "-01"),
                    ToDate = await LastDay(fromDate)
                });
                while (fromDate < toDate)
                {
                    fromDate = await NextMonth(fromDate);
                    if (fromDate < toDate)
                    {
                        dataDate.Add(new DataDateDto
                        {
                            FromDate = fromDate,
                            ToDate = await LastDay(fromDate)
                        });
                    }
                }
                foreach (var itemDate in dataDate)
                {
                    // gán lại quyết định áp dụng
                    int decideApply = 0;
                    var year = filterDto.FromDate.Year;
                    if (filterDto.DecideApply == null || filterDto.DecideApply == 0)
                    {
                        var yearCategory = await _yearCategoryService.GetQueryableAsync();
                        yearCategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                            && p.Year == year);
                        decideApply = yearCategory.Select(p => p.UsingDecision).First() ?? 0;
                    }
                    else
                    {
                        decideApply = filterDto.DecideApply ?? 0;
                    }

                    // gọi thủ tục AutoAllotmentForwardGrp (PB_KC_TD_GRP)
                    var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
                    allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && (filterDto.LstOrdGrp == "" || filterDto.LstOrdGrp == null || filterDto.LstOrdGrp.Contains(p.OrdGrp))
                                                                   && p.Year == year
                                                                   && p.DecideApply == decideApply).ToList();
                    var iQAallotmentForwardCategory = from a in allotmentForwardCategory
                                                      orderby a.OrdGrp, a.Type
                                                      group new { a } by new
                                                      {
                                                          a.OrdGrp,
                                                          a.Type
                                                      } into gr
                                                      select new
                                                      {
                                                          OrdGrp = gr.Key.OrdGrp,
                                                          Type = gr.Key.Type,
                                                      };
                    var lstAllotmentForwardCategory = iQAallotmentForwardCategory.ToList();
                    foreach (var itemAallotmentForwardCategory in lstAllotmentForwardCategory)
                    {
                        var autoAllotmentForwardGrpFilterDto = new AutoAllotmentForwardGrpFilterDto
                        {
                            Year = year,
                            DecideApply = decideApply,
                            FromDate = itemDate.FromDate,
                            ToDate = itemDate.ToDate,
                            FProductWork = filterDto.FProductWork,
                            Type = itemAallotmentForwardCategory.Type,
                            OrdGrp = itemAallotmentForwardCategory.OrdGrp,
                            ProductionPeriodCode = ""
                        };
                        // excute PB_KC_TD_GRP
                        await AutoAllotmentForwardGrp(autoAllotmentForwardGrpFilterDto);
                    }
                }
            }
            else
            {
                DateTime fromDate = filterDto.FromDate;
                DateTime toDate = filterDto.ToDate;
                var year = fromDate.Year;
                int decideApply = 0;
                if (filterDto.DecideApply == null || filterDto.DecideApply == 0)
                {
                    var yearCategory = await _yearCategoryService.GetQueryableAsync();
                    yearCategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                        && p.Year == year);
                    decideApply = yearCategory.Select(p => p.UsingDecision).First() ?? 0;
                }
                else
                {
                    decideApply = filterDto.DecideApply ?? 0;
                }
                // gọi thủ tục AutoAllotmentForwardGrp (PB_KC_TD_GRP)
                var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
                allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                               && (filterDto.LstOrdGrp == "" || filterDto.LstOrdGrp == null || filterDto.LstOrdGrp.Contains(p.OrdGrp))
                                                               && p.Year == year
                                                               && p.DecideApply == decideApply).ToList();
                var iQAallotmentForwardCategory = from a in allotmentForwardCategory
                                                  orderby a.OrdGrp, a.Type
                                                  group new { a } by new
                                                  {
                                                      a.OrdGrp,
                                                      a.Type
                                                  } into gr
                                                  select new
                                                  {
                                                      OrdGrp = gr.Key.OrdGrp,
                                                      Type = gr.Key.Type,
                                                  };
                var lstAllotmentForwardCategory = iQAallotmentForwardCategory.ToList();
                foreach (var itemAallotmentForwardCategory in lstAllotmentForwardCategory)
                {
                    var autoAllotmentForwardGrpFilterDto = new AutoAllotmentForwardGrpFilterDto
                    {
                        Year = year,
                        DecideApply = decideApply,
                        FromDate = fromDate,
                        ToDate = toDate,
                        FProductWork = filterDto.FProductWork,
                        Type = itemAallotmentForwardCategory.Type,
                        OrdGrp = itemAallotmentForwardCategory.OrdGrp,
                        ProductionPeriodCode = ""
                    };
                    // excute PB_KC_TD_GRP
                    await AutoAllotmentForwardGrp(autoAllotmentForwardGrpFilterDto);
                }
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Kết chuyển tự động hoàn thành";
            return res;
        }
        #region Private
        private async Task AutoAllotmentForwardGrp(AutoAllotmentForwardGrpFilterDto filterDto)
        {
            //--Xóa dữ liệu chứng từ kết chuyển
            await AllotmentForwardDeleteVoucherGrp(filterDto);
            //--Thực hiện phân bổ - kết chuyển 
             switch (filterDto.Type)
            {
                case "D":
                    await AllotmentForwardAutoGrpD(filterDto);
                    break;
                case "L":
                    await AllotmentForwardAutoGrpL(filterDto);// PB_KC_TD_GRP_L
                    break;
                case "H":
                    await AllotmentForwardAutoGrpH(filterDto);
                    break;
                default:
                    await AllotmentForwardAutoGrpT(filterDto);
                    break;
            }
            // Tạo chứng từ phân bổ - kết chuyển
            var createVoucherDto = new CreateVoucherDto()
            {
                FromDate = filterDto.FromDate,
                ToDate = filterDto.ToDate,
                Year = filterDto.Year,
                FProductWork = filterDto.FProductWork,
                OrdGrp = filterDto.OrdGrp,
                Type = filterDto.Type,
                ProductionPeriodCode = filterDto.ProductionPeriodCode,
            };
            await CreateVoucher(createVoucherDto);
        }

        private async Task AllotmentForwardDeleteVoucherGrp(AutoAllotmentForwardGrpFilterDto filterDto) // PB_KC_TD_GRP
        {
            var voucherCode = "PKC";
            if(filterDto.FProductWork != "A") voucherCode = "ZSX";
            // Lấy dữ liệu từ InfoExportAuto
            var infoExportAuto = await _infoExportAutoService.GetQueryableAsync();
            infoExportAuto = from a in infoExportAuto
                             where a.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && a.Year == filterDto.Year
                                && a.VoucherCode == voucherCode
                                && a.ProductionPeriodCode == filterDto.ProductionPeriodCode
                                && a.FProductWork == filterDto.FProductWork
                                && a.OrdGrp == filterDto.OrdGrp
                                && a.Type == filterDto.Type
                                && a.BeginDate >= filterDto.FromDate 
                                && a.EndDate <= filterDto.ToDate
                            select a;
            var accVoucher = await _accVoucherService.GetQueryableAsync();
            accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            // xóa dữ liệu ở sổ cái
            var ledger = await _ledgerService.GetQueryableAsync();
            ledger = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            ledger = from a in ledger
                     join b in infoExportAuto on a.VoucherId equals b.OrdRec
                     select a;
            await _ledgerService.DeleteManyAsync(ledger, true);

            // xóa dữ liệu ở chi tiết KT
            var accVoucherDetailService = await _accVoucherDetailService.GetQueryableAsync();
            accVoucherDetailService = accVoucherDetailService.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            accVoucherDetailService = from a in accVoucherDetailService
                                      join b in infoExportAuto on a.AccVoucherId equals b.OrdRec
                                      select a;
            await _accVoucherDetailService.DeleteManyAsync(accVoucherDetailService, true);

            // xóa dữ liệu ở đầu phiếu KT
            accVoucher = from a in accVoucher
                         join b in infoExportAuto on a.Id equals b.OrdRec
                         select a;
            await _accVoucherService.DeleteManyAsync(accVoucher, true);
            // xóa dữ liệu ở InfoExportAuto
            await _infoExportAutoService.DeleteManyAsync(infoExportAuto, true);
        }

        private async Task AllotmentForwardAutoGrpD(AutoAllotmentForwardGrpFilterDto filterDto) // PB_KC_TD_GRP_D
        {
            // khai báo đầu phiếu định mức sp
            var fProductWorkNorm = await _fProductWorkNormService.GetQueryableAsync();
            fProductWorkNorm = fProductWorkNorm.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNorm = fProductWorkNorm.ToList();
            //khai báo chi tiết định mức
            var fProductWorkNormDetail = await _fProductWorkNormDetailService.GetQueryableAsync();
            fProductWorkNormDetail = fProductWorkNormDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNormDetail = fProductWorkNormDetail.ToList();
            //khai báo chi tiết nhóm hệ số
            var groupCoefficientDetail = await _groupCoefficientDetailService.GetQueryableAsync();
            groupCoefficientDetail = groupCoefficientDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.FProductWork == filterDto.FProductWork
                                                                    && p.Year == filterDto.Year);
            var lstGroupCoefficientDetail = groupCoefficientDetail.Select(p => ObjectMapper.Map<GroupCoefficientDetail, GroupCoefficientDetailDto>(p)).ToList();
            // Lấy list mã thành phẩm
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherCode = voucherType.Where(p => p.Code == "PTP").Select(p => p.ListVoucher).First();
            var getQuantityFProductFilterDto = new GetQuantityFProductFilterDto
                                                {
                                                    Year = filterDto.Year,
                                                    FromDate = filterDto.FromDate,
                                                    ToDate = filterDto.ToDate,
                                                    LstVoucherCode = lstVoucherCode,
                                                    ProductionPeriodCode = filterDto.ProductionPeriodCode
            };
            // Lấy dữ liệu số lượng thành phẩm
            var lstQuantityFProduct = await GetQuantityFProduct(getQuantityFProductFilterDto);
            // Xóa dữ liệu InfoZ
            var infoZDelete = await _infoZService.GetQueryableAsync();
            infoZDelete = infoZDelete.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && p.ProductionPeriodCode == filterDto.ProductionPeriodCode
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && ((p.BeginM >= filterDto.FromDate && p.BeginM <= filterDto.ToDate) || (p.EndM >= filterDto.FromDate && p.EndM <= filterDto.ToDate)));
            await _infoZService.DeleteManyAsync(infoZDelete, true);

            // lấy dữ liệu AllotmentForwardCategory
            var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
            allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && (p.ProductionPeriodCode ?? "") == filterDto.ProductionPeriodCode
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && p.Active == 1
                                              && p.DecideApply == filterDto.DecideApply).OrderBy(p => p.Code).ToList();
            var lstAllotmentForwardCategory = allotmentForwardCategory.ToList();
            foreach (var itemAFC in lstAllotmentForwardCategory)
            {
                var accCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditAcc : itemAFC.DebitAcc;
                var sectionCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditSectionCode : itemAFC.DebitSectionCode;
                var accCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitAcc : itemAFC.CreditAcc;
                var sectionCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitSectionCode : itemAFC.CreditSectionCode;
                var iQAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
                iQAccountSystem = iQAccountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                          && p.Year == _webHelper.GetCurrentYear()
                                                          && p.AccCode == accCode0).ToList();
                var dataAccountSystem = iQAccountSystem.FirstOrDefault();
                // Update dở dang cuối kỳ nếu có
                if ("S,C".Contains(filterDto.FProductWork))
                {
                    var updatePriceUnfinishedInventoryFilterDto = new UpdatePriceUnfinishedInventoryFilterDto
                    {
                        Year = filterDto.Year,
                        FromDate = filterDto.FromDate,
                        ToDate = filterDto.ToDate,
                        AccCode = accCode,
                        SectionCode = sectionCode,
                        DebitCredit = itemAFC.DebitCredit
                    };
                    await UpdatePriceUnfinishedInventory(updatePriceUnfinishedInventoryFilterDto);
                }
                // - - - -
                var getIncurredExpensesFilterDto = new GetIncurredExpensesFilterDto
                                                    {
                                                        Year = filterDto.Year,
                                                        FromDate = filterDto.FromDate,
                                                        ToDate = filterDto.ToDate,
                                                        AccCode = accCode,
                                                        SectionCode = sectionCode,
                                                        DebitCredit = itemAFC.DebitCredit,
                                                        OrdRec = null,
                                                        ForwardType = itemAFC.ForwardType,
                                                        FProductWork = filterDto.FProductWork,
                                                        AttachProduct = itemAFC.AttachProduct
                                                    };
                var lstIncurredExpenses = await GetIncurredExpenses(getIncurredExpensesFilterDto);
                // Phân bổ từ lstIncurredExpenses vào bảng lstQuantityFProduct
                var lstGroupCoefficientDetail2 = lstGroupCoefficientDetail.Where(p => p.GroupCoefficientCode == itemAFC.GroupCoefficientCode).ToList();
                var iQRatio = from a in lstFProductWorkNorm
                              join b in lstFProductWorkNormDetail on a.Id equals b.FProductWorkNormId
                              join c in lstQuantityFProduct on a.FProductWorkCode equals c.FProductWorkCode
                              join d in lstGroupCoefficientDetail2 on a.FProductWorkCode equals d.FProductWorkCode into ajd
                              from d in ajd.DefaultIfEmpty()
                              where b.Year == filterDto.Year
                                 && b.Month == filterDto.FromDate.Month
                                 && (itemAFC.GroupCoefficientCode == "" || (d != null && d.FProductWorkCode != null))
                              group new { a, b, c, d } by new
                              {
                                  a.OrgCode,
                                  c.WorkPlaceCode,
                                  c.ProductCode,
                                  a.FProductWorkCode,
                                  QuantityNorm = a.Quantity,
                                  c.Quantity,
                                  c.UnfinishedQuantity,
                                  c.HTPercentage,
                                  c.UnfinishedQuantity2,
                                  c.HTPercentage2
                              } into gr
                              select new RatioDto
                              {
                                  Id = GetNewObjectId(),
                                  OrgCode = gr.Key.OrgCode,
                                  WorkPlaceCode = gr.Key.WorkPlaceCode,
                                  ProductCode = gr.Key.ProductCode,
                                  FProductWorkCode = gr.Key.FProductWorkCode,
                                  QuantityNorm = (gr.Key.QuantityNorm <= 0) ? 1 : gr.Key.QuantityNorm,
                                  Quantity = gr.Sum(p => p.b.Quantity),
                                  Amount = gr.Sum(p => p.b.Amount),
                                  ImportQuantity = gr.Key.Quantity,
                                  ExportQuantity = gr.Key.Quantity + Math.Round((gr.Key.UnfinishedQuantity2*gr.Key.HTPercentage2 - gr.Key.UnfinishedQuantity* gr.Key.HTPercentage)/100 ?? 0, 6)
                              };
                var lstRatio = iQRatio.ToList();
                // Update giá trị phân bổ
                foreach (var itemRatio in lstRatio)
                {
                    itemRatio.AllotmentValue = (itemAFC.NormType == "1" ? itemRatio.Amount : itemRatio.Quantity) * (itemAFC.QuantityType == "1" ? itemRatio.ExportQuantity : itemRatio.ImportQuantity);
                }

                // Tính tỷ lệ theo phân xưởng
                var iQGroupRatio = from a in lstRatio
                                   group new { a } by new { a.WorkPlaceCode } into gr
                                   orderby gr.Key.WorkPlaceCode
                                   select new
                                   {
                                       WorkPlaceCode = gr.Key.WorkPlaceCode,
                                       AllotmentValue = gr.Sum(p => p.a.AllotmentValue)
                                   };
                var lstGroupRatio = iQGroupRatio.ToList();
                foreach (var itemGroupRatio in lstGroupRatio)
                {
                    if (itemGroupRatio.AllotmentValue != 0)
                    {
                        // update Ty_le
                        decimal totalAllotmentValueItemRatio = 0;
                        foreach (var itemRatio in lstRatio)
                        {
                            if (itemRatio.WorkPlaceCode == itemGroupRatio.WorkPlaceCode)
                            {
                                itemRatio.Ratio = Math.Round(itemRatio.AllotmentValue*100/itemGroupRatio.AllotmentValue ?? 0, 10);
                                totalAllotmentValueItemRatio += itemRatio.Ratio ?? 0;
                            }
                        }
                        // update tỉ lệ của phần tử đầu tiên trong trường hợp làm tròn số mà tổng tỷ lệ khác 100%
                        if (totalAllotmentValueItemRatio != 100)
                        {
                            var idItemRatio = lstRatio.Where(p => p.WorkPlaceCode == itemGroupRatio.WorkPlaceCode
                                                               && p.Ratio != 0).Select(p => p.Id).FirstOrDefault();
                            if(idItemRatio != null && idItemRatio!= "")
                            {
                                foreach (var itemRatio in lstRatio)
                                {
                                    if (itemRatio.Id == idItemRatio) itemRatio.Ratio += 100 - totalAllotmentValueItemRatio;
                                    break;
                                }
                            }
                        }
                    }
                }
                // Tính tỷ lệ theo mã thành phẩm
                var totalAllotmentValue = lstRatio.Sum(p => p.AllotmentValue);
                if (totalAllotmentValue != 0)
                {
                    // update Ty_le
                    decimal totalAllotmentValueItemRatio = 0;
                    foreach (var itemRatio in lstRatio)
                    {
                        itemRatio.RatioAll = Math.Round(itemRatio.AllotmentValue * 100 / totalAllotmentValue ?? 0, 10);
                        totalAllotmentValueItemRatio += itemRatio.RatioAll ?? 0;
                    }
                    // update tỉ lệ của phần tử đầu tiên trong trường hợp làm tròn số mà tổng tỷ lệ khác 100%
                    if (totalAllotmentValueItemRatio != 100)
                    {
                        var idItemRatio = lstRatio.Where(p => p.RatioAll != 0).Select(p => p.Id).FirstOrDefault();
                        if (idItemRatio != null && idItemRatio != "")
                        {
                            foreach (var itemRatio in lstRatio)
                            {
                                if (itemRatio.Id == idItemRatio) itemRatio.RatioAll += 100 - totalAllotmentValueItemRatio;
                                break;
                            }
                        }
                    }
                }

                // Phân bổ theo tỷ lệ
                var lstAllocationCostIncurred = new List<AllocationCostIncurredDto>();
                // 1. Có mã phân xưởng
                var iQHaveWorkPlaceCode = from a in lstIncurredExpenses
                         join b in lstRatio on a.WorkPlaceCode equals b.WorkPlaceCode
                         where a.WorkPlaceCode != ""
                         select new AllocationCostIncurredDto
                         {
                             Id = GetNewObjectId(),
                             AllocationId = a.Id,
                             OrgCode = a.OrgCode,
                             Year = a.Year,
                             AccCode = a.AccCode,
                             PartnerCode = a.PartnerCode,
                             ContractCode = a.ContractCode,
                             WorkPlaceCode = a.WorkPlaceCode,
                             FProductWorkCode = b.FProductWorkCode,
                             FProductWorkCode0 = b.FProductWorkCode,
                             SectionCode = a.SectionCode,
                             ProductCode = a.ProductCode,
                             Quantity = a.Quantity,
                             Amount = a.Amount,
                             AmountCur = a.AmountCur,
                             BeginQuantity = a.BeginQuantity,
                             BeginAmount = a.BeginAmount,
                             EndQuantity = a.EndQuantity,
                             EndAmount = a.EndAmount,
                             Ratio = b.Ratio,
                             Quantity0 = Math.Round(a.Quantity*b.Ratio ?? 0,10),
                             Amount0 = Math.Round(a.Amount*b.Ratio ?? 0,10),
                             AmountCur0 = Math.Round(a.AmountCur*b.Ratio ?? 0,10),
                             BeginAmount0 = Math.Round(a.BeginAmount*b.Ratio ?? 0,10),
                             BeginQuantity0 = Math.Round(a.BeginQuantity*b.Ratio ?? 0,10),
                             EndAmount0 = Math.Round(a.EndAmount*b.Ratio ?? 0,10),
                             EndQuantity0 = Math.Round(a.EndQuantity*b.Ratio ?? 0,10)
                         };
                var lstHaveWorkPlaceCode = iQHaveWorkPlaceCode.ToList();
                lstAllocationCostIncurred = Enumerable.Concat(lstAllocationCostIncurred, lstHaveWorkPlaceCode).ToList();

                // 1. Không có mã phân xưởng
                var iQNoWorkPlaceCode = from a in lstIncurredExpenses
                                          join b in lstRatio on a.OrgCode equals b.OrgCode
                                          where a.WorkPlaceCode == ""
                                          select new AllocationCostIncurredDto
                                          {
                                              Id = GetNewObjectId(),
                                              AllocationId = a.Id,
                                              OrgCode = a.OrgCode,
                                              Year = a.Year,
                                              AccCode = a.AccCode,
                                              PartnerCode = a.PartnerCode,
                                              ContractCode = a.ContractCode,
                                              WorkPlaceCode = a.WorkPlaceCode,
                                              FProductWorkCode = b.FProductWorkCode,
                                              FProductWorkCode0 = b.FProductWorkCode,
                                              SectionCode = a.SectionCode,
                                              ProductCode = a.ProductCode,
                                              Quantity = a.Quantity,
                                              Amount = a.Amount,
                                              AmountCur = a.AmountCur,
                                              BeginQuantity = a.BeginQuantity,
                                              BeginAmount = a.BeginAmount,
                                              EndQuantity = a.EndQuantity,
                                              EndAmount = a.EndAmount,
                                              Ratio = b.RatioAll,
                                              Quantity0 = Math.Round(a.Quantity * b.RatioAll/100 ?? 0, 10),
                                              Amount0 = Math.Round(a.Amount * b.RatioAll / 100 ?? 0, 10),
                                              AmountCur0 = Math.Round(a.AmountCur * b.RatioAll / 100 ?? 0, 10),
                                              BeginAmount0 = Math.Round(a.BeginAmount * b.RatioAll / 100 ?? 0, 10),
                                              BeginQuantity0 = Math.Round(a.BeginQuantity * b.RatioAll / 100 ?? 0, 10),
                                              EndAmount0 = Math.Round(a.EndAmount * b.RatioAll / 100 ?? 0, 10),
                                              EndQuantity0 = Math.Round(a.EndQuantity * b.RatioAll / 100 ?? 0, 10)
                                          };
                var lstNoWorkPlaceCode = iQNoWorkPlaceCode.ToList();
                lstAllocationCostIncurred = Enumerable.Concat(lstAllocationCostIncurred, lstNoWorkPlaceCode).ToList();

                // Tính phần chênh lệch tiền do phân bổ
                var iQAllocationCostIncurredRemaining = from a in lstIncurredExpenses
                                                        join b in lstAllocationCostIncurred on a.Id equals b.AllocationId
                                                        group new { a, b } by new
                                                        {
                                                            a.Id,
                                                            a.Quantity,
                                                            a.AmountCur,
                                                            a.Amount,
                                                            a.BeginQuantity,
                                                            a.BeginAmount,
                                                            a.EndAmount,
                                                            a.EndQuantity
                                                        } into gr
                                                        where gr.Key.Quantity - gr.Sum(p => p.b.Quantity0) != 0
                                                           || gr.Key.Amount - gr.Sum(p => p.b.Amount0) != 0
                                                           || gr.Key.AmountCur - gr.Sum(p => p.b.AmountCur0) != 0
                                                           || gr.Key.BeginQuantity - gr.Sum(p => p.b.BeginQuantity0) != 0
                                                           || gr.Key.BeginAmount - gr.Sum(p => p.b.BeginAmount0) != 0
                                                           || gr.Key.EndAmount - gr.Sum(p => p.b.EndAmount0) != 0
                                                           || gr.Key.EndQuantity - gr.Sum(p => p.b.EndQuantity0) != 0
                                                        select new AllocationCostIncurredDto
                                                        {
                                                            Id = gr.Key.Id,
                                                            Quantity = gr.Key.Quantity - gr.Sum(p => p.b.Quantity0),
                                                            Amount = gr.Key.Amount - gr.Sum(p => p.b.Amount0),
                                                            AmountCur = gr.Key.AmountCur - gr.Sum(p => p.b.AmountCur0),
                                                            BeginQuantity = gr.Key.BeginQuantity - gr.Sum(p => p.b.BeginQuantity0),
                                                            BeginAmount = gr.Key.BeginAmount - gr.Sum(p => p.b.BeginAmount0),
                                                            EndAmount = gr.Key.EndAmount - gr.Sum(p => p.b.EndAmount0),
                                                            EndQuantity = gr.Key.EndQuantity - gr.Sum(p => p.b.EndQuantity0),
                                                        };
                var lstAllocationCostIncurredRemaining = iQAllocationCostIncurredRemaining.ToList();
                foreach (var itemACR in lstAllocationCostIncurredRemaining)
                {
                    foreach (var itemAllocationCostIncurred in lstAllocationCostIncurred)
                    {
                        // Số lượng
                        if (itemACR.Quantity != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.Quantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.Quantity0 += itemACR.Quantity;
                            }
                        }

                        // Tiền
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.Amount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.Amount0 += itemACR.Amount;
                            }
                        }

                        // Tiền ngoại tệ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.AmountCur0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.AmountCur0 += itemACR.AmountCur;
                            }
                        }

                        // Số lượng đầu kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.BeginQuantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.BeginQuantity0 += itemACR.BeginQuantity;
                            }
                        }

                        // Tiền đầu kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.BeginAmount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.BeginAmount0 += itemACR.BeginAmount;
                            }
                        }

                        // Số lượng cuối kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.EndQuantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.EndQuantity0 += itemACR.EndQuantity;
                            }
                        }

                        // Tiền cuối kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.EndAmount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.EndAmount0 += itemACR.EndAmount;
                            }
                        }
                    }
                }

                // End Tính phần chênh lệch tiền do phân bổ

                // Insert dữ liệu vào InfoZ
                var lstAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());

                var iQInfoZ = from a in lstAllocationCostIncurred
                              join b in lstAccountSystem on new { a.Year, a.AccCode } equals new { b.Year, b.AccCode } into ajb
                              from b in ajb.DefaultIfEmpty()
                              select new CrudInfoZDto
                              {
                                  Id = GetNewObjectId(),
                                  OrgCode = _webHelper.GetCurrentOrgUnit(),
                                  FProductWork = itemAFC.FProductWork,
                                  Year = itemAFC.Year,
                                  Month = filterDto.ToDate.Month,
                                  BeginM = filterDto.FromDate,
                                  EndM = filterDto.ToDate,
                                  OrdGrp = filterDto.OrdGrp,
                                  Type = filterDto.Type,
                                  AllotmentForwardCode = itemAFC.Code,
                                  ProductionPeriodCode = filterDto.ProductionPeriodCode,
                                  DebitAcc = (itemAFC.DebitCredit == 1 ? a.AccCode : accCode0),
                                  DebitSectionCode = (itemAFC.DebitCredit == 1 ? a.SectionCode : sectionCode0),
                                  DebitFProductWorkCode = ((itemAFC.DebitCredit == 1 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 2 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  CreditAcc = (itemAFC.DebitCredit == 2 ? a.AccCode : accCode0),
                                  CreditSectionCode = (itemAFC.DebitCredit == 2 ? a.SectionCode : sectionCode0),
                                  CreditFProductWorkCode = ((itemAFC.DebitCredit == 2 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 1 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  WorkPlaceCode = a.WorkPlaceCode,
                                  FProductWorkCode = (a.FProductWorkCode == "") ? a.FProductWorkCode0 : a.FProductWorkCode,
                                  ProductCode = a.ProductCode,
                                  PartnerCode = a.PartnerCode,
                                  ContractCode = a.ContractCode,
                                  Quantity = a.Quantity0 ?? 0,
                                  Amount = a.Amount0 ?? 0,
                                  AmountCur = a.AmountCur0 ?? 0,
                                  RecordBook = itemAFC.RecordBook,
                                  Ratio = a.Ratio ?? 0,
                                  BeginQuantity = a.BeginQuantity0 ?? 0,
                                  BeginAmount = a.BeginAmount0 ?? 0,
                                  EndQuantity = a.EndQuantity0 ?? 0,
                                  EndAmount = a.EndAmount0 ?? 0,
                              };
                var lstInfoZ = iQInfoZ.Select(p => ObjectMapper.Map<CrudInfoZDto, InfoZ>(p)).ToList();
                await _infoZService.CreateManyAsync(lstInfoZ, true);
            }
        }

        private async Task AllotmentForwardAutoGrpH(AutoAllotmentForwardGrpFilterDto filterDto) // PB_KC_TD_GRP_H
        {
            // khai báo đầu phiếu định mức sp
            var fProductWorkNorm = await _fProductWorkNormService.GetQueryableAsync();
            fProductWorkNorm = fProductWorkNorm.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNorm = fProductWorkNorm.ToList();
            //khai báo chi tiết định mức
            var fProductWorkNormDetail = await _fProductWorkNormDetailService.GetQueryableAsync();
            fProductWorkNormDetail = fProductWorkNormDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNormDetail = fProductWorkNormDetail.ToList();
            // khai báo đầu phiếu nhóm hệ số
            var groupCoefficient = await _groupCoefficientService.GetQueryableAsync();
            groupCoefficient = groupCoefficient.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstGroupCoefficient = groupCoefficient.ToList();
            //khai báo chi tiết nhóm hệ số
            var groupCoefficientDetail = await _groupCoefficientDetailService.GetQueryableAsync();
            groupCoefficientDetail = groupCoefficientDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.FProductWork == filterDto.FProductWork
                                                                    && p.Year == filterDto.Year);
            var lstGroupCoefficientDetail = groupCoefficientDetail.Select(p => ObjectMapper.Map<GroupCoefficientDetail, GroupCoefficientDetailDto>(p)).ToList();
            // Lấy list mã thành phẩm
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherCode = voucherType.Where(p => p.Code == "PTP").Select(p => p.ListVoucher).First();
            var getQuantityFProductFilterDto = new GetQuantityFProductFilterDto
                                                {
                                                    Year = filterDto.Year,
                                                    FromDate = filterDto.FromDate,
                                                    ToDate = filterDto.ToDate,
                                                    LstVoucherCode = lstVoucherCode,
                                                    ProductionPeriodCode = filterDto.ProductionPeriodCode
            };
            // Lấy dữ liệu số lượng thành phẩm
            var lstQuantityFProduct = await GetQuantityFProduct(getQuantityFProductFilterDto);
            // Xóa dữ liệu InfoZ
            var infoZDelete = await _infoZService.GetQueryableAsync();
            infoZDelete = infoZDelete.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && (p.ProductionPeriodCode ?? "") == (filterDto.ProductionPeriodCode ?? "")
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && ((p.BeginM >= filterDto.FromDate && p.BeginM <= filterDto.ToDate) || (p.EndM >= filterDto.FromDate && p.EndM <= filterDto.ToDate)));
            await _infoZService.DeleteManyAsync(infoZDelete, true);

            // lấy dữ liệu AllotmentForwardCategory
            var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
            allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && (p.ProductionPeriodCode ?? "") == filterDto.ProductionPeriodCode
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && p.Active == 1
                                              && p.DecideApply == filterDto.DecideApply).OrderBy(p => p.Code).ToList();
            var lstAllotmentForwardCategory = allotmentForwardCategory.ToList();
            foreach (var itemAFC in lstAllotmentForwardCategory)
            {
                var accCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditAcc : itemAFC.DebitAcc;
                var sectionCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditSectionCode : itemAFC.DebitSectionCode;
                var accCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitAcc : itemAFC.CreditAcc;
                var sectionCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitSectionCode : itemAFC.CreditSectionCode;
                var iQAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
                iQAccountSystem = iQAccountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                          && p.Year == _webHelper.GetCurrentYear()
                                                          && p.AccCode == accCode0).ToList();
                var dataAccountSystem = iQAccountSystem.FirstOrDefault();
                // Update dở dang cuối kỳ nếu có
                if ("S,C".Contains(filterDto.FProductWork))
                {
                    var updatePriceUnfinishedInventoryFilterDto = new UpdatePriceUnfinishedInventoryFilterDto
                    {
                        Year = filterDto.Year,
                        FromDate = filterDto.FromDate,
                        ToDate = filterDto.ToDate,
                        AccCode = accCode,
                        SectionCode = sectionCode,
                        DebitCredit = itemAFC.DebitCredit
                    };
                    await UpdatePriceUnfinishedInventory(updatePriceUnfinishedInventoryFilterDto);
                }
                // - - - -
                var getIncurredExpensesFilterDto = new GetIncurredExpensesFilterDto
                {
                    Year = filterDto.Year,
                    FromDate = filterDto.FromDate,
                    ToDate = filterDto.ToDate,
                    AccCode = accCode,
                    SectionCode = sectionCode,
                    DebitCredit = itemAFC.DebitCredit,
                    OrdRec = null,
                    ForwardType = itemAFC.ForwardType,
                    FProductWork = filterDto.FProductWork,
                    AttachProduct = itemAFC.AttachProduct
                };
                var lstIncurredExpenses = await GetIncurredExpenses(getIncurredExpensesFilterDto);
                // Phân bổ từ lstIncurredExpenses vào bảng lstQuantityFProduct
                var lstRatio = new List<RatioDto>();
                if (filterDto.FProductWork == "S")
                {
                    var lstGroupCoefficientDetail2 = lstGroupCoefficientDetail.Where(p => p.GroupCoefficientCode == itemAFC.GroupCoefficientCode).ToList();
                    var iQRatio = from a in lstGroupCoefficient
                                  join b in lstGroupCoefficientDetail on a.Id equals b.GroupCoefficientId
                                  join c in lstQuantityFProduct on b.FProductWorkCode equals c.FProductWorkCode
                                  join d in lstGroupCoefficientDetail2 on new { b.FProductWorkCode, b.id } equals new { d.FProductWorkCode, d.id } into ajd
                                  from d in ajd.DefaultIfEmpty()
                                  where b.Year == filterDto.Year
                                     && (itemAFC.GroupCoefficientCode == "" || d?.FProductWorkCode != null)
                                  group new { a, b, c, d } by new
                                  {
                                      b.Id,
                                      a.OrgCode,
                                      c.WorkPlaceCode,
                                      c.ProductCode,
                                      b.FProductWorkCode,
                                      c.Quantity,
                                      c.UnfinishedQuantity,
                                      c.HTPercentage,
                                      c.UnfinishedQuantity2,
                                      c.HTPercentage2
                                  } into gr
                                  select new RatioDto
                                  {
                                      Id = GetNewObjectId(),
                                      GroupCoefficientDetailId = gr.Key.Id,
                                      OrgCode = gr.Key.OrgCode,
                                      WorkPlaceCode = gr.Key.WorkPlaceCode,
                                      ProductCode = gr.Key.ProductCode,
                                      FProductWorkCode = gr.Key.FProductWorkCode,
                                      ImportQuantity = gr.Key.Quantity,
                                      ExportQuantity = gr.Key.Quantity + Math.Round((gr.Key.UnfinishedQuantity2 * gr.Key.HTPercentage2 - gr.Key.UnfinishedQuantity * gr.Key.HTPercentage) / 100 ?? 0, 6),
                                      Coefficient = 0
                                  };
                    lstRatio = iQRatio.ToList();
                }
                else
                {
                    var lstGroupCoefficientDetail2 = lstGroupCoefficientDetail.Where(p => p.GroupCoefficientCode == itemAFC.GroupCoefficientCode).ToList();
                    var iQRatio = from a in lstGroupCoefficient
                                  join b in lstGroupCoefficientDetail on a.Id equals b.GroupCoefficientId
                                  join d in lstGroupCoefficientDetail2 on new { b.FProductWorkCode } equals new { d.FProductWorkCode } into ajd
                                  from d in ajd.DefaultIfEmpty()
                                  where b.Year == filterDto.Year
                                     && (itemAFC.GroupCoefficientCode == "" || d.FProductWorkCode != null)
                                     && (itemAFC.GroupCoefficientCode == "" || (d != null && d.FProductWorkCode != null))
                                  group new { a, b, d } by new
                                  {
                                      b.Id,
                                      a.OrgCode,
                                      b.FProductWorkCode,
                                  } into gr
                                  select new RatioDto
                                  {
                                      Id = GetNewObjectId(),
                                      GroupCoefficientDetailId = gr.Key.Id,
                                      OrgCode = gr.Key.OrgCode,
                                      WorkPlaceCode = "",
                                      ProductCode = gr.Key.FProductWorkCode,
                                      FProductWorkCode = gr.Key.FProductWorkCode,
                                      ImportQuantity = 1,
                                      ExportQuantity = 1,
                                      Coefficient = 0
                                  };
                    lstRatio = iQRatio.ToList();
                }
                // Update giá trị phân bổ
                foreach (var itemRatio in lstRatio)
                {
                    switch (filterDto.FromDate.Month)
                    {
                        case 1:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.January).First();
                            break;
                        case 2:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.February).First();
                            break;
                        case 3:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.March).First();
                            break;
                        case 4:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.April).First();
                            break;
                        case 5:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.May).First();
                            break;
                        case 6:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.June).First();
                            break;
                        case 7:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.July).First();
                            break;
                        case 8:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.August).First();
                            break;
                        case 9:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.September).First();
                            break;
                        case 10:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.October).First();
                            break;
                        case 11:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.November).First();
                            break;
                        default:
                            itemRatio.Coefficient = lstGroupCoefficientDetail.Where(p => p.Id == itemRatio.GroupCoefficientDetailId).Select(p => p.December).First();
                            break;
                    }
                    itemRatio.AllotmentValue = itemRatio.Coefficient * (itemAFC.QuantityType == "1" ? itemRatio.ExportQuantity : itemRatio.ImportQuantity);
                }
                // Tính tỷ lệ theo phân xưởng
                var iQGroupRatio = from a in lstRatio
                                   group new { a } by new { a.WorkPlaceCode } into gr
                                   orderby gr.Key.WorkPlaceCode
                                   select new
                                   {
                                       WorkPlaceCode = gr.Key.WorkPlaceCode,
                                       AllotmentValue = gr.Sum(p => p.a.AllotmentValue)
                                   };
                var lstGroupRatio = iQGroupRatio.ToList();
                foreach (var itemGroupRatio in lstGroupRatio)
                {
                    if (itemGroupRatio.AllotmentValue != 0)
                    {
                        // update Ty_le
                        decimal totalAllotmentValueItemRatio = 0;
                        foreach (var itemRatio in lstRatio)
                        {
                            if (itemRatio.WorkPlaceCode == itemGroupRatio.WorkPlaceCode)
                            {
                                itemRatio.Ratio = Math.Round(itemRatio.AllotmentValue * 100 / itemGroupRatio.AllotmentValue ?? 0, 10);
                                totalAllotmentValueItemRatio += itemRatio.Ratio ?? 0;
                            }
                        }
                        // update tỉ lệ của phần tử đầu tiên trong trường hợp làm tròn số mà tổng tỷ lệ khác 100%
                        if (totalAllotmentValueItemRatio != 100)
                        {
                            var idItemRatio = lstRatio.Where(p => p.WorkPlaceCode == itemGroupRatio.WorkPlaceCode
                                                               && p.Ratio != 0).Select(p => p.Id).FirstOrDefault();
                            if (idItemRatio != null && idItemRatio != "")
                            {
                                foreach (var itemRatio in lstRatio)
                                {
                                    if (itemRatio.Id == idItemRatio) itemRatio.Ratio += 100 - totalAllotmentValueItemRatio;
                                    break;
                                }
                            }
                        }
                    }
                }
                // Tính tỷ lệ theo mã thành phẩm
                var totalAllotmentValue = lstRatio.Sum(p => p.AllotmentValue);
                if (totalAllotmentValue != 0)
                {
                    // update Ty_le
                    decimal totalAllotmentValueItemRatio = 0;
                    foreach (var itemRatio in lstRatio)
                    {
                        itemRatio.RatioAll = Math.Round(itemRatio.AllotmentValue * 100 / totalAllotmentValue ?? 0, 10);
                        totalAllotmentValueItemRatio += itemRatio.RatioAll ?? 0;
                    }
                    // update tỉ lệ của phần tử đầu tiên trong trường hợp làm tròn số mà tổng tỷ lệ khác 100%
                    if (totalAllotmentValueItemRatio != 100)
                    {
                        var idItemRatio = lstRatio.Where(p => p.RatioAll != 0).Select(p => p.Id).FirstOrDefault();
                        if (idItemRatio != null && idItemRatio != "")
                        {
                            foreach (var itemRatio in lstRatio)
                            {
                                if (itemRatio.Id == idItemRatio) itemRatio.RatioAll += 100 - totalAllotmentValueItemRatio;
                                break;
                            }
                        }
                    }
                }

                // Phân bổ theo tỷ lệ
                var lstAllocationCostIncurred = new List<AllocationCostIncurredDto>();
                // 1. Có mã phân xưởng
                var iQHaveWorkPlaceCode = from a in lstIncurredExpenses
                                          join b in lstRatio on a.WorkPlaceCode equals b.WorkPlaceCode
                                          where a.WorkPlaceCode != ""
                                          select new AllocationCostIncurredDto
                                          {
                                              Id = GetNewObjectId(),
                                              AllocationId = a.Id,
                                              OrgCode = a.OrgCode,
                                              Year = a.Year,
                                              AccCode = a.AccCode,
                                              PartnerCode = a.PartnerCode,
                                              ContractCode = a.ContractCode,
                                              WorkPlaceCode = a.WorkPlaceCode,
                                              FProductWorkCode = b.FProductWorkCode,
                                              FProductWorkCode0 = b.FProductWorkCode,
                                              SectionCode = a.SectionCode,
                                              ProductCode = a.ProductCode,
                                              Quantity = a.Quantity,
                                              Amount = a.Amount,
                                              AmountCur = a.AmountCur,
                                              BeginQuantity = a.BeginQuantity,
                                              BeginAmount = a.BeginAmount,
                                              EndQuantity = a.EndQuantity,
                                              EndAmount = a.EndAmount,
                                              Ratio = b.Ratio,
                                              Quantity0 = Math.Round(a.Quantity * b.Ratio / 100 ?? 0, 10),
                                              Amount0 = Math.Round(a.Amount * b.Ratio / 100 ?? 0, 10),
                                              AmountCur0 = Math.Round(a.AmountCur * b.Ratio / 100 ?? 0, 10),
                                              BeginAmount0 = Math.Round(a.BeginAmount * b.Ratio / 100 ?? 0, 10),
                                              BeginQuantity0 = Math.Round(a.BeginQuantity * b.Ratio / 100 ?? 0, 10),
                                              EndAmount0 = Math.Round(a.EndAmount * b.Ratio / 100 ?? 0, 10),
                                              EndQuantity0 = Math.Round(a.EndQuantity * b.Ratio / 100 ?? 0, 10)
                                          };
                var lstHaveWorkPlaceCode = iQHaveWorkPlaceCode.ToList();
                lstAllocationCostIncurred = Enumerable.Concat(lstAllocationCostIncurred, lstHaveWorkPlaceCode).ToList();

                // 1. Không có mã phân xưởng
                var iQNoWorkPlaceCode = from a in lstIncurredExpenses
                                        join b in lstRatio on a.OrgCode equals b.OrgCode
                                        where a.WorkPlaceCode == ""
                                        select new AllocationCostIncurredDto
                                        {
                                            Id = GetNewObjectId(),
                                            AllocationId = a.Id,
                                            OrgCode = a.OrgCode,
                                            Year = a.Year,
                                            AccCode = a.AccCode,
                                            PartnerCode = a.PartnerCode,
                                            ContractCode = a.ContractCode,
                                            WorkPlaceCode = a.WorkPlaceCode,
                                            FProductWorkCode = b.FProductWorkCode,
                                            FProductWorkCode0 = b.FProductWorkCode,
                                            SectionCode = a.SectionCode,
                                            ProductCode = a.ProductCode,
                                            Quantity = a.Quantity,
                                            Amount = a.Amount,
                                            AmountCur = a.AmountCur,
                                            BeginQuantity = a.BeginQuantity,
                                            BeginAmount = a.BeginAmount,
                                            EndQuantity = a.EndQuantity,
                                            EndAmount = a.EndAmount,
                                            Ratio = b.RatioAll,
                                            Quantity0 = Math.Round(a.Quantity * b.RatioAll / 100 ?? 0, 10),
                                            Amount0 = Math.Round(a.Amount * b.RatioAll / 100 ?? 0, 10),
                                            AmountCur0 = Math.Round(a.AmountCur * b.RatioAll / 100 ?? 0, 10),
                                            BeginAmount0 = Math.Round(a.BeginAmount * b.RatioAll / 100 ?? 0, 10),
                                            BeginQuantity0 = Math.Round(a.BeginQuantity * b.RatioAll / 100 ?? 0, 10),
                                            EndAmount0 = Math.Round(a.EndAmount * b.RatioAll / 100 ?? 0, 10),
                                            EndQuantity0 = Math.Round(a.EndQuantity * b.RatioAll / 100 ?? 0, 10)
                                        };
                var lstNoWorkPlaceCode = iQNoWorkPlaceCode.ToList();
                lstAllocationCostIncurred = Enumerable.Concat(lstAllocationCostIncurred, lstNoWorkPlaceCode).ToList();

                // Tính phần chênh lệch tiền do phân bổ
                var iQAllocationCostIncurredRemaining = from a in lstIncurredExpenses
                                                        join b in lstAllocationCostIncurred on a.Id equals b.AllocationId
                                                        group new { a, b } by new
                                                        {
                                                            a.Id,
                                                            a.Quantity,
                                                            a.AmountCur,
                                                            a.Amount,
                                                            a.BeginQuantity,
                                                            a.BeginAmount,
                                                            a.EndAmount,
                                                            a.EndQuantity
                                                        } into gr
                                                        where gr.Key.Quantity - gr.Sum(p => p.b.Quantity0) != 0
                                                           || gr.Key.Amount - gr.Sum(p => p.b.Amount0) != 0
                                                           || gr.Key.AmountCur - gr.Sum(p => p.b.AmountCur0) != 0
                                                           || gr.Key.BeginQuantity - gr.Sum(p => p.b.BeginQuantity0) != 0
                                                           || gr.Key.BeginAmount - gr.Sum(p => p.b.BeginAmount0) != 0
                                                           || gr.Key.EndAmount - gr.Sum(p => p.b.EndAmount0) != 0
                                                           || gr.Key.EndQuantity - gr.Sum(p => p.b.EndQuantity0) != 0
                                                        select new AllocationCostIncurredDto
                                                        {
                                                            Id = gr.Key.Id,
                                                            Quantity = gr.Key.Quantity - gr.Sum(p => p.b.Quantity0),
                                                            Amount = gr.Key.Amount - gr.Sum(p => p.b.Amount0),
                                                            AmountCur = gr.Key.AmountCur - gr.Sum(p => p.b.AmountCur0),
                                                            BeginQuantity = gr.Key.BeginQuantity - gr.Sum(p => p.b.BeginQuantity0),
                                                            BeginAmount = gr.Key.BeginAmount - gr.Sum(p => p.b.BeginAmount0),
                                                            EndAmount = gr.Key.EndAmount - gr.Sum(p => p.b.EndAmount0),
                                                            EndQuantity = gr.Key.EndQuantity - gr.Sum(p => p.b.EndQuantity0),
                                                        };
                var lstAllocationCostIncurredRemaining = iQAllocationCostIncurredRemaining.ToList();
                foreach (var itemACR in lstAllocationCostIncurredRemaining)
                {
                    foreach (var itemAllocationCostIncurred in lstAllocationCostIncurred)
                    {
                        // Số lượng
                        if (itemACR.Quantity != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.Quantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.Quantity0 += itemACR.Quantity;
                            }
                        }

                        // Tiền
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.Amount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.Amount0 += itemACR.Amount;
                            }
                        }

                        // Tiền ngoại tệ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.AmountCur0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.AmountCur0 += itemACR.AmountCur;
                            }
                        }

                        // Số lượng đầu kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.BeginQuantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.BeginQuantity0 += itemACR.BeginQuantity;
                            }
                        }

                        // Tiền đầu kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.BeginAmount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.BeginAmount0 += itemACR.BeginAmount;
                            }
                        }

                        // Số lượng cuối kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.EndQuantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.EndQuantity0 += itemACR.EndQuantity;
                            }
                        }

                        // Tiền cuối kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.EndAmount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.EndAmount0 += itemACR.EndAmount;
                            }
                        }
                    }
                }

                // End Tính phần chênh lệch tiền do phân bổ

                // Insert dữ liệu vào InfoZ
                var lstAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit() ,_webHelper.GetCurrentYear());

                var iQInfoZ = from a in lstAllocationCostIncurred
                              join b in lstAccountSystem on new { a.Year, a.AccCode } equals new { b.Year, b.AccCode } into ajb
                              from b in ajb.DefaultIfEmpty()
                              select new CrudInfoZDto
                              {
                                  Id = GetNewObjectId(),
                                  OrgCode = _webHelper.GetCurrentOrgUnit(),
                                  FProductWork = itemAFC.FProductWork,
                                  Year = itemAFC.Year,
                                  Month = filterDto.ToDate.Month,
                                  BeginM = filterDto.FromDate,
                                  EndM = filterDto.ToDate,
                                  OrdGrp = filterDto.OrdGrp,
                                  Type = filterDto.Type,
                                  AllotmentForwardCode = itemAFC.Code,
                                  ProductionPeriodCode = filterDto.ProductionPeriodCode,
                                  DebitAcc = (itemAFC.DebitCredit == 1 ? a.AccCode : accCode0),
                                  DebitSectionCode = (itemAFC.DebitCredit == 1 ? a.SectionCode : sectionCode0),
                                  DebitFProductWorkCode = ((itemAFC.DebitCredit == 1 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 2 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  CreditAcc = (itemAFC.DebitCredit == 2 ? a.AccCode : accCode0),
                                  CreditSectionCode = (itemAFC.DebitCredit == 2 ? a.SectionCode : sectionCode0),
                                  CreditFProductWorkCode = ((itemAFC.DebitCredit == 2 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 1 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  WorkPlaceCode = a.WorkPlaceCode,
                                  FProductWorkCode = (a.FProductWorkCode == "") ? a.FProductWorkCode0 : a.FProductWorkCode,
                                  ProductCode = a.ProductCode,
                                  PartnerCode = a.PartnerCode,
                                  ContractCode = a.ContractCode,
                                  Quantity = a.Quantity0 ?? 0,
                                  Amount = a.Amount0 ?? 0,
                                  AmountCur = a.AmountCur0 ?? 0,
                                  RecordBook = itemAFC.RecordBook,
                                  Ratio = a.Ratio ?? 0,
                                  BeginQuantity = a.BeginQuantity0 ?? 0,
                                  BeginAmount = a.BeginAmount0 ?? 0,
                                  EndQuantity = a.EndQuantity0 ?? 0,
                                  EndAmount = a.EndAmount0 ?? 0,
                              };
                var lstInfoZ = iQInfoZ.Select(p => ObjectMapper.Map<CrudInfoZDto, InfoZ>(p)).ToList();
                await _infoZService.CreateManyAsync(lstInfoZ, true);
            }
        }

        private async Task AllotmentForwardAutoGrpL(AutoAllotmentForwardGrpFilterDto filterDto) // PB_KC_TD_GRP_L
        {
            // khai báo đầu phiếu định mức sp
            var fProductWorkNorm = await _fProductWorkNormService.GetQueryableAsync();
            fProductWorkNorm = fProductWorkNorm.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNorm = fProductWorkNorm.ToList();
            //khai báo chi tiết định mức
            var fProductWorkNormDetail = await _fProductWorkNormDetailService.GetQueryableAsync();
            fProductWorkNormDetail = fProductWorkNormDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNormDetail = fProductWorkNormDetail.ToList();
            //khai báo chi tiết nhóm hệ số
            var groupCoefficientDetail = await _groupCoefficientDetailService.GetQueryableAsync();
            groupCoefficientDetail = groupCoefficientDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.FProductWork == filterDto.FProductWork
                                                                    && p.Year == filterDto.Year);
            var lstGroupCoefficientDetail = groupCoefficientDetail.Select(p => ObjectMapper.Map<GroupCoefficientDetail, GroupCoefficientDetailDto>(p)).ToList();

            //khai báo infoZ
            var infoZ0 = await _infoZService.GetQueryableAsync();
            infoZ0 = infoZ0.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                    && p.FProductWork == filterDto.FProductWork
                                    && p.Year == filterDto.Year
                                    && p.BeginM >= filterDto.FromDate
                                    && p.EndM <= filterDto.ToDate);
            var lstInfoZ0 = infoZ0.ToList();
            // Lấy list mã thành phẩm
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherCode = voucherType.Where(p => p.Code == "PTP").Select(p => p.ListVoucher).First();
            var getQuantityFProductFilterDto = new GetQuantityFProductFilterDto
            {
                Year = filterDto.Year,
                FromDate = filterDto.FromDate,
                ToDate = filterDto.ToDate,
                LstVoucherCode = lstVoucherCode,
                ProductionPeriodCode = filterDto.ProductionPeriodCode
            };
            // Lấy dữ liệu số lượng thành phẩm
            var lstQuantityFProduct = await GetQuantityFProduct(getQuantityFProductFilterDto);
            // Xóa dữ liệu InfoZ
            var infoZDelete = await _infoZService.GetQueryableAsync();
            infoZDelete = infoZDelete.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && p.ProductionPeriodCode == filterDto.ProductionPeriodCode
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && ((p.BeginM >= filterDto.FromDate && p.BeginM <= filterDto.ToDate) || (p.EndM >= filterDto.FromDate && p.EndM <= filterDto.ToDate)));
            await _infoZService.DeleteManyAsync(infoZDelete, true);

            // lấy dữ liệu AllotmentForwardCategory
            var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
            allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && (p.ProductionPeriodCode ?? "") == (filterDto.ProductionPeriodCode ?? "")
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && p.Active == 1
                                              && p.DecideApply == filterDto.DecideApply).OrderBy(p => p.Code).ToList();
            var lstAllotmentForwardCategory = allotmentForwardCategory.ToList();
            foreach (var itemAFC in lstAllotmentForwardCategory)
            {
                var accCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditAcc : itemAFC.DebitAcc;
                var sectionCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditSectionCode : itemAFC.DebitSectionCode;
                var accCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitAcc : itemAFC.CreditAcc;
                var sectionCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitSectionCode : itemAFC.CreditSectionCode;
                var iQAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
                iQAccountSystem = iQAccountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                          && p.Year == _webHelper.GetCurrentYear()
                                                          && p.AccCode == accCode0).ToList();
                var dataAccountSystem = iQAccountSystem.FirstOrDefault();
                // Update dở dang cuối kỳ nếu có
                if ("S,C".Contains(filterDto.FProductWork))
                {
                    var updatePriceUnfinishedInventoryFilterDto = new UpdatePriceUnfinishedInventoryFilterDto
                    {
                        Year = filterDto.Year,
                        FromDate = filterDto.FromDate,
                        ToDate = filterDto.ToDate,
                        AccCode = accCode,
                        SectionCode = sectionCode,
                        DebitCredit = itemAFC.DebitCredit
                    };
                    await UpdatePriceUnfinishedInventory(updatePriceUnfinishedInventoryFilterDto);
                }
                // - - - -
                var getIncurredExpensesFilterDto = new GetIncurredExpensesFilterDto
                {
                    Year = filterDto.Year,
                    FromDate = filterDto.FromDate,
                    ToDate = filterDto.ToDate,
                    AccCode = accCode,
                    SectionCode = sectionCode,
                    DebitCredit = itemAFC.DebitCredit,
                    OrdRec = null,
                    ForwardType = itemAFC.ForwardType,
                    FProductWork = filterDto.FProductWork,
                    AttachProduct = itemAFC.AttachProduct
                };
                var lstIncurredExpenses = await GetIncurredExpenses(getIncurredExpensesFilterDto);
                // Phân bổ từ lstIncurredExpenses vào bảng lstQuantityFProduct
                var lstGroupCoefficientDetail2 = lstGroupCoefficientDetail.Where(p => (p.GroupCoefficientCode ?? "") == (itemAFC.GroupCoefficientCode ?? "")).ToList();
                var iQRatio = from a in lstInfoZ0
                              join d in lstGroupCoefficientDetail2 on a.FProductWorkCode equals d.FProductWorkCode into ajd
                              from d in ajd.DefaultIfEmpty()
                              where itemAFC.LstCode.Contains(a.AllotmentForwardCode)
                                 && (itemAFC.GroupCoefficientCode == null || itemAFC.GroupCoefficientCode == "" || (d != null && d.FProductWorkCode != null))
                              group new { a, d } by new
                              {
                                  a.OrgCode,
                                  a.WorkPlaceCode,
                                  a.ProductCode,
                                  a.FProductWorkCode
                              } into gr
                              select new RatioDto
                              {
                                  Id = GetNewObjectId(),
                                  OrgCode = gr.Key.OrgCode,
                                  WorkPlaceCode = gr.Key.WorkPlaceCode,
                                  ProductCode = gr.Key.ProductCode,
                                  FProductWorkCode = gr.Key.FProductWorkCode,
                                  Amount = gr.Sum(p => p.a.Amount),
                                  AmountCur = gr.Sum(p => p.a.AmountCur),
                              };
                var lstRatio = iQRatio.ToList();
                // Update giá trị phân bổ
                var iQGroupRatio = from a in lstRatio
                                   group new { a } by new { a.WorkPlaceCode } into gr
                                   orderby gr.Key.WorkPlaceCode
                                   select new
                                   {
                                       WorkPlaceCode = gr.Key.WorkPlaceCode,
                                       TotalAmount = gr.Sum(p => p.a.Amount),
                                       TotalAmountCur = gr.Sum(p => p.a.AmountCur)
                                   };
                var lstGroupRatio = iQGroupRatio.ToList();
                foreach (var itemGroupRatio in lstGroupRatio)
                {
                    if (itemGroupRatio.TotalAmount != 0)
                    {
                        // update Ty_le
                        decimal totalAllotmentValueItemRatio = 0;
                        foreach (var itemRatio in lstRatio)
                        {
                            if (itemRatio.WorkPlaceCode == itemGroupRatio.WorkPlaceCode)
                            {
                                itemRatio.Ratio = Math.Round(itemRatio.Amount * 100 / itemGroupRatio.TotalAmount ?? 0, 10);
                                totalAllotmentValueItemRatio += itemRatio.Ratio ?? 0;
                            }
                        }
                        // update tỉ lệ của phần tử đầu tiên trong trường hợp làm tròn số mà tổng tỷ lệ khác 100%
                        if (totalAllotmentValueItemRatio != 100)
                        {
                            var idItemRatio = lstRatio.Where(p => p.WorkPlaceCode == itemGroupRatio.WorkPlaceCode
                                                               && p.Ratio != 0).Select(p => p.Id).FirstOrDefault();
                            if (idItemRatio != null && idItemRatio != "")
                            {
                                foreach (var itemRatio in lstRatio)
                                {
                                    if (itemRatio.Id == idItemRatio) itemRatio.Ratio += 100 - totalAllotmentValueItemRatio;
                                    break;
                                }
                            }
                        }
                    }
                }
                // Tính tỷ lệ theo mã thành phẩm
                var totalAmount = lstRatio.Sum(p => p.Amount);
                var totalAmountCur = lstRatio.Sum(p => p.AmountCur);
                if (totalAmount != 0)
                {
                    // update Ty_le
                    decimal totalAllotmentValueItemRatio = 0;
                    foreach (var itemRatio in lstRatio)
                    {
                        itemRatio.RatioAll = Math.Round(itemRatio.Amount * 100 / totalAmount ?? 0, 10);
                        totalAllotmentValueItemRatio += itemRatio.RatioAll ?? 0;
                    }
                    // update tỉ lệ của phần tử đầu tiên trong trường hợp làm tròn số mà tổng tỷ lệ khác 100%
                    if (totalAllotmentValueItemRatio != 100)
                    {
                        var idItemRatio = lstRatio.Where(p => p.RatioAll != 0).Select(p => p.Id).FirstOrDefault();
                        if (idItemRatio != null && idItemRatio != "")
                        {
                            foreach (var itemRatio in lstRatio)
                            {
                                if (itemRatio.Id == idItemRatio) itemRatio.RatioAll += 100 - totalAllotmentValueItemRatio;
                                break;
                            }
                        }
                    }
                }
                // Phân bổ theo tỷ lệ
                var lstAllocationCostIncurred = new List<AllocationCostIncurredDto>();
                // 1. Có mã phân xưởng
                var iQHaveWorkPlaceCode = from a in lstIncurredExpenses
                                          join b in lstRatio on a.WorkPlaceCode equals b.WorkPlaceCode
                                          where a.WorkPlaceCode != ""
                                          select new AllocationCostIncurredDto
                                          {
                                              Id = GetNewObjectId(),
                                              AllocationId = a.Id,
                                              OrgCode = a.OrgCode,
                                              Year = a.Year,
                                              AccCode = a.AccCode,
                                              PartnerCode = a.PartnerCode,
                                              ContractCode = a.ContractCode,
                                              WorkPlaceCode = a.WorkPlaceCode,
                                              FProductWorkCode = b.FProductWorkCode,
                                              FProductWorkCode0 = b.FProductWorkCode,
                                              SectionCode = a.SectionCode,
                                              ProductCode = a.ProductCode,
                                              Quantity = a.Quantity,
                                              Amount = a.Amount,
                                              AmountCur = a.AmountCur,
                                              BeginQuantity = a.BeginQuantity,
                                              BeginAmount = a.BeginAmount,
                                              EndQuantity = a.EndQuantity,
                                              EndAmount = a.EndAmount,
                                              Ratio = b.Ratio,
                                              Quantity0 = Math.Round(a.Quantity * b.Ratio/100 ?? 0, 10),
                                              Amount0 = Math.Round(a.Amount * b.Ratio / 100 ?? 0, 10),
                                              AmountCur0 = Math.Round(a.AmountCur * b.Ratio / 100 ?? 0, 10),
                                              BeginAmount0 = Math.Round(a.BeginAmount * b.Ratio / 100 ?? 0, 10),
                                              BeginQuantity0 = Math.Round(a.BeginQuantity * b.Ratio / 100 ?? 0, 10),
                                              EndAmount0 = Math.Round(a.EndAmount * b.Ratio / 100 ?? 0, 10),
                                              EndQuantity0 = Math.Round(a.EndQuantity * b.Ratio / 100 ?? 0, 10)
                                          };
                var lstHaveWorkPlaceCode = iQHaveWorkPlaceCode.ToList();
                lstAllocationCostIncurred = Enumerable.Concat(lstAllocationCostIncurred, lstHaveWorkPlaceCode).ToList();

                // 1. Không có mã phân xưởng
                var iQNoWorkPlaceCode = from a in lstIncurredExpenses
                                        join b in lstRatio on a.OrgCode equals b.OrgCode
                                        where a.WorkPlaceCode == ""
                                        select new AllocationCostIncurredDto
                                        {
                                            Id = GetNewObjectId(),
                                            AllocationId = a.Id,
                                            OrgCode = a.OrgCode,
                                            Year = a.Year,
                                            AccCode = a.AccCode,
                                            PartnerCode = a.PartnerCode,
                                            ContractCode = a.ContractCode,
                                            WorkPlaceCode = a.WorkPlaceCode,
                                            FProductWorkCode = b.FProductWorkCode,
                                            FProductWorkCode0 = b.FProductWorkCode,
                                            SectionCode = a.SectionCode,
                                            ProductCode = a.ProductCode,
                                            Quantity = a.Quantity,
                                            Amount = a.Amount,
                                            AmountCur = a.AmountCur,
                                            BeginQuantity = a.BeginQuantity,
                                            BeginAmount = a.BeginAmount,
                                            EndQuantity = a.EndQuantity,
                                            EndAmount = a.EndAmount,
                                            Ratio = b.RatioAll,
                                            Quantity0 = Math.Round(a.Quantity * b.RatioAll/100 ?? 0, 10),
                                            Amount0 = Math.Round(a.Amount * b.RatioAll / 100 ?? 0, 10),
                                            AmountCur0 = Math.Round(a.AmountCur * b.RatioAll / 100 ?? 0, 10),
                                            BeginAmount0 = Math.Round(a.BeginAmount * b.RatioAll / 100 ?? 0, 10),
                                            BeginQuantity0 = Math.Round(a.BeginQuantity * b.RatioAll / 100 ?? 0, 10),
                                            EndAmount0 = Math.Round(a.EndAmount * b.RatioAll / 100 ?? 0, 10),
                                            EndQuantity0 = Math.Round(a.EndQuantity * b.RatioAll / 100 ?? 0, 10)
                                        };
                var lstNoWorkPlaceCode = iQNoWorkPlaceCode.ToList();
                lstAllocationCostIncurred = Enumerable.Concat(lstAllocationCostIncurred, lstNoWorkPlaceCode).ToList();

                // Tính phần chênh lệch tiền do phân bổ
                var iQAllocationCostIncurredRemaining = from a in lstIncurredExpenses
                                                        join b in lstAllocationCostIncurred on a.Id equals b.AllocationId
                                                        group new { a, b } by new
                                                        {
                                                            a.Id,
                                                            a.Quantity,
                                                            a.AmountCur,
                                                            a.Amount,
                                                            a.BeginQuantity,
                                                            a.BeginAmount,
                                                            a.EndAmount,
                                                            a.EndQuantity
                                                        } into gr
                                                        where gr.Key.Quantity - gr.Sum(p => p.b.Quantity0) != 0
                                                           || gr.Key.Amount - gr.Sum(p => p.b.Amount0) != 0
                                                           || gr.Key.AmountCur - gr.Sum(p => p.b.AmountCur0) != 0
                                                           || gr.Key.BeginQuantity - gr.Sum(p => p.b.BeginQuantity0) != 0
                                                           || gr.Key.BeginAmount - gr.Sum(p => p.b.BeginAmount0) != 0
                                                           || gr.Key.EndAmount - gr.Sum(p => p.b.EndAmount0) != 0
                                                           || gr.Key.EndQuantity - gr.Sum(p => p.b.EndQuantity0) != 0
                                                        select new AllocationCostIncurredDto
                                                        {
                                                            Id = gr.Key.Id,
                                                            Quantity = gr.Key.Quantity - gr.Sum(p => p.b.Quantity0),
                                                            Amount = gr.Key.Amount - gr.Sum(p => p.b.Amount0),
                                                            AmountCur = gr.Key.AmountCur - gr.Sum(p => p.b.AmountCur0),
                                                            BeginQuantity = gr.Key.BeginQuantity - gr.Sum(p => p.b.BeginQuantity0),
                                                            BeginAmount = gr.Key.BeginAmount - gr.Sum(p => p.b.BeginAmount0),
                                                            EndAmount = gr.Key.EndAmount - gr.Sum(p => p.b.EndAmount0),
                                                            EndQuantity = gr.Key.EndQuantity - gr.Sum(p => p.b.EndQuantity0),
                                                        };
                var lstAllocationCostIncurredRemaining = iQAllocationCostIncurredRemaining.ToList();
                foreach (var itemACR in lstAllocationCostIncurredRemaining)
                {
                    foreach (var itemAllocationCostIncurred in lstAllocationCostIncurred)
                    {
                        // Số lượng
                        if (itemACR.Quantity != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.Quantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.Quantity0 += itemACR.Quantity;
                            }
                        }

                        // Tiền
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.Amount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.Amount0 += itemACR.Amount;
                            }
                        }

                        // Tiền ngoại tệ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.AmountCur0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.AmountCur0 += itemACR.AmountCur;
                            }
                        }

                        // Số lượng đầu kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.BeginQuantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.BeginQuantity0 += itemACR.BeginQuantity;
                            }
                        }

                        // Tiền đầu kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.BeginAmount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.BeginAmount0 += itemACR.BeginAmount;
                            }
                        }

                        // Số lượng cuối kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.EndQuantity0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.EndQuantity0 += itemACR.EndQuantity;
                            }
                        }

                        // Tiền cuối kỳ
                        if (itemACR.Amount != 0)
                        {
                            var allocationCostIncurredId = lstAllocationCostIncurred.Where(p => p.AllocationId == itemACR.Id && p.EndAmount0 != 0).Select(p => p.Id).FirstOrDefault();
                            if (allocationCostIncurredId != null && allocationCostIncurredId != "")
                            {
                                if (itemAllocationCostIncurred.Id == allocationCostIncurredId) itemAllocationCostIncurred.EndAmount0 += itemACR.EndAmount;
                            }
                        }
                    }
                }

                // End Tính phần chênh lệch tiền do phân bổ

                // Insert dữ liệu vào InfoZ
                var lstAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());

                var iQInfoZ = from a in lstAllocationCostIncurred
                              join b in lstAccountSystem on new { a.Year, a.AccCode } equals new { b.Year, b.AccCode } into ajb
                              from b in ajb.DefaultIfEmpty()
                              select new CrudInfoZDto
                              {
                                  Id = GetNewObjectId(),
                                  OrgCode = _webHelper.GetCurrentOrgUnit(),
                                  FProductWork = itemAFC.FProductWork,
                                  Year = itemAFC.Year,
                                  Month = filterDto.ToDate.Month,
                                  BeginM = filterDto.FromDate,
                                  EndM = filterDto.ToDate,
                                  OrdGrp = filterDto.OrdGrp,
                                  Type = filterDto.Type,
                                  AllotmentForwardCode = itemAFC.Code,
                                  ProductionPeriodCode = filterDto.ProductionPeriodCode,
                                  DebitAcc = (itemAFC.DebitCredit == 1 ? a.AccCode : accCode0),
                                  DebitSectionCode = (itemAFC.DebitCredit == 1 ? a.SectionCode : sectionCode0),
                                  DebitFProductWorkCode = ((itemAFC.DebitCredit == 1 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 2 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  CreditAcc = (itemAFC.DebitCredit == 2 ? a.AccCode : accCode0),
                                  CreditSectionCode = (itemAFC.DebitCredit == 2 ? a.SectionCode : sectionCode0),
                                  CreditFProductWorkCode = ((itemAFC.DebitCredit == 2 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 1 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  WorkPlaceCode = a.WorkPlaceCode,
                                  FProductWorkCode = (a.FProductWorkCode == "") ? a.FProductWorkCode0 : a.FProductWorkCode,
                                  ProductCode = a.ProductCode,
                                  PartnerCode = a.PartnerCode,
                                  ContractCode = a.ContractCode,
                                  Quantity = a.Quantity0 ?? 0,
                                  Amount = a.Amount0 ?? 0,
                                  AmountCur = a.AmountCur0 ?? 0,
                                  RecordBook = itemAFC.RecordBook,
                                  Ratio = a.Ratio ?? 0,
                                  BeginQuantity = a.BeginQuantity0 ?? 0,
                                  BeginAmount = a.BeginAmount0 ?? 0,
                                  EndQuantity = a.EndQuantity0 ?? 0,
                                  EndAmount = a.EndAmount0 ?? 0,
                              };
                var lstInfoZ = iQInfoZ.Select(p => ObjectMapper.Map<CrudInfoZDto, InfoZ>(p)).ToList();
                await _infoZService.CreateManyAsync(lstInfoZ, true);
            }
        }

        private async Task AllotmentForwardAutoGrpT(AutoAllotmentForwardGrpFilterDto filterDto) // PB_KC_TD_GRP_T
        {
            // khai báo đầu phiếu định mức sp
            var fProductWorkNorm = await _fProductWorkNormService.GetQueryableAsync();
            fProductWorkNorm = fProductWorkNorm.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNorm = fProductWorkNorm.ToList();
            //khai báo chi tiết định mức
            var fProductWorkNormDetail = await _fProductWorkNormDetailService.GetQueryableAsync();
            fProductWorkNormDetail = fProductWorkNormDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstFProductWorkNormDetail = fProductWorkNormDetail.ToList();
            //khai báo chi tiết nhóm hệ số
            var groupCoefficientDetail = await _groupCoefficientDetailService.GetQueryableAsync();
            groupCoefficientDetail = groupCoefficientDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                    && p.FProductWork == filterDto.FProductWork
                                                                    && p.Year == filterDto.Year);
            var lstGroupCoefficientDetail = groupCoefficientDetail.Select(p => ObjectMapper.Map<GroupCoefficientDetail, GroupCoefficientDetailDto>(p)).ToList();

            //khai báo infoZ
            var infoZ0 = await _infoZService.GetQueryableAsync();
            infoZ0 = infoZ0.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                    && p.FProductWork == filterDto.FProductWork
                                    && p.Year == filterDto.Year
                                    && p.BeginM >= filterDto.FromDate
                                    && p.EndM <= filterDto.ToDate);
            var lstInfoZ0 = infoZ0.ToList();

            // lấy dữ liệu AllotmentForwardCategory
            var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());
            allotmentForwardCategory = allotmentForwardCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && (p.ProductionPeriodCode ?? "") == (filterDto.ProductionPeriodCode ?? "")
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && p.Active == 1
                                              && p.DecideApply == filterDto.DecideApply).OrderBy(p => p.Code).ToList();
            var lstAllotmentForwardCategory = allotmentForwardCategory.ToList();
            string lstAllotmentForwardCode = null;
            if (filterDto.Type == "V")
            {
                lstAllotmentForwardCode = "";
                var iQGRAllotmentForwardCategory = from a in lstAllotmentForwardCategory
                                                   group new { a } by new
                                                   {
                                                       a.Ord
                                                   } into gr
                                                   orderby gr.Key.Ord
                                                   select new
                                                   {
                                                       Ord = gr.Key.Ord
                                                   };
                var lstGRAllotmentForwardCategory = iQGRAllotmentForwardCategory.ToList();
                foreach (var itemGRAFC in lstGRAllotmentForwardCategory)
                {
                    decimal amountMin = -1;
                    var allotmentForwardCodeVat = "";
                    foreach (var itemAFC in lstAllotmentForwardCategory)
                    {
                        var accCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitAcc : itemAFC.CreditAcc;
                        var sectionCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitSectionCode : itemAFC.CreditSectionCode;
                        var nC = (accCode.StartsWith("333") && itemAFC.ForwardType == 1) ? "C" : "";
                        // - - - - lấy dữ liệu phát sinh chi phí (PsCfZ_Get)
                        var getIncurredExpensesFilterDto = new GetIncurredExpensesFilterDto
                        {
                            Year = filterDto.Year,
                            FromDate = filterDto.FromDate,
                            ToDate = filterDto.ToDate,
                            AccCode = accCode,
                            SectionCode = sectionCode,
                            DebitCredit = itemAFC.DebitCredit,
                            OrdRec = null,
                            ForwardType = itemAFC.ForwardType,
                            FProductWork = filterDto.FProductWork,
                            AttachProduct = "K",
                            NC = nC,
                        };
                        var lstIncurredExpenses = await GetIncurredExpenses(getIncurredExpensesFilterDto); // dữ liệu phát sinh chi phí
                        decimal? amount = 0;
                        amount = lstIncurredExpenses.Select(p => p.Amount).FirstOrDefault();
                        amount = amount ?? 0;
                        if (amountMin == -1)
                        {
                            amountMin = amount ?? 0;
                            if(amountMin > 0) allotmentForwardCodeVat = itemAFC.Code;
                        }
                        else
                        {
                            if (amountMin > 0) 
                            {
                                if (amountMin > amount && amount > 0)
                                { 
                                    allotmentForwardCodeVat = itemAFC.Code;
                                }
                                else
                                {
                                    amount = 0;
                                    allotmentForwardCodeVat = "";
                                }
                            } 
                        }
                    }

                    if (amountMin <= 0) allotmentForwardCodeVat = "";
                    if (allotmentForwardCodeVat != "") lstAllotmentForwardCode += allotmentForwardCodeVat + ",";
                }
            }
            // Xóa dữ liệu InfoZ
            var infoZDelete = await _infoZService.GetQueryableAsync();
            infoZDelete = infoZDelete.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                              && p.Year == filterDto.Year
                                              && p.ProductionPeriodCode == filterDto.ProductionPeriodCode
                                              && p.FProductWork == filterDto.FProductWork
                                              && p.OrdGrp == filterDto.OrdGrp
                                              && p.Type == filterDto.Type
                                              && ((p.BeginM >= filterDto.FromDate && p.BeginM <= filterDto.ToDate) || (p.EndM >= filterDto.FromDate && p.EndM <= filterDto.ToDate)));
            await _infoZService.DeleteManyAsync(infoZDelete, true);
            // - - - 
            lstAllotmentForwardCategory = lstAllotmentForwardCategory.Where(p => lstAllotmentForwardCode == null 
                                                                              || lstAllotmentForwardCode.Contains(p.Code)).ToList();
            foreach (var itemAFC in lstAllotmentForwardCategory)
            {
                var accCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditAcc : itemAFC.DebitAcc;
                var sectionCode0 = itemAFC.DebitCredit == 1 ? itemAFC.CreditSectionCode : itemAFC.DebitSectionCode;
                var accCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitAcc : itemAFC.CreditAcc;
                var sectionCode = itemAFC.DebitCredit == 1 ? itemAFC.DebitSectionCode : itemAFC.CreditSectionCode;
                var iQAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
                iQAccountSystem = iQAccountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                          && p.Year == _webHelper.GetCurrentYear()
                                                          && p.AccCode == accCode0).ToList();
                var dataAccountSystem = iQAccountSystem.FirstOrDefault();
                var nC = (accCode.StartsWith("333") && itemAFC.ForwardType == 1 && filterDto.Type == "V") ? "C" : "";
                // Update dở dang cuối kỳ nếu có
                if ("S,C".Contains(filterDto.FProductWork))
                {
                    var updatePriceUnfinishedInventoryFilterDto = new UpdatePriceUnfinishedInventoryFilterDto
                    {
                        Year = filterDto.Year,
                        FromDate = filterDto.FromDate,
                        ToDate = filterDto.ToDate,
                        AccCode = accCode,
                        SectionCode = sectionCode,
                        DebitCredit = itemAFC.DebitCredit
                    };
                    await UpdatePriceUnfinishedInventory(updatePriceUnfinishedInventoryFilterDto);
                }
                // - - - -
                var getIncurredExpensesFilterDto = new GetIncurredExpensesFilterDto
                {
                    Year = filterDto.Year,
                    FromDate = filterDto.FromDate,
                    ToDate = filterDto.ToDate,
                    AccCode = accCode,
                    SectionCode = sectionCode,
                    DebitCredit = itemAFC.DebitCredit,
                    OrdRec = null,
                    ForwardType = itemAFC.ForwardType,
                    FProductWork = filterDto.FProductWork,
                    AttachProduct = itemAFC.AttachProduct
                };
                var lstIncurredExpenses = await GetIncurredExpenses(getIncurredExpensesFilterDto);

                var lstAccountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());

                var iQInfoZ = from a in lstIncurredExpenses
                              join b in lstAccountSystem on new { a.Year, a.AccCode } equals new { b.Year, b.AccCode } into ajb
                              from b in ajb.DefaultIfEmpty()
                              select new CrudInfoZDto
                              {
                                  Id = GetNewObjectId(),
                                  OrgCode = _webHelper.GetCurrentOrgUnit(),
                                  FProductWork = itemAFC.FProductWork,
                                  Year = itemAFC.Year,
                                  Month = filterDto.ToDate.Month,
                                  BeginM = filterDto.FromDate,
                                  EndM = filterDto.ToDate,
                                  OrdGrp = filterDto.OrdGrp,
                                  Type = filterDto.Type,
                                  AllotmentForwardCode = itemAFC.Code,
                                  ProductionPeriodCode = filterDto.ProductionPeriodCode,
                                  DebitAcc = (itemAFC.DebitCredit == 1 ? a.AccCode : accCode0),
                                  DebitSectionCode = (itemAFC.DebitCredit == 1 ? a.SectionCode : (dataAccountSystem.AttachAccSection == "C" && sectionCode0 == "" ? a.SectionCode : sectionCode0)),
                                  DebitFProductWorkCode = ((itemAFC.DebitCredit == 1 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 2 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  CreditAcc = (itemAFC.DebitCredit == 2 ? a.AccCode : accCode0),
                                  CreditSectionCode = (itemAFC.DebitCredit == 2 ? a.SectionCode : (dataAccountSystem.AttachAccSection == "C" && sectionCode0 == "" ? a.SectionCode : sectionCode0)),
                                  CreditFProductWorkCode = ((itemAFC.DebitCredit == 2 && b.AttachProductCost == "C") || (itemAFC.DebitCredit == 1 && dataAccountSystem.AttachProductCost == "C") ? a.FProductWorkCode : ""),
                                  WorkPlaceCode = a.WorkPlaceCode,
                                  FProductWorkCode = (a.FProductWorkCode == "") ? a.FProductWorkCode0 : a.FProductWorkCode,
                                  ProductCode = a.ProductCode,
                                  PartnerCode = a.PartnerCode,
                                  ContractCode = a.ContractCode,
                                  Quantity = a.Quantity ?? 0,
                                  Amount = a.Amount ?? 0,
                                  AmountCur = a.AmountCur ?? 0,
                                  RecordBook = itemAFC.RecordBook,
                                  Ratio = 100,
                                  BeginQuantity = a.BeginQuantity ?? 0,
                                  BeginAmount = a.BeginAmount ?? 0,
                                  EndQuantity = a.EndQuantity ?? 0,
                                  EndAmount = a.EndAmount ?? 0,
                              };
                var lstInfoZ = iQInfoZ.Select(p => ObjectMapper.Map<CrudInfoZDto, InfoZ>(p)).ToList();
                await _infoZService.CreateManyAsync(lstInfoZ, true);
            }
        }

        public async Task<List<QuantityFProductDto>> GetQuantityFProduct(GetQuantityFProductFilterDto filterDto) // (SL_TP_GET) Lấy dữ liệu số lượng thành phẩm
        {
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            warehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var productVoucher = await _productVoucherService.GetQueryableAsync();
            productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
            productVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var product = await _productService.GetQueryableAsync();
            product = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstProduct = product.ToList();
            var quantityBegin = from a in productVoucher
                                join b in productVoucherDetail on a.Id equals b.ProductVoucherId into ajb
                                from b in ajb.DefaultIfEmpty()
                                join c in product on b.ProductCode equals c.Code into bjc
                                from c in bjc
                                where a.Year == filterDto.Year
                                   && a.VoucherDate == filterDto.FromDate
                                   && b.DebitAcc.StartsWith("155")
                                   && b.CreditAcc.StartsWith("154")
                                   && a.VoucherCode == "KKD"
                                   && b.Quantity > 0
                                   && (filterDto.ProductionPeriodCode == null || filterDto.ProductionPeriodCode == "" || c.ProductionPeriodCode == filterDto.ProductionPeriodCode)
                                group new { a, b, c } by new
                                {
                                    b.OrgCode,
                                    b.WorkPlaceCode,
                                    b.ProductCode
                                } into gr
                                select new QuantityFProductDto
                                {
                                    OrgCode = gr.Key.OrgCode,
                                    WorkPlaceCode = gr.Key.WorkPlaceCode,
                                    FProductWorkCode = gr.Max(p => p.b.FProductWorkCode),
                                    ProductCode = gr.Key.ProductCode,
                                    UnfinishedQuantity = gr.Sum(p => p.b.Quantity),
                                    CompletedQuantity = Math.Round(gr.Sum(p => p.b.Quantity * (p.b.HTPercentage == null ? 100 : p.b.HTPercentage) / 100) ?? 0, CostProduction.Round),
                                    HTPercentage = gr.Max(p => (p.b.HTPercentage == null ? 100 : p.b.HTPercentage)),
                                    UnfinishedQuantity2 = 0,
                                    CompletedQuantity2 = 0,
                                    HTPercentage2 = 0
                                };
            var lstQuantityBegin = quantityBegin.ToList();
            foreach (var itemQuantityBegin in lstQuantityBegin)
            {
                itemQuantityBegin.HTPercentage = Math.Round((itemQuantityBegin.CompletedQuantity * 100 / itemQuantityBegin.UnfinishedQuantity) ?? 0, CostProduction.Round);
            }

            var quantityEnd = from a in productVoucher
                                join b in productVoucherDetail on a.Id equals b.ProductVoucherId into ajb
                                from b in ajb.DefaultIfEmpty()
                                join c in product on b.ProductCode equals c.Code into bjc
                                from c in bjc
                                where a.Year == filterDto.Year
                                   && a.VoucherDate == filterDto.ToDate
                                   && b.DebitAcc.StartsWith("155")
                                   && b.CreditAcc.StartsWith("154")
                                   && a.VoucherCode == "KKD"
                                   && b.Quantity > 0
                                   && (filterDto.ProductionPeriodCode == null || filterDto.ProductionPeriodCode == "" || c.ProductionPeriodCode == filterDto.ProductionPeriodCode)
                                group new { a, b, c } by new
                                {
                                    b.OrgCode,
                                    b.WorkPlaceCode,
                                    b.ProductCode
                                } into gr
                                select new QuantityFProductDto
                                {
                                    OrgCode = gr.Key.OrgCode,
                                    WorkPlaceCode = gr.Key.WorkPlaceCode,
                                    FProductWorkCode = gr.Max(p => p.b.FProductWorkCode),
                                    ProductCode = gr.Key.ProductCode,
                                    UnfinishedQuantity2 = gr.Sum(p => p.b.Quantity),
                                    CompletedQuantity2 = Math.Round(gr.Sum(p => p.b.Quantity * (p.b.HTPercentage == null ? 100 : p.b.HTPercentage) / 100) ?? 0, CostProduction.Round),
                                    HTPercentage2 = gr.Max(p => (p.b.HTPercentage == null ? 100 : p.b.HTPercentage)),
                                    UnfinishedQuantity = 0,
                                    CompletedQuantity = 0,
                                    HTPercentage = 0
                                };
            var lstQuantityEnd = quantityEnd.ToList();
            foreach (var itemQuantityEnd in lstQuantityEnd)
            {
                if(itemQuantityEnd.UnfinishedQuantity != 0)
                {
                    itemQuantityEnd.HTPercentage = Math.Round((itemQuantityEnd.CompletedQuantity * 100 / itemQuantityEnd.UnfinishedQuantity) ?? 0, CostProduction.Round);
                }
            }

            var monthFromDate = filterDto.FromDate.Month;
            var monthToDate = filterDto.ToDate.Month;
            var yearFromDate = filterDto.FromDate.Year;

            var grWarehouseBook = from a in warehouseBook
                           join b in product on a.ProductCode equals b.Code
                           where a.Year == filterDto.Year && (a.Status == "1" || a.Status == "0") && a.VoucherDate >= filterDto.FromDate && a.VoucherDate <= filterDto.ToDate
                              && filterDto.LstVoucherCode.Contains(a.VoucherCode)
                              && ((filterDto.ProductionPeriodCode ?? "") == "" || b.ProductionPeriodCode == filterDto.ProductionPeriodCode)
                           group new { a, b } by new
                           {
                               a.OrgCode,
                               a.WorkPlaceCode,
                               b.Code
                           } into gr
                           select new QuantityFProductDto
                           {
                               OrgCode = gr.Key.OrgCode,
                               WorkPlaceCode = gr.Key.WorkPlaceCode,
                               ProductCode = gr.Key.Code,
                               Quantity = gr.Sum(p => p.a.Quantity),
                               HTPercentage = gr.Max(p => p.a.DiscountPercentage == null ? 100 : p.a.DiscountPercentage)
                           };
            var lstWarehouseBook = grWarehouseBook.ToList();
            var quantityFProduct = from a in lstProduct
                                   join b in lstWarehouseBook on a.Code equals b.ProductCode into ajb
                                   from b in ajb.DefaultIfEmpty()
                                   where a.ProductType == "T"
                                   select new QuantityFProductDto
                                   {
                                       OrgCode = a.OrgCode,
                                       UnfinishedQuantity = 0,
                                       HTPercentage = 0,
                                       CompletedQuantity = 0,
                                       WorkPlaceCode = b?.WorkPlaceCode ?? "",
                                       FProductWorkCode = a?.FProductWorkCode ?? "",
                                       ProductCode = a.Code,
                                       Quantity = b?.Quantity ?? 0,
                                       UnfinishedQuantity2 = 0,
                                       HTPercentage2 = 0,
                                       CompletedQuantity2 = 0
                                   };
            var lstQuantityFProduct = quantityFProduct.ToList();
            lstQuantityFProduct = Enumerable.Concat(lstQuantityFProduct, lstQuantityBegin).ToList();
            lstQuantityFProduct = Enumerable.Concat(lstQuantityFProduct, lstQuantityEnd).ToList();

            var res = from a in lstQuantityFProduct
                      group new { a } by new
                      {
                          a.OrgCode,
                          a.FProductWorkCode,
                          a.ProductCode,
                          a.WorkPlaceCode
                      } into gr
                      select new QuantityFProductDto
                      {
                          OrgCode = gr.Key.OrgCode,
                          Year = filterDto.Year,
                          WorkPlaceCode = gr.Key.WorkPlaceCode != null ? gr.Key.WorkPlaceCode : "",
                          ProductCode = gr.Key.ProductCode != null ? gr.Key.ProductCode : "",
                          FProductWorkCode = gr.Key.FProductWorkCode != null ? gr.Key.FProductWorkCode : "",
                          UnfinishedQuantity = gr.Sum(p => p.a.UnfinishedQuantity),
                          HTPercentage = gr.Max(p => p.a.HTPercentage),
                          CompletedQuantity = gr.Sum(p => p.a.CompletedQuantity),
                          Quantity = gr.Sum(p => p.a.Quantity),
                          UnfinishedQuantity2 = gr.Sum(p => p.a.UnfinishedQuantity2),
                          HTPercentage2 = gr.Max(p => p.a.HTPercentage2),
                          CompletedQuantity2 = gr.Sum(p => p.a.CompletedQuantity2),
                      };
            var lstRes = res.Where(p => (p.UnfinishedQuantity ?? 0) + (p.Quantity ?? 0) + (p.UnfinishedQuantity2 ?? 0) != 0).ToList();
            return lstRes;
        }

        private async Task UpdatePriceUnfinishedInventory(UpdatePriceUnfinishedInventoryFilterDto filterDto) //Update_gia_Kkd
        {
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                    && p.VoucherCode == "KKD"
                                                    && (p.Status == "1" || p.Status == "2")
                                                    && p.VoucherDate == filterDto.FromDate);
            var lstProductVoucher0 = productVoucher.ToList();

            var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
            productVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                && p.CreditAcc.StartsWith(filterDto.AccCode)
                                                                && p.SectionCode == filterDto.SectionCode);
            var lstProductVoucherDetail0 = productVoucherDetail.ToList();

            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            warehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var dataWarehouseBook = from a in warehouseBook
                                  where (String.Compare(a.Status, "2") < 0) && a.VoucherGroup == 2
                                        && a.VoucherDate <= filterDto.FromDate && a.VoucherDate >= filterDto.ToDate
                                        && (filterDto.DebitCredit == 2 ? a.DebitAcc : a.CreditAcc).StartsWith(filterDto.AccCode)
                                        && a.SectionCode == filterDto.SectionCode
                                  group new { a } by new
                                  {
                                      a.ProductCode,
                                      a.ProductLotCode,
                                      a.ProductOriginCode,
                                      a.WorkPlaceCode,
                                      a.FProductWorkCode
                                  } into gr
                                  select new UpdatePriceUnfinishedInventoryDto
                                  {
                                      ProductCode = gr.Key.ProductCode,
                                      ProductLotCode = gr.Key.ProductLotCode,
                                      ProductOriginCode = gr.Key.ProductOriginCode,
                                      WorkPlaceCode = gr.Key.WorkPlaceCode,
                                      FProductWorkCode = gr.Key.FProductWorkCode,
                                      ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                                      ExportAmount = gr.Sum(p => p.a.ExportAmount),
                                      BeginQuantity = 0,
                                      BeginAmount = 0,
                                  };
            var lstWarehouseBook = dataWarehouseBook.ToList();

            // Đầu kỳ
            var dataProductVoucher = from a in productVoucher
                                     join b in productVoucherDetail on a.Id equals b.ProductVoucherId
                                     group new { a, b } by new
                                     {
                                         b.ProductCode,
                                         b.ProductLotCode,
                                         b.ProductOriginCode,
                                         b.WorkPlaceCode,
                                         b.FProductWorkCode
                                     } into gr
                                     select new UpdatePriceUnfinishedInventoryDto
                                     {
                                         ProductCode = gr.Key.ProductCode,
                                         ProductLotCode = gr.Key.ProductLotCode,
                                         ProductOriginCode = gr.Key.ProductOriginCode,
                                         WorkPlaceCode = gr.Key.WorkPlaceCode,
                                         FProductWorkCode = gr.Key.FProductWorkCode,
                                         ExportQuantity = 0,
                                         ExportAmount = 0,
                                         BeginQuantity = gr.Sum(p => p.b.Quantity),
                                         BeginAmount = gr.Sum(p => p.b.Amount),
                                     };
            var lstProductVoucher = dataProductVoucher.ToList();
            var data = Enumerable.Concat(lstWarehouseBook, lstProductVoucher);
            var lstData = data.ToList();

            // Update nếu ko có phát sinh sẽ update đầu kỳ vào cuối kỳ
            var iQData = from a in lstData
                         group new { a } by new
                         {
                             a.ProductCode,
                             a.ProductLotCode,
                             a.ProductOriginCode,
                             a.WorkPlaceCode,
                             a.FProductWorkCode
                         } into gr
                         select new UpdatePriceUnfinishedInventoryDto
                         {
                             ProductCode = gr.Key.ProductCode,
                             ProductLotCode = gr.Key.ProductLotCode,
                             ProductOriginCode = gr.Key.ProductOriginCode,
                             WorkPlaceCode = gr.Key.WorkPlaceCode,
                             FProductWorkCode = gr.Key.FProductWorkCode,
                             ExportQuantity = gr.Sum(p => p.a.ExportQuantity),
                             ExportAmount = gr.Sum(p => p.a.ExportAmount),
                             BeginQuantity = gr.Sum(p => p.a.BeginQuantity),
                             BeginAmount = gr.Sum(p => p.a.BeginAmount),
                             ExportPrice = (gr.Sum(p => p.a.ExportQuantity) != 0) ? gr.Sum(p => p.a.ExportAmount)/ gr.Sum(p => p.a.ExportQuantity) : 0,
                             BeginPrice = (gr.Sum(p => p.a.BeginQuantity) != 0) ? gr.Sum(p => p.a.BeginAmount) / gr.Sum(p => p.a.BeginQuantity) : 0,
                         };
            lstData = iQData.ToList();
            var iQProductVoucherDetail = from a in lstProductVoucher0
                                         join b in lstProductVoucherDetail0 on a.Id equals b.ProductVoucherId
                                     join c in lstData on new {b.ProductCode, b.ProductLotCode, b.ProductOriginCode, b.WorkPlaceCode, b.FProductWorkCode } equals new { c.ProductCode, c.ProductLotCode, c.ProductOriginCode, c.WorkPlaceCode, c.FProductWorkCode } into bjc
                                     from c in bjc.DefaultIfEmpty()
                                     where b.Quantity != 0
                                     select b;
            var lstProductVoucherDetail = iQProductVoucherDetail.ToList();
            foreach (var itemProductVoucherDetail in lstProductVoucherDetail)
            {
                var itemData = lstData.Where(p => p.ProductCode == itemProductVoucherDetail.ProductCode
                                        && p.ProductLotCode == itemProductVoucherDetail.ProductLotCode
                                        && p.ProductOriginCode == itemProductVoucherDetail.ProductOriginCode
                                        && p.WorkPlaceCode == itemProductVoucherDetail.WorkPlaceCode
                                        && p.FProductWorkCode == itemProductVoucherDetail.FProductWorkCode
                                        ).FirstOrDefault();
                if (itemData != null)
                {
                    itemProductVoucherDetail.Price = (itemData.ExportPrice == 0) ? itemData.BeginPrice : itemData.ExportPrice;
                    itemProductVoucherDetail.Amount = (itemData.ExportPrice == 0) ? itemData.BeginAmount*itemProductVoucherDetail.Quantity : itemData.ExportPrice* itemProductVoucherDetail.Quantity;
                }
                else
                {
                    itemProductVoucherDetail.Price = 0;
                    itemProductVoucherDetail.Amount = 0;
                }
                await _productVoucherDetailService.UpdateAsync(itemProductVoucherDetail, true);
            }
        }

        private async Task<List<IncurredExpensesDto>> GetIncurredExpenses(GetIncurredExpensesFilterDto filterDto) // PsCfZ_Get lấy dữ liệu phát sinh chi phí
        {
            var year = filterDto.FromDate.Year;
            if (filterDto.ProductionPeriodCode == null) filterDto.ProductionPeriodCode = "";
            var product = await _productService.GetQueryableAsync();
            product = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productT = product.Where(p => p.ProductType == "T").Select(p => p.Code).ToList();
            var productT2 = product.Where(p => p.ProductType != "T"
                                            || (p.ProductType == "T" && String.Compare(p.ProductionPeriodCode, filterDto.ProductionPeriodCode) <= 0)).Select(p => p.Code).ToList();

            //khai báo đầu phiếu hàng hóa
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // khai báo chi tiết danh mục hàng hóa
            var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
            productVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // khai báo đầu kỳ tài khoản
            var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
            accOpeningBalance = accOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                          && p.Year == year);
            // Khai báo sổ cái
            var ledger = await _ledgerService.GetQueryableAsync();
            ledger = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // khai báo danh mục khoản mục
            var accSection = await _accSectionRepository.GetQueryableAsync();
            accSection = accSection.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstAccSection = accSection.ToList();

            // khai báo danh mục tài khoản
            var accountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            accountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                  && p.Year == filterDto.Year
                                                  && p.AttachProductCost != "C").ToList();

            var date = filterDto.FromDate;
            var debitCredit = filterDto.DebitCredit == null ? 2 : filterDto.DebitCredit;
            var productionPeriodCode = filterDto.ProductionPeriodCode == null ? "" : filterDto.ProductionPeriodCode;
            var round = filterDto.Round == null ? "K" : filterDto.Round;
            filterDto.NC = filterDto.NC == null ? "" : filterDto.NC;
            var accCode = filterDto.AccCode == "" ? null : filterDto.AccCode;
            var sectionCode = filterDto.SectionCode == "" ? null : filterDto.SectionCode;
            var removeDuplicate = await _tenantSettingService.GetValue("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            if (filterDto.ForwardType == 2) {
                var dateTenant = await _tenantSettingService.GetValue("M_NGAY_DN_TC", _webHelper.GetCurrentOrgUnit());
                if (dateTenant == null || dateTenant == "") dateTenant = "01/01";
                filterDto.FromDate = DateTime.Parse(dateTenant + "/" + filterDto.FromDate.Year);
            }
            //
            var lstIncurredExpenses = new List<IncurredExpensesDto>();
            if (filterDto.DebitCredit == 2)
            {
                // Nhan So du dau ky va cuoi ky
                if ("SC".Contains(filterDto.FProductWork))
                {
                    // Dau ky
                    var dataBegin = from a in productVoucher
                                    join b in productVoucherDetail on a.Id equals b.ProductVoucherId
                                    where a.VoucherCode == "KKD" && a.VoucherDate == date
                                       && (filterDto.AccCode == null || filterDto.AccCode == "" || b.CreditAcc.StartsWith(filterDto.AccCode))
                                       && (filterDto.SectionCode == null || filterDto.SectionCode == "" || (b.SectionCode ?? "") == filterDto.SectionCode)
                                    group new { a, b } by new
                                    {
                                        a.OrgCode,
                                        a.Year,
                                        b.CreditAcc,
                                        a.PartnerCode0,
                                        b.ContractCode,
                                        b.WorkPlaceCode,
                                        b.FProductWorkCode,
                                        b.SectionCode,
                                        b.ProductCode
                                    } into gr
                                    select new IncurredExpensesDto
                                    {
                                        OrgCode = gr.Key.OrgCode,
                                        Year = gr.Key.Year,
                                        AccCode = gr.Key.CreditAcc,
                                        PartnerCode = gr.Key.PartnerCode0 == null ? "" : gr.Key.PartnerCode0,
                                        ContractCode = gr.Key.ContractCode == null ? "" : gr.Key.ContractCode,
                                        WorkPlaceCode = gr.Key.WorkPlaceCode == null ? "" : gr.Key.WorkPlaceCode,
                                        FProductWorkCode = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                        FProductWorkCode0 = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                        SectionCode = gr.Key.SectionCode == null ? "" : gr.Key.SectionCode,
                                        ProductCode = gr.Key.ProductCode == null ? "" : gr.Key.ProductCode,
                                        Quantity = 0,
                                        Amount = 0,
                                        AmountCur = 0,
                                        BeginQuantity = gr.Sum(p => (p.b.Quantity == null) ? 0 : p.b.Quantity) ?? 0,
                                        BeginAmount = gr.Sum(p => (p.b.Amount == null) ? 0 : p.b.Amount) ?? 0,
                                        EndQuantity = 0,
                                        EndAmount = 0
                                    };
                    var lstBegin = dataBegin.ToList();
                    var eConcatBegin = Enumerable.Concat(lstIncurredExpenses, lstBegin);
                    lstIncurredExpenses = eConcatBegin.ToList();

                    // CUOI KY
                    var dataEnd = from a in productVoucher
                                  join b in productVoucherDetail on a.Id equals b.ProductVoucherId
                                  where a.VoucherCode == "KKD" && a.VoucherDate == filterDto.ToDate
                                     && (filterDto.AccCode == null || filterDto.AccCode == "" || b.CreditAcc.StartsWith(filterDto.AccCode))
                                     && (filterDto.SectionCode == null || filterDto.SectionCode == "" || (b.SectionCode ?? "") == filterDto.SectionCode)
                                  group new { a, b } by new
                                  {
                                      a.OrgCode,
                                      a.Year,
                                      b.CreditAcc,
                                      a.PartnerCode0,
                                      b.ContractCode,
                                      b.WorkPlaceCode,
                                      b.FProductWorkCode,
                                      b.SectionCode,
                                      b.ProductCode
                                  } into gr
                                  select new IncurredExpensesDto
                                  {
                                      OrgCode = gr.Key.OrgCode,
                                      Year = gr.Key.Year,
                                      AccCode = gr.Key.CreditAcc,
                                      PartnerCode = gr.Key.PartnerCode0 == null ? "" : gr.Key.PartnerCode0,
                                      ContractCode = gr.Key.ContractCode == null ? "" : gr.Key.ContractCode,
                                      WorkPlaceCode = gr.Key.WorkPlaceCode == null ? "" : gr.Key.WorkPlaceCode,
                                      FProductWorkCode = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                      FProductWorkCode0 = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                      SectionCode = gr.Key.SectionCode == null ? "" : gr.Key.SectionCode,
                                      ProductCode = gr.Key.ProductCode == null ? "" : gr.Key.ProductCode,
                                      Quantity = 0,
                                      Amount = 0,
                                      AmountCur = 0,
                                      BeginQuantity = 0,
                                      BeginAmount = 0,
                                      EndQuantity = gr.Sum(p => (p.b.Quantity == null) ? 0 : p.b.Quantity) ?? 0,
                                      EndAmount = gr.Sum(p => (p.b.Amount == null) ? 0 : p.b.Amount) ?? 0
                                  };
                    var lstEnd = dataEnd.ToList();
                    var eConcatEnd = Enumerable.Concat(lstIncurredExpenses, lstEnd);
                    lstIncurredExpenses = eConcatEnd.ToList();
                }

                // Neu lay theo so du
                if (filterDto.ForwardType == 2)
                {
                    var dataAccOpeningBalance = from a in accOpeningBalance
                                                where (filterDto.AccCode == null || filterDto.AccCode == "" || a.AccCode.StartsWith(filterDto.AccCode))
                                                   && (filterDto.SectionCode == null || (a.AccSectionCode ?? "") == filterDto.SectionCode)
                                                select new IncurredExpensesDto
                                                {
                                                    OrgCode = a.OrgCode,
                                                    Year = a.Year,
                                                    AccCode = a.AccCode,
                                                    PartnerCode = a.PartnerCode == null ? "" : a.PartnerCode,
                                                    ContractCode = a.ContractCode == null ? "" : a.ContractCode,
                                                    WorkPlaceCode = a.WorkPlaceCode == null ? "" : a.WorkPlaceCode,
                                                    FProductWorkCode = a.FProductWorkCode == null ? "" : a.FProductWorkCode,
                                                    FProductWorkCode0 = a.FProductWorkCode == null ? "" : a.FProductWorkCode,
                                                    SectionCode = a.AccSectionCode == null ? "" : a.AccSectionCode,
                                                    ProductCode = "",
                                                    Quantity = 0,
                                                    Amount = a.Debit - a.Credit,
                                                    AmountCur = a.DebitCur - a.CreditCur,
                                                    BeginQuantity = 0,
                                                    BeginAmount = 0,
                                                    EndQuantity = 0,
                                                    EndAmount = 0
                                                };
                    var lstAccOpeningBalance = dataAccOpeningBalance.ToList();
                    var eConcatAccOpeningBalance = Enumerable.Concat(lstIncurredExpenses, lstAccOpeningBalance);
                    lstIncurredExpenses = eConcatAccOpeningBalance.ToList();
                }

                // Phat sinh tang
                if (filterDto.NC == "" || filterDto.NC == "C")
                {
                    var dataLedger = from a in ledger
                                     where (removeDuplicate != "C" || a.CheckDuplicate != "C")
                                        && (String.Compare(a.Status, "2") < 0)
                                        && a.VoucherDate >= filterDto.FromDate && a.VoucherDate <= filterDto.ToDate
                                        && (filterDto.AccCode == null || filterDto.AccCode == "" || a.DebitAcc.StartsWith(filterDto.AccCode))
                                        && (filterDto.OrdRec == null || a.VoucherId != filterDto.OrdRec)
                                        && (filterDto.SectionCode == null || filterDto.SectionCode == "" || (a.SectionCode ?? "") == filterDto.SectionCode)
                                        && (!"C,S".Contains(filterDto.FProductWork) || filterDto.Round == "K"
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || a.Ord0.Substring(0, 1) != "A"
                                                || !productT.Contains(a.ProductCode)))
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || productT2.Contains(a.ProductCode))))
                                     group new { a } by new
                                     {
                                         a.OrgCode,
                                         a.Year,
                                         a.DebitAcc,
                                         a.DebitPartnerCode,
                                         a.DebitContractCode,
                                         a.DebitWorkPlaceCode,
                                         a.DebitFProductWorkCode,
                                         a.FProductWorkCode,
                                         a.DebitSectionCode,
                                         a.ProductCode,
                                     } into gr
                                     select new IncurredExpensesDto
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         Year = gr.Key.Year,
                                         AccCode = gr.Key.DebitAcc == null ? "" : gr.Key.DebitAcc,
                                         PartnerCode = gr.Key.DebitPartnerCode == null ? "" : gr.Key.DebitPartnerCode,
                                         ContractCode = gr.Key.DebitContractCode == null ? "" : gr.Key.DebitContractCode,
                                         WorkPlaceCode = gr.Key.DebitWorkPlaceCode == null ? "" : gr.Key.DebitWorkPlaceCode,
                                         FProductWorkCode = gr.Key.DebitFProductWorkCode == null ? "" : gr.Key.DebitFProductWorkCode,
                                         FProductWorkCode0 = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                         SectionCode = gr.Key.DebitSectionCode == null ? "" : gr.Key.DebitSectionCode,
                                         ProductCode = gr.Key.ProductCode == null ? "" : gr.Key.ProductCode,
                                         Quantity = gr.Sum(p => p.a.Quantity == null ? 0 : p.a.Quantity) ?? 0,
                                         Amount = gr.Sum(p => p.a.Amount == null ? 0 : p.a.Amount) ?? 0,
                                         AmountCur = gr.Sum(p => p.a.DebitAmountCur == null ? 0 : p.a.DebitAmountCur) ?? 0,
                                         BeginQuantity = 0,
                                         BeginAmount = 0,
                                         EndQuantity = 0,
                                         EndAmount = 0
                                     };
                    var lstLedger = dataLedger.ToList();
                    var eConcatLedger = Enumerable.Concat(lstIncurredExpenses, lstLedger);
                    lstIncurredExpenses = eConcatLedger.ToList();
                }

                // Phat sinh giam
                if (filterDto.NC == "" || filterDto.NC == "N")
                {
                    var dataLedger = from a in ledger
                                     where (removeDuplicate != "C" || a.CheckDuplicate != "N")
                                        && (String.Compare(a.Status, "2") < 0)
                                        && a.VoucherDate >= filterDto.FromDate && a.VoucherDate <= filterDto.ToDate
                                        && (filterDto.AccCode == null || filterDto.AccCode == "" || a.CreditAcc.StartsWith(filterDto.AccCode))
                                        && (filterDto.OrdRec == null || a.VoucherId != filterDto.OrdRec)
                                        && (filterDto.SectionCode == null || (a.SectionCode ?? "") == filterDto.SectionCode)
                                        && (!"C,S".Contains(filterDto.FProductWork) || filterDto.Round == "K"
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || a.Ord0.Substring(0, 1) != "A"
                                                || !productT.Contains(a.ProductCode)))
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || productT2.Contains(a.ProductCode))))
                                     group new { a } by new
                                     {
                                         a.OrgCode,
                                         a.Year,
                                         a.CreditAcc,
                                         a.CreditPartnerCode,
                                         a.CreditContractCode,
                                         a.CreditWorkPlaceCode,
                                         a.CreditFProductWorkCode,
                                         a.FProductWorkCode,
                                         a.CreditSectionCode,
                                         a.ProductCode,
                                     } into gr
                                     select new IncurredExpensesDto
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         Year = gr.Key.Year,
                                         AccCode = gr.Key.CreditAcc == null ? "" : gr.Key.CreditAcc,
                                         PartnerCode = gr.Key.CreditPartnerCode == null ? "" : gr.Key.CreditPartnerCode,
                                         ContractCode = gr.Key.CreditContractCode == null ? "" : gr.Key.CreditContractCode,
                                         WorkPlaceCode = gr.Key.CreditWorkPlaceCode == null ? "" : gr.Key.CreditWorkPlaceCode,
                                         FProductWorkCode = gr.Key.CreditFProductWorkCode == null ? "" : gr.Key.CreditFProductWorkCode,
                                         FProductWorkCode0 = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                         SectionCode = gr.Key.CreditSectionCode == null ? "" : gr.Key.CreditSectionCode,
                                         ProductCode = gr.Key.ProductCode == null ? "" : gr.Key.ProductCode,
                                         Quantity = gr.Sum(p => p.a.Quantity == null ? 0 : p.a.Quantity * (-1)) ?? 0,
                                         Amount = gr.Sum(p => p.a.Amount == null ? 0 : p.a.Amount * (-1)) ?? 0,
                                         AmountCur = gr.Sum(p => p.a.CreditAmountCur == null ? 0 : p.a.CreditAmountCur * (-1)) ?? 0,
                                         BeginQuantity = 0,
                                         BeginAmount = 0,
                                         EndQuantity = 0,
                                         EndAmount = 0
                                     };
                    var lstLedger = dataLedger.ToList();
                    var eConcatLedger = Enumerable.Concat(lstIncurredExpenses, lstLedger);
                    lstIncurredExpenses = eConcatLedger.ToList();
                }
            }
            else
            {
                // Neu lay theo so du
                if (filterDto.ForwardType == 2)
                {
                    var dataAccOpeningBalance = from a in accOpeningBalance
                                                where (filterDto.AccCode == null || filterDto.AccCode == "" || a.AccCode.StartsWith(filterDto.AccCode))
                                                   && (filterDto.SectionCode == null || (a.AccSectionCode ?? "") == filterDto.SectionCode)
                                                select new IncurredExpensesDto
                                                {
                                                    OrgCode = a.OrgCode,
                                                    Year = a.Year,
                                                    AccCode = a.AccCode,
                                                    PartnerCode = a.PartnerCode == null ? "" : a.PartnerCode,
                                                    ContractCode = a.ContractCode == null ? "" : a.ContractCode,
                                                    WorkPlaceCode = a.WorkPlaceCode == null ? "" : a.WorkPlaceCode,
                                                    FProductWorkCode = a.FProductWorkCode == null ? "" : a.FProductWorkCode,
                                                    FProductWorkCode0 = a.FProductWorkCode == null ? "" : a.FProductWorkCode,
                                                    SectionCode = a.AccSectionCode == null ? "" : a.AccSectionCode,
                                                    ProductCode = "",
                                                    Quantity = 0,
                                                    Amount = a.Credit - a.Debit,
                                                    AmountCur = a.CreditCur - a.DebitCur,
                                                    BeginQuantity = 0,
                                                    BeginAmount = 0,
                                                    EndQuantity = 0,
                                                    EndAmount = 0
                                                };
                    var lstAccOpeningBalance = dataAccOpeningBalance.ToList();
                    var eConcatAccOpeningBalance = Enumerable.Concat(lstIncurredExpenses, lstAccOpeningBalance);
                    lstIncurredExpenses = eConcatAccOpeningBalance.ToList();
                }

                // Phat sinh tang
                if (filterDto.NC == "" || filterDto.NC == "C")
                {
                    var dataLedger = from a in ledger
                                     where (removeDuplicate != "C" || a.CheckDuplicate != "N")
                                        && (String.Compare(a.Status, "2") < 0)
                                        && a.VoucherDate >= filterDto.FromDate && a.VoucherDate <= filterDto.ToDate
                                        && (filterDto.AccCode == null || filterDto.AccCode == "" || a.CreditAcc.StartsWith(filterDto.AccCode))
                                        && ((filterDto.OrdRec ?? "") == "" || a.VoucherId != filterDto.OrdRec)
                                        && ((filterDto.SectionCode ?? "") == "" || (a.SectionCode ?? "") == filterDto.SectionCode)
                                        && (!"C,S".Contains(filterDto.FProductWork) || filterDto.Round == "K"
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || a.Ord0.Substring(0, 1) != "A"
                                                || !productT.Contains(a.ProductCode)))
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || productT2.Contains(a.ProductCode))))
                                     group new { a } by new
                                     {
                                         a.OrgCode,
                                         a.Year,
                                         a.CreditAcc,
                                         a.CreditPartnerCode,
                                         a.CreditContractCode,
                                         a.CreditWorkPlaceCode,
                                         a.CreditFProductWorkCode,
                                         a.FProductWorkCode,
                                         a.CreditSectionCode,
                                         a.ProductCode,
                                     } into gr
                                     select new IncurredExpensesDto
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         Year = gr.Key.Year,
                                         AccCode = gr.Key.CreditAcc == null ? "" : gr.Key.CreditAcc,
                                         PartnerCode = gr.Key.CreditPartnerCode == null ? "" : gr.Key.CreditPartnerCode,
                                         ContractCode = gr.Key.CreditContractCode == null ? "" : gr.Key.CreditContractCode,
                                         WorkPlaceCode = gr.Key.CreditWorkPlaceCode == null ? "" : gr.Key.CreditWorkPlaceCode,
                                         FProductWorkCode = gr.Key.CreditFProductWorkCode == null ? "" : gr.Key.CreditFProductWorkCode,
                                         FProductWorkCode0 = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                         SectionCode = gr.Key.CreditSectionCode == null ? "" : gr.Key.CreditSectionCode,
                                         ProductCode = gr.Key.ProductCode == null ? "" : gr.Key.ProductCode,
                                         Quantity = gr.Sum(p => p.a.Quantity == null ? 0 : p.a.Quantity) ?? 0,
                                         Amount = gr.Sum(p => p.a.Amount == null ? 0 : p.a.Amount) ?? 0,
                                         AmountCur = gr.Sum(p => p.a.CreditAmountCur == null ? 0 : p.a.CreditAmountCur) ?? 0,
                                         BeginQuantity = 0,
                                         BeginAmount = 0,
                                         EndQuantity = 0,
                                         EndAmount = 0
                                     };
                    var lstLedger = dataLedger.ToList();
                    var eConcatLedger = Enumerable.Concat(lstIncurredExpenses, lstLedger);
                    lstIncurredExpenses = eConcatLedger.ToList();
                }

                // Phat sinh giam
                if (filterDto.NC == "" || filterDto.NC == "N")
                {
                    var dataLedger = from a in ledger
                                     where (removeDuplicate != "C" || a.CheckDuplicate != "C")
                                        && (String.Compare(a.Status, "2") < 0)
                                        && a.VoucherDate >= filterDto.FromDate && a.VoucherDate <= filterDto.ToDate
                                        && (filterDto.AccCode == null || filterDto.AccCode == "" || a.DebitAcc.StartsWith(filterDto.AccCode))
                                        && (filterDto.OrdRec == null || a.VoucherId != filterDto.OrdRec)
                                        && (filterDto.SectionCode == null || (a.SectionCode ?? "") == filterDto.SectionCode)
                                        && (!"C,S".Contains(filterDto.FProductWork) || filterDto.Round == "K"
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || a.Ord0.Substring(0, 1) != "A"
                                                || !productT.Contains(a.ProductCode)))
                                            || (filterDto.ProductionPeriodCode == "" &&
                                                (a.ProductCode == ""
                                                || productT2.Contains(a.ProductCode))))
                                     group new { a } by new
                                     {
                                         a.OrgCode,
                                         a.Year,
                                         a.DebitAcc,
                                         a.DebitPartnerCode,
                                         a.DebitContractCode,
                                         a.DebitWorkPlaceCode,
                                         a.DebitFProductWorkCode,
                                         a.FProductWorkCode,
                                         a.DebitSectionCode,
                                         a.ProductCode,
                                     } into gr
                                     select new IncurredExpensesDto
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         Year = gr.Key.Year,
                                         AccCode = gr.Key.DebitAcc == null ? "" : gr.Key.DebitAcc,
                                         PartnerCode = gr.Key.DebitPartnerCode == null ? "" : gr.Key.DebitPartnerCode,
                                         ContractCode = gr.Key.DebitContractCode == null ? "" : gr.Key.DebitContractCode,
                                         WorkPlaceCode = gr.Key.DebitWorkPlaceCode == null ? "" : gr.Key.DebitWorkPlaceCode,
                                         FProductWorkCode = gr.Key.DebitFProductWorkCode == null ? "" : gr.Key.DebitFProductWorkCode,
                                         FProductWorkCode0 = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                         SectionCode = gr.Key.DebitSectionCode == null ? "" : gr.Key.DebitSectionCode,
                                         ProductCode = gr.Key.ProductCode == null ? "" : gr.Key.ProductCode,
                                         Quantity = gr.Sum(p => p.a.Quantity == null ? 0 : p.a.Quantity * (-1)) ?? 0,
                                         Amount = gr.Sum(p => p.a.Amount == null ? 0 : p.a.Amount * (-1)) ?? 0,
                                         AmountCur = gr.Sum(p => p.a.DebitAmountCur == null ? 0 : p.a.DebitAmountCur * (-1)) ?? 0,
                                         BeginQuantity = 0,
                                         BeginAmount = 0,
                                         EndQuantity = 0,
                                         EndAmount = 0
                                     };
                    var lstLedger = dataLedger.ToList();
                    var eConcatLedger = Enumerable.Concat(lstIncurredExpenses, lstLedger);
                    lstIncurredExpenses = eConcatLedger.ToList();
                }
            }

            // Update FProductWorkCode0
            var iQIncurredExpenses = from a in lstIncurredExpenses
                                     join b in lstAccSection on a.SectionCode equals b.Code into ajb
                                     from b in ajb.DefaultIfEmpty()
                                     select new IncurredExpensesDto
                                     {
                                         OrgCode = a.OrgCode,
                                         Year = a.Year,
                                         AccCode = a.AccCode,
                                         PartnerCode = a.PartnerCode,
                                         ContractCode = a.ContractCode,
                                         WorkPlaceCode = a.WorkPlaceCode,
                                         FProductWorkCode = a.FProductWorkCode,
                                         FProductWorkCode0 = (a.SectionCode == "" || (b != null && b.AttachProductCost != "C")) ? "" : a.FProductWorkCode0,
                                         SectionCode = a.SectionCode,
                                         ProductCode = a.ProductCode,
                                         Quantity = a.Quantity,
                                         Amount = a.Amount,
                                         AmountCur = a.AmountCur,
                                         BeginQuantity = a.BeginQuantity,
                                         BeginAmount = a.BeginAmount,
                                         EndQuantity = a.EndQuantity,
                                         EndAmount = a.EndAmount
                                     };
            // Update Ma_cs, Ma_km
            iQIncurredExpenses = from a in iQIncurredExpenses
                                 join b in accountSystem on a.AccCode equals b.AccCode into ajb
                                 from b in ajb.DefaultIfEmpty()
                                 select new IncurredExpensesDto
                                 {
                                     OrgCode = a.OrgCode,
                                     Year = a.Year,
                                     AccCode = a.AccCode,
                                     PartnerCode = (b != null ? (b.AttachPartner != "C" ? "" : a.PartnerCode) : a.PartnerCode),
                                     ContractCode = (b != null ? (b.AttachContract != "C" ? "" : a.ContractCode) : a.ContractCode),
                                     WorkPlaceCode = (b != null ? (b.AttachWorkPlace != "C" ? "" : a.WorkPlaceCode) : a.WorkPlaceCode),
                                     FProductWorkCode = b != null ? "" : a.FProductWorkCode,
                                     FProductWorkCode0 = a.FProductWorkCode0,
                                     SectionCode = (b != null ? (b.AttachAccSection != "C" ? "" : a.SectionCode) : a.SectionCode),
                                     ProductCode = a.ProductCode,
                                     Quantity = a.Quantity,
                                     Amount = a.Amount,
                                     AmountCur = a.AmountCur,
                                     BeginQuantity = a.BeginQuantity,
                                     BeginAmount = a.BeginAmount,
                                     EndQuantity = a.EndQuantity,
                                     EndAmount = a.EndAmount
                                 };
            lstIncurredExpenses = iQIncurredExpenses.ToList();
            if (filterDto.AttachProduct != "C")
            {
                foreach (var itemIncurredExpenses in lstIncurredExpenses)
                {
                    itemIncurredExpenses.ProductCode = "";
                }
            }

            // Group lai
            if (!"SC".Contains(filterDto.FProductWork))
            {
                var grData = from a in lstIncurredExpenses
                             group new { a } by new
                             {
                                 a.OrgCode,
                                 a.Year,
                                 a.AccCode,
                                 a.PartnerCode,
                                 a.ContractCode,
                                 a.WorkPlaceCode,
                                 a.FProductWorkCode,
                                 a.SectionCode,
                                 a.ProductCode,
                             } into gr
                             where gr.Sum(p => p.a.Amount) > 0
                             select new IncurredExpensesDto
                             {
                                 Id = GetNewObjectId(),
                                 OrgCode = gr.Key.OrgCode,
                                 Year = gr.Key.Year,
                                 AccCode = gr.Key.AccCode,
                                 PartnerCode = gr.Key.PartnerCode,
                                 ContractCode = gr.Key.ContractCode,
                                 WorkPlaceCode = gr.Key.WorkPlaceCode,
                                 FProductWorkCode = gr.Key.FProductWorkCode,
                                 FProductWorkCode0 = "",
                                 SectionCode = gr.Key.SectionCode,
                                 ProductCode = "",
                                 Quantity = 0,
                                 Amount = gr.Sum(p => p.a.Amount),
                                 AmountCur = gr.Sum(p => p.a.AmountCur),
                                 BeginQuantity = gr.Sum(p => p.a.BeginQuantity),
                                 BeginAmount = gr.Sum(p => p.a.BeginAmount),
                                 EndQuantity = gr.Sum(p => p.a.EndQuantity),
                                 EndAmount = gr.Sum(p => p.a.EndAmount)
                             };
                lstIncurredExpenses = grData.ToList();
                return lstIncurredExpenses;
            }
            else
            {
                if (filterDto.AttachProduct != "C")
                {
                    var grData = from a in lstIncurredExpenses
                                 group new { a } by new
                                 {
                                     a.OrgCode,
                                     a.Year,
                                     a.AccCode,
                                     a.PartnerCode,
                                     a.ContractCode,
                                     a.WorkPlaceCode,
                                     a.FProductWorkCode,
                                     a.FProductWorkCode0,
                                     a.SectionCode,
                                     a.ProductCode,
                                 } into gr
                                 where gr.Sum(p => p.a.Quantity + p.a.Amount + p.a.BeginQuantity + p.a.BeginAmount + p.a.EndQuantity + p.a.EndAmount) > 0
                                 select new IncurredExpensesDto
                                 {
                                     Id = GetNewObjectId(),
                                     OrgCode = gr.Key.OrgCode,
                                     Year = gr.Key.Year,
                                     AccCode = gr.Key.AccCode,
                                     PartnerCode = gr.Key.PartnerCode,
                                     ContractCode = gr.Key.ContractCode,
                                     WorkPlaceCode = gr.Key.WorkPlaceCode,
                                     FProductWorkCode = gr.Key.FProductWorkCode,
                                     FProductWorkCode0 = gr.Key.FProductWorkCode0,
                                     SectionCode = gr.Key.SectionCode,
                                     ProductCode = "",
                                     Quantity = 0,
                                     Amount = gr.Sum(p => p.a.Amount),
                                     AmountCur = gr.Sum(p => p.a.AmountCur),
                                     BeginQuantity = gr.Sum(p => p.a.BeginQuantity),
                                     BeginAmount = gr.Sum(p => p.a.BeginAmount),
                                     EndQuantity = gr.Sum(p => p.a.EndQuantity),
                                     EndAmount = gr.Sum(p => p.a.EndAmount)
                                 };
                    lstIncurredExpenses = grData.ToList();
                    return lstIncurredExpenses;
                }
                else
                {
                    var grData = from a in lstIncurredExpenses
                                 group new { a } by new
                                 {
                                     a.OrgCode,
                                     a.Year,
                                     a.AccCode,
                                     a.PartnerCode,
                                     a.ContractCode,
                                     a.WorkPlaceCode,
                                     a.FProductWorkCode,
                                     a.FProductWorkCode0,
                                     a.SectionCode,
                                     a.ProductCode,
                                 } into gr
                                 where gr.Sum(p => p.a.Quantity + p.a.Amount + p.a.BeginQuantity + p.a.BeginAmount + p.a.EndQuantity + p.a.EndAmount) > 0
                                 select new IncurredExpensesDto
                                 {
                                     Id = GetNewObjectId(),
                                     OrgCode = gr.Key.OrgCode,
                                     Year = gr.Key.Year,
                                     AccCode = gr.Key.AccCode,
                                     PartnerCode = gr.Key.PartnerCode,
                                     ContractCode = gr.Key.ContractCode,
                                     WorkPlaceCode = gr.Key.WorkPlaceCode,
                                     FProductWorkCode = gr.Key.FProductWorkCode,
                                     FProductWorkCode0 = gr.Key.FProductWorkCode0,
                                     SectionCode = gr.Key.SectionCode,
                                     ProductCode = "",
                                     Quantity = gr.Sum(p => p.a.Quantity),
                                     Amount = gr.Sum(p => p.a.Amount),
                                     AmountCur = gr.Sum(p => p.a.AmountCur),
                                     BeginQuantity = gr.Sum(p => p.a.BeginQuantity),
                                     BeginAmount = gr.Sum(p => p.a.BeginAmount),
                                     EndQuantity = gr.Sum(p => p.a.EndQuantity),
                                     EndAmount = gr.Sum(p => p.a.EndAmount)
                                 };
                    lstIncurredExpenses = grData.ToList();
                    return lstIncurredExpenses;
                }
            }
        }

        private async Task UpdatePriceFProduct(UpdatePriceFProductFilterDto filterDto) // (UPDATE_GIA_TP) Update giá thành phẩm
        {
            var year = filterDto.FromDate.Year;
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherCode = voucherType.Where(p => p.Code == "PTP").Select(p => p.Code).ToList();
            var getQuantityFProductFilterDto = new GetQuantityFProductFilterDto
            {
                Year = year,
                FromDate = filterDto.FromDate,
                ToDate = filterDto.ToDate,
                LstVoucherCode = string.Join(",", lstVoucherCode),
                ProductionPeriodCode = filterDto.ProductionPeriodCode,
            };
            var lstQuantityFProduct = await GetQuantityFProduct(getQuantityFProductFilterDto);

            // khai báo đầu phiếu hàng hóa 
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucher0 = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // khai báo chi tiết hàng hóa 
            var productVoucherDtail = await _productVoucherDetailService.GetQueryableAsync();
            var lstProductVoucherDtail0 = productVoucherDtail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // khai báo sổ kho
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            var lstWarehouseBook0 = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // khai báo sổ cái
            var ledger = await _ledgerService.GetQueryableAsync();
            var lstLedger0 = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // khai báo infoZ
            var infoZ = await _infoZService.GetQueryableAsync();
            infoZ = infoZ.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                  && p.Year == year
                                  && p.BeginM >= filterDto.FromDate
                                  && p.EndM <= filterDto.ToDate
                                  && p.DebitAcc.StartsWith("154")
                                  && p.FProductWork == filterDto.FProductWork);
            var lstTotalZ = (from a in infoZ
                          group new { a } by new
                          {
                              a.OrgCode,
                              a.Year,
                              a.WorkPlaceCode,
                              a.FProductWorkCode
                          } into gr
                          select new TotalZDto
                          {
                              OrgCode = gr.Key.OrgCode,
                              WorkPlaceCode = gr.Key.WorkPlaceCode,
                              FProductWorkCode = gr.Key.FProductWorkCode,
                              TotalZ = gr.Sum(p => p.a.BeginAmount + p.a.Amount - p.a.EndAmount)
                          }).ToList();

            lstTotalZ.AddRange((from a in lstProductVoucher0
                                join b in lstProductVoucherDtail0 on a.Id equals b.ProductVoucherId
                                where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == filterDto.FromDate
                                    && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154")
                                select new TotalZDto
                                {
                                    OrgCode = a.OrgCode,
                                    WorkPlaceCode = b.WorkPlaceCode,
                                    FProductWorkCode = b.FProductWorkCode,
                                    TotalZ = b.Amount ?? 0
                                }).ToList());

            lstTotalZ.AddRange((from a in lstProductVoucher0
                                join b in lstProductVoucherDtail0 on a.Id equals b.ProductVoucherId
                                where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == filterDto.ToDate
                                   && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154")
                                   && b.Quantity*b.HTPercentage == 0
                                select new TotalZDto
                                {
                                    OrgCode = a.OrgCode,
                                    WorkPlaceCode = b.WorkPlaceCode,
                                    FProductWorkCode = b.FProductWorkCode,
                                    TotalZ = -1*b.Amount ?? 0
                                }).ToList());

            lstTotalZ = (from a in lstTotalZ
                         group new { a } by new
                        {
                            a.OrgCode,
                            a.WorkPlaceCode,
                            a.FProductWorkCode
                        } into gr
                        select new TotalZDto
                        {
                            OrgCode = gr.Key.OrgCode,
                            WorkPlaceCode = gr.Key.WorkPlaceCode,
                            FProductWorkCode = gr.Key.FProductWorkCode,
                            TotalZ = gr.Sum(p => p.a.TotalZ)
                        }).ToList();

            var iQPriceFProduct = from a in lstQuantityFProduct
                                  join b in lstTotalZ on new { a.WorkPlaceCode, a.FProductWorkCode } equals new { b.WorkPlaceCode, b.FProductWorkCode }
                                  select new PriceFProductDto
                                  {
                                      OrgCode = _webHelper.GetCurrentOrgUnit(),
                                      Year = a.Year,
                                      WorkPlaceCode = a.WorkPlaceCode,
                                      FProductWorkCode = a.FProductWorkCode,
                                      Quantity = a.Quantity,
                                      CompletedQuantity2 = a.CompletedQuantity2,
                                      Price = (a.Quantity + a.CompletedQuantity2 == 0 || b.FProductWorkCode == null) ? 0 : Math.Round(b.TotalZ / (a.Quantity + a.CompletedQuantity2) ?? 0, 6),
                                      Amount = (a.Quantity + a.CompletedQuantity2 == 0) ? 0 : Math.Round(b.TotalZ * a.Quantity / (a.Quantity + a.CompletedQuantity2) ?? 0, 6),
                                      TotalZ = b.TotalZ ?? 0
                                  };
            var lstPriceFProduct = iQPriceFProduct.ToList();
            // lấy dữ liệu phiếu thành phẩm từ sổ kho
            var iQWarehouseBook = from a in lstWarehouseBook0
                                  join b in lstPriceFProduct on new { WorkPlaceCode = a.WorkPlaceCode ?? "", FProductWorkCode = a.FProductWorkCode ?? "" } 
                                                         equals new { WorkPlaceCode = b.WorkPlaceCode ?? "", FProductWorkCode = b.FProductWorkCode ?? "" }
                                  where a.Year == year && (String.Compare(a.Status, "2") < 0)
                                     && a.VoucherDate >= filterDto.FromDate && a.VoucherDate <= filterDto.ToDate
                                     && a.VoucherGroup == 1 && lstVoucherCode.Contains(a.VoucherCode)
                                  select new DataPTPDto
                                  {
                                      Id = a.Id,
                                      OrgCode = a.OrgCode,
                                      Year = a.Year,
                                      ProductVoucherId = a.ProductVoucherId,
                                      Ord0 = a.Ord0,
                                      WorkPlaceCode = a.WorkPlaceCode ?? "",
                                      FProductWorkCode = a.FProductWorkCode,
                                      Quantity = a.Quantity,
                                      Price = b.Price ?? 0,
                                      Amount = Math.Round(a.Quantity * b.Price ?? 0, 6)
                                  };
            var lstDataPTP = iQWarehouseBook.ToList();

            // Tính tiền chênh lệch
            var iQGrDataPTP = from a in lstDataPTP
                              group new { a } by new
                              {
                                  a.OrgCode,
                                  a.Year,
                                  a.WorkPlaceCode,
                                  a.FProductWorkCode
                              } into gr
                              select new
                              {
                                  OrgCode = gr.Key.OrgCode,
                                  Year = gr.Key.Year,
                                  WorkPlaceCode = gr.Key.WorkPlaceCode,
                                  FProductWorkCode = gr.Key.FProductWorkCode,
                                  Amount = gr.Sum(p => p.a.Amount)
                              };
            var lstGrDataPTP = iQGrDataPTP.ToList();
            var iQCalculateAmount = from a in lstPriceFProduct
                                    join b in lstGrDataPTP on new { WorkPlaceCode = a.WorkPlaceCode ?? "", FProductWorkCode = a.FProductWorkCode ?? "" }
                                                       equals new { WorkPlaceCode = b.WorkPlaceCode ?? "", FProductWorkCode = b.FProductWorkCode ?? "" }
                                    where a.Amount - b.Amount != 0
                                    select new
                                    {
                                        WorkPlaceCode = a.WorkPlaceCode,
                                        FProductWorkCode = a.FProductWorkCode,
                                        AmountRemaining = a.Amount - b.Amount
                                    };
            var lstCalculateAmount = iQCalculateAmount.ToList();
            foreach (var itemCalculateAmount in lstCalculateAmount)
            {
                var dataPTP = lstDataPTP.Where(p => p.WorkPlaceCode == itemCalculateAmount.WorkPlaceCode && p.FProductWorkCode == itemCalculateAmount.FProductWorkCode).FirstOrDefault();
                if (dataPTP != null)
                {
                    foreach (var itemDataPTP in lstDataPTP)
                    {
                        if (itemDataPTP.Id == dataPTP.Id) itemDataPTP.Amount += itemCalculateAmount.AmountRemaining;
                    }
                }
            }

            // Update vào đầu phiếu
            var iQGrDataPTPByPVId = from a in lstDataPTP
                                    group new { a } by new
                                    {
                                        a.ProductVoucherId
                                    } into gr
                                    select new
                                    {
                                        ProductVoucherId = gr.Key.ProductVoucherId,
                                        TotalAmount = gr.Sum(p => p.a.Amount)
                                    };
            var lstGrDataPTPByPVId = iQGrDataPTPByPVId.ToList();
            var iQUpdateProductVoucher = from a in lstProductVoucher0
                                         join b in lstGrDataPTPByPVId on a.Id equals b.ProductVoucherId
                                         select a;
            var lstUpdateProductVoucher = iQUpdateProductVoucher.ToList();
            foreach (var itemUpdateProductVoucher in lstUpdateProductVoucher)
            {
                var totalAmount = lstGrDataPTPByPVId.Where(p => p.ProductVoucherId == itemUpdateProductVoucher.Id).Select(p => p.TotalAmount).FirstOrDefault();
                itemUpdateProductVoucher.TotalAmount = totalAmount ?? 0;
                await _productVoucherService.UpdateAsync(itemUpdateProductVoucher, true);
            }

            // Update vào chi tiết
            var iQUpdateProductVoucherDetail = from a in lstProductVoucherDtail0
                                               join b in lstDataPTP on new { a.ProductVoucherId, a.Ord0 } equals new { b.ProductVoucherId, b.Ord0 }
                                               select a;
            var lstUpdateProductVoucherDetail = iQUpdateProductVoucherDetail.ToList();
            foreach (var itemUpdateProductVoucherDetail in lstUpdateProductVoucherDetail)
            {
                var DataPTP = lstDataPTP.Where(p => p.ProductVoucherId == itemUpdateProductVoucherDetail.ProductVoucherId && p.Ord0 == itemUpdateProductVoucherDetail.Ord0).FirstOrDefault();
                if (DataPTP != null)
                {
                    itemUpdateProductVoucherDetail.Amount += DataPTP.Amount;
                    itemUpdateProductVoucherDetail.Price += DataPTP.Price;
                }
                await _productVoucherDetailService.UpdateAsync(itemUpdateProductVoucherDetail);
            }
            lstProductVoucherDtail0 = (await _productVoucherDetailService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            // Update vào sổ kho
            var iQUpdateWarehouseBook = from a in lstWarehouseBook0
                                        join b in lstDataPTP on new { a.ProductVoucherId, a.Ord0 } equals new { b.ProductVoucherId, b.Ord0 }
                                        select a;
            var lstUpdateWarehouseBook = iQUpdateWarehouseBook.ToList();
            foreach (var itemUpdateWarehouseBook in lstUpdateWarehouseBook)
            {
                var DataPTP = lstDataPTP.Where(p => p.ProductVoucherId == itemUpdateWarehouseBook.ProductVoucherId && p.Ord0 == itemUpdateWarehouseBook.Ord0).FirstOrDefault();
                if (DataPTP != null)
                {
                    itemUpdateWarehouseBook.Amount = DataPTP.Amount;
                    itemUpdateWarehouseBook.ImportAmount = DataPTP.Amount;
                    itemUpdateWarehouseBook.Price = DataPTP.Price;
                    itemUpdateWarehouseBook.Price0 = DataPTP.Price;
                }
                await _warehouseBookService.UpdateAsync(itemUpdateWarehouseBook, true);
            }

            // Update vào sổ cái
            var iQUpdateLedger = from a in lstLedger0
                                 join b in lstDataPTP on new { ProductVoucherId = a.VoucherId, a.Ord0 } equals new { ProductVoucherId = b.ProductVoucherId, b.Ord0 }
                                 select a;
            var lstUpdateLedger = iQUpdateLedger.ToList();
            foreach (var itemUpdateLedger in lstUpdateLedger)
            {
                var DataPTP = lstDataPTP.Where(p => p.ProductVoucherId == itemUpdateLedger.VoucherId && p.Ord0 == itemUpdateLedger.Ord0).FirstOrDefault();
                if (DataPTP != null)
                {
                    itemUpdateLedger.Amount = DataPTP.Amount;
                    itemUpdateLedger.Price = DataPTP.Price;
                }
                await _ledgerService.UpdateAsync(itemUpdateLedger, true);
            }

            // Update vào kiểm kê dở dang
            var iQUpdateUnfinishedInventory = from a in lstProductVoucher0
                                              join b in lstProductVoucherDtail0 on a.Id equals b.ProductVoucherId
                                              join c in lstPriceFProduct on new { OrgCode = a.OrgCode, WorkPlaceCode = b.WorkPlaceCode, FProductWorkCode = b.FProductWorkCode }
                                                                     equals new { OrgCode = c.OrgCode, WorkPlaceCode = c.WorkPlaceCode, FProductWorkCode = c.FProductWorkCode }
                                              where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == filterDto.ToDate
                                                 && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154") && b.Quantity != 0
                                              select b;
            var lstUpdateUnfinishedInventory = iQUpdateUnfinishedInventory.ToList();
            foreach (var itemUpdateUnfinishedInventory in lstUpdateUnfinishedInventory)
            {
                var DataPriceFProduct = lstPriceFProduct.Where(p => p.WorkPlaceCode == itemUpdateUnfinishedInventory.WorkPlaceCode && p.FProductWorkCode == itemUpdateUnfinishedInventory.FProductWorkCode).FirstOrDefault();
                if (DataPriceFProduct != null)
                {
                    itemUpdateUnfinishedInventory.Amount = Math.Round((itemUpdateUnfinishedInventory.Quantity * itemUpdateUnfinishedInventory.HTPercentage * DataPriceFProduct.Price) / 100 ?? 0, 6);
                    itemUpdateUnfinishedInventory.Price = DataPriceFProduct.Price;
                }
                await _productVoucherDetailService.UpdateAsync(itemUpdateUnfinishedInventory, true);
            }
            lstProductVoucherDtail0 = (await _productVoucherDetailService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            // Update chênh lệch
            var iQDifference = from a in lstProductVoucher0
                                     join b in lstProductVoucherDtail0 on a.Id equals b.ProductVoucherId
                                     join c in lstPriceFProduct on new { OrgCode = a.OrgCode ?? "", WorkPlaceCode = b.WorkPlaceCode ?? "", FProductWorkCode = b.FProductWorkCode ?? "" }
                                                            equals new { OrgCode = c.OrgCode ?? "", WorkPlaceCode = c.WorkPlaceCode ?? "", FProductWorkCode = c.FProductWorkCode ?? "" }
                                     where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == filterDto.ToDate
                                        && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154") && b.Quantity != 0
                                     group new {a,b,c} by new
                                     {
                                         b.OrgCode,
                                         b.WorkPlaceCode,
                                         b.FProductWorkCode
                                     } into gr
                                     where gr.Sum(p => p.c.TotalZ - p.b.Amount) != 0
                                     select new
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         WorkPlaceCode = gr.Key.WorkPlaceCode,
                                         FProductWorkCode = gr.Key.FProductWorkCode,
                                         AmountRemaining = gr.Sum(p => p.c.TotalZ - p.c.Amount - p.b.Amount)
                                     };
            var lstDifference = iQDifference.ToList();
            if (lstDifference.Count > 0)
            {
                var iQDifferenceFix = from a in lstProductVoucher0
                                      join b in lstProductVoucherDtail0 on a.Id equals b.ProductVoucherId
                                      join c in lstDifference on new { OrgCode = a.OrgCode ?? "", WorkPlaceCode = b.WorkPlaceCode ?? "", FProductWorkCode = b.FProductWorkCode ?? "" }
                                                             equals new { OrgCode = c.OrgCode ?? "", WorkPlaceCode = c.WorkPlaceCode ?? "", FProductWorkCode = c.FProductWorkCode ?? "" }
                                      where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == filterDto.ToDate
                                         && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154")
                                      group new { a, b, c } by new
                                      {
                                          c.OrgCode,
                                          c.WorkPlaceCode,
                                          b.FProductWorkCode,
                                          c.AmountRemaining
                                      } into gr
                                      select new
                                      {
                                          Id = gr.Max(p => p.b.Id),
                                          OrgCode = gr.Key.OrgCode,
                                          WorkPlaceCode = gr.Key.WorkPlaceCode,
                                          FProductWorkCode = gr.Key.FProductWorkCode,
                                          AmountRemaining = gr.Key.AmountRemaining
                                      };
                var lstDifferenceFix = iQDifferenceFix.ToList();
                var iQDifferenceDetail = from a in lstProductVoucherDtail0
                                         join b in lstDifferenceFix on a.Id equals b.Id
                                         select a;
                var lstDifferenceDetail = iQDifferenceDetail.ToList();
                foreach (var itemDifferenceDetail in lstDifferenceDetail)
                {
                    var DataPriceFProduct = lstDifferenceFix.Where(p => p.Id == itemDifferenceDetail.Id).FirstOrDefault();
                    if (DataPriceFProduct != null)
                    {
                        itemDifferenceDetail.Amount += DataPriceFProduct.AmountRemaining;
                    }
                    await _productVoucherDetailService.UpdateAsync(itemDifferenceDetail, true);
                }
            }
            lstProductVoucherDtail0 = (await _productVoucherDetailService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            // Update vào tổng tiền
            var iQGrDataProduct = from a in lstProductVoucher0
                                  join b in lstProductVoucherDtail0 on a.Id equals b.ProductVoucherId
                                  where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == filterDto.ToDate
                                     && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154")
                                  group new { a, b } by new
                                  {
                                      b.ProductVoucherId,
                                  } into gr
                                  select new
                                  {
                                      ProductVoucherId = gr.Key.ProductVoucherId,
                                      TotalAmount = gr.Sum(p => p.b.Amount)
                                  };
            var lstGrDataProduct = iQGrDataProduct.ToList();
            var iQUpdateProductVoucherTotalAmount = from a in productVoucher
                                                    join b in lstGrDataProduct on a.Id equals b.ProductVoucherId
                                                    select a;
            var lstUpdateProductVoucherTotalAmount = iQUpdateProductVoucher.ToList();
            foreach (var itemUpdateProductVoucherTotalAmount in lstUpdateProductVoucherTotalAmount)
            {
                var totalAmount = lstGrDataProduct.Where(p => p.ProductVoucherId == itemUpdateProductVoucherTotalAmount.Id).Select(p => p.TotalAmount).FirstOrDefault();
                itemUpdateProductVoucherTotalAmount.TotalAmount = totalAmount ?? 0;
                await _productVoucherService.UpdateAsync(itemUpdateProductVoucherTotalAmount, true);
            }
        }

        private async Task BalanceTransfer(DateTime date) // CHUYEN_SD_154
        {
            var year = date.Year;
            var dateAdd = date.AddDays(1);
            var yearAdd = dateAdd.Year;
            var m = dateAdd.Month;
            var d = dateAdd.Day;

            // khai báo đầu phiếu hàng hóa 
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                    && p.Year == year && p.VoucherCode == "KKD");

            // khai báo chi tiết hàng hóa 
            var productVoucherDtail = await _productVoucherDetailService.GetQueryableAsync();
            productVoucherDtail = productVoucherDtail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // xóa dữ liệu
            var iQProductVoucherDelete = from a in productVoucher
                                         where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == dateAdd
                                         select a;
            var iQProductVoucherDetailDelete = from a in iQProductVoucherDelete
                                               join b in productVoucherDtail on a.Id equals b.ProductVoucherId
                                         where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == dateAdd
                                         select b;
            productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() 
                                                           && p.Year == year && p.VoucherCode == "KKD" && p.VoucherDate == date).ToList();
            var ord = 1;
            // lấy dữ liệu
            var lstProductVoucherAdd = new List<CrudProductVoucherDto>();
            var lstProductVoucherDetailAdd = new List<CrudProductVoucherDetailDto>();
            foreach (var itemProductVoucher in lstProductVoucher)
            {
                var newId = GetNewObjectId();
                var voucherNumber = "KK" + dateAdd.Month.ToString().PadLeft(2, '0') + "/" + ord.ToString().PadLeft(2, '0');
                var productVoucherAdd = productVoucher.Where(p => p.Id == itemProductVoucher.Id).Select(p => ObjectMapper.Map<ProductVoucher, CrudProductVoucherDto>(p)).First();
                productVoucherAdd.Id = newId;
                productVoucherAdd.Year = yearAdd;
                productVoucherAdd.VoucherDate = dateAdd;
                productVoucherAdd.VoucherNumber = voucherNumber;
                productVoucherAdd.ProductVoucherDetails = null;
                lstProductVoucherAdd.Add(productVoucherAdd);

                var iQProductVoucherDetailAdd = from a in productVoucher
                                                join b in productVoucherDtail on a.Id equals b.ProductVoucherId
                                                where a.Id == itemProductVoucher.Id
                                                select b;
                var lstItemProductVoucherDetailAdd = iQProductVoucherDetailAdd.Select(p => ObjectMapper.Map<ProductVoucherDetail, CrudProductVoucherDetailDto>(p)).ToList();
                foreach (var itemProductVoucherDetailAdd in lstItemProductVoucherDetailAdd)
                {
                    itemProductVoucherDetailAdd.Id = GetNewObjectId();
                    itemProductVoucherDetailAdd.Year = yearAdd;
                    itemProductVoucherDetailAdd.ProductVoucherId = newId;
                    lstProductVoucherDetailAdd.Add(itemProductVoucherDetailAdd);
                }
                ord++;
            }
            var entityProductVoucher = lstProductVoucherAdd.Select(p => ObjectMapper.Map<CrudProductVoucherDto, ProductVoucher>(p)).ToList();
            var entityProductVoucherDetail = lstProductVoucherDetailAdd.Select(p => ObjectMapper.Map<CrudProductVoucherDetailDto, ProductVoucherDetail>(p)).ToList();
            if (entityProductVoucher.Count() > 0) // có ps mới xóa
            {
                await _productVoucherDetailService.DeleteManyAsync(iQProductVoucherDetailDelete, true);
                await _productVoucherService.DeleteManyAsync(iQProductVoucherDelete, true);
                await _productVoucherService.CreateManyAsync(entityProductVoucher, true);
                await _productVoucherDetailService.CreateManyAsync(entityProductVoucherDetail, true);
            }
        }

        private async Task CreateCapitalTransfer(CreateCapitalTransferFilterDto filterDto) // (TAO_BT_KC_VON) tạo bút toán kết chuyển vốn
        {
            var voucherCode = "ZSX";
            var productCostAcc = (filterDto.ProductCostAcc == null || filterDto.ProductCostAcc == "") ? "632" : filterDto.ProductCostAcc;
            var currencyCode = "VND";
            var description = "Kết chuyển vốn công trình, dự án";

            // khai báo đầu phiếu KT
            var accVoucher = await _accVoucherService.GetQueryableAsync();
            accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // khai báo chi tiết KT
            var accVoucherDetail = await _accVoucherDetailService.GetQueryableAsync();
            accVoucherDetail = accVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            // Kiểm tra chứng từ đã tạo trước đó, có thì xóa đi
            var accVoucherDelete = from a in accVoucher
                                   join b in accVoucherDetail on a.Id equals b.AccVoucherId
                                   where a.VoucherCode == voucherCode && a.VoucherDate == filterDto.ToDate
                                      && b.DebitAcc.StartsWith(productCostAcc)
                                      && b.CreditAcc.StartsWith("154")
                                   group new { a, b } by new
                                   {
                                       a.Id
                                   } into gr
                                   select new
                                   {
                                       Id = gr.Key.Id
                                   };
            var lstDccVoucherDelete = accVoucherDelete.ToList();
            foreach (var itemDccVoucherDelete in lstDccVoucherDelete)
            {
                await DeleteAccVoucher(itemDccVoucherDelete.Id);
            }

            // Tạo bút toán kết chuyển vốn
            var infoz = await _infoZService.GetQueryableAsync();
            infoz = infoz.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var data = from a in infoz
                       where a.BeginM >= filterDto.FromDate
                          && a.BeginM <= filterDto.ToDate
                          && a.DebitAcc.StartsWith("154")
                          && a.FProductWork == filterDto.FProductWork
                       group new { a } by new
                       {
                           a.OrgCode,
                           a.Year,
                           a.DebitAcc,
                           a.FProductWorkCode,
                           a.DebitSectionCode
                       } into gr
                       orderby gr.Key.DebitAcc, gr.Key.FProductWorkCode, gr.Key.DebitSectionCode
                       select new
                       {
                           OrgCode = gr.Key.OrgCode,
                           Year = gr.Key.Year,
                           DebitAcc = productCostAcc,
                           CreditAcc = gr.Key.DebitAcc,
                           FProductWorkCode = gr.Key.FProductWorkCode,
                           SectionCode = gr.Key.DebitSectionCode,
                           TotalZ = gr.Sum(p => p.a.BeginAmount + p.a.Amount - p.a.EndAmount)
                       };
            var totalAmount = data.Sum(p => p.TotalZ);
            var dataAccVoucherDetail = from a in data
                                       orderby a.DebitAcc, a.CreditAcc, a.FProductWorkCode, a.SectionCode
                                       select new CrudAccVoucherDetailDto
                                       {
                                           DebitAcc = a.DebitAcc,
                                           CreditAcc = a.CreditAcc,
                                           FProductWorkCode = a.FProductWorkCode,
                                           SectionCode = a.SectionCode,
                                           Note = description,
                                           Amount = a.TotalZ,
                                           AmountCur = 0
                                       };
            var lstAccVoucherDetail = dataAccVoucherDetail.ToList();
            var crudAccVoucherDto = new CrudAccVoucherDto();
            crudAccVoucherDto.VoucherCode = voucherCode;
            crudAccVoucherDto.VoucherGroup = 3;
            crudAccVoucherDto.VoucherDate = filterDto.ToDate;
            crudAccVoucherDto.Description = description;
            crudAccVoucherDto.CurrencyCode = currencyCode;
            crudAccVoucherDto.ExchangeRate = 1;
            crudAccVoucherDto.TotalAmountWithoutVat = totalAmount ?? 0;
            crudAccVoucherDto.TotalAmountCur = 0;
            crudAccVoucherDto.TotalAmount = totalAmount ?? 0;
            crudAccVoucherDto.Status = "1";
            crudAccVoucherDto.AccVoucherDetails = lstAccVoucherDetail;
            if (lstAccVoucherDetail != null && lstAccVoucherDetail.Count() > 0)
            {
                await CreateAccVoucher(crudAccVoucherDto);
            }
        }

        private async Task CreateVoucher(CreateVoucherDto dto)
        {
            var currencyCode = await _tenantSettingService.GetTenantSettingByKeyAsync("M_MA_NT0", _webHelper.GetCurrentOrgUnit());
            if (currencyCode == null)
            {
                var defaultTenantSetting = await _defaultTenantSettingService.GetByKeyAsync("M_MA_NT0");
                currencyCode = ObjectMapper.Map<DefaultTenantSetting, TenantSetting>(defaultTenantSetting);
            }
            var titleDate = " Từ ngày " + dto.FromDate.Day.ToString().PadLeft(2, '0') + "/" + dto.FromDate.Month.ToString().PadLeft(2, '0') + "/" + dto.FromDate.Year
                          + " đến ngày " + dto.ToDate.Day.ToString().PadLeft(2, '0') + "/" + dto.ToDate.Month.ToString().PadLeft(2, '0') + "/" + dto.ToDate.Year;
            decimal exchangeRate = 1;
            var voucherCode = "PKC";
            if (dto.FProductWork != "A")
            {
                voucherCode = "ZSX";
            }

            // Khai báo infoZ
            var infoz = await _infoZService.GetQueryableAsync();
            infoz = infoz.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());

            // Khai báo danh mục ĐT
            var accPartner = await _accPartnerService.GetQueryableAsync();
            accPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // Khai báo danh mục phân bổ kết chuyển
            var allotmentForwardCategory = await _allotmentForwardCategoryService.GetData(_webHelper.GetCurrentOrgUnit());

            // Khai báo danh mục tài khoản
            var accountSystem = await GetListAccountAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            accountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var iQData = from a in infoz.ToList()
                         join b in accPartner.ToList() on a.PartnerCode equals b.Code into ajb
                         from b in ajb.DefaultIfEmpty()
                         join c in allotmentForwardCategory on new { a.FProductWork, a.Year, AllotmentForwardCode = a.AllotmentForwardCode, a.Type }
                                                        equals new { c.FProductWork, c.Year, AllotmentForwardCode = c.Code, c.Type } into ajc
                         from c in ajc.DefaultIfEmpty()
                         join d in accountSystem on new { a.Year, DebitAcc = a.DebitAcc } equals new { d.Year, DebitAcc = d.AccCode } into ajd
                         from d in ajd.DefaultIfEmpty()
                         join e in accountSystem on new { a.Year, CreditAcc = a.CreditAcc } equals new { e.Year, CreditAcc = e.AccCode } into aje
                         from e in aje.DefaultIfEmpty()
                         where a.Year == dto.Year && a.RecordBook == "C" && a.FProductWork == dto.FProductWork
                            && a.ProductionPeriodCode == dto.ProductionPeriodCode && a.OrdGrp == dto.OrdGrp
                            && a.Type == dto.Type && a.BeginM >= dto.FromDate && a.EndM <= dto.ToDate
                         group new { a, b, c } by new
                         {
                             Ord = (c == null) ? 0 : c.Ord,
                             a.DebitAcc,
                             a.DebitFProductWorkCode,
                             a.DebitSectionCode,
                             a.CreditAcc,
                             a.CreditFProductWorkCode,
                             a.CreditSectionCode,
                             a.WorkPlaceCode,
                             a.FProductWorkCode,
                             a.PartnerCode,
                             PartnerName = b?.Name ?? "",
                             a.ContractCode,
                             AttachProductCostD = d.AttachProductCost ?? "",
                             AttachProductCostE = e.AttachProductCost ?? "",
                             Note = c.Note ?? ""
                         } into gr
                         select new
                         {
                             Ord = gr.Key.Ord,
                             DebitAcc = gr.Key.DebitAcc,
                             DebitFProductWorkCode = (gr.Key.AttachProductCostD == "C" && gr.Key.DebitFProductWorkCode == "") ? gr.Key.FProductWorkCode : gr.Key.DebitFProductWorkCode,
                             DebitSectionCode = gr.Key.DebitSectionCode,
                             CreditAcc = gr.Key.CreditAcc,
                             CreditFProductWorkCode = (gr.Key.AttachProductCostE == "C" && gr.Key.CreditFProductWorkCode == "") ? gr.Key.FProductWorkCode : gr.Key.CreditFProductWorkCode,
                             CreditSectionCode = gr.Key.CreditSectionCode,
                             WorkPlaceCode = gr.Key.WorkPlaceCode,
                             FProductWorkCode = gr.Key.FProductWorkCode,
                             PartnerCode = gr.Key.PartnerCode,
                             PartnerName = gr.Key.PartnerName,
                             ContractCode = gr.Key.ContractCode,
                             Note = gr.Key.Note + titleDate,
                             Amount = gr.Sum(p => p.a.Amount)
                         };
            var lstData = iQData.ToList();
            var iQGRData = from a in lstData
                           group new { a } by new { a.Ord } into gr
                           orderby gr.Key.Ord
                           select new
                           {
                               Ord = gr.Key.Ord,
                               Amount = gr.Sum(p => p.a.Amount),
                               Note = gr.Max(p => p.a.Note)
                           };
            var lstGRData = iQGRData.ToList();
            foreach (var itemData in lstGRData)
            {
                var dataAccVoucherDetail = from a in lstData
                                           where a.Ord == itemData.Ord
                                           group new { a } by new
                                           {
                                               a.DebitAcc,
                                               a.DebitFProductWorkCode,
                                               a.DebitSectionCode,
                                               a.CreditAcc,
                                               a.CreditFProductWorkCode,
                                               a.CreditSectionCode,
                                               a.WorkPlaceCode,
                                               a.FProductWorkCode,
                                               a.PartnerCode,
                                               a.PartnerName,
                                               a.ContractCode,
                                               a.Note
                                           } into gr
                                           orderby gr.Key.DebitAcc, gr.Key.CreditAcc
                                           select new CrudAccVoucherDetailDto
                                           {
                                               DebitAcc = gr.Key.DebitAcc,
                                               CreditAcc = gr.Key.CreditAcc,
                                               FProductWorkCode = gr.Key.DebitFProductWorkCode,
                                               SectionCode = gr.Key.DebitSectionCode,
                                               ClearingFProductWorkCode = gr.Key.CreditFProductWorkCode,
                                               ClearingSectionCode = gr.Key.CreditSectionCode,
                                               PartnerCode = gr.Key.PartnerCode,
                                               PartnerName = gr.Key.PartnerName,
                                               ContractCode = gr.Key.ContractCode,
                                               WorkPlaceCode = gr.Key.WorkPlaceCode,
                                               Note = gr.Key.Note,
                                               Amount = gr.Sum(p => p.a.Amount),
                                               AmountCur = 0,

                                           };
                var lstAccVoucherDetail = dataAccVoucherDetail.ToList();
                var crudAccVoucherDto = new CrudAccVoucherDto();
                crudAccVoucherDto.VoucherCode = voucherCode;
                crudAccVoucherDto.VoucherGroup = 3;
                crudAccVoucherDto.VoucherDate = dto.ToDate;
                crudAccVoucherDto.Description = itemData.Note;
                crudAccVoucherDto.CurrencyCode = currencyCode.Value;
                crudAccVoucherDto.ExchangeRate = exchangeRate;
                crudAccVoucherDto.TotalAmountWithoutVat = itemData.Amount ?? 0;
                crudAccVoucherDto.TotalAmount = itemData.Amount ?? 0;
                crudAccVoucherDto.Status = "1";
                crudAccVoucherDto.AccVoucherDetails = lstAccVoucherDetail;
                var res = await CreateAccVoucher(crudAccVoucherDto);

                var infoExportAuto = new CrudInfoExportAutoDto();
                infoExportAuto.Id = GetNewObjectId();
                infoExportAuto.OrgCode = _webHelper.GetCurrentOrgUnit();
                infoExportAuto.Year = dto.Year;
                infoExportAuto.VoucherCode = voucherCode;
                infoExportAuto.BeginDate = dto.FromDate;
                infoExportAuto.EndDate = dto.ToDate;
                infoExportAuto.EndDate = dto.ToDate;
                infoExportAuto.ProductionPeriodCode = dto.ProductionPeriodCode;
                infoExportAuto.FProductWork = dto.FProductWork;
                infoExportAuto.OrdGrp = dto.OrdGrp;
                infoExportAuto.Type = dto.Type;
                infoExportAuto.OrdRec = res.Id;
                var entity = ObjectMapper.Map<CrudInfoExportAutoDto, InfoExportAuto>(infoExportAuto);
                await _infoExportAutoService.CreateAsync(entity, true);
            }
        }

        private async Task<DateTime> NextMonth(DateTime date)
        {
            var year = date.Year;
            var month = date.Month;
            month++;
            if (month == 13)
            {
                month = 1;
                year++;
            }
            return Convert.ToDateTime(year + "-" + ((month < 10) ? "0" + month : month) + "-01");
        }

        private async Task<DateTime> LastDay(DateTime date)
        {
            var month = date.Month;
            var day = date.Day;
            if (month == 2)
            {
                if (date.Year % 100 != 0 && date.Year % 4 == 0)
                {
                    day = 29;
                }
                else
                {
                    day = 28;
                }
            }
            else if ("135781012".Contains(month.ToString()))
            {
                day = 31;
            }
            else
            {
                day = 30;
            }
            return Convert.ToDateTime(date.Year + "-" + ((month < 10) ? "0" + month : month) + "-" + ((day < 10) ? "0" + day : day));
        }

        private decimal Abs(decimal? item)
        {
            if (item < 0)
            {
                return item * -1 ?? 0;
            }
            else
            {
                return item ?? 0;
            }
        }

        private async Task<AccVoucherDto> CreateAccVoucher(CrudAccVoucherDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            var id = this.GetNewObjectId();
            dto.Id = id;
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = dto.Year != 0 ? dto.Year : _webHelper.GetCurrentYear();
            var unitOfWork = _unitOfWorkManager.Begin();
            if ((dto.BusinessCode ?? "") == "")
            {
                if ((dto.VoucherNumber ?? "") == "")
                {
                    var voucherNumber = await _voucherNumberBusiness.AutoVoucherNumberAsync(dto.VoucherCode, dto.VoucherDate);
                    dto.VoucherNumber = voucherNumber.VoucherNumber;
                }
                else
                {
                    await _voucherNumberBusiness.UpdateVoucherNumberAsync(dto.VoucherCode, dto.VoucherNumber, dto.VoucherDate);
                }
            }
            else
            {
                if ((dto.VoucherNumber ?? "") == "")
                {
                    var voucherNumber = await _voucherNumberBusiness.AutoBusinessVoucherNumberAsync(dto.VoucherCode, dto.BusinessCode, dto.VoucherDate);
                    dto.VoucherNumber = voucherNumber.VoucherNumber;
                }
                else
                {
                    await _voucherNumberBusiness.UpdateBusinessVoucherNumberAsync(dto.VoucherCode, dto.BusinessCode, dto.VoucherNumber, dto.VoucherDate);
                }
            }
            dto = this.MapDetail(dto);
            //await _accVoucherService.CheckAccVoucher(dto);
            var entity = ObjectMapper.Map<CrudAccVoucherDto, AccVoucher>(dto);
            await _accVoucherService.CheckLockVoucher(entity);
            try
            {
                var result = await _accVoucherService.CreateAsync(entity, true);
                //Post sổ cái
                List<CrudLedgerDto> ledgers = await _ledgerService.MapLedger(dto);
                foreach (var ledger in ledgers)
                {
                    ledger.Id = this.GetNewObjectId();
                    var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                    ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity, true);
                }
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AccVoucher, AccVoucherDto>(result);
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        private CrudAccVoucherDto MapDetail(CrudAccVoucherDto dto)
        {
            if (dto.AccVoucherDetails != null)
                for (int i = 0; i < dto.AccVoucherDetails.Count; i++)
                {
                    int ord = i + 1;
                    var accVoucherDetail = dto.AccVoucherDetails[i];
                    accVoucherDetail.Id = this.GetNewObjectId();
                    accVoucherDetail.AccVoucherId = dto.Id;
                    accVoucherDetail.OrgCode = dto.OrgCode;
                    accVoucherDetail.Year = dto.Year;
                    accVoucherDetail.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                }
            if (dto.AccTaxDetails != null)
                for (int i = 0; i < dto.AccTaxDetails.Count; i++)
                {
                    int ord = i + 1;
                    var accTaxDetail = dto.AccTaxDetails[i];
                    accTaxDetail.Id = this.GetNewObjectId();
                    accTaxDetail.AccVoucherId = dto.Id;
                    accTaxDetail.OrgCode = dto.OrgCode;
                    accTaxDetail.VoucherDate = dto.VoucherDate;
                    accTaxDetail.Year = dto.Year;
                    accTaxDetail.Ord0 = "Z" + ord.ToString().PadLeft(9, '0');
                }
            return dto;
        }

        public async Task DeleteAccVoucher(string id)
        {
            var entity = await _accVoucherService.GetAsync(id);
            try
            {
                var ledgers = await _ledgerService.GetByAccVoucherIdAsync(id);
                var accVoucherDetails = await _accVoucherDetailService.GetByAccVoucherIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (ledgers != null)
                {
                    await _ledgerService.DeleteManyAsync(ledgers, true);
                }
                if (accVoucherDetails != null)
                {
                    await _accVoucherDetailService.DeleteManyAsync(accVoucherDetails, true);
                }
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
            await _accVoucherService.DeleteAsync(id, true);
        }
        #endregion
        private async Task<List<AccountSystemDto>> GetListAccountAsync(string orgCode, int year)
        {

            var lstAccountSystem = await _accountingCacheManager.GetAccountSystemsAsync(year);

            if (lstAccountSystem.Count != 0) { return lstAccountSystem; }
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);

            var defaultAccountSystems = await _accountingCacheManager.GetDefaultAccountSystemsAsync(yearCategory.UsingDecision.Value);
            return defaultAccountSystems.Select(p => new AccountSystemDto()
            {
                AccCode = p.AccCode,
                AccName = p.AccName,
                AccNameEn = p.AccNameEn,
                AccNameTemp = p.AccNameTemp,
                AccNameTempE = p.AccNameTempE,
                AccPattern = p.AccPattern,
                AccRank = p.AccRank,
                AccSectionCode = p.AccSectionCode,
                AccType = p.AccType,
                AssetOrEquity = p.AssetOrEquity,
                AttachAccSection = p.AttachAccSection,
                AttachContract = p.AttachContract,
                AttachCurrency = p.AttachCurrency,
                AttachPartner = p.AttachPartner,
                AttachProductCost = p.AttachProductCost,
                AttachVoucher = p.AttachVoucher,
                AttachWorkPlace = p.AttachWorkPlace,
                BankAccountNumber = p.BankAccountNumber,
                BankName = p.BankName,
                CreationTime = p.CreationTime,
                CreatorId = p.CreatorId,
                Id = p.Id,
                IsBalanceSheetAcc = p.IsBalanceSheetAcc,
                LastModificationTime = p.LastModificationTime,
                LastModifierId = p.LastModifierId,
                OrgCode = orgCode,
                ParentAccId = p.ParentAccId,
                Year = year,
                SortPath = p.SortPath,
                Province = p.Province,
                ParentCode = p.ParentCode,
            }).ToList();
        }

    }
}
