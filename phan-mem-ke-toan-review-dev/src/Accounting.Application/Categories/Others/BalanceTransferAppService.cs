using System;
using Accounting.BaseDtos;
using Accounting.Catgories.Others.BusinessCategories;
using System.Threading.Tasks;
using Accounting.DomainServices.Windows;
using Accounting.Windows;
using System.Collections.Generic;
using System.Linq;
using Accounting.Catgories.ProductVouchers;
using Accounting.Reports.Others;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.Categories.AssetTools;
using Accounting.Catgories.AssetTools;
using Accounting.Constants;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Accounting.DomainServices.Categories;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Accounts;
using Accounting.Exceptions;
using Accounting.Categories.CostProductions;
using Accounting.Reports;
using Accounting.DomainServices.Reports;
using Accounting.Catgories.Others.Other;
using Accounting.Reports.Financials;
using Accounting.Catgories.FProductWorkNorms;
using Accounting.Categories.Products;
using Accounting.Catgories.CostProductions;
using Accounting.Reports.Financials.Tenant;
using Accounting.Reports.Statements.T133.Tenants;
using Accounting.DomainServices.Reports.TT200;
using Accounting.Reports.Statements.T200.Tenants;
using NPOI.SS.Formula.Functions;
using Accounting.Catgories.ProductOpeningBalances;
using Accounting.Catgories.Accounts.AccOpeningBalances;
using System.Diagnostics;
using Accounting.DomainServices.Ledgers;
using StackExchange.Redis;
using static NPOI.POIFS.Crypt.CryptoFunctions;
using Accounting.DomainServices.Reports.TT133;
using Accounting.DomainServices.Categories.CostProduction;
using Accounting.Reports.Tenants.TenantStatementTaxs;
using Accounting.Reports.Statements.T200.Defaults;
using Accounting.DomainServices.Configs;
using Volo.Abp.MultiTenancy;
using Accounting.Business;

namespace Accounting.Categories.Others
{
    public class BalanceTransferAppService : AccountingAppService
    {
        private readonly ConfigForwardYearService _configForwardYearService;
        private readonly AssetToolAccountService _assetToolAccountService;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AccountSystemService _accountSystemService;
        private readonly AccountSystemAppService _accountSystemAppService;
        private readonly AllotmentForwardCategoryService _allotmentForwardCategoryService;
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly TenantBusinessResultService _tenantBusinessResultService;
        public readonly TenantCashFollowStatementService _tenantCashFollowStatementService;
        private readonly LicenseBusiness _licenseBusiness;
        public readonly SoTHZService _soTHZService;
        private readonly FProductWorkNormService _fProductWorkNormService;
        private readonly FProductWorkNormDetailService _fProductWorkNormDetailService;
        private readonly GroupCoefficientDetailService _groupCoefficientDetailService;
        private readonly FStatement133L02Service _fStatement133L02Service;
        private readonly FStatement133L01Service _fStatement133L01Service;
        private readonly FStatement133L03Service _fStatement133L03Service;
        private readonly FStatement133L04Service _fStatement133L04Service;
        private readonly FStatement133L05Service _fStatement133L05Service;
        private readonly FStatement133L06Service _fStatement133L06Service;
        private readonly FStatement133L07Service _fStatement133L07Service;
        private readonly FStatement200L01Service _fStatement200L01Service;
        private readonly FStatement200L02Service _fStatement200L02Service;
        private readonly FStatement200L03Service _fStatement200L03Service;
        private readonly FStatement200L04Service _fStatement200L04Service;
        private readonly FStatement200L05Service _fStatement200L05Service;
        private readonly FStatement200L06Service _fStatement200L06Service;
        private readonly FStatement200L07Service _fStatement200L07Service;
        private readonly FStatement200L08Service _fStatement200L08Service;
        private readonly FStatement200L09Service _fStatement200L09Service;
        private readonly FStatement200L10Service _fStatement200L10Service;
        private readonly FStatement200L11Service _fStatement200L11Service;
        private readonly FStatement200L12Service _fStatement200L12Service;
        private readonly FStatement200L13Service _fStatement200L13Service;
        private readonly FStatement200L14Service _fStatement200L14Service;
        private readonly FStatement200L15Service _fStatement200L15Service;
        private readonly FStatement200L16Service _fStatement200L16Service;
        private readonly FStatement200L17Service _fStatement200L17Service;
        private readonly FStatement200L18Service _fStatement200L18Service;
        private readonly FStatement200L19Service _fStatement200L19Service;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly LedgerService _ledgerService;
        private readonly DefaultAllotmentForwardCategoryService _defaultAllotmentForwardCategoryService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly DefaultAccBalanceSheetService _defaultAccBalanceSheetService;
        private readonly DefaultBusinessResultService _defaultBusinessResultService;
        private readonly DefaultCashFollowStatementService _defaultCashFollowStatementService;
        private readonly DefaultStatementTaxService _defaultStatementTaxService;
        private readonly TenantStatementTaxService _tenantStatementTaxService;
        private readonly DefaultFStatement133L01Service _defaultFStatement133L01Service;
        private readonly DefaultFStatement133L02Service _defaultFStatement133L02Service;
        private readonly DefaultFStatement133L03Service _defaultFStatement133L03Service;
        private readonly DefaultFStatement133L04Service _defaultFStatement133L04Service;
        private readonly DefaultFStatement133L05Service _defaultFStatement133L05Service;
        private readonly DefaultFStatement133L06Service _defaultFStatement133L06Service;
        private readonly DefaultFStatement133L07Service _defaultFStatement133L07Service;
        private readonly DefaultFStatement200L01Service _defaultFStatement200L01Service;
        private readonly DefaultFStatement200L02Service _defaultFStatement200L02Service;
        private readonly DefaultFStatement200L03Service _defaultFStatement200L03Service;
        private readonly DefaultFStatement200L04Service _defaultFStatement200L04Service;
        private readonly DefaultFStatement200L05Service _defaultFStatement200L05Service;
        private readonly DefaultFStatement200L06Service _defaultFStatement200L06Service;
        private readonly DefaultFStatement200L07Service _defaultFStatement200L07Service;
        private readonly DefaultFStatement200L08Service _defaultFStatement200L08Service;
        private readonly DefaultFStatement200L09Service _defaultFStatement200L09Service;
        private readonly DefaultFStatement200L10Service _defaultFStatement200L10Service;
        private readonly DefaultFStatement200L11Service _defaultFStatement200L11Service;
        private readonly DefaultFStatement200L12Service _defaultFStatement200L12Service;
        private readonly DefaultFStatement200L13Service _defaultFStatement200L13Service;
        private readonly DefaultFStatement200L14Service _defaultFStatement200L14Service;
        private readonly DefaultFStatement200L15Service _defaultFStatement200L15Service;
        private readonly DefaultFStatement200L16Service _defaultFStatement200L16Service;
        private readonly DefaultFStatement200L17Service _defaultFStatement200L17Service;
        private readonly DefaultFStatement200L18Service _defaultFStatement200L18Service;
        private readonly DefaultFStatement200L19Service _defaultFStatement200L19Service;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ICurrentTenant _currentTenant;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        public BalanceTransferAppService(ConfigForwardYearService configForwardYearService,
                  UserService userService,
                  WebHelper webHelper,
                  IUnitOfWorkManager unitOfWorkManager,
                  AssetToolAccountService assetToolAccountService,
                  AccountSystemService accountSystemService,
                  AccountSystemAppService accountSystemAppService,
                  AllotmentForwardCategoryService allotmentForwardCategoryService,
                  TenantAccBalanceSheetService tenantAccBalanceSheetService,
                  TenantBusinessResultService tenantBusinessResultService,
                  TenantCashFollowStatementService tenantCashFollowStatementService,
                  LicenseBusiness licenseBusiness,
                  SoTHZService soTHZService,
                  FProductWorkNormService fProductWorkNormService,
                  FProductWorkNormDetailService fProductWorkNormDetailService,
                  GroupCoefficientDetailService groupCoefficientDetailService,
                  FStatement133L02Service fStatement133L02Service,
                  FStatement133L01Service fStatement133L01Service,
                  FStatement133L03Service fStatement133L03Service,
                  FStatement133L04Service fStatement133L04Service,
                  FStatement133L05Service fStatement133L05Service,
                  FStatement133L06Service fStatement133L06Service,
                  FStatement133L07Service fStatement133L07Service,
                  FStatement200L01Service fStatement200L01Service,
                  FStatement200L02Service fStatement200L02Service,
                  FStatement200L03Service fStatement200L03Service,
                   FStatement200L04Service fStatement200L04Service,
                   FStatement200L05Service fStatement200L05Service,
                   FStatement200L06Service fStatement200L06Service,
                   FStatement200L07Service fStatement200L07Service,
                   FStatement200L08Service fStatement200L08Service,
                   FStatement200L09Service fStatement200L09Service,
                   FStatement200L10Service fStatement200L10Service,
                   FStatement200L11Service fStatement200L11Service,
                   FStatement200L12Service fStatement200L12Service,
                   FStatement200L13Service fStatement200L13Service,
                   FStatement200L14Service fStatement200L14Service,
                   FStatement200L15Service fStatement200L15Service,
                   FStatement200L16Service fStatement200L16Service,
                   FStatement200L17Service fStatement200L17Service,
                   FStatement200L18Service fStatement200L18Service,
                   FStatement200L19Service fStatement200L19Service,
                   ProductOpeningBalanceService productOpeningBalanceService,
                   WarehouseBookService warehouseBookService,
                   TenantSettingService tenantSettingService,
                   AccOpeningBalanceService accOpeningBalanceService,
                   LedgerService ledgerService,
                   DefaultAllotmentForwardCategoryService defaultAllotmentForwardCategory,
                   YearCategoryService yearCategoryService,
                   DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                   DefaultBusinessResultService defaultBusinessResultService,
                   DefaultCashFollowStatementService defaultCashFollowStatementService,
                   DefaultStatementTaxService defaultStatementTaxService,
                   TenantStatementTaxService tenantStatementTaxService,
                   DefaultFStatement133L01Service defaultFStatement133L01Service,
                   DefaultFStatement133L02Service defaultFStatement133L02Service,
                   DefaultFStatement133L03Service defaultFStatement133L03Service,
                   DefaultFStatement133L04Service defaultFStatement133L04Service,
                   DefaultFStatement133L05Service defaultFStatement133L05Service,
                   DefaultFStatement133L06Service defaultFStatement133L06Service,
                   DefaultFStatement133L07Service defaultFStatement133L07Service,
                   DefaultFStatement200L01Service defaultFStatement200L01Service,
                   DefaultFStatement200L02Service defaultFStatement200L02Service,
                   DefaultFStatement200L03Service defaultFStatement200L03Service,
                   DefaultFStatement200L04Service defaultFStatement200L04Service,
                   DefaultFStatement200L05Service defaultFStatement200L05Service,
                   DefaultFStatement200L06Service defaultFStatement200L06Service,
                   DefaultFStatement200L07Service defaultFStatement200L07Service,
                   DefaultFStatement200L08Service defaultFStatement200L08Service,
                   DefaultFStatement200L09Service defaultFStatement200L09Service,
                   DefaultFStatement200L10Service defaultFStatement200L10Service,
                   DefaultFStatement200L11Service defaultFStatement200L11Service,
                   DefaultFStatement200L12Service defaultFStatement200L12Service,
                   DefaultFStatement200L13Service defaultFStatement200L13Service,
                   DefaultFStatement200L14Service defaultFStatement200L14Service,
                   DefaultFStatement200L15Service defaultFStatement200L15Service,
                   DefaultFStatement200L16Service defaultFStatement200L16Service,
                   DefaultFStatement200L17Service defaultFStatement200L17Service,
                   DefaultFStatement200L18Service defaultFStatement200L18Service,
                   DefaultFStatement200L19Service defaultFStatement200L19Service,
                   TenantExtendInfoService tenantExtendInfoService,
                   ICurrentTenant currentTenant,
                   DefaultTenantSettingService defaultTenantSettingService
            )
        {
            _configForwardYearService = configForwardYearService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _assetToolAccountService = assetToolAccountService;
            _accountSystemService = accountSystemService;
            _accountSystemAppService = accountSystemAppService;
            _allotmentForwardCategoryService = allotmentForwardCategoryService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _tenantBusinessResultService = tenantBusinessResultService;
            _tenantCashFollowStatementService = tenantCashFollowStatementService;
            _licenseBusiness = licenseBusiness;
            _soTHZService = soTHZService;
            _fProductWorkNormService = fProductWorkNormService;
            _fProductWorkNormDetailService = fProductWorkNormDetailService;
            _groupCoefficientDetailService = groupCoefficientDetailService;
            _fStatement133L02Service = fStatement133L02Service;
            _fStatement133L01Service = fStatement133L01Service;
            _fStatement133L03Service = fStatement133L03Service;
            _fStatement133L04Service = fStatement133L04Service;
            _fStatement133L05Service = fStatement133L05Service;
            _fStatement133L06Service = fStatement133L06Service;
            _fStatement133L07Service = fStatement133L07Service;
            _fStatement200L01Service = fStatement200L01Service;
            _fStatement200L02Service = fStatement200L02Service;
            _fStatement200L03Service = fStatement200L03Service;
            _fStatement200L04Service = fStatement200L04Service;
            _fStatement200L05Service = fStatement200L05Service;
            _fStatement200L06Service = fStatement200L06Service;
            _fStatement200L07Service = fStatement200L07Service;
            _fStatement200L08Service = fStatement200L08Service;
            _fStatement200L09Service = fStatement200L09Service;
            _fStatement200L10Service = fStatement200L10Service;
            _fStatement200L11Service = fStatement200L11Service;
            _fStatement200L12Service = fStatement200L12Service;
            _fStatement200L13Service = fStatement200L13Service;
            _fStatement200L14Service = fStatement200L14Service;
            _fStatement200L15Service = fStatement200L15Service;
            _fStatement200L16Service = fStatement200L16Service;
            _fStatement200L17Service = fStatement200L17Service;
            _fStatement200L18Service = fStatement200L18Service;
            _fStatement200L19Service = fStatement200L19Service;
            _productOpeningBalanceService = productOpeningBalanceService;
            _warehouseBookService = warehouseBookService;
            _tenantSettingService = tenantSettingService;
            _accOpeningBalanceService = accOpeningBalanceService;
            _ledgerService = ledgerService;
            _defaultAllotmentForwardCategoryService = defaultAllotmentForwardCategory;
            _yearCategoryService = yearCategoryService;
            _defaultAccBalanceSheetService = defaultAccBalanceSheetService;
            _defaultBusinessResultService = defaultBusinessResultService;
            _defaultCashFollowStatementService = defaultCashFollowStatementService;
            _defaultStatementTaxService = defaultStatementTaxService;
            _tenantStatementTaxService = tenantStatementTaxService;
            _defaultFStatement133L01Service = defaultFStatement133L01Service;
            _defaultFStatement133L02Service = defaultFStatement133L02Service;
            _defaultFStatement133L03Service = defaultFStatement133L03Service;
            _defaultFStatement133L04Service = defaultFStatement133L04Service;
            _defaultFStatement133L05Service = defaultFStatement133L05Service;
            _defaultFStatement133L06Service = defaultFStatement133L06Service;
            _defaultFStatement133L07Service = defaultFStatement133L07Service;
            _defaultFStatement200L01Service = defaultFStatement200L01Service;
            _defaultFStatement200L02Service = defaultFStatement200L02Service;
            _defaultFStatement200L03Service = defaultFStatement200L03Service;
            _defaultFStatement200L04Service = defaultFStatement200L04Service;
            _defaultFStatement200L05Service = defaultFStatement200L05Service;
            _defaultFStatement200L06Service = defaultFStatement200L06Service;
            _defaultFStatement200L07Service = defaultFStatement200L07Service;
            _defaultFStatement200L08Service = defaultFStatement200L08Service;
            _defaultFStatement200L09Service = defaultFStatement200L09Service;
            _defaultFStatement200L10Service = defaultFStatement200L10Service;
            _defaultFStatement200L11Service = defaultFStatement200L11Service;
            _defaultFStatement200L12Service = defaultFStatement200L12Service;
            _defaultFStatement200L13Service = defaultFStatement200L13Service;
            _defaultFStatement200L14Service = defaultFStatement200L14Service;
            _defaultFStatement200L15Service = defaultFStatement200L15Service;
            _defaultFStatement200L16Service = defaultFStatement200L16Service;
            _defaultFStatement200L17Service = defaultFStatement200L17Service;
            _defaultFStatement200L18Service = defaultFStatement200L18Service;
            _defaultFStatement200L19Service = defaultFStatement200L19Service;
            _tenantExtendInfoService = tenantExtendInfoService;
            _currentTenant = currentTenant;
            _defaultTenantSettingService = defaultTenantSettingService;
        }
        public async Task<List<CrudConfigForwardYearDto>> GetListAsync()
        {

            var configForwardYear = await _configForwardYearService.GetQueryableAsync();
            var tenantType = await this.GetTenantType();
            var lstConfigForwardYear = configForwardYear.OrderBy(p => p.Ord).ToList();
            List<CrudConfigForwardYearDto> crudConfigForwardYearDtos = new List<CrudConfigForwardYearDto>();

            if (tenantType == 2)
            {
                var lstConfigForwardYearHKD = lstConfigForwardYear.Where(p => p.BusinessType == 2).ToList();
                foreach (var item in lstConfigForwardYearHKD)
                {
                    CrudConfigForwardYearDto crudConfigForwardYearDto = new CrudConfigForwardYearDto();
                    crudConfigForwardYearDto.Id = item.Id;
                    crudConfigForwardYearDto.TableName = item.TableName;
                    crudConfigForwardYearDto.FieldNot = item.FieldNot;
                    crudConfigForwardYearDto.FieldValues = item.FieldValues;
                    crudConfigForwardYearDto.Title = item.Title;
                    crudConfigForwardYearDto.SelectRow = 0;
                    crudConfigForwardYearDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crudConfigForwardYearDtos.Add(crudConfigForwardYearDto);
                }
            }
            else
            {
                var lstConfigForwardYears = lstConfigForwardYear.Where(p => p.BusinessType == 1).ToList();
                foreach (var item in lstConfigForwardYears)
                {
                    CrudConfigForwardYearDto crudConfigForwardYearDto = new CrudConfigForwardYearDto();
                    crudConfigForwardYearDto.Id = item.Id;
                    crudConfigForwardYearDto.TableName = item.TableName;
                    crudConfigForwardYearDto.FieldNot = item.FieldNot;
                    crudConfigForwardYearDto.FieldValues = item.FieldValues;
                    crudConfigForwardYearDto.Title = item.Title;
                    crudConfigForwardYearDto.SelectRow = 0;
                    crudConfigForwardYearDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crudConfigForwardYearDtos.Add(crudConfigForwardYearDto);
                }
            }

            return crudConfigForwardYearDtos;
        }
        private async Task<int?> GetTenantType()
        {
            var tenantExtendInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantExtendInfo == null) return null;
            return tenantExtendInfo.TenantType;
        }
        public async Task<BalanceTransferDto> CreateAsync(BalanceTransferDto dto)
        {
            await _licenseBusiness.CheckExpired();
            BalanceTransferDto crud = new BalanceTransferDto();

            var year = int.Parse(_webHelper.GetCurrentYear().ToString());
            var yearCategory = await _yearCategoryService.GetQueryableAsync();
            var lsrYearCategory = yearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).FirstOrDefault();
            if (dto.ProductOpeningBalance == 1)
            {

                var productOpningBalance = await _productOpeningBalanceService.GetQueryableAsync();
                var lstProductOpningBalance = productOpningBalance.Where(p => p.Year == year && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

                var productOpeningResul0 = (from a in lstProductOpningBalance
                                            group new
                                            {
                                                a.WarehouseCode,
                                                a.ProductCode,
                                                a.ProductLotCode,
                                                a.ProductOriginCode,
                                                a.AccCode,
                                                a.Quantity,
                                                a.Amount,
                                                a.AmountCur
                                            } by new
                                            {
                                                a.WarehouseCode,
                                                a.ProductCode,
                                                a.ProductLotCode,
                                                a.ProductOriginCode
                                            } into gr
                                            select new
                                            {
                                                WarehouseCode = gr.Key.WarehouseCode,
                                                ProductCode = gr.Key.ProductCode,
                                                ProductLotCode = gr.Key.ProductLotCode,
                                                ProductOriginCode = gr.Key.ProductOriginCode,
                                                AccCode = gr.Max(p => p.AccCode),
                                                Quantity = gr.Sum(p => p.Quantity),
                                                Amount = gr.Sum(p => p.Amount),
                                                AmountCur = gr.Sum(p => p.AmountCur)
                                            }).ToList();
                var wareHouseBook = await _warehouseBookService.GetQueryableAsync();
                var lstWareHouseBook = wareHouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year && p.Status != "2").ToList();
                var resulWareHouse = (from a in lstWareHouseBook
                                      group new
                                      {
                                          a.WarehouseCode,
                                          a.ProductCode,
                                          a.ProductLotCode,
                                          a.ProductOriginCode,
                                          a.ExportAcc,
                                          a.ImportAcc,
                                          a.ImportQuantity,
                                          a.ExportQuantity,
                                          a.ImportAmount,
                                          a.ImportAmountCur,
                                          a.ExportAmount,
                                          a.ExportAmountCur
                                      } by new
                                      {

                                          a.WarehouseCode,
                                          a.ProductCode,
                                          a.ProductLotCode,
                                          a.ProductOriginCode,

                                      } into gr
                                      select new
                                      {
                                          WarehouseCode = gr.Key.WarehouseCode,
                                          ProductCode = gr.Key.ProductCode,
                                          ProductLotCode = gr.Key.ProductLotCode,
                                          ProductOriginCode = gr.Key.ProductOriginCode,
                                          AccCode = (gr.Max(p => p.ImportAcc) != "" ? gr.Max(p => p.ImportAcc) : gr.Max(p => p.ExportAcc)),
                                          Quantity = (decimal)(gr.Sum(p => p.ImportQuantity) - gr.Sum(p => p.ExportQuantity)),
                                          Amount = (decimal)(gr.Sum(p => p.ImportAmount) - gr.Sum(p => p.ExportAmount)),
                                          AmountCur = (decimal)(gr.Sum(p => p.ImportAmountCur) - gr.Sum(p => p.ExportAmountCur))
                                      }).ToList();
                productOpeningResul0.AddRange(resulWareHouse);
                productOpeningResul0 = (from a in productOpeningResul0
                                        group new
                                        {

                                            a.WarehouseCode,
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode,
                                            a.AccCode,
                                            a.Quantity,
                                            a.Amount,
                                            a.AmountCur
                                        } by new
                                        {

                                            a.WarehouseCode,
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode,
                                        } into gr
                                        select new
                                        {

                                            WarehouseCode = gr.Key.WarehouseCode,
                                            ProductCode = gr.Key.ProductCode,
                                            ProductLotCode = gr.Key.ProductLotCode,
                                            ProductOriginCode = gr.Key.ProductOriginCode,
                                            AccCode = gr.Max(p => p.AccCode),
                                            Quantity = gr.Sum(p => p.Quantity),
                                            Amount = gr.Sum(p => p.Amount),
                                            AmountCur = gr.Sum(p => p.AmountCur)
                                        }).ToList();
                var lstProductOpningBalances = productOpningBalance.Where(p => p.Year == _webHelper.GetCurrentYear() + 1 && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                if (lstProductOpningBalances.Count > 0)
                {
                    foreach (var item in lstProductOpningBalances)
                    {
                        await _productOpeningBalanceService.DeleteAsync(item.Id);
                    }
                }
                int i = 0;
                foreach (var item in productOpeningResul0)
                {

                    //if (item.Quantity + item.Amount + item.AmountCur > 0)
                    //{
                    CrudProductOpeningBalanceDto productOpeningBalance = new CrudProductOpeningBalanceDto();
                    productOpeningBalance.OrgCode = _webHelper.GetCurrentOrgUnit();
                    productOpeningBalance.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                    productOpeningBalance.Id = this.GetNewObjectId();
                    productOpeningBalance.Ord0 = "A00000" + i;
                    productOpeningBalance.WarehouseCode = item.WarehouseCode;
                    productOpeningBalance.ProductCode = item.ProductCode;
                    productOpeningBalance.ProductLotCode = item.ProductLotCode;
                    productOpeningBalance.ProductOriginCode = item.ProductOriginCode;
                    productOpeningBalance.AccCode = item.AccCode;
                    productOpeningBalance.Quantity = item.Quantity;
                    productOpeningBalance.Price = item.Quantity != 0 ? item.Amount / item.Quantity : 0;
                    productOpeningBalance.PriceCur = item.Quantity != 0 ? item.AmountCur / item.Quantity : 0;
                    productOpeningBalance.Amount = item.Amount;
                    productOpeningBalance.AmountCur = item.AmountCur;
                    var entity = ObjectMapper.Map<CrudProductOpeningBalanceDto, ProductOpeningBalance>(productOpeningBalance);

                    await _productOpeningBalanceService.CreateAsync(entity);

                    //}

                }

            }
            if (dto.AccountOpeningBalance == 1)
            {
                var sterilizations = await _tenantSettingService.GetTenantSettingByKeyAsync("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
                var check = "";
                if (sterilizations == null)
                {
                    var defaulTenanSetting = await _defaultTenantSettingService.GetQueryableAsync();
                    var defaulTenanSettings = defaulTenanSetting.Where(p => p.Key == "VHT_CO_KHU_TRUNG").FirstOrDefault();
                    check = defaulTenanSettings.Type;
                }
                else
                {
                    check = sterilizations.Value;

                }

                //CrudAccOpeningBalanceDto

                var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
                var lstAccOpeningBalance = accOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();

                var accountSystem = await _accountSystemService.GetQueryableAsync();
                var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var accBalanceRessul0 = (from a in lstAccOpeningBalance
                                         join b in lstAccountSystem on a.AccCode equals b.AccCode into c
                                         from ac in c.DefaultIfEmpty()
                                         group new
                                         {
                                             a.Year,
                                             a.OrgCode,
                                             a.AccCode,
                                             a.CurrencyCode,
                                             a.PartnerCode,
                                             a.ContractCode,
                                             a.FProductWorkCode,
                                             a.WorkPlaceCode,
                                             a.AccSectionCode,
                                             Debit = a.Debit,
                                             a.DebitCur,
                                             a.Credit,
                                             a.CreditCur,
                                             AccSectionCode0 = ac != null ? ac.AccSectionCode : null
                                         } by new
                                         {
                                             a.Year,
                                             a.OrgCode,
                                             a.AccCode,
                                             a.CurrencyCode,
                                             a.PartnerCode,
                                             a.ContractCode,
                                             a.FProductWorkCode,
                                             a.WorkPlaceCode,
                                             a.AccSectionCode,
                                             AccSectionCode0 = ac != null ? ac.AccSectionCode : null
                                         } into gr
                                         select new
                                         {
                                             OrgCode = gr.Key.OrgCode,
                                             year = gr.Key.Year,
                                             AccCode = gr.Key.AccCode,
                                             CurrencyCode = gr.Key.CurrencyCode,
                                             PartnerCode = gr.Key.PartnerCode == null ? "" : gr.Key.PartnerCode,
                                             ContractCode = gr.Key.ContractCode == null ? "" : gr.Key.ContractCode,
                                             FProductWorkCode = gr.Key.FProductWorkCode == null ? "" : gr.Key.FProductWorkCode,
                                             WorkPlaceCode = gr.Key.WorkPlaceCode == null ? "" : gr.Key.WorkPlaceCode,
                                             SectionCode = gr.Key.AccSectionCode == null ? "" : gr.Key.AccSectionCode,
                                             SectionCode0 = gr.Key.AccSectionCode0 == null ? "" : gr.Key.AccSectionCode0,
                                             Debit = gr.Sum(p => p.Debit),
                                             DebitCur = gr.Sum(p => p.DebitCur),
                                             Credit = gr.Sum(p => p.Credit),
                                             CreditCur = gr.Sum(p => p.CreditCur)
                                         }).ToList();

                var legers = await _ledgerService.GetQueryableAsync();
                var lstLedger = legers.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year && p.Status != "2").ToList();
                var resulLedger = (from a in lstLedger
                                   join b in lstAccountSystem on a.DebitAcc equals b.AccCode into c
                                   from ac in c.DefaultIfEmpty()

                                   where (check != "C" || a.CheckDuplicate != "C") && a.CheckDuplicate0 != "C"
                                   group new
                                   {
                                       a.OrgCode,
                                       a.Year,
                                       a.DebitAcc,
                                       a.DebitCurrencyCode,
                                       a.DebitPartnerCode,
                                       a.DebitContractCode,
                                       a.DebitFProductWorkCode,
                                       a.DebitWorkPlaceCode,
                                       a.DebitSectionCode,
                                       AccSectionCode0 = ac != null ? ac.AccSectionCode : null,
                                       a.Amount,
                                       a.DebitAmountCur
                                   } by new
                                   {
                                       a.OrgCode,
                                       a.Year,
                                       a.DebitAcc,
                                       a.DebitCurrencyCode,
                                       a.DebitPartnerCode,
                                       a.DebitContractCode,
                                       a.DebitFProductWorkCode,
                                       a.DebitWorkPlaceCode,
                                       a.DebitSectionCode,
                                       AccSectionCode0 = ac != null ? ac.AccSectionCode : null,
                                   } into gr
                                   select new
                                   {

                                       OrgCode = gr.Key.OrgCode,
                                       year = gr.Key.Year,
                                       AccCode = gr.Key.DebitAcc,
                                       CurrencyCode = gr.Key.DebitCurrencyCode,
                                       PartnerCode = gr.Key.DebitPartnerCode,
                                       ContractCode = gr.Key.DebitContractCode,
                                       FProductWorkCode = gr.Key.DebitContractCode,
                                       WorkPlaceCode = gr.Key.DebitWorkPlaceCode,
                                       SectionCode = gr.Key.DebitSectionCode,
                                       SectionCode0 = gr.Key.AccSectionCode0,
                                       Debit = (decimal)gr.Sum(p => p.Amount),
                                       DebitCur = (decimal)gr.Sum(p => p.DebitAmountCur),
                                       Credit = (decimal)0,
                                       CreditCur = (decimal)0
                                   }).ToList();
                accBalanceRessul0.AddRange(resulLedger);
                var resulLedger2 = (from a in lstLedger
                                    join b in lstAccountSystem on a.CreditAcc equals b.AccCode into c
                                    from ac in c.DefaultIfEmpty()

                                    where (check != "C" || a.CheckDuplicate != "N") && a.CheckDuplicate0 != "N"
                                    group new
                                    {
                                        a.OrgCode,
                                        a.Year,
                                        a.CreditAcc,
                                        a.CreditCurrencyCode,
                                        a.CreditPartnerCode,
                                        a.CreditContractCode,
                                        a.CreditFProductWorkCode,
                                        a.CreditWorkPlaceCode,
                                        a.CreditSectionCode,
                                        AccSectionCode0 = ac != null ? ac.AccSectionCode : null,
                                        a.Amount,
                                        a.CreditAmountCur
                                    } by new
                                    {
                                        a.OrgCode,
                                        a.Year,
                                        a.CreditAcc,
                                        a.CreditCurrencyCode,
                                        a.CreditPartnerCode,
                                        a.CreditContractCode,
                                        a.CreditFProductWorkCode,
                                        a.CreditWorkPlaceCode,
                                        a.CreditSectionCode,
                                        AccSectionCode0 = ac != null ? ac.AccSectionCode : null
                                    } into gr
                                    select new
                                    {

                                        OrgCode = gr.Key.OrgCode,
                                        year = gr.Key.Year,
                                        AccCode = gr.Key.CreditAcc,
                                        CurrencyCode = gr.Key.CreditCurrencyCode,
                                        PartnerCode = gr.Key.CreditPartnerCode,
                                        ContractCode = gr.Key.CreditContractCode,
                                        FProductWorkCode = gr.Key.CreditFProductWorkCode,
                                        WorkPlaceCode = gr.Key.CreditWorkPlaceCode,
                                        SectionCode = gr.Key.CreditSectionCode,
                                        SectionCode0 = gr.Key.AccSectionCode0,
                                        Debit = (decimal)gr.Sum(p => p.Amount) < 0 ? -(decimal)gr.Sum(p => p.Amount) : 0,
                                        DebitCur = (decimal)0,
                                        Credit = (decimal)gr.Sum(p => p.Amount) < 0 ? 0 : (decimal)gr.Sum(p => p.Amount),
                                        CreditCur = (decimal)gr.Sum(p => p.CreditAmountCur)
                                    }).ToList();
                accBalanceRessul0.AddRange(resulLedger2);
                accBalanceRessul0 = (from a in accBalanceRessul0
                                     group new
                                     {
                                         a.OrgCode,
                                         a.year,
                                         a.AccCode,
                                         a.CurrencyCode,
                                         a.PartnerCode,
                                         a.ContractCode,
                                         a.FProductWorkCode,
                                         a.WorkPlaceCode,
                                         a.SectionCode,
                                         a.SectionCode0,
                                         a.Debit,
                                         a.DebitCur,
                                         a.Credit,
                                         a.CreditCur
                                     } by new
                                     {
                                         a.OrgCode,
                                         a.year,
                                         a.AccCode,
                                         //a.CurrencyCode,
                                         a.PartnerCode,
                                         a.ContractCode,
                                         a.FProductWorkCode,
                                         a.WorkPlaceCode,
                                         a.SectionCode,
                                         a.SectionCode0
                                     }
                                    into gr
                                     select new
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         year = gr.Key.year,
                                         AccCode = gr.Key.AccCode,
                                         CurrencyCode = gr.Max(p => p.CurrencyCode),
                                         PartnerCode = gr.Key.PartnerCode,
                                         ContractCode = gr.Key.ContractCode,
                                         FProductWorkCode = gr.Key.FProductWorkCode,
                                         WorkPlaceCode = gr.Key.WorkPlaceCode,
                                         SectionCode = gr.Key.SectionCode,
                                         SectionCode0 = gr.Key.SectionCode0,
                                         Debit = gr.Sum(p => p.Debit),
                                         DebitCur = gr.Sum(p => p.DebitCur),
                                         Credit = (decimal)gr.Sum(p => p.Credit),
                                         CreditCur = (decimal)gr.Sum(p => p.CreditCur)
                                     }).ToList();
                accBalanceRessul0 = (from a in accBalanceRessul0

                                     select new
                                     {
                                         a.OrgCode,
                                         a.year,
                                         a.AccCode,
                                         a.CurrencyCode,
                                         a.PartnerCode,
                                         a.ContractCode,
                                         a.FProductWorkCode,
                                         a.WorkPlaceCode,
                                         SectionCode = a.SectionCode0 != "" ? a.SectionCode0 : a.SectionCode,
                                         a.SectionCode0,
                                         a.Debit,
                                         a.DebitCur,
                                         a.Credit,
                                         a.CreditCur
                                     }).ToList();
                var accBalanceRessul2 = (from a in accBalanceRessul0
                                         group new
                                         {
                                             a.OrgCode,
                                             a.year,
                                             a.AccCode,
                                             a.CurrencyCode,
                                             a.PartnerCode,
                                             a.ContractCode,
                                             a.FProductWorkCode,
                                             a.WorkPlaceCode,
                                             a.SectionCode,
                                             a.Debit,
                                             a.DebitCur,
                                             a.Credit,
                                             a.CreditCur
                                         } by new
                                         {
                                             a.OrgCode,
                                             a.year,
                                             a.AccCode,
                                             a.CurrencyCode,
                                             a.PartnerCode,
                                             a.ContractCode,
                                             a.FProductWorkCode,
                                             a.SectionCode
                                         } into gr
                                         select new
                                         {
                                             OrgCode = gr.Key.OrgCode,
                                             year = gr.Key.year,
                                             AccCode = gr.Key.AccCode,
                                             CurrencyCode = gr.Key.CurrencyCode,
                                             PartnerCode = gr.Key.PartnerCode,
                                             ContractCode = gr.Max(p => p.ContractCode),
                                             FProductWorkCode = gr.Max(p => p.FProductWorkCode),
                                             WorkPlaceCode = gr.Max(p => p.WorkPlaceCode),
                                             SectionCode = gr.Max(p => p.SectionCode),
                                             Debit = gr.Sum(p => p.Debit),
                                             DebitCur = gr.Sum(p => p.DebitCur),
                                             Credit = gr.Sum(p => p.Credit),
                                             CreditCur = gr.Sum(p => p.CreditCur)
                                         }).OrderBy(p => p.AccCode).ToList();
                accBalanceRessul2 = (from a in accBalanceRessul2
                                     group new
                                     {
                                         a.AccCode,
                                         a.OrgCode,
                                         a.year,
                                         a.CurrencyCode,
                                         a.PartnerCode,
                                         a.ContractCode,
                                         a.FProductWorkCode,
                                         a.WorkPlaceCode,
                                         a.SectionCode,
                                         a.Debit,
                                         a.DebitCur,
                                         a.Credit,
                                         a.CreditCur
                                     } by new
                                     {
                                         a.AccCode,
                                         a.OrgCode,
                                         a.year,
                                         a.CurrencyCode,
                                         a.PartnerCode,

                                     } into gr
                                     select new
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         year = gr.Key.year,
                                         AccCode = gr.Key.AccCode,
                                         CurrencyCode = gr.Key.CurrencyCode,
                                         PartnerCode = gr.Key.PartnerCode,
                                         ContractCode = gr.Max(p => p.ContractCode),
                                         FProductWorkCode = gr.Max(p => p.FProductWorkCode),
                                         WorkPlaceCode = gr.Max(p => p.WorkPlaceCode),
                                         SectionCode = gr.Max(p => p.SectionCode),
                                         Debit = gr.Sum(p => p.Debit),
                                         DebitCur = gr.Sum(p => p.DebitCur),
                                         Credit = gr.Sum(p => p.Credit),
                                         CreditCur = gr.Sum(p => p.CreditCur)
                                     }).ToList();
                accBalanceRessul2 = (from a in accBalanceRessul2
                                     where (a.Credit + a.CreditCur + a.Debit + a.DebitCur) > 0
                                     select new
                                     {
                                         OrgCode = a.OrgCode,
                                         year = a.year,
                                         AccCode = a.AccCode,
                                         CurrencyCode = a.CurrencyCode,
                                         PartnerCode = a.PartnerCode,
                                         ContractCode = a.ContractCode,
                                         FProductWorkCode = a.FProductWorkCode,
                                         WorkPlaceCode = a.WorkPlaceCode,
                                         SectionCode = a.SectionCode,
                                         Debit = a.Debit == a.Credit ? 0 : a.Debit,
                                         DebitCur = a.DebitCur == a.CreditCur ? 0 : a.DebitCur,
                                         Credit = a.Credit == a.Debit ? 0 : a.Credit,
                                         CreditCur = a.CreditCur == a.DebitCur ? 0 : a.CreditCur
                                     }).OrderBy(p => p.AccCode).ToList();
                var lstAccOpeningBalances = accOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() + 1).ToList();
                if (lstAccOpeningBalances.Count > 0)
                {
                    foreach (var item in lstAccOpeningBalances)
                    {
                        await _accOpeningBalanceService.DeleteAsync(item.Id);
                    }
                }
                var accOpeningBalances = (from a in lstAccountSystem.ToList()
                                          join d in accBalanceRessul2 on a.AccCode equals d.AccCode


                                          select new CrudAccOpeningBalanceDto
                                          {
                                              OrgCode = _webHelper.GetCurrentOrgUnit(),
                                              Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1,
                                              AccCode = a.AccCode,
                                              CurrencyCode = a.AttachCurrency == "C" ? d.CurrencyCode : null,
                                              PartnerCode = a.AttachPartner == "C" ? d.PartnerCode : null,
                                              ContractCode = a.AttachContract == "C" ? d.ContractCode : null,
                                              FProductWorkCode = a.AttachProductCost == "C" ? d.FProductWorkCode : null,
                                              AccSectionCode = a.AttachAccSection == "C" ? d.SectionCode : null,
                                              WorkPlaceCode = a.AttachWorkPlace == "C" ? d.WorkPlaceCode : null,
                                              Debit = d != null ? d.Debit : d.Debit,
                                              DebitCur = d != null ? d.DebitCur : d.DebitCur,
                                              Credit = d != null ? d.Credit : d.Credit,
                                              CreditCur = d != null ? d.CreditCur : d.CreditCur

                                          }).ToList();
                accOpeningBalances = (from a in accOpeningBalances
                                      group new
                                      {
                                          a.OrgCode,
                                          a.Year,
                                          a.AccCode,
                                          a.CurrencyCode,
                                          a.PartnerCode,
                                          a.ContractCode,
                                          a.FProductWorkCode,
                                          a.AccSectionCode,
                                          a.WorkPlaceCode,
                                          a.Debit,
                                          a.DebitCur,
                                          a.Credit,
                                          a.CreditCur
                                      } by new
                                      {
                                          a.OrgCode,
                                          a.Year,
                                          a.AccCode,
                                          a.CurrencyCode,
                                          a.PartnerCode,
                                          a.ContractCode,
                                          a.FProductWorkCode,
                                          a.AccSectionCode,
                                          a.WorkPlaceCode,
                                      } into gr
                                      select new CrudAccOpeningBalanceDto
                                      {
                                          OrgCode = gr.Key.OrgCode,
                                          Year = gr.Key.Year,
                                          AccCode = gr.Key.AccCode,
                                          CurrencyCode = gr.Key.CurrencyCode,
                                          PartnerCode = gr.Key.PartnerCode,
                                          ContractCode = gr.Key.ContractCode,
                                          FProductWorkCode = gr.Key.FProductWorkCode,
                                          AccSectionCode = gr.Key.AccSectionCode,
                                          WorkPlaceCode = gr.Key.WorkPlaceCode,
                                          Debit = gr.Sum(p => p.Debit),
                                          DebitCur = gr.Sum(p => p.DebitCur),
                                          Credit = gr.Sum(p => p.Credit),
                                          CreditCur = gr.Sum(p => p.CreditCur)
                                      }).ToList();
                foreach (var item in accOpeningBalances)
                {
                    CrudAccOpeningBalanceDto crudAccOpeningBalanceDto = new CrudAccOpeningBalanceDto();
                    crudAccOpeningBalanceDto.Id = this.GetNewObjectId();
                    crudAccOpeningBalanceDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crudAccOpeningBalanceDto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                    crudAccOpeningBalanceDto.AccCode = item.AccCode;
                    crudAccOpeningBalanceDto.CurrencyCode = item.CurrencyCode;
                    crudAccOpeningBalanceDto.PartnerCode = item.PartnerCode;
                    crudAccOpeningBalanceDto.ContractCode = item.ContractCode;
                    crudAccOpeningBalanceDto.FProductWorkCode = item.FProductWorkCode;
                    crudAccOpeningBalanceDto.AccSectionCode = item.AccSectionCode;
                    crudAccOpeningBalanceDto.WorkPlaceCode = item.WorkPlaceCode;
                    crudAccOpeningBalanceDto.Debit = item.Debit > item.Credit ? item.Debit - item.Credit : 0;
                    crudAccOpeningBalanceDto.DebitCur = item.DebitCur > item.CreditCur ? item.DebitCur - item.CreditCur : 0;
                    crudAccOpeningBalanceDto.Credit = item.Credit > item.Debit ? item.Credit - item.Debit : 0;
                    crudAccOpeningBalanceDto.CreditCur = item.CreditCur > item.DebitCur ? item.CreditCur - item.DebitCur : 0;
                    var entity = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(crudAccOpeningBalanceDto);
                    await _accOpeningBalanceService.CreateAsync(entity);

                }
            }
            if (dto.LstDataForYear.Contains("AssetToolAccount") == true)
            {
                var assetToolAccount = await _assetToolAccountService.GetQueryableAsync();
                var lstAssetToolAccount = assetToolAccount.Where(p => p.Year == year).ToList();
                for (int i = 0; i < lstAssetToolAccount.Count; i++)
                {
                    CrudAssetToolAccountDto crud1 = new CrudAssetToolAccountDto();
                    crud1.CreatorName = await _userService.GetCurrentUserNameAsync();
                    crud1.Id = this.GetNewObjectId();
                    crud1.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crud1.AssetOrTool = AssetToolConst.Tool;
                    crud1.Month = lstAssetToolAccount[i].Month;
                    crud1.AssetToolId = lstAssetToolAccount[i].AssetToolId;
                    crud1.Ord0 = lstAssetToolAccount[i].Ord0;
                    crud1.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                    crud1.DepreciationDate = DateTime.Parse(lstAssetToolAccount[i].DepreciationDate.ToString()).AddYears(1);
                    crud1.DepartmentCode = lstAssetToolAccount[i].DepartmentCode;
                    crud1.DebitAcc = lstAssetToolAccount[i].DebitAcc;
                    crud1.CreditAcc = lstAssetToolAccount[i].CreditAcc;
                    crud1.PartnerCode = lstAssetToolAccount[i].PartnerCode;
                    crud1.WorkPlaceCode = lstAssetToolAccount[i].WorkPlaceCode;
                    crud1.FProductWorkCode = lstAssetToolAccount[i].FProductWorkCode;
                    crud1.SectionCode = lstAssetToolAccount[i].SectionCode;
                    crud1.CaseCode = lstAssetToolAccount[i].CaseCode;
                    crud1.Note = lstAssetToolAccount[i].Note;
                    var entity = ObjectMapper.Map<CrudAssetToolAccountDto, AssetToolAccount>(crud1);
                    try
                    {
                        //var assetToolDetails = await _assetToolAccountService.GetByAssetToolIdAsync(dto.AssetToolId);
                        using var unitOfWork = _unitOfWorkManager.Begin();
                        //await _assetToolAccountService.DeleteManyAsync(assetToolDetails);
                        var result = await _assetToolAccountService.CreateAsync(entity);
                        await unitOfWork.CompleteAsync();
                        //return ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(result);
                    }
                    catch (Exception)
                    {
                        await _unitOfWorkManager.Current.RollbackAsync();
                        throw;
                    }
                }

            }
            if (dto.LstDataForYear.Contains("AccountSystem") == true)
            {
                var accountSystemService = await _accountSystemService.GetQueryableAsync();
                var lstAccountSystemService = accountSystemService.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).OrderBy(p => p.AccCode).ToList();
                List<AccountSystem> lacc = new List<AccountSystem>();

                List<AccountSystemDto> laccs = new List<AccountSystemDto>();
                var orgCode = _webHelper.GetCurrentOrgUnit();
                for (int i = 0; i < lstAccountSystemService.Count; i++)
                {
                    var lstAccountSystemService1 = accountSystemService.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1 && p.AccCode == lstAccountSystemService[i].AccCode).OrderBy(p => p.AccCode).ToList();
                    if (lstAccountSystemService1.Count > 0)
                    {
                        await _accountSystemService.DeleteAsync(lstAccountSystemService1[0], true);
                    }

                    var entity = ObjectMapper.Map<AccountSystem, CruAccountSystemDto>(lstAccountSystemService[i]);
                    CruAccountSystemDto cruAccountSystemDto = new CruAccountSystemDto();

                    cruAccountSystemDto.CreatorName = await _userService.GetCurrentUserNameAsync();
                    cruAccountSystemDto.Id = this.GetNewObjectId();
                    cruAccountSystemDto.OrgCode = orgCode;
                    cruAccountSystemDto.Year = year + 1;
                    cruAccountSystemDto.AccCode = entity.AccCode != null ? entity.AccCode : null;
                    cruAccountSystemDto.AccPattern = entity.AccPattern;
                    cruAccountSystemDto.AssetOrEquity = entity.AssetOrEquity;
                    cruAccountSystemDto.AccName = entity.AccName;
                    cruAccountSystemDto.AccNameEn = entity.AccNameEn;
                    cruAccountSystemDto.AccRank = entity.AccRank;
                    cruAccountSystemDto.AccNameTemp = entity.AccNameTemp;
                    cruAccountSystemDto.AccNameTempE = entity.AccNameTempE;
                    cruAccountSystemDto.BankAccountNumber = entity.BankAccountNumber;
                    cruAccountSystemDto.BankName = entity.BankName;
                    cruAccountSystemDto.Province = entity.Province;
                    cruAccountSystemDto.AttachPartner = entity.AttachPartner;
                    cruAccountSystemDto.AttachAccSection = entity.AttachAccSection;
                    cruAccountSystemDto.AttachContract = entity.AttachContract;
                    cruAccountSystemDto.AttachCurrency = entity.AttachCurrency;
                    cruAccountSystemDto.AttachProductCost = entity.AttachProductCost;
                    cruAccountSystemDto.AttachVoucher = entity.AttachVoucher;
                    cruAccountSystemDto.AttachWorkPlace = entity.AttachWorkPlace;

                    var tets = lacc.Where(p => p.AccCode == entity.ParentCode).FirstOrDefault();
                    if (tets != null)
                    {
                        cruAccountSystemDto.ParentAccId = tets.Id;
                    }
                    else
                    {
                        cruAccountSystemDto.ParentAccId = "";
                    }
                    cruAccountSystemDto.ParentCode = entity.ParentCode;
                    cruAccountSystemDto.AccType = entity.AccType;
                    cruAccountSystemDto.IsBalanceSheetAcc = entity.IsBalanceSheetAcc;
                    cruAccountSystemDto.AccSectionCode = entity.AccSectionCode;
                    var entitys = ObjectMapper.Map<CruAccountSystemDto, AccountSystem>(cruAccountSystemDto);
                    lacc.Add(entitys);



                }

                try
                {
                    //var assetToolDetails = await _assetToolAccountService.GetByAssetToolIdAsync(dto.AssetToolId);
                    using var unitOfWork = _unitOfWorkManager.Begin();
                    //await _assetToolAccountService.DeleteManyAsync(assetToolDetails);
                    await _accountSystemService.CreateManyAsync(lacc);

                    await unitOfWork.CompleteAsync();
                    //return ObjectMapper.Map<AssetToolAccount, AssetToolAccountDto>(result);
                }
                catch (Exception)
                {
                    await _unitOfWorkManager.Current.RollbackAsync();
                    throw;
                }


            }
            if (dto.LstDataForYear.Contains("AllotmentForwardCategory") == true)
            {
                var defaultAlot = await _defaultAllotmentForwardCategoryService.GetQueryableAsync();

                var allotmentForward = await _allotmentForwardCategoryService.GetQueryableAsync();
                var lstAllotmentForward = allotmentForward.Where(p => p.Year == year && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();


                defaultAlot = defaultAlot.Where(p => p.DecideApply == lsrYearCategory.UsingDecision);

                var lstAllotmentForwards = allotmentForward.Where(p => p.Year == year + 1 && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                foreach (var item in lstAllotmentForwards)
                {
                    await _allotmentForwardCategoryService.DeleteAsync(item, true);
                }

                if (lstAllotmentForward.Count > 0)
                {
                    foreach (var item in lstAllotmentForward)
                    {
                        CrudAllotmentForwardCategoryDto allotmentForwardCategory = new CrudAllotmentForwardCategoryDto();
                        allotmentForwardCategory.OrgCode = item.OrgCode;
                        allotmentForwardCategory.Ord = item.Ord;
                        allotmentForwardCategory.DecideApply = item.DecideApply;
                        allotmentForwardCategory.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        allotmentForwardCategory.FProductWork = item.FProductWork;
                        allotmentForwardCategory.Type = item.Type;
                        allotmentForwardCategory.OrdGrp = item.OrdGrp;
                        allotmentForwardCategory.Code = item.Code;
                        allotmentForwardCategory.ForwardType = item.ForwardType;
                        allotmentForwardCategory.LstCode = item.LstCode;
                        allotmentForwardCategory.GroupCoefficientCode = item.GroupCoefficientCode;
                        allotmentForwardCategory.ProductionPeriodCode = item.ProductionPeriodCode;
                        allotmentForwardCategory.DebitAcc = item.DebitAcc;
                        allotmentForwardCategory.DebitSectionCode = item.DebitSectionCode;
                        allotmentForwardCategory.DebitCredit = item.DebitCredit;
                        allotmentForwardCategory.CreditAcc = item.CreditAcc;
                        allotmentForwardCategory.CreditSectionCode = item.CreditSectionCode;
                        allotmentForwardCategory.Note = item.Note;
                        allotmentForwardCategory.Active = item.Active;
                        allotmentForwardCategory.RecordBook = item.RecordBook;
                        allotmentForwardCategory.AttachProduct = item.AttachProduct;
                        allotmentForwardCategory.QuantityType = item.QuantityType;
                        allotmentForwardCategory.NormType = item.NormType;
                        allotmentForwardCategory.ProductionPeriodType = item.ProductionPeriodType;
                        allotmentForwardCategory.Id = this.GetNewObjectId();

                        var entitys = ObjectMapper.Map<CrudAllotmentForwardCategoryDto, AllotmentForwardCategory>(allotmentForwardCategory);
                        await _allotmentForwardCategoryService.CreateAsync(entitys);

                    }
                }
                else
                {
                    foreach (var item in defaultAlot)
                    {
                        CrudAllotmentForwardCategoryDto allotmentForwardCategory = new CrudAllotmentForwardCategoryDto();
                        allotmentForwardCategory.OrgCode = _webHelper.GetCurrentOrgUnit();
                        allotmentForwardCategory.Ord = item.Ord;
                        allotmentForwardCategory.DecideApply = item.DecideApply;
                        allotmentForwardCategory.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        allotmentForwardCategory.FProductWork = item.FProductWork;
                        allotmentForwardCategory.Type = item.Type;
                        allotmentForwardCategory.OrdGrp = item.OrdGrp;
                        allotmentForwardCategory.Code = item.Code;
                        allotmentForwardCategory.ForwardType = item.ForwardType;
                        allotmentForwardCategory.LstCode = item.LstCode;
                        allotmentForwardCategory.GroupCoefficientCode = item.GroupCoefficientCode;
                        allotmentForwardCategory.ProductionPeriodCode = item.ProductionPeriodCode;
                        allotmentForwardCategory.DebitAcc = item.DebitAcc;
                        allotmentForwardCategory.DebitSectionCode = item.DebitSectionCode;
                        allotmentForwardCategory.DebitCredit = item.DebitCredit;
                        allotmentForwardCategory.CreditAcc = item.CreditAcc;
                        allotmentForwardCategory.CreditSectionCode = item.CreditSectionCode;
                        allotmentForwardCategory.Note = item.Note;
                        allotmentForwardCategory.Active = item.Active;
                        allotmentForwardCategory.RecordBook = item.RecordBook;
                        allotmentForwardCategory.AttachProduct = item.AttachProduct;
                        allotmentForwardCategory.QuantityType = item.QuantityType;
                        allotmentForwardCategory.NormType = item.NormType;
                        allotmentForwardCategory.ProductionPeriodType = item.ProductionPeriodType;
                        allotmentForwardCategory.Id = this.GetNewObjectId();
                        var entitys = ObjectMapper.Map<CrudAllotmentForwardCategoryDto, AllotmentForwardCategory>(allotmentForwardCategory);
                        await _allotmentForwardCategoryService.CreateAsync(entitys);

                    }
                }

            }
            if (dto.LstDataForYear.Contains("AccBalanceSheet") == true)
            {
                var accBalanceSheet = await _defaultAccBalanceSheetService.GetQueryableAsync();
                accBalanceSheet = accBalanceSheet.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var tenantAccBalanceSheet = await _tenantAccBalanceSheetService.GetQueryableAsync();
                var lstTenantAccBalanceSheet = tenantAccBalanceSheet.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstTenantAccBalanceSheets = tenantAccBalanceSheet.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstTenantAccBalanceSheets)
                {
                    await _tenantAccBalanceSheetService.DeleteAsync(item);
                }
                if (lstTenantAccBalanceSheet.Count > 0)
                {
                    foreach (var item in lstTenantAccBalanceSheet)
                    {
                        CrudAccBalanceSheetDto tenantAccBalance = new CrudAccBalanceSheetDto();
                        tenantAccBalance.Id = this.GetNewObjectId();
                        tenantAccBalance.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        tenantAccBalance.Ord = item.Ord;
                        tenantAccBalance.UsingDecision = item.UsingDecision;
                        tenantAccBalance.Printable = item.Printable;
                        tenantAccBalance.Bold = item.Bold;
                        tenantAccBalance.DebitOrCredit = item.DebitOrCredit;
                        tenantAccBalance.Type = item.Type;
                        tenantAccBalance.Acc = item.Acc;
                        tenantAccBalance.NumberCode = item.NumberCode;
                        tenantAccBalance.Rank = item.Rank;
                        tenantAccBalance.Formular = item.Formular;
                        tenantAccBalance.TargetCode = item.TargetCode;
                        tenantAccBalance.Htkt = item.Htkt;
                        tenantAccBalance.Description = item.Description;
                        tenantAccBalance.DescriptionE = item.DescriptionE;
                        tenantAccBalance.OpeningAmount = item.OpeningAmount;
                        tenantAccBalance.EndingAmount = item.EndingAmount;
                        tenantAccBalance.EndingAmountCur = item.EndingAmountCur;
                        tenantAccBalance.CarryingCurrency = item.CarryingCurrency;
                        tenantAccBalance.IsSummary = item.IsSummary;
                        tenantAccBalance.Edit = item.Edit;
                        tenantAccBalance.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudAccBalanceSheetDto, TenantAccBalanceSheet>(tenantAccBalance);
                        await _tenantAccBalanceSheetService.CreateAsync(entitys);

                    }
                }
                else
                {
                    foreach (var item in accBalanceSheet)
                    {
                        CrudAccBalanceSheetDto tenantAccBalance = new CrudAccBalanceSheetDto();
                        tenantAccBalance.Id = this.GetNewObjectId();
                        tenantAccBalance.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        tenantAccBalance.Ord = item.Ord;
                        tenantAccBalance.OrgCode = _webHelper.GetCurrentOrgUnit();
                        tenantAccBalance.UsingDecision = item.UsingDecision;
                        tenantAccBalance.Printable = item.Printable;
                        tenantAccBalance.Bold = item.Bold;
                        tenantAccBalance.DebitOrCredit = item.DebitOrCredit;
                        tenantAccBalance.Type = item.Type;
                        tenantAccBalance.Acc = item.Acc;
                        tenantAccBalance.NumberCode = item.NumberCode;
                        tenantAccBalance.Rank = item.Rank;
                        tenantAccBalance.Formular = item.Formular;
                        tenantAccBalance.TargetCode = item.TargetCode;
                        tenantAccBalance.Htkt = item.Htkt;
                        tenantAccBalance.Description = item.Description;
                        tenantAccBalance.DescriptionE = item.DescriptionE;
                        tenantAccBalance.OpeningAmount = item.OpeningAmount;
                        tenantAccBalance.EndingAmount = item.EndingAmount;
                        tenantAccBalance.EndingAmountCur = item.EndingAmountCur;
                        tenantAccBalance.CarryingCurrency = item.CarryingCurrency;
                        tenantAccBalance.IsSummary = item.IsSummary;
                        tenantAccBalance.Edit = item.Edit;

                        var entitys = ObjectMapper.Map<CrudAccBalanceSheetDto, TenantAccBalanceSheet>(tenantAccBalance);
                        await _tenantAccBalanceSheetService.CreateAsync(entitys);

                    }
                }

            }
            if (dto.LstDataForYear.Contains("BusinessResult") == true)
            {
                var defaubusinessResult = await _defaultBusinessResultService.GetQueryableAsync();
                defaubusinessResult = defaubusinessResult.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var businessResult = await _tenantBusinessResultService.GetQueryableAsync();
                var lstBusinessResult = businessResult.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstBusinessResults = businessResult.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstBusinessResults)
                {
                    await _tenantBusinessResultService.DeleteAsync(item, true);
                }
                if (lstBusinessResult.Count > 0)
                {
                    foreach (var item in lstBusinessResult)
                    {
                        CrudTenantBusinessResultDto tenantBusinessResult = new CrudTenantBusinessResultDto();
                        tenantBusinessResult.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        tenantBusinessResult.Id = this.GetNewObjectId();
                        tenantBusinessResult.Ord = item.Ord;
                        tenantBusinessResult.OrgCode = item.OrgCode;
                        tenantBusinessResult.UsingDecision = item.UsingDecision;
                        tenantBusinessResult.Printable = item.Printable;
                        tenantBusinessResult.Bold = item.Bold;
                        tenantBusinessResult.NumberCode = item.NumberCode;
                        tenantBusinessResult.Rank = item.Rank;
                        tenantBusinessResult.Formular = item.Formular;
                        tenantBusinessResult.DebitAcc = item.DebitAcc;
                        tenantBusinessResult.CreditAcc = item.CreditAcc;
                        tenantBusinessResult.PartnerCode = item.PartnerCode;
                        tenantBusinessResult.FProductWorkCode = item.FProductWorkCode;
                        tenantBusinessResult.SectionCode = item.SectionCode;
                        tenantBusinessResult.TargetCode = item.TargetCode;
                        tenantBusinessResult.Htkt = item.Htkt;
                        tenantBusinessResult.Description = item.Description;
                        tenantBusinessResult.DescriptionE = item.DescriptionE;
                        tenantBusinessResult.LastPeriod = item.LastPeriod;
                        tenantBusinessResult.ThisPeriod = item.ThisPeriod;
                        tenantBusinessResult.AccumulatedLastPeriod = item.AccumulatedLastPeriod;
                        tenantBusinessResult.AccumulatedThisPeriod = item.AccumulatedThisPeriod;
                        tenantBusinessResult.CarryingCurrency = item.CarryingCurrency;
                        tenantBusinessResult.IsSummary = item.IsSummary;
                        tenantBusinessResult.Edit = item.Edit;
                        var entitys = ObjectMapper.Map<CrudTenantBusinessResultDto, TenantBusinessResult>(tenantBusinessResult);
                        await _tenantBusinessResultService.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaubusinessResult)
                    {
                        CrudTenantBusinessResultDto tenantBusinessResult = new CrudTenantBusinessResultDto();
                        tenantBusinessResult.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        tenantBusinessResult.Id = this.GetNewObjectId();
                        tenantBusinessResult.Ord = item.Ord;
                        tenantBusinessResult.OrgCode = _webHelper.GetCurrentOrgUnit();
                        tenantBusinessResult.UsingDecision = item.UsingDecision;
                        tenantBusinessResult.Printable = item.Printable;
                        tenantBusinessResult.Bold = item.Bold;
                        tenantBusinessResult.NumberCode = item.NumberCode;
                        tenantBusinessResult.Rank = item.Rank;
                        tenantBusinessResult.Formular = item.Formular;
                        tenantBusinessResult.DebitAcc = item.DebitAcc;
                        tenantBusinessResult.CreditAcc = item.CreditAcc;
                        tenantBusinessResult.PartnerCode = item.PartnerCode;
                        tenantBusinessResult.FProductWorkCode = item.FProductWorkCode;
                        tenantBusinessResult.SectionCode = item.SectionCode;
                        tenantBusinessResult.TargetCode = item.TargetCode;
                        tenantBusinessResult.Htkt = item.Htkt;
                        tenantBusinessResult.Description = item.Description;
                        tenantBusinessResult.DescriptionE = item.DescriptionE;
                        tenantBusinessResult.LastPeriod = item.LastPeriod;
                        tenantBusinessResult.ThisPeriod = item.ThisPeriod;
                        tenantBusinessResult.AccumulatedLastPeriod = item.AccumulatedLastPeriod;
                        tenantBusinessResult.AccumulatedThisPeriod = item.AccumulatedThisPeriod;
                        tenantBusinessResult.CarryingCurrency = item.CarryingCurrency;
                        tenantBusinessResult.IsSummary = item.IsSummary;
                        tenantBusinessResult.Edit = item.Edit;
                        var entitys = ObjectMapper.Map<CrudTenantBusinessResultDto, TenantBusinessResult>(tenantBusinessResult);
                        await _tenantBusinessResultService.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("CashFollowStatement") == true)
            {
                var defaultCashFollowStatements = await _defaultCashFollowStatementService.GetQueryableAsync();
                defaultCashFollowStatements = defaultCashFollowStatements.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var tenantCashFollowStatement = await _tenantCashFollowStatementService.GetQueryableAsync();
                var lstTenantCashFollowStatementService = tenantCashFollowStatement.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstTenantCashFollowStatementServices = tenantCashFollowStatement.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();

                foreach (var item in lstTenantCashFollowStatementServices)
                {
                    await _tenantCashFollowStatementService.DeleteAsync(item, true);
                }

                if (lstTenantCashFollowStatementService.Count > 0)
                {
                    foreach (var item in lstTenantCashFollowStatementService)
                    {
                        CrudTenantCashFollowStatementDto crudTenantCashFollowStatementDto = new CrudTenantCashFollowStatementDto();
                        crudTenantCashFollowStatementDto.Id = this.GetNewObjectId();
                        crudTenantCashFollowStatementDto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudTenantCashFollowStatementDto.Ord = item.Ord;
                        crudTenantCashFollowStatementDto.UsingDecision = item.UsingDecision;
                        crudTenantCashFollowStatementDto.Printable = item.Printable;
                        crudTenantCashFollowStatementDto.Bold = item.Bold;
                        crudTenantCashFollowStatementDto.NumberCode = item.NumberCode;
                        crudTenantCashFollowStatementDto.Rank = item.Rank;
                        crudTenantCashFollowStatementDto.Formular = item.Formular;
                        crudTenantCashFollowStatementDto.DebitAcc = item.DebitAcc;
                        crudTenantCashFollowStatementDto.CreditAcc = item.CreditAcc;
                        crudTenantCashFollowStatementDto.PartnerCode = item.PartnerCode;
                        crudTenantCashFollowStatementDto.FProductWorkCode = item.FProductWorkCode;
                        crudTenantCashFollowStatementDto.SectionCode = item.SectionCode;
                        crudTenantCashFollowStatementDto.TargetCode = item.TargetCode;
                        crudTenantCashFollowStatementDto.Htkt = item.Htkt;
                        crudTenantCashFollowStatementDto.Method = item.Method;
                        crudTenantCashFollowStatementDto.Description = item.Description;
                        crudTenantCashFollowStatementDto.DescriptionE = item.DescriptionE;
                        crudTenantCashFollowStatementDto.LastPeriod = item.LastPeriod;
                        crudTenantCashFollowStatementDto.ThisPeriod = item.ThisPeriod;
                        crudTenantCashFollowStatementDto.AccumulatedLastPeriod = item.AccumulatedLastPeriod;
                        crudTenantCashFollowStatementDto.AccumulatedThisPeriod = item.AccumulatedThisPeriod;
                        crudTenantCashFollowStatementDto.CarryingCurrency = item.CarryingCurrency;
                        crudTenantCashFollowStatementDto.IsSummary = item.IsSummary;
                        crudTenantCashFollowStatementDto.Edit = item.Edit;
                        crudTenantCashFollowStatementDto.Negative = item.Negative;
                        crudTenantCashFollowStatementDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudTenantCashFollowStatementDto, TenantCashFollowStatement>(crudTenantCashFollowStatementDto);
                        await _tenantCashFollowStatementService.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultCashFollowStatements)
                    {
                        CrudTenantCashFollowStatementDto crudTenantCashFollowStatementDto = new CrudTenantCashFollowStatementDto();
                        crudTenantCashFollowStatementDto.Id = this.GetNewObjectId();
                        crudTenantCashFollowStatementDto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudTenantCashFollowStatementDto.Ord = item.Ord;
                        crudTenantCashFollowStatementDto.UsingDecision = item.UsingDecision;
                        crudTenantCashFollowStatementDto.Printable = item.Printable;
                        crudTenantCashFollowStatementDto.Bold = item.Bold;
                        crudTenantCashFollowStatementDto.NumberCode = item.NumberCode;
                        crudTenantCashFollowStatementDto.Rank = item.Rank;
                        crudTenantCashFollowStatementDto.Formular = item.Formular;
                        crudTenantCashFollowStatementDto.DebitAcc = item.DebitAcc;
                        crudTenantCashFollowStatementDto.CreditAcc = item.CreditAcc;
                        crudTenantCashFollowStatementDto.PartnerCode = item.PartnerCode;
                        crudTenantCashFollowStatementDto.FProductWorkCode = item.FProductWorkCode;
                        crudTenantCashFollowStatementDto.SectionCode = item.SectionCode;
                        crudTenantCashFollowStatementDto.TargetCode = item.TargetCode;
                        crudTenantCashFollowStatementDto.Htkt = item.Htkt;
                        crudTenantCashFollowStatementDto.Method = item.Method;
                        crudTenantCashFollowStatementDto.Description = item.Description;
                        crudTenantCashFollowStatementDto.DescriptionE = item.DescriptionE;
                        crudTenantCashFollowStatementDto.LastPeriod = item.LastPeriod;
                        crudTenantCashFollowStatementDto.ThisPeriod = item.ThisPeriod;
                        crudTenantCashFollowStatementDto.AccumulatedLastPeriod = item.AccumulatedLastPeriod;
                        crudTenantCashFollowStatementDto.AccumulatedThisPeriod = item.AccumulatedThisPeriod;
                        crudTenantCashFollowStatementDto.CarryingCurrency = item.CarryingCurrency;
                        crudTenantCashFollowStatementDto.IsSummary = item.IsSummary;
                        crudTenantCashFollowStatementDto.Edit = item.Edit;
                        crudTenantCashFollowStatementDto.Negative = item.Negative;
                        crudTenantCashFollowStatementDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudTenantCashFollowStatementDto, TenantCashFollowStatement>(crudTenantCashFollowStatementDto);
                        await _tenantCashFollowStatementService.CreateAsync(entitys);
                    }
                }


            }
            if (dto.LstDataForYear.Contains("SO_THZ") == true)
            {
                var soTHZ = await _soTHZService.GetQueryableAsync();

                var lstSoTHZ = soTHZ.Where(p => p.Year == year && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var lstSoTHZs = soTHZ.Where(p => p.Year == year + 1 && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                if (lstSoTHZs.Count > 0)
                {
                    await _soTHZService.DeleteManyAsync(lstSoTHZs);
                }

                foreach (var item in lstSoTHZ)
                {
                    CrudSoTHZRpDto CrudSoTHZRpDto = new CrudSoTHZRpDto();
                    CrudSoTHZRpDto.Id = this.GetNewObjectId();
                    CrudSoTHZRpDto.OrgCode = item.OrgCode;
                    CrudSoTHZRpDto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                    CrudSoTHZRpDto.UsingDecision = item.UsingDecision;
                    CrudSoTHZRpDto.Ord = item.Ord;
                    CrudSoTHZRpDto.FinanceDecision = item.FinanceDecision;
                    CrudSoTHZRpDto.FProductOrWork = item.FProductOrWork;
                    CrudSoTHZRpDto.FieldName = item.FieldName;
                    CrudSoTHZRpDto.FieldType = item.FieldType;
                    CrudSoTHZRpDto.DebitAcc = item.DebitAcc;
                    CrudSoTHZRpDto.DebitSection = item.DebitSection;
                    CrudSoTHZRpDto.DebitFProductWork = item.DebitFProductWork;
                    CrudSoTHZRpDto.CreditAcc = item.CreditAcc;
                    CrudSoTHZRpDto.CreditSection = item.CreditSection;
                    CrudSoTHZRpDto.CreditFProductWork = item.CreditFProductWork;
                    CrudSoTHZRpDto.TSum = item.TSum;
                    CrudSoTHZRpDto.TGet = item.TGet;
                    CrudSoTHZRpDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                    var entitys = ObjectMapper.Map<CrudSoTHZRpDto, SoTHZ>(CrudSoTHZRpDto);
                    await _soTHZService.CreateAsync(entitys);
                }
            }
            if (dto.LstDataForYear.Contains("StatementTax") == true)
            {
                var defaultStatementTaxes = await _defaultStatementTaxService.GetQueryableAsync();
                var tenantStatementTaxes = await _tenantStatementTaxService.GetQueryableAsync();

                var lsttenantStatementTaxe = tenantStatementTaxes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lsttenantStatementTaxes = tenantStatementTaxes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lsttenantStatementTaxes)
                {
                    await _tenantStatementTaxService.DeleteAsync(item, true);
                }

                if (lsttenantStatementTaxe.Count > 0)
                {
                    foreach (var item in lsttenantStatementTaxe)
                    {
                        item.Year = year + 1;
                        item.OrgCode = _webHelper.GetCurrentOrgUnit();
                        await _tenantStatementTaxService.CreateAsync(item);
                    }
                }
                else
                {
                    foreach (var item in defaultStatementTaxes)
                    {
                        CrudTenantStatementTaxDto crudTenantStatementTaxDto = new CrudTenantStatementTaxDto();
                        crudTenantStatementTaxDto.Year = year + 1;
                        crudTenantStatementTaxDto.Ord = item.Ord;
                        crudTenantStatementTaxDto.Printable = item.Printable;
                        crudTenantStatementTaxDto.Bold = item.Bold;
                        crudTenantStatementTaxDto.Ord0 = item.Ord0;
                        crudTenantStatementTaxDto.Description = item.Description;
                        crudTenantStatementTaxDto.DescriptionE = item.DescriptionE;
                        crudTenantStatementTaxDto.Rank = item.Rank;
                        crudTenantStatementTaxDto.NumberCode = item.NumberCode;
                        crudTenantStatementTaxDto.Formular = item.Formular;
                        crudTenantStatementTaxDto.DebitAcc = item.DebitAcc;
                        crudTenantStatementTaxDto.CreditAcc = item.CreditAcc;
                        crudTenantStatementTaxDto.Condition = item.Condition;
                        crudTenantStatementTaxDto.Sign = item.Sign;
                        crudTenantStatementTaxDto.NumberCode1 = item.NumberCode1;
                        crudTenantStatementTaxDto.Amount1 = item.Amount1;
                        crudTenantStatementTaxDto.NumberCode2 = item.NumberCode2;
                        crudTenantStatementTaxDto.Amount2 = item.Amount2;
                        crudTenantStatementTaxDto.PrintWhen = item.PrintWhen;
                        crudTenantStatementTaxDto.Id11 = item.Id11;
                        crudTenantStatementTaxDto.Id12 = item.Id12;
                        crudTenantStatementTaxDto.Id21 = item.Id21;
                        crudTenantStatementTaxDto.Id22 = item.Id22;
                        crudTenantStatementTaxDto.En1 = item.En1;
                        crudTenantStatementTaxDto.En2 = item.En2;
                        crudTenantStatementTaxDto.Re1 = item.Re1;
                        crudTenantStatementTaxDto.Re2 = item.Re2;
                        crudTenantStatementTaxDto.Va1 = item.Va1;
                        crudTenantStatementTaxDto.Va2 = item.Va2;
                        crudTenantStatementTaxDto.Mt1 = item.Mt1;
                        crudTenantStatementTaxDto.Mt2 = item.Mt2;
                        crudTenantStatementTaxDto.AssignValue = item.AssignValue;
                        crudTenantStatementTaxDto.Id = this.GetNewObjectId();
                        var entitys = ObjectMapper.Map<CrudTenantStatementTaxDto, TenantStatementTax>(crudTenantStatementTaxDto);
                        await _tenantStatementTaxService.CreateAsync(entitys);
                    }
                }



            }
            if (dto.LstDataForYear.Contains("FProductWorkNorm") == true)
            {
                var fProductWorkNormService = await _fProductWorkNormService.GetQueryableAsync();
                var lstFProductWorkNormService = fProductWorkNormService.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstFProductWorkNormServiceDe = fProductWorkNormService.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();

                foreach (var item in lstFProductWorkNormServiceDe)
                {
                    await _fProductWorkNormService.DeleteAsync(item, true);
                }
                foreach (var item in lstFProductWorkNormService)
                {
                    var lstDetail = await _fProductWorkNormDetailService.GetByFProductWorkNormIdAsync(item.Id);
                    var id = this.GetNewObjectId();
                    CrudFProductWorkNormDto crudFProductWorkNormDto = new CrudFProductWorkNormDto();
                    crudFProductWorkNormDto.Id = id;
                    crudFProductWorkNormDto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                    crudFProductWorkNormDto.FProductWorkCode = item.FProductWorkCode;
                    crudFProductWorkNormDto.Quantity = item.Quantity;
                    crudFProductWorkNormDto.Note = item.Note;
                    crudFProductWorkNormDto.OrgCode = item.OrgCode;
                    List<CrudFProductWorkNormDetailDto> crudFProductWorkNormDetail = new List<CrudFProductWorkNormDetailDto>();
                    foreach (var items in lstDetail)
                    {
                        CrudFProductWorkNormDetailDto crudFProductWorkNormDetailDto = new CrudFProductWorkNormDetailDto();
                        crudFProductWorkNormDetailDto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFProductWorkNormDetailDto.Id = this.GetNewObjectId();
                        crudFProductWorkNormDetailDto.FProductWorkNormId = id;
                        crudFProductWorkNormDetailDto.Month = items.Month;
                        crudFProductWorkNormDetailDto.BeginDate = items.BeginDate;
                        crudFProductWorkNormDetailDto.EndDate = items.EndDate;
                        crudFProductWorkNormDetailDto.Ord0 = items.Ord0;
                        crudFProductWorkNormDetailDto.AccCode = items.AccCode;
                        crudFProductWorkNormDetailDto.WorkPlaceCode = items.WorkPlaceCode;
                        crudFProductWorkNormDetailDto.SectionCode = items.SectionCode;
                        crudFProductWorkNormDetailDto.WarehouseCode = items.WarehouseCode;
                        crudFProductWorkNormDetailDto.ProductCode = items.ProductCode;
                        crudFProductWorkNormDetailDto.ProductLotCode = items.ProductLotCode;
                        crudFProductWorkNormDetailDto.ProductOrigin = items.ProductOrigin;
                        crudFProductWorkNormDetailDto.UnitCode = items.UnitCode;
                        crudFProductWorkNormDetailDto.Quantity = items.Quantity;
                        crudFProductWorkNormDetailDto.QuantityLoss = items.QuantityLoss;
                        crudFProductWorkNormDetailDto.PercentLoss = items.PercentLoss;
                        crudFProductWorkNormDetailDto.Price = items.Price;
                        crudFProductWorkNormDetailDto.PriceCur = items.PriceCur;
                        crudFProductWorkNormDetailDto.Amount = items.Amount;
                        crudFProductWorkNormDetailDto.AmountCur = items.AmountCur;
                        crudFProductWorkNormDetailDto.ApplicableDate1 = items.ApplicableDate1;
                        crudFProductWorkNormDetailDto.ApplicableDate2 = items.ApplicableDate2;
                        crudFProductWorkNormDetailDto.OrgCode = items.OrgCode;
                        crudFProductWorkNormDetail.Add(crudFProductWorkNormDetailDto);

                    }
                    crudFProductWorkNormDto.FProductWorkNormDetails = crudFProductWorkNormDetail;
                    var entitys = ObjectMapper.Map<CrudFProductWorkNormDto, FProductWorkNorm>(crudFProductWorkNormDto);

                    await _fProductWorkNormService.CreateAsync(entitys);

                }


            }
            if (dto.LstDataForYear.Contains("GroupCoefficientDetail") == true)
            {
                var groupCoefficientDetail = await _groupCoefficientDetailService.GetQueryableAsync();
                var lstGroupCoefficientDetail = groupCoefficientDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstGroupCoefficientDetails = groupCoefficientDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                if (lstGroupCoefficientDetails.Count > 0)
                {
                    await _groupCoefficientDetailService.DeleteManyAsync(lstGroupCoefficientDetails);
                }
                foreach (var item in lstGroupCoefficientDetail)
                {
                    CrudGroupCoefficientDetailDto crudGroupCoefficientDetailDto = new CrudGroupCoefficientDetailDto();
                    crudGroupCoefficientDetailDto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                    crudGroupCoefficientDetailDto.Id = this.GetNewObjectId();
                    crudGroupCoefficientDetailDto.FProductWork = item.FProductWork;
                    crudGroupCoefficientDetailDto.GroupCoefficientId = item.GroupCoefficientId;
                    crudGroupCoefficientDetailDto.FProductWorkCode = item.FProductWorkCode;
                    crudGroupCoefficientDetailDto.GroupCoefficientCode = item.GroupCoefficientCode;
                    crudGroupCoefficientDetailDto.January = item.January;
                    crudGroupCoefficientDetailDto.February = item.February;
                    crudGroupCoefficientDetailDto.March = item.March;
                    crudGroupCoefficientDetailDto.April = item.April;
                    crudGroupCoefficientDetailDto.May = item.May;
                    crudGroupCoefficientDetailDto.June = item.June;
                    crudGroupCoefficientDetailDto.August = item.August;
                    crudGroupCoefficientDetailDto.September = item.September;
                    crudGroupCoefficientDetailDto.October = item.October;
                    crudGroupCoefficientDetailDto.November = item.November;
                    crudGroupCoefficientDetailDto.December = item.December;
                    crudGroupCoefficientDetailDto.OrgCode = item.OrgCode;
                    var entitys = ObjectMapper.Map<CrudGroupCoefficientDetailDto, GroupCoefficientDetail>(crudGroupCoefficientDetailDto);
                    await _groupCoefficientDetailService.CreateAsync(entitys);
                }
            }
            if (dto.LstDataForYear.Contains("FStatement133L01") == true)
            {
                var defaultfStatement133L01s = await _defaultFStatement133L01Service.GetQueryableAsync();
                defaultfStatement133L01s = defaultfStatement133L01s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);


                var fStatement133L01 = await _fStatement133L01Service.GetQueryableAsync();
                var lstFStatement133L01 = fStatement133L01.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstFStatement133L01s = fStatement133L01.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstFStatement133L01s)
                {
                    await _fStatement133L01Service.DeleteAsync(item, true);
                }
                if (lstFStatement133L01.Count > 0)
                {
                    foreach (var item in lstFStatement133L01)
                    {
                        CrudFStatement133L01Dto crudTenantFStatement133L01Dto = new CrudFStatement133L01Dto();
                        crudTenantFStatement133L01Dto.Id = this.GetNewObjectId();
                        crudTenantFStatement133L01Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudTenantFStatement133L01Dto.Ord = item.Ord;
                        crudTenantFStatement133L01Dto.UsingDecision = item.UsingDecision;
                        crudTenantFStatement133L01Dto.Sort = item.Sort;
                        crudTenantFStatement133L01Dto.Bold = item.Bold;
                        crudTenantFStatement133L01Dto.Printable = item.Printable;
                        crudTenantFStatement133L01Dto.GroupId = item.GroupId;
                        crudTenantFStatement133L01Dto.Description1 = item.Description1;
                        crudTenantFStatement133L01Dto.Description2 = item.Description2;
                        crudTenantFStatement133L01Dto.Title = item.Title;
                        var entitys = ObjectMapper.Map<CrudFStatement133L01Dto, TenantFStatement133L01>(crudTenantFStatement133L01Dto);
                        await _fStatement133L01Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultfStatement133L01s)
                    {
                        CrudFStatement133L01Dto crudTenantFStatement133L01Dto = new CrudFStatement133L01Dto();
                        crudTenantFStatement133L01Dto.Id = this.GetNewObjectId();
                        crudTenantFStatement133L01Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudTenantFStatement133L01Dto.Ord = item.Ord;
                        crudTenantFStatement133L01Dto.UsingDecision = item.UsingDecision;
                        crudTenantFStatement133L01Dto.Sort = item.Sort;
                        crudTenantFStatement133L01Dto.Bold = item.Bold;
                        crudTenantFStatement133L01Dto.Printable = item.Printable;
                        crudTenantFStatement133L01Dto.GroupId = item.GroupId;
                        crudTenantFStatement133L01Dto.Description1 = item.Description1;
                        crudTenantFStatement133L01Dto.Description2 = item.Description2;
                        crudTenantFStatement133L01Dto.Title = item.Title;
                        var entitys = ObjectMapper.Map<CrudFStatement133L01Dto, TenantFStatement133L01>(crudTenantFStatement133L01Dto);
                        await _fStatement133L01Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement133L02") == true)
            {
                var defaultFStatement133L02s = await _defaultFStatement133L02Service.GetQueryableAsync();
                defaultFStatement133L02s = defaultFStatement133L02s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement133L02Service = await _fStatement133L02Service.GetQueryableAsync();
                var lstFStatement133L02 = fStatement133L02Service.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstFStatement133L02s = fStatement133L02Service.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstFStatement133L02s)
                {
                    await _fStatement133L02Service.DeleteAsync(item, true);
                }
                if (lstFStatement133L02.Count > 0)
                {
                    foreach (var item in lstFStatement133L02)
                    {
                        CrudFStatement133L02Dto crudTenantFStatement133L02Dto = new CrudFStatement133L02Dto();
                        crudTenantFStatement133L02Dto.Id = this.GetNewObjectId();
                        crudTenantFStatement133L02Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudTenantFStatement133L02Dto.UsingDecision = item.UsingDecision;
                        crudTenantFStatement133L02Dto.Ord = item.Ord;
                        crudTenantFStatement133L02Dto.Sort = item.Sort;
                        crudTenantFStatement133L02Dto.Bold = item.Bold;
                        crudTenantFStatement133L02Dto.Printable = item.Printable;
                        crudTenantFStatement133L02Dto.GroupId = item.GroupId;
                        crudTenantFStatement133L02Dto.Description = item.Description;
                        crudTenantFStatement133L02Dto.DebitOrCredit = item.DebitOrCredit;
                        crudTenantFStatement133L02Dto.Type = item.Type;
                        crudTenantFStatement133L02Dto.Acc = item.Acc;
                        crudTenantFStatement133L02Dto.NumberCode = item.NumberCode;
                        crudTenantFStatement133L02Dto.Formular = item.Formular;
                        crudTenantFStatement133L02Dto.Method = item.Method;
                        crudTenantFStatement133L02Dto.DebitAcc = item.DebitAcc;
                        crudTenantFStatement133L02Dto.CreditAcc = item.CreditAcc;
                        crudTenantFStatement133L02Dto.OpeningAmount = item.OpeningAmount;
                        crudTenantFStatement133L02Dto.ClosingAmount = item.ClosingAmount;
                        crudTenantFStatement133L02Dto.Rank = item.Rank;
                        crudTenantFStatement133L02Dto.Title = item.Title;
                        crudTenantFStatement133L02Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement133L02Dto, TenantFStatement133L02>(crudTenantFStatement133L02Dto);
                        await _fStatement133L02Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement133L02s)
                    {
                        CrudFStatement133L02Dto crudTenantFStatement133L02Dto = new CrudFStatement133L02Dto();
                        crudTenantFStatement133L02Dto.Id = this.GetNewObjectId();
                        crudTenantFStatement133L02Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudTenantFStatement133L02Dto.UsingDecision = item.UsingDecision;
                        crudTenantFStatement133L02Dto.Ord = item.Ord;
                        crudTenantFStatement133L02Dto.Sort = item.Sort;
                        crudTenantFStatement133L02Dto.Bold = item.Bold;
                        crudTenantFStatement133L02Dto.Printable = item.Printable;
                        crudTenantFStatement133L02Dto.GroupId = item.GroupId;
                        crudTenantFStatement133L02Dto.Description = item.Description;
                        crudTenantFStatement133L02Dto.DebitOrCredit = item.DebitOrCredit;
                        crudTenantFStatement133L02Dto.Type = item.Type;
                        crudTenantFStatement133L02Dto.Acc = item.Acc;
                        crudTenantFStatement133L02Dto.NumberCode = item.NumberCode;
                        crudTenantFStatement133L02Dto.Formular = item.Formular;
                        crudTenantFStatement133L02Dto.Method = item.Method;
                        crudTenantFStatement133L02Dto.DebitAcc = item.DebitAcc;
                        crudTenantFStatement133L02Dto.CreditAcc = item.CreditAcc;
                        crudTenantFStatement133L02Dto.OpeningAmount = item.OpeningAmount;
                        crudTenantFStatement133L02Dto.ClosingAmount = item.ClosingAmount;
                        crudTenantFStatement133L02Dto.Rank = item.Rank;
                        crudTenantFStatement133L02Dto.Title = item.Title;
                        crudTenantFStatement133L02Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement133L02Dto, TenantFStatement133L02>(crudTenantFStatement133L02Dto);
                        await _fStatement133L02Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement133L03") == true)
            {
                var defaultFStatement133L03s = await _defaultFStatement133L03Service.GetQueryableAsync();
                defaultFStatement133L03s = defaultFStatement133L03s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement133L03 = await _fStatement133L03Service.GetQueryableAsync();
                var lstfStatement133L03 = fStatement133L03.Where(p => p.Year == year && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var lstfStatement133L03s = fStatement133L03.Where(p => p.Year == year && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                foreach (var item in lstfStatement133L03s)
                {
                    await _fStatement133L03Service.DeleteAsync(item, true);
                }
                if (lstfStatement133L03.Count > 0)
                {
                    foreach (var item in defaultFStatement133L03s)
                    {
                        CrudFStatement133L03Dto crudFStatement133L03Dto = new CrudFStatement133L03Dto();
                        crudFStatement133L03Dto.Id = this.GetNewObjectId();
                        crudFStatement133L03Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L03Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L03Dto.Ord = item.Ord;
                        crudFStatement133L03Dto.Sort = item.Sort;
                        crudFStatement133L03Dto.Bold = item.Bold;
                        crudFStatement133L03Dto.Printable = item.Printable;
                        crudFStatement133L03Dto.GroupId = item.GroupId;
                        crudFStatement133L03Dto.Description = item.Description;
                        crudFStatement133L03Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L03Dto.Type = item.Type;
                        crudFStatement133L03Dto.Acc = item.Acc;
                        crudFStatement133L03Dto.NumberCode = item.NumberCode;
                        crudFStatement133L03Dto.Formular = item.Formular;
                        crudFStatement133L03Dto.Method = item.Method;
                        crudFStatement133L03Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L03Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L03Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement133L03Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement133L03Dto.Rank = item.Rank;
                        crudFStatement133L03Dto.Title = item.Title;
                        crudFStatement133L03Dto.IncreaseAmount = item.IncreaseAmount;
                        crudFStatement133L03Dto.DecreaseAmount = item.DecreaseAmount;
                        crudFStatement133L03Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement133L03Dto, TenantFStatement133L03>(crudFStatement133L03Dto);
                        await _fStatement133L03Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in lstfStatement133L03)
                    {
                        CrudFStatement133L03Dto crudFStatement133L03Dto = new CrudFStatement133L03Dto();
                        crudFStatement133L03Dto.Id = this.GetNewObjectId();
                        crudFStatement133L03Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L03Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L03Dto.Ord = item.Ord;
                        crudFStatement133L03Dto.Sort = item.Sort;
                        crudFStatement133L03Dto.Bold = item.Bold;
                        crudFStatement133L03Dto.Printable = item.Printable;
                        crudFStatement133L03Dto.GroupId = item.GroupId;
                        crudFStatement133L03Dto.Description = item.Description;
                        crudFStatement133L03Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L03Dto.Type = item.Type;
                        crudFStatement133L03Dto.Acc = item.Acc;
                        crudFStatement133L03Dto.NumberCode = item.NumberCode;
                        crudFStatement133L03Dto.Formular = item.Formular;
                        crudFStatement133L03Dto.Method = item.Method;
                        crudFStatement133L03Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L03Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L03Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement133L03Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement133L03Dto.Rank = item.Rank;
                        crudFStatement133L03Dto.Title = item.Title;
                        crudFStatement133L03Dto.IncreaseAmount = item.IncreaseAmount;
                        crudFStatement133L03Dto.DecreaseAmount = item.DecreaseAmount;
                        crudFStatement133L03Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement133L03Dto, TenantFStatement133L03>(crudFStatement133L03Dto);
                        await _fStatement133L03Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement133L04") == true)
            {
                var defaultFStatement133L04s = await _defaultFStatement133L04Service.GetQueryableAsync();
                defaultFStatement133L04s = defaultFStatement133L04s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement133L04 = await _fStatement133L04Service.GetQueryableAsync();
                var lstfStatement133L04 = fStatement133L04.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement133L04s = fStatement133L04.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement133L04s)
                {
                    await _fStatement133L04Service.DeleteAsync(item, true);
                }
                if (lstfStatement133L04.Count > 0)
                {
                    foreach (var item in lstfStatement133L04)
                    {
                        CrudFStatement133L04Dto crudFStatement133L04Dto = new CrudFStatement133L04Dto();
                        crudFStatement133L04Dto.Id = this.GetNewObjectId();
                        crudFStatement133L04Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L04Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L04Dto.Ord = item.Ord;
                        crudFStatement133L04Dto.Sort = item.Sort;
                        crudFStatement133L04Dto.Bold = item.Bold;
                        crudFStatement133L04Dto.Printable = item.Printable;
                        crudFStatement133L04Dto.GroupId = item.GroupId;
                        crudFStatement133L04Dto.Description = item.Description;
                        crudFStatement133L04Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L04Dto.Type = item.Type;
                        crudFStatement133L04Dto.Acc = item.Acc;
                        crudFStatement133L04Dto.NumberCode = item.NumberCode;
                        crudFStatement133L04Dto.Formular = item.Formular;
                        crudFStatement133L04Dto.Method = item.Method;
                        crudFStatement133L04Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L04Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L04Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement133L04Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement133L04Dto.Rank = item.Rank;
                        crudFStatement133L04Dto.Title = item.Title;
                        crudFStatement133L04Dto.IncreaseAmount = item.IncreaseAmount;
                        crudFStatement133L04Dto.DecreaseAmount = item.DecreaseAmount;
                        crudFStatement133L04Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement133L04Dto, TenantFStatement133L04>(crudFStatement133L04Dto);
                        await _fStatement133L04Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement133L04s)
                    {
                        CrudFStatement133L04Dto crudFStatement133L04Dto = new CrudFStatement133L04Dto();
                        crudFStatement133L04Dto.Id = this.GetNewObjectId();
                        crudFStatement133L04Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L04Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L04Dto.Ord = item.Ord;
                        crudFStatement133L04Dto.Sort = item.Sort;
                        crudFStatement133L04Dto.Bold = item.Bold;
                        crudFStatement133L04Dto.Printable = item.Printable;
                        crudFStatement133L04Dto.GroupId = item.GroupId;
                        crudFStatement133L04Dto.Description = item.Description;
                        crudFStatement133L04Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L04Dto.Type = item.Type;
                        crudFStatement133L04Dto.Acc = item.Acc;
                        crudFStatement133L04Dto.NumberCode = item.NumberCode;
                        crudFStatement133L04Dto.Formular = item.Formular;
                        crudFStatement133L04Dto.Method = item.Method;
                        crudFStatement133L04Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L04Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L04Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement133L04Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement133L04Dto.Rank = item.Rank;
                        crudFStatement133L04Dto.Title = item.Title;
                        crudFStatement133L04Dto.IncreaseAmount = item.IncreaseAmount;
                        crudFStatement133L04Dto.DecreaseAmount = item.DecreaseAmount;
                        crudFStatement133L04Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement133L04Dto, TenantFStatement133L04>(crudFStatement133L04Dto);
                        await _fStatement133L04Service.CreateAsync(entitys);
                    }
                }


            }
            if (dto.LstDataForYear.Contains("FStatement133L05") == true)
            {
                var defaultFStatement133L05s = await _defaultFStatement133L05Service.GetQueryableAsync();
                defaultFStatement133L05s = defaultFStatement133L05s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement133L05 = await _fStatement133L05Service.GetQueryableAsync();
                var lstfStatement133L05 = fStatement133L05.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement133L05s = fStatement133L05.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement133L05s)
                {
                    await _fStatement133L05Service.DeleteAsync(item, true);
                }
                if (lstfStatement133L05.Count > 0)
                {
                    foreach (var item in lstfStatement133L05)
                    {
                        CrudFStatement133L05Dto crudFStatement133L05Dto = new CrudFStatement133L05Dto();
                        crudFStatement133L05Dto.Id = this.GetNewObjectId();
                        crudFStatement133L05Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L05Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L05Dto.Ord = item.Ord;
                        crudFStatement133L05Dto.Sort = item.Sort;
                        crudFStatement133L05Dto.Bold = item.Bold;
                        crudFStatement133L05Dto.Printable = item.Printable;
                        crudFStatement133L05Dto.GroupId = item.GroupId;
                        crudFStatement133L05Dto.Description = item.Description;
                        crudFStatement133L05Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L05Dto.Type = item.Type;
                        crudFStatement133L05Dto.Acc = item.Acc;
                        crudFStatement133L05Dto.NumberCode = item.NumberCode;
                        crudFStatement133L05Dto.Formular = item.Formular;
                        crudFStatement133L05Dto.Method = item.Method;
                        crudFStatement133L05Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L05Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L05Dto.DebitBalance1 = item.DebitBalance1;
                        crudFStatement133L05Dto.DebitBalance2 = item.DebitBalance2;
                        crudFStatement133L05Dto.CreditBalance1 = item.CreditBalance1;
                        crudFStatement133L05Dto.CreditBalance2 = item.CreditBalance2;
                        crudFStatement133L05Dto.Debit = item.Debit;
                        crudFStatement133L05Dto.Credit = item.Credit;
                        crudFStatement133L05Dto.Rank = item.Rank;
                        crudFStatement133L05Dto.Title = item.Title;
                        crudFStatement133L05Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement133L05Dto, TenantFStatement133L05>(crudFStatement133L05Dto);
                        await _fStatement133L05Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement133L05s)
                    {
                        CrudFStatement133L05Dto crudFStatement133L05Dto = new CrudFStatement133L05Dto();
                        crudFStatement133L05Dto.Id = this.GetNewObjectId();
                        crudFStatement133L05Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L05Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L05Dto.Ord = item.Ord;
                        crudFStatement133L05Dto.Sort = item.Sort;
                        crudFStatement133L05Dto.Bold = item.Bold;
                        crudFStatement133L05Dto.Printable = item.Printable;
                        crudFStatement133L05Dto.GroupId = item.GroupId;
                        crudFStatement133L05Dto.Description = item.Description;
                        crudFStatement133L05Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L05Dto.Type = item.Type;
                        crudFStatement133L05Dto.Acc = item.Acc;
                        crudFStatement133L05Dto.NumberCode = item.NumberCode;
                        crudFStatement133L05Dto.Formular = item.Formular;
                        crudFStatement133L05Dto.Method = item.Method;
                        crudFStatement133L05Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L05Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L05Dto.DebitBalance1 = item.DebitBalance1;
                        crudFStatement133L05Dto.DebitBalance2 = item.DebitBalance2;
                        crudFStatement133L05Dto.CreditBalance1 = item.CreditBalance1;
                        crudFStatement133L05Dto.CreditBalance2 = item.CreditBalance2;
                        crudFStatement133L05Dto.Debit = item.Debit;
                        crudFStatement133L05Dto.Credit = item.Credit;
                        crudFStatement133L05Dto.Rank = item.Rank;
                        crudFStatement133L05Dto.Title = item.Title;
                        crudFStatement133L05Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement133L05Dto, TenantFStatement133L05>(crudFStatement133L05Dto);
                        await _fStatement133L05Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement133L06") == true)
            {
                var defaultFStatement133L06s = await _defaultFStatement133L06Service.GetQueryableAsync();
                defaultFStatement133L06s = defaultFStatement133L06s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement133L06 = await _fStatement133L06Service.GetQueryableAsync();
                var lstffStatement133L06 = fStatement133L06.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstffStatement133L06s = fStatement133L06.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstffStatement133L06s)
                {
                    await _fStatement133L06Service.DeleteAsync(item, true);
                }
                if (lstffStatement133L06.Count > 0)
                {
                    foreach (var item in lstffStatement133L06)
                    {
                        CrudFStatement133L06Dto crudFStatement133L06Dto = new CrudFStatement133L06Dto();
                        crudFStatement133L06Dto.Id = this.GetNewObjectId();
                        crudFStatement133L06Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L06Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L06Dto.Ord = item.Ord;
                        crudFStatement133L06Dto.Sort = item.Sort;
                        crudFStatement133L06Dto.Bold = item.Bold;
                        crudFStatement133L06Dto.Printable = item.Printable;
                        crudFStatement133L06Dto.GroupId = item.GroupId;
                        crudFStatement133L06Dto.Description = item.Description;
                        crudFStatement133L06Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L06Dto.Type = item.Type;
                        crudFStatement133L06Dto.Acc = item.Acc;
                        crudFStatement133L06Dto.NumberCode = item.NumberCode;
                        crudFStatement133L06Dto.Formular = item.Formular;
                        crudFStatement133L06Dto.Method = item.Method;
                        crudFStatement133L06Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L06Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L06Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement133L06Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement133L06Dto.IncreaseAmount = item.IncreaseAmount;
                        crudFStatement133L06Dto.DecreaseAmount = item.DecreaseAmount;

                        crudFStatement133L06Dto.Rank = item.Rank;
                        crudFStatement133L06Dto.Title = item.Title;
                        crudFStatement133L06Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement133L06Dto, TenantFStatement133L06>(crudFStatement133L06Dto);
                        await _fStatement133L06Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement133L06s)
                    {
                        CrudFStatement133L06Dto crudFStatement133L06Dto = new CrudFStatement133L06Dto();
                        crudFStatement133L06Dto.Id = this.GetNewObjectId();
                        crudFStatement133L06Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L06Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L06Dto.Ord = item.Ord;
                        crudFStatement133L06Dto.Sort = item.Sort;
                        crudFStatement133L06Dto.Bold = item.Bold;
                        crudFStatement133L06Dto.Printable = item.Printable;
                        crudFStatement133L06Dto.GroupId = item.GroupId;
                        crudFStatement133L06Dto.Description = item.Description;
                        crudFStatement133L06Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L06Dto.Type = item.Type;
                        crudFStatement133L06Dto.Acc = item.Acc;
                        crudFStatement133L06Dto.NumberCode = item.NumberCode;
                        crudFStatement133L06Dto.Formular = item.Formular;
                        crudFStatement133L06Dto.Method = item.Method;
                        crudFStatement133L06Dto.DebitAcc = item.DebitAcc;
                        crudFStatement133L06Dto.CreditAcc = item.CreditAcc;
                        crudFStatement133L06Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement133L06Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement133L06Dto.IncreaseAmount = item.IncreaseAmount;
                        crudFStatement133L06Dto.DecreaseAmount = item.DecreaseAmount;

                        crudFStatement133L06Dto.Rank = item.Rank;
                        crudFStatement133L06Dto.Title = item.Title;
                        crudFStatement133L06Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement133L06Dto, TenantFStatement133L06>(crudFStatement133L06Dto);
                        await _fStatement133L06Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement133L07") == true)
            {
                var defaultFStatement133L07s = await _defaultFStatement133L07Service.GetQueryableAsync();
                defaultFStatement133L07s = defaultFStatement133L07s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement133L07 = await _fStatement133L07Service.GetQueryableAsync();
                var lstffStatement133L07 = fStatement133L07.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstffStatement133L07s = fStatement133L07.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstffStatement133L07s)
                {
                    await _fStatement133L07Service.DeleteAsync(item, true);
                }
                if (lstffStatement133L07.Count > 0)
                {
                    foreach (var item in lstffStatement133L07)
                    {
                        CrudFStatement133L07Dto crudFStatement133L07Dto = new CrudFStatement133L07Dto();
                        crudFStatement133L07Dto.Id = this.GetNewObjectId();
                        crudFStatement133L07Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L07Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L07Dto.Ord = item.Ord;
                        crudFStatement133L07Dto.Sort = item.Sort;
                        crudFStatement133L07Dto.Bold = item.Bold;
                        crudFStatement133L07Dto.Printable = item.Printable;
                        crudFStatement133L07Dto.GroupId = item.GroupId;
                        crudFStatement133L07Dto.Description = item.Description;
                        crudFStatement133L07Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L07Dto.Type = item.Type;
                        crudFStatement133L07Dto.Acc = item.Acc;
                        crudFStatement133L07Dto.NumberCode = item.NumberCode;
                        crudFStatement133L07Dto.Formular = item.Formular;
                        crudFStatement133L07Dto.Rank = item.Rank;
                        crudFStatement133L07Dto.Title = item.Title;
                        crudFStatement133L07Dto.Amount1 = item.Amount1;
                        crudFStatement133L07Dto.Amount2 = item.Amount2;
                        crudFStatement133L07Dto.Amount3 = item.Amount3;
                        crudFStatement133L07Dto.Amount4 = item.Amount4;
                        crudFStatement133L07Dto.Amount5 = item.Amount5;
                        crudFStatement133L07Dto.Amount6 = item.Amount6;
                        crudFStatement133L07Dto.Amount7 = item.Amount7;
                        crudFStatement133L07Dto.Amount8 = item.Amount8;
                        crudFStatement133L07Dto.Total = item.Total;
                        crudFStatement133L07Dto.Rank = item.Rank;
                        crudFStatement133L07Dto.Title = item.Title;
                        crudFStatement133L07Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement133L07Dto, TenantFStatement133L07>(crudFStatement133L07Dto);
                        await _fStatement133L07Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement133L07s)
                    {
                        CrudFStatement133L07Dto crudFStatement133L07Dto = new CrudFStatement133L07Dto();
                        crudFStatement133L07Dto.Id = this.GetNewObjectId();
                        crudFStatement133L07Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement133L07Dto.UsingDecision = item.UsingDecision;
                        crudFStatement133L07Dto.Ord = item.Ord;
                        crudFStatement133L07Dto.Sort = item.Sort;
                        crudFStatement133L07Dto.Bold = item.Bold;
                        crudFStatement133L07Dto.Printable = item.Printable;
                        crudFStatement133L07Dto.GroupId = item.GroupId;
                        crudFStatement133L07Dto.Description = item.Description;
                        crudFStatement133L07Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement133L07Dto.Type = item.Type;
                        crudFStatement133L07Dto.Acc = item.Acc;
                        crudFStatement133L07Dto.NumberCode = item.NumberCode;
                        crudFStatement133L07Dto.Formular = item.Formular;
                        crudFStatement133L07Dto.Rank = item.Rank;
                        crudFStatement133L07Dto.Title = item.Title;
                        crudFStatement133L07Dto.Amount1 = item.Amount1;
                        crudFStatement133L07Dto.Amount2 = item.Amount2;
                        crudFStatement133L07Dto.Amount3 = item.Amount3;
                        crudFStatement133L07Dto.Amount4 = item.Amount4;
                        crudFStatement133L07Dto.Amount5 = item.Amount5;
                        crudFStatement133L07Dto.Amount6 = item.Amount6;
                        crudFStatement133L07Dto.Amount7 = item.Amount7;
                        crudFStatement133L07Dto.Amount8 = item.Amount8;
                        crudFStatement133L07Dto.Total = item.Total;
                        crudFStatement133L07Dto.Rank = item.Rank;
                        crudFStatement133L07Dto.Title = item.Title;
                        crudFStatement133L07Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement133L07Dto, TenantFStatement133L07>(crudFStatement133L07Dto);
                        await _fStatement133L07Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L01") == true)
            {
                var defaultFStatement200L01s = await _defaultFStatement200L01Service.GetQueryableAsync();
                defaultFStatement200L01s = defaultFStatement200L01s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement200L01 = await _fStatement200L01Service.GetQueryableAsync();
                var lstfStatement200L01 = fStatement200L01.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L01s = fStatement200L01.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L01s)
                {
                    await _fStatement200L01Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L01.Count > 0)
                {
                    foreach (var item in lstfStatement200L01)
                    {
                        CrudFStatement200L01Dto crudFStatement200L01Dto = new CrudFStatement200L01Dto();
                        crudFStatement200L01Dto.Id = this.GetNewObjectId();
                        crudFStatement200L01Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L01Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L01Dto.Ord = item.Ord;
                        crudFStatement200L01Dto.Sort = item.Sort;
                        crudFStatement200L01Dto.Bold = item.Bold;
                        crudFStatement200L01Dto.Printable = item.Printable;
                        crudFStatement200L01Dto.GroupId = item.GroupId;
                        crudFStatement200L01Dto.Description = item.Description;
                        crudFStatement200L01Dto.DescriptionE = item.DescriptionE;
                        crudFStatement200L01Dto.Title = item.Title;
                        crudFStatement200L01Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L01Dto, TenantFStatement200L01>(crudFStatement200L01Dto);
                        await _fStatement200L01Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L01s)
                    {
                        CrudFStatement200L01Dto crudFStatement200L01Dto = new CrudFStatement200L01Dto();
                        crudFStatement200L01Dto.Id = this.GetNewObjectId();
                        crudFStatement200L01Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L01Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L01Dto.Ord = item.Ord;
                        crudFStatement200L01Dto.Sort = item.Sort;
                        crudFStatement200L01Dto.Bold = item.Bold;
                        crudFStatement200L01Dto.Printable = item.Printable;
                        crudFStatement200L01Dto.GroupId = item.GroupId;
                        crudFStatement200L01Dto.Description = item.Description;
                        crudFStatement200L01Dto.DescriptionE = item.DescriptionE;
                        crudFStatement200L01Dto.Title = item.Title;
                        crudFStatement200L01Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L01Dto, TenantFStatement200L01>(crudFStatement200L01Dto);
                        await _fStatement200L01Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L02") == true)
            {
                var defaultFStatement200L02s = await _defaultFStatement200L02Service.GetQueryableAsync();
                defaultFStatement200L02s = defaultFStatement200L02s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L02 = await _fStatement200L02Service.GetQueryableAsync();
                var lstfStatement200L02 = fStatement200L02.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();

                var lstfStatement200L02s = fStatement200L02.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L02s)
                {
                    await _fStatement200L02Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L02.Count > 0)
                {
                    foreach (var item in lstfStatement200L02)
                    {
                        CrudFStatement200L02Dto crudFStatement200L02Dto = new CrudFStatement200L02Dto();
                        crudFStatement200L02Dto.Id = this.GetNewObjectId();
                        crudFStatement200L02Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L02Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L02Dto.Ord = item.Ord;
                        crudFStatement200L02Dto.Sort = item.Sort;
                        crudFStatement200L02Dto.Bold = item.Bold;
                        crudFStatement200L02Dto.Printable = item.Printable;
                        crudFStatement200L02Dto.GroupId = item.GroupId;
                        crudFStatement200L02Dto.Description = item.Description;
                        crudFStatement200L02Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L02Dto.Acc = item.Acc;
                        crudFStatement200L02Dto.NumberCode = item.NumberCode;
                        crudFStatement200L02Dto.Formular = item.Formular;
                        crudFStatement200L02Dto.Method = item.Method;
                        crudFStatement200L02Dto.DebitAcc = item.DebitAcc;
                        crudFStatement200L02Dto.CreditAcc = item.CreditAcc;
                        crudFStatement200L02Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement200L02Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement200L02Dto.Rank = item.Rank;
                        crudFStatement200L02Dto.Title = item.Title;
                        crudFStatement200L02Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L02Dto, TenantFStatement200L02>(crudFStatement200L02Dto);
                        await _fStatement200L02Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L02s)
                    {
                        CrudFStatement200L02Dto crudFStatement200L02Dto = new CrudFStatement200L02Dto();
                        crudFStatement200L02Dto.Id = this.GetNewObjectId();
                        crudFStatement200L02Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L02Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L02Dto.Ord = item.Ord;
                        crudFStatement200L02Dto.Sort = item.Sort;
                        crudFStatement200L02Dto.Bold = item.Bold;
                        crudFStatement200L02Dto.Printable = item.Printable;
                        crudFStatement200L02Dto.GroupId = item.GroupId;
                        crudFStatement200L02Dto.Description = item.Description;
                        crudFStatement200L02Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L02Dto.Acc = item.Acc;
                        crudFStatement200L02Dto.NumberCode = item.NumberCode;
                        crudFStatement200L02Dto.Formular = item.Formular;
                        crudFStatement200L02Dto.Method = item.Method;
                        crudFStatement200L02Dto.DebitAcc = item.DebitAcc;
                        crudFStatement200L02Dto.CreditAcc = item.CreditAcc;
                        crudFStatement200L02Dto.OpeningAmount = item.OpeningAmount;
                        crudFStatement200L02Dto.ClosingAmount = item.ClosingAmount;
                        crudFStatement200L02Dto.Rank = item.Rank;
                        crudFStatement200L02Dto.Title = item.Title;
                        crudFStatement200L02Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L02Dto, TenantFStatement200L02>(crudFStatement200L02Dto);
                        await _fStatement200L02Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L03") == true)
            {
                var defaultFStatement200L03s = await _defaultFStatement200L03Service.GetQueryableAsync();
                defaultFStatement200L03s = defaultFStatement200L03s.Where(p => p.UsingDecision == lsrYearCategory.Year);
                var fStatement200L03 = await _fStatement200L03Service.GetQueryableAsync();
                var lstfStatement200L03 = fStatement200L03.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L03s = fStatement200L03.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L03s)
                {
                    await _fStatement200L03Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L03.Count > 0)
                {
                    foreach (var item in lstfStatement200L03)
                    {
                        CrudFStatement200L03Dto crudFStatement200L03Dto = new CrudFStatement200L03Dto();
                        crudFStatement200L03Dto.Id = this.GetNewObjectId();
                        crudFStatement200L03Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L03Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L03Dto.Ord = item.Ord;
                        crudFStatement200L03Dto.Sort = item.Sort;
                        crudFStatement200L03Dto.Bold = item.Bold;
                        crudFStatement200L03Dto.Printable = item.Printable;
                        crudFStatement200L03Dto.GroupId = item.GroupId;
                        crudFStatement200L03Dto.Description = item.Description;
                        crudFStatement200L03Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L03Dto.Type = item.Type;
                        crudFStatement200L03Dto.NumberCode = item.NumberCode;
                        crudFStatement200L03Dto.Rank = item.Rank;
                        crudFStatement200L03Dto.Formular = item.Formular;
                        crudFStatement200L03Dto.OriginalPriceAcc = item.OriginalPriceAcc;
                        crudFStatement200L03Dto.RecordingPriceAcc = item.RecordingPriceAcc;
                        crudFStatement200L03Dto.PreventivePriceAcc = item.PreventivePriceAcc;
                        crudFStatement200L03Dto.OriginalPrice2 = item.OriginalPrice2;
                        crudFStatement200L03Dto.RecordingPrice2 = item.RecordingPrice2;
                        crudFStatement200L03Dto.PreventivePrice2 = item.PreventivePrice2;
                        crudFStatement200L03Dto.OriginalPrice1 = item.OriginalPrice1;
                        crudFStatement200L03Dto.RecordingPrice1 = item.RecordingPrice1;
                        crudFStatement200L03Dto.PreventivePrice1 = item.PreventivePrice1;
                        crudFStatement200L03Dto.Title = item.Title;
                        crudFStatement200L03Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L03Dto, TenantFStatement200L03>(crudFStatement200L03Dto);
                        await _fStatement200L03Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L03s)
                    {
                        CrudFStatement200L03Dto crudFStatement200L03Dto = new CrudFStatement200L03Dto();
                        crudFStatement200L03Dto.Id = this.GetNewObjectId();
                        crudFStatement200L03Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L03Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L03Dto.Ord = item.Ord;
                        crudFStatement200L03Dto.Sort = item.Sort;
                        crudFStatement200L03Dto.Bold = item.Bold;
                        crudFStatement200L03Dto.Printable = item.Printable;
                        crudFStatement200L03Dto.GroupId = item.GroupId;
                        crudFStatement200L03Dto.Description = item.Description;
                        crudFStatement200L03Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L03Dto.Type = item.Type;
                        crudFStatement200L03Dto.NumberCode = item.NumberCode;
                        crudFStatement200L03Dto.Rank = item.Rank;
                        crudFStatement200L03Dto.Formular = item.Formular;
                        crudFStatement200L03Dto.OriginalPriceAcc = item.OriginalPriceAcc;
                        crudFStatement200L03Dto.RecordingPriceAcc = item.RecordingPriceAcc;
                        crudFStatement200L03Dto.PreventivePriceAcc = item.PreventivePriceAcc;
                        crudFStatement200L03Dto.OriginalPrice2 = item.OriginalPrice2;
                        crudFStatement200L03Dto.RecordingPrice2 = item.RecordingPrice2;
                        crudFStatement200L03Dto.PreventivePrice2 = item.PreventivePrice2;
                        crudFStatement200L03Dto.OriginalPrice1 = item.OriginalPrice1;
                        crudFStatement200L03Dto.RecordingPrice1 = item.RecordingPrice1;
                        crudFStatement200L03Dto.PreventivePrice1 = item.PreventivePrice1;
                        crudFStatement200L03Dto.Title = item.Title;
                        crudFStatement200L03Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L03Dto, TenantFStatement200L03>(crudFStatement200L03Dto);
                        await _fStatement200L03Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L04") == true)
            {
                var defaultFStatement200L04s = await _defaultFStatement200L04Service.GetQueryableAsync();
                defaultFStatement200L04s = defaultFStatement200L04s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement200L04 = await _fStatement200L04Service.GetQueryableAsync();
                var lstfStatement200L04 = fStatement200L04.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L04s = fStatement200L04.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L04s)
                {
                    await _fStatement200L04Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L04.Count > 0)
                {
                    foreach (var item in lstfStatement200L04)
                    {
                        CrudFStatement200L04Dto crudFStatement200L04Dto = new CrudFStatement200L04Dto();
                        crudFStatement200L04Dto.Id = this.GetNewObjectId();
                        crudFStatement200L04Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L04Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L04Dto.Ord = item.Ord;
                        crudFStatement200L04Dto.Sort = item.Sort;
                        crudFStatement200L04Dto.Bold = item.Bold;
                        crudFStatement200L04Dto.Printable = item.Printable;
                        crudFStatement200L04Dto.GroupId = item.GroupId;
                        crudFStatement200L04Dto.Description = item.Description;
                        crudFStatement200L04Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L04Dto.Type = item.Type;
                        crudFStatement200L04Dto.NumberCode = item.NumberCode;
                        crudFStatement200L04Dto.Rank = item.Rank;
                        crudFStatement200L04Dto.Formular = item.Formular;
                        crudFStatement200L04Dto.ValueAcc = item.ValueAcc;
                        crudFStatement200L04Dto.PreventiveAcc = item.PreventiveAcc;
                        crudFStatement200L04Dto.ValueAmount1 = item.ValueAmount1;
                        crudFStatement200L04Dto.PreventiveAmount1 = item.PreventiveAmount1;
                        crudFStatement200L04Dto.ValueAmount2 = item.ValueAmount2;
                        crudFStatement200L04Dto.PreventiveAmount2 = item.PreventiveAmount2;
                        crudFStatement200L04Dto.Title = item.Title;
                        crudFStatement200L04Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L04Dto, TenantFStatement200L04>(crudFStatement200L04Dto);
                        await _fStatement200L04Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L04s)
                    {
                        CrudFStatement200L04Dto crudFStatement200L04Dto = new CrudFStatement200L04Dto();
                        crudFStatement200L04Dto.Id = this.GetNewObjectId();
                        crudFStatement200L04Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L04Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L04Dto.Ord = item.Ord;
                        crudFStatement200L04Dto.Sort = item.Sort;
                        crudFStatement200L04Dto.Bold = item.Bold;
                        crudFStatement200L04Dto.Printable = item.Printable;
                        crudFStatement200L04Dto.GroupId = item.GroupId;
                        crudFStatement200L04Dto.Description = item.Description;
                        crudFStatement200L04Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L04Dto.Type = item.Type;
                        crudFStatement200L04Dto.NumberCode = item.NumberCode;
                        crudFStatement200L04Dto.Rank = item.Rank;
                        crudFStatement200L04Dto.Formular = item.Formular;
                        crudFStatement200L04Dto.ValueAcc = item.ValueAcc;
                        crudFStatement200L04Dto.PreventiveAcc = item.PreventiveAcc;
                        crudFStatement200L04Dto.ValueAmount1 = item.ValueAmount1;
                        crudFStatement200L04Dto.PreventiveAmount1 = item.PreventiveAmount1;
                        crudFStatement200L04Dto.ValueAmount2 = item.ValueAmount2;
                        crudFStatement200L04Dto.PreventiveAmount2 = item.PreventiveAmount2;
                        crudFStatement200L04Dto.Title = item.Title;
                        crudFStatement200L04Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L04Dto, TenantFStatement200L04>(crudFStatement200L04Dto);
                        await _fStatement200L04Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L05") == true)
            {
                var defaultFStatement200L05s = await _defaultFStatement200L05Service.GetQueryableAsync();
                defaultFStatement200L05s = defaultFStatement200L05s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L05 = await _fStatement200L05Service.GetQueryableAsync();

                var lstfStatement200L05 = fStatement200L05.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L05s = fStatement200L05.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L05s)
                {
                    await _fStatement200L05Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L05s.Count > 0)
                {
                    foreach (var item in lstfStatement200L05)
                    {
                        CrudFStatement200L05Dto crudFStatement200L05Dto = new CrudFStatement200L05Dto();
                        crudFStatement200L05Dto.Id = this.GetNewObjectId();
                        crudFStatement200L05Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L05Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L05Dto.Ord = item.Ord;
                        crudFStatement200L05Dto.Sort = item.Sort;
                        crudFStatement200L05Dto.Bold = item.Bold;
                        crudFStatement200L05Dto.Printable = item.Printable;
                        crudFStatement200L05Dto.GroupId = item.GroupId;
                        crudFStatement200L05Dto.Description = item.Description;
                        crudFStatement200L05Dto.Title = item.Title;
                        crudFStatement200L05Dto.Printable = item.Printable;
                        crudFStatement200L05Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L05Dto, TenantFStatement200L05>(crudFStatement200L05Dto);
                        await _fStatement200L05Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L05s)
                    {
                        CrudFStatement200L05Dto crudFStatement200L05Dto = new CrudFStatement200L05Dto();
                        crudFStatement200L05Dto.Id = this.GetNewObjectId();
                        crudFStatement200L05Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L05Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L05Dto.Ord = item.Ord;
                        crudFStatement200L05Dto.Sort = item.Sort;
                        crudFStatement200L05Dto.Bold = item.Bold;
                        crudFStatement200L05Dto.Printable = item.Printable;
                        crudFStatement200L05Dto.GroupId = item.GroupId;
                        crudFStatement200L05Dto.Description = item.Description;
                        crudFStatement200L05Dto.Title = item.Title;
                        crudFStatement200L05Dto.Printable = item.Printable;
                        crudFStatement200L05Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L05Dto, TenantFStatement200L05>(crudFStatement200L05Dto);
                        await _fStatement200L05Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L06") == true)
            {
                var defaultFStatement200L06s = await _defaultFStatement200L06Service.GetQueryableAsync();
                defaultFStatement200L06s = defaultFStatement200L06s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L06 = await _fStatement200L06Service.GetQueryableAsync();
                var lstfStatement200L06 = fStatement200L06.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L06s = fStatement200L06.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L06s)
                {
                    await _fStatement200L06Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L06.Count > 0)
                {
                    foreach (var item in lstfStatement200L06)
                    {
                        CrudFStatement200L06Dto crudFStatement200L06Dto = new CrudFStatement200L06Dto();
                        crudFStatement200L06Dto.Id = this.GetNewObjectId();
                        crudFStatement200L06Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L06Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L06Dto.Ord = item.Ord;
                        crudFStatement200L06Dto.Sort = item.Sort;
                        crudFStatement200L06Dto.Bold = item.Bold;
                        crudFStatement200L06Dto.Printable = item.Printable;
                        crudFStatement200L06Dto.GroupId = item.GroupId;
                        crudFStatement200L06Dto.Description = item.Description;
                        crudFStatement200L06Dto.Title = item.Title;
                        crudFStatement200L06Dto.Printable = item.Printable;
                        crudFStatement200L06Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L06Dto, TenantFStatement200L06>(crudFStatement200L06Dto);
                        await _fStatement200L06Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L06s)
                    {
                        CrudFStatement200L06Dto crudFStatement200L06Dto = new CrudFStatement200L06Dto();
                        crudFStatement200L06Dto.Id = this.GetNewObjectId();
                        crudFStatement200L06Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L06Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L06Dto.Ord = item.Ord;
                        crudFStatement200L06Dto.Sort = item.Sort;
                        crudFStatement200L06Dto.Bold = item.Bold;
                        crudFStatement200L06Dto.Printable = item.Printable;
                        crudFStatement200L06Dto.GroupId = item.GroupId;
                        crudFStatement200L06Dto.Description = item.Description;
                        crudFStatement200L06Dto.Title = item.Title;
                        crudFStatement200L06Dto.Printable = item.Printable;
                        crudFStatement200L06Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L06Dto, TenantFStatement200L06>(crudFStatement200L06Dto);
                        await _fStatement200L06Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L07") == true)
            {
                var defaultFStatement200L07s = await _defaultFStatement200L07Service.GetQueryableAsync();
                defaultFStatement200L07s = defaultFStatement200L07s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L07 = await _fStatement200L07Service.GetQueryableAsync();
                var lstfStatement200L07 = fStatement200L07.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L07s = fStatement200L07.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L07s)
                {
                    await _fStatement200L07Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L07.Count > 0)
                {
                    foreach (var item in lstfStatement200L07)
                    {
                        CrudFStatement200L07Dto crudFStatement200L07Dto = new CrudFStatement200L07Dto();
                        crudFStatement200L07Dto.Id = this.GetNewObjectId();
                        crudFStatement200L07Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L07Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L07Dto.Ord = item.Ord;
                        crudFStatement200L07Dto.Sort = item.Sort;
                        crudFStatement200L07Dto.Bold = item.Bold;
                        crudFStatement200L07Dto.Printable = item.Printable;
                        crudFStatement200L07Dto.GroupId = item.GroupId;
                        crudFStatement200L07Dto.Description = item.Description;
                        crudFStatement200L07Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L07Dto.Type = item.Type;
                        crudFStatement200L07Dto.NumberCode = item.NumberCode;
                        crudFStatement200L07Dto.Rank = item.Rank;
                        crudFStatement200L07Dto.Formular = item.Formular;
                        crudFStatement200L07Dto.OriginalPriceAcc = item.OriginalPriceAcc;
                        crudFStatement200L07Dto.PreventivePriceAcc = item.PreventivePriceAcc;
                        crudFStatement200L07Dto.OriginalPrice2 = item.OriginalPrice2;
                        crudFStatement200L07Dto.PreventivePrice2 = item.PreventivePrice2;
                        crudFStatement200L07Dto.OriginalPrice1 = item.OriginalPrice1;
                        crudFStatement200L07Dto.PreventivePrice1 = item.PreventivePrice1;
                        crudFStatement200L07Dto.Title = item.Title;
                        crudFStatement200L07Dto.Printable = item.Printable;
                        crudFStatement200L07Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L07Dto, TenantFStatement200L07>(crudFStatement200L07Dto);
                        await _fStatement200L07Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L07s)
                    {
                        CrudFStatement200L07Dto crudFStatement200L07Dto = new CrudFStatement200L07Dto();
                        crudFStatement200L07Dto.Id = this.GetNewObjectId();
                        crudFStatement200L07Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L07Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L07Dto.Ord = item.Ord;
                        crudFStatement200L07Dto.Sort = item.Sort;
                        crudFStatement200L07Dto.Bold = item.Bold;
                        crudFStatement200L07Dto.Printable = item.Printable;
                        crudFStatement200L07Dto.GroupId = item.GroupId;
                        crudFStatement200L07Dto.Description = item.Description;
                        crudFStatement200L07Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L07Dto.Type = item.Type;
                        crudFStatement200L07Dto.NumberCode = item.NumberCode;
                        crudFStatement200L07Dto.Rank = item.Rank;
                        crudFStatement200L07Dto.Formular = item.Formular;
                        crudFStatement200L07Dto.OriginalPriceAcc = item.OriginalPriceAcc;
                        crudFStatement200L07Dto.PreventivePriceAcc = item.PreventivePriceAcc;
                        crudFStatement200L07Dto.OriginalPrice2 = item.OriginalPrice2;
                        crudFStatement200L07Dto.PreventivePrice2 = item.PreventivePrice2;
                        crudFStatement200L07Dto.OriginalPrice1 = item.OriginalPrice1;
                        crudFStatement200L07Dto.PreventivePrice1 = item.PreventivePrice1;
                        crudFStatement200L07Dto.Title = item.Title;
                        crudFStatement200L07Dto.Printable = item.Printable;
                        crudFStatement200L07Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L07Dto, TenantFStatement200L07>(crudFStatement200L07Dto);
                        await _fStatement200L07Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L08") == true)
            {
                var defaultFStatement200L08s = await _defaultFStatement200L08Service.GetQueryableAsync();
                defaultFStatement200L08s = defaultFStatement200L08s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L08 = await _fStatement200L08Service.GetQueryableAsync();
                var lstfStatement200L08 = fStatement200L08.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L08s = fStatement200L08.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L08s)
                {
                    await _fStatement200L08Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L08.Count > 0)
                {
                    foreach (var item in lstfStatement200L08)
                    {
                        CrudFStatement200L08Dto crudFStatement200L08Dto = new CrudFStatement200L08Dto();
                        crudFStatement200L08Dto.Id = this.GetNewObjectId();
                        crudFStatement200L08Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L08Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L08Dto.Ord = item.Ord;
                        crudFStatement200L08Dto.Sort = item.Sort;
                        crudFStatement200L08Dto.Bold = item.Bold;
                        crudFStatement200L08Dto.Ord = item.Ord;
                        crudFStatement200L08Dto.Printable = item.Printable;
                        crudFStatement200L08Dto.GroupId = item.GroupId;
                        crudFStatement200L08Dto.Description = item.Description;

                        crudFStatement200L08Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L08Dto, TenantFStatement200L08>(crudFStatement200L08Dto);
                        await _fStatement200L08Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L08s)
                    {
                        CrudFStatement200L08Dto crudFStatement200L08Dto = new CrudFStatement200L08Dto();
                        crudFStatement200L08Dto.Id = this.GetNewObjectId();
                        crudFStatement200L08Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L08Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L08Dto.Ord = item.Ord;
                        crudFStatement200L08Dto.Sort = item.Sort;
                        crudFStatement200L08Dto.Bold = item.Bold;
                        crudFStatement200L08Dto.Ord = item.Ord;
                        crudFStatement200L08Dto.Printable = item.Printable;
                        crudFStatement200L08Dto.GroupId = item.GroupId;
                        crudFStatement200L08Dto.Description = item.Description;

                        crudFStatement200L08Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L08Dto, TenantFStatement200L08>(crudFStatement200L08Dto);
                        await _fStatement200L08Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L09") == true)
            {
                var defaultFStatement200L09s = await _defaultFStatement200L09Service.GetQueryableAsync();
                defaultFStatement200L09s = defaultFStatement200L09s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L09 = await _fStatement200L09Service.GetQueryableAsync();
                var lstfStatement200L09 = fStatement200L09.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L09s = fStatement200L09.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L09s)
                {
                    await _fStatement200L09Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L09.Count > 0)
                {
                    foreach (var item in lstfStatement200L09)
                    {
                        CrudFStatement200L09Dto crudFStatement200L09Dto = new CrudFStatement200L09Dto();
                        crudFStatement200L09Dto.Id = this.GetNewObjectId();
                        crudFStatement200L09Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L09Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L09Dto.Ord = item.Ord;
                        crudFStatement200L09Dto.Sort = item.Sort;
                        crudFStatement200L09Dto.Bold = item.Bold;
                        crudFStatement200L09Dto.Ord = item.Ord;
                        crudFStatement200L09Dto.Printable = item.Printable;
                        crudFStatement200L09Dto.GroupId = item.GroupId;
                        crudFStatement200L09Dto.Description = item.Description;
                        crudFStatement200L09Dto.NumberCode = item.NumberCode;
                        crudFStatement200L09Dto.Rank = item.Rank;
                        crudFStatement200L09Dto.Formular = item.Formular;
                        crudFStatement200L09Dto.FieldName = item.FieldName;
                        crudFStatement200L09Dto.Condition = item.Condition;
                        crudFStatement200L09Dto.HH1 = item.HH1;
                        crudFStatement200L09Dto.HH2 = item.HH2;
                        crudFStatement200L09Dto.HH3 = item.HH3;
                        crudFStatement200L09Dto.HH4 = item.HH4;
                        crudFStatement200L09Dto.HH5 = item.HH5;
                        crudFStatement200L09Dto.HH6 = item.HH6;
                        crudFStatement200L09Dto.HH7 = item.HH7;
                        crudFStatement200L09Dto.Total = item.Total;
                        crudFStatement200L09Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L09Dto, TenantFStatement200L09>(crudFStatement200L09Dto);
                        await _fStatement200L09Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L09s)
                    {
                        CrudFStatement200L09Dto crudFStatement200L09Dto = new CrudFStatement200L09Dto();
                        crudFStatement200L09Dto.Id = this.GetNewObjectId();
                        crudFStatement200L09Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L09Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L09Dto.Ord = item.Ord;
                        crudFStatement200L09Dto.Sort = item.Sort;
                        crudFStatement200L09Dto.Bold = item.Bold;
                        crudFStatement200L09Dto.Ord = item.Ord;
                        crudFStatement200L09Dto.Printable = item.Printable;
                        crudFStatement200L09Dto.GroupId = item.GroupId;
                        crudFStatement200L09Dto.Description = item.Description;
                        crudFStatement200L09Dto.NumberCode = item.NumberCode;
                        crudFStatement200L09Dto.Rank = item.Rank;
                        crudFStatement200L09Dto.Formular = item.Formular;
                        crudFStatement200L09Dto.FieldName = item.FieldName;
                        crudFStatement200L09Dto.Condition = item.Condition;
                        crudFStatement200L09Dto.HH1 = item.HH1;
                        crudFStatement200L09Dto.HH2 = item.HH2;
                        crudFStatement200L09Dto.HH3 = item.HH3;
                        crudFStatement200L09Dto.HH4 = item.HH4;
                        crudFStatement200L09Dto.HH5 = item.HH5;
                        crudFStatement200L09Dto.HH6 = item.HH6;
                        crudFStatement200L09Dto.HH7 = item.HH7;
                        crudFStatement200L09Dto.Total = item.Total;
                        crudFStatement200L09Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L09Dto, TenantFStatement200L09>(crudFStatement200L09Dto);
                        await _fStatement200L09Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L10") == true)
            {
                var defaultFStatement200L10s = await _defaultFStatement200L10Service.GetQueryableAsync();
                defaultFStatement200L10s = defaultFStatement200L10s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L10 = await _fStatement200L10Service.GetQueryableAsync();
                var lstfStatement200L10 = fStatement200L10.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L10s = fStatement200L10.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L10s)
                {
                    await _fStatement200L10Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L10.Count > 0)
                {
                    foreach (var item in lstfStatement200L10)
                    {
                        CrudFStatement200L10Dto crudFStatement200L10Dto = new CrudFStatement200L10Dto();
                        crudFStatement200L10Dto.Id = this.GetNewObjectId();
                        crudFStatement200L10Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L10Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L10Dto.Ord = item.Ord;
                        crudFStatement200L10Dto.Sort = item.Sort;
                        crudFStatement200L10Dto.Bold = item.Bold;
                        crudFStatement200L10Dto.Ord = item.Ord;
                        crudFStatement200L10Dto.Printable = item.Printable;
                        crudFStatement200L10Dto.GroupId = item.GroupId;
                        crudFStatement200L10Dto.Description = item.Description;
                        crudFStatement200L10Dto.NumberCode = item.NumberCode;
                        crudFStatement200L10Dto.Rank = item.Rank;
                        crudFStatement200L10Dto.Formular = item.Formular;
                        crudFStatement200L10Dto.FieldName = item.FieldName;
                        crudFStatement200L10Dto.Condition = item.Condition;
                        crudFStatement200L10Dto.VH1 = item.VH1;
                        crudFStatement200L10Dto.VH2 = item.VH2;
                        crudFStatement200L10Dto.VH3 = item.VH3;
                        crudFStatement200L10Dto.VH4 = item.VH4;
                        crudFStatement200L10Dto.VH5 = item.VH5;
                        crudFStatement200L10Dto.VH6 = item.VH6;
                        crudFStatement200L10Dto.VH7 = item.VH7;
                        crudFStatement200L10Dto.Total = item.Total;
                        crudFStatement200L10Dto.Title = item.Title;
                        crudFStatement200L10Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L10Dto, TenantFStatement200L10>(crudFStatement200L10Dto);
                        await _fStatement200L10Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L10s)
                    {
                        CrudFStatement200L10Dto crudFStatement200L10Dto = new CrudFStatement200L10Dto();
                        crudFStatement200L10Dto.Id = this.GetNewObjectId();
                        crudFStatement200L10Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L10Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L10Dto.Ord = item.Ord;
                        crudFStatement200L10Dto.Sort = item.Sort;
                        crudFStatement200L10Dto.Bold = item.Bold;
                        crudFStatement200L10Dto.Ord = item.Ord;
                        crudFStatement200L10Dto.Printable = item.Printable;
                        crudFStatement200L10Dto.GroupId = item.GroupId;
                        crudFStatement200L10Dto.Description = item.Description;
                        crudFStatement200L10Dto.NumberCode = item.NumberCode;
                        crudFStatement200L10Dto.Rank = item.Rank;
                        crudFStatement200L10Dto.Formular = item.Formular;
                        crudFStatement200L10Dto.FieldName = item.FieldName;
                        crudFStatement200L10Dto.Condition = item.Condition;
                        crudFStatement200L10Dto.VH1 = item.VH1;
                        crudFStatement200L10Dto.VH2 = item.VH2;
                        crudFStatement200L10Dto.VH3 = item.VH3;
                        crudFStatement200L10Dto.VH4 = item.VH4;
                        crudFStatement200L10Dto.VH5 = item.VH5;
                        crudFStatement200L10Dto.VH6 = item.VH6;
                        crudFStatement200L10Dto.VH7 = item.VH7;
                        crudFStatement200L10Dto.Total = item.Total;
                        crudFStatement200L10Dto.Title = item.Title;
                        crudFStatement200L10Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L10Dto, TenantFStatement200L10>(crudFStatement200L10Dto);
                        await _fStatement200L10Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L11") == true)
            {
                var defaultFStatement200L11s = await _defaultFStatement200L11Service.GetQueryableAsync();
                defaultFStatement200L11s = defaultFStatement200L11s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L11 = await _fStatement200L11Service.GetQueryableAsync();
                var lstfStatement200L11 = fStatement200L11.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L11s = fStatement200L11.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L11s)
                {
                    await _fStatement200L11Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L11.Count > 0)
                {
                    foreach (var item in fStatement200L11)
                    {
                        CrudFStatement200L11Dto crudFStatement200L11Dto = new CrudFStatement200L11Dto();
                        crudFStatement200L11Dto.Id = this.GetNewObjectId();
                        crudFStatement200L11Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L11Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L11Dto.Ord = item.Ord;
                        crudFStatement200L11Dto.Sort = item.Sort;
                        crudFStatement200L11Dto.Bold = item.Bold;
                        crudFStatement200L11Dto.Ord = item.Ord;
                        crudFStatement200L11Dto.Printable = item.Printable;
                        crudFStatement200L11Dto.GroupId = item.GroupId;
                        crudFStatement200L11Dto.Description = item.Description;
                        crudFStatement200L11Dto.NumberCode = item.NumberCode;
                        crudFStatement200L11Dto.Rank = item.Rank;
                        crudFStatement200L11Dto.Formular = item.Formular;
                        crudFStatement200L11Dto.FieldName = item.FieldName;
                        crudFStatement200L11Dto.Condition = item.Condition;
                        crudFStatement200L11Dto.TC1 = item.TC1;
                        crudFStatement200L11Dto.TC2 = item.TC2;
                        crudFStatement200L11Dto.TC3 = item.TC3;
                        crudFStatement200L11Dto.TC4 = item.TC4;
                        crudFStatement200L11Dto.TC5 = item.TC5;
                        crudFStatement200L11Dto.TC6 = item.TC6;
                        crudFStatement200L11Dto.TC7 = item.TC7;
                        crudFStatement200L11Dto.Total = item.Total;
                        crudFStatement200L11Dto.Title = item.Title;
                        crudFStatement200L11Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L11Dto, TenantFStatement200L11>(crudFStatement200L11Dto);
                        await _fStatement200L11Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L11s)
                    {
                        CrudFStatement200L11Dto crudFStatement200L11Dto = new CrudFStatement200L11Dto();
                        crudFStatement200L11Dto.Id = this.GetNewObjectId();
                        crudFStatement200L11Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L11Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L11Dto.Ord = item.Ord;
                        crudFStatement200L11Dto.Sort = item.Sort;
                        crudFStatement200L11Dto.Bold = item.Bold;
                        crudFStatement200L11Dto.Ord = item.Ord;
                        crudFStatement200L11Dto.Printable = item.Printable;
                        crudFStatement200L11Dto.GroupId = item.GroupId;
                        crudFStatement200L11Dto.Description = item.Description;
                        crudFStatement200L11Dto.NumberCode = item.NumberCode;
                        crudFStatement200L11Dto.Rank = item.Rank;
                        crudFStatement200L11Dto.Formular = item.Formular;
                        crudFStatement200L11Dto.FieldName = item.FieldName;
                        crudFStatement200L11Dto.Condition = item.Condition;
                        crudFStatement200L11Dto.TC1 = item.TC1;
                        crudFStatement200L11Dto.TC2 = item.TC2;
                        crudFStatement200L11Dto.TC3 = item.TC3;
                        crudFStatement200L11Dto.TC4 = item.TC4;
                        crudFStatement200L11Dto.TC5 = item.TC5;
                        crudFStatement200L11Dto.TC6 = item.TC6;
                        crudFStatement200L11Dto.TC7 = item.TC7;
                        crudFStatement200L11Dto.Total = item.Total;
                        crudFStatement200L11Dto.Title = item.Title;
                        crudFStatement200L11Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L11Dto, TenantFStatement200L11>(crudFStatement200L11Dto);
                        await _fStatement200L11Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L12") == true)
            {
                var defaultFStatement200L12s = await _defaultFStatement200L12Service.GetQueryableAsync();
                defaultFStatement200L12s = defaultFStatement200L12s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L12 = await _fStatement200L12Service.GetQueryableAsync();
                var lstfStatement200L12 = fStatement200L12.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L12s = fStatement200L12.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L12s)
                {
                    await _fStatement200L12Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L12.Count > 0)
                {
                    foreach (var item in lstfStatement200L12)
                    {
                        CrudFStatement200L12Dto crudFStatement200L12Dto = new CrudFStatement200L12Dto();
                        crudFStatement200L12Dto.Id = this.GetNewObjectId();
                        crudFStatement200L12Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L12Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L12Dto.Ord = item.Ord;
                        crudFStatement200L12Dto.Sort = item.Sort;
                        crudFStatement200L12Dto.Bold = item.Bold;
                        crudFStatement200L12Dto.Ord = item.Ord;
                        crudFStatement200L12Dto.Printable = item.Printable;
                        crudFStatement200L12Dto.GroupId = item.GroupId;
                        crudFStatement200L12Dto.Description = item.Description;

                        crudFStatement200L12Dto.Title = item.Title;
                        crudFStatement200L12Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L12Dto, TenantFStatement200L12>(crudFStatement200L12Dto);
                        await _fStatement200L12Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L12s)
                    {
                        CrudFStatement200L12Dto crudFStatement200L12Dto = new CrudFStatement200L12Dto();
                        crudFStatement200L12Dto.Id = this.GetNewObjectId();
                        crudFStatement200L12Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L12Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L12Dto.Ord = item.Ord;
                        crudFStatement200L12Dto.Sort = item.Sort;
                        crudFStatement200L12Dto.Bold = item.Bold;
                        crudFStatement200L12Dto.Ord = item.Ord;
                        crudFStatement200L12Dto.Printable = item.Printable;
                        crudFStatement200L12Dto.GroupId = item.GroupId;
                        crudFStatement200L12Dto.Description = item.Description;

                        crudFStatement200L12Dto.Title = item.Title;
                        crudFStatement200L12Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L12Dto, TenantFStatement200L12>(crudFStatement200L12Dto);
                        await _fStatement200L12Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L13") == true)
            {
                var defaultFStatement200L13s = await _defaultFStatement200L13Service.GetQueryableAsync();
                defaultFStatement200L13s = defaultFStatement200L13s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L13 = await _fStatement200L13Service.GetQueryableAsync();
                var lstfStatement200L13 = fStatement200L13.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L13s = fStatement200L13.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L13s)
                {
                    await _fStatement200L13Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L13.Count > 0)
                {
                    foreach (var item in lstfStatement200L13)
                    {
                        CrudFStatement200L13Dto crudFStatement200L13Dto = new CrudFStatement200L13Dto();
                        crudFStatement200L13Dto.Id = this.GetNewObjectId();
                        crudFStatement200L13Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L13Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L13Dto.Ord = item.Ord;
                        crudFStatement200L13Dto.Sort = item.Sort;
                        crudFStatement200L13Dto.Bold = item.Bold;
                        crudFStatement200L13Dto.Ord = item.Ord;
                        crudFStatement200L13Dto.Printable = item.Printable;
                        crudFStatement200L13Dto.GroupId = item.GroupId;
                        crudFStatement200L13Dto.Description = item.Description;
                        crudFStatement200L13Dto.Type = item.Type;
                        crudFStatement200L13Dto.NumberCode = item.NumberCode;
                        crudFStatement200L13Dto.Rank = item.Rank;
                        crudFStatement200L13Dto.Formular = item.Formular;
                        crudFStatement200L13Dto.Acc = item.Acc;
                        crudFStatement200L13Dto.ValueAmount1 = item.ValueAmount1;
                        crudFStatement200L13Dto.InterestAmount1 = item.InterestAmount1;
                        crudFStatement200L13Dto.DebtPayingAmount1 = item.DebtPayingAmount1;
                        crudFStatement200L13Dto.ValueAmount2 = item.ValueAmount2;
                        crudFStatement200L13Dto.InterestAmount2 = item.InterestAmount2;
                        crudFStatement200L13Dto.DebtPayingAmount2 = item.DebtPayingAmount2;
                        crudFStatement200L13Dto.Up = item.Up;
                        crudFStatement200L13Dto.Down = item.Down;
                        crudFStatement200L13Dto.Title = item.Title;
                        crudFStatement200L13Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L13Dto, TenantFStatement200L13>(crudFStatement200L13Dto);
                        await _fStatement200L13Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L13s)
                    {
                        CrudFStatement200L13Dto crudFStatement200L13Dto = new CrudFStatement200L13Dto();
                        crudFStatement200L13Dto.Id = this.GetNewObjectId();
                        crudFStatement200L13Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L13Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L13Dto.Ord = item.Ord;
                        crudFStatement200L13Dto.Sort = item.Sort;
                        crudFStatement200L13Dto.Bold = item.Bold;
                        crudFStatement200L13Dto.Ord = item.Ord;
                        crudFStatement200L13Dto.Printable = item.Printable;
                        crudFStatement200L13Dto.GroupId = item.GroupId;
                        crudFStatement200L13Dto.Description = item.Description;
                        crudFStatement200L13Dto.Type = item.Type;
                        crudFStatement200L13Dto.NumberCode = item.NumberCode;
                        crudFStatement200L13Dto.Rank = item.Rank;
                        crudFStatement200L13Dto.Formular = item.Formular;
                        crudFStatement200L13Dto.Acc = item.Acc;
                        crudFStatement200L13Dto.ValueAmount1 = item.ValueAmount1;
                        crudFStatement200L13Dto.InterestAmount1 = item.InterestAmount1;
                        crudFStatement200L13Dto.DebtPayingAmount1 = item.DebtPayingAmount1;
                        crudFStatement200L13Dto.ValueAmount2 = item.ValueAmount2;
                        crudFStatement200L13Dto.InterestAmount2 = item.InterestAmount2;
                        crudFStatement200L13Dto.DebtPayingAmount2 = item.DebtPayingAmount2;
                        crudFStatement200L13Dto.Up = item.Up;
                        crudFStatement200L13Dto.Down = item.Down;
                        crudFStatement200L13Dto.Title = item.Title;
                        crudFStatement200L13Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L13Dto, TenantFStatement200L13>(crudFStatement200L13Dto);
                        await _fStatement200L13Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L14") == true)
            {
                var defaultFStatement200L14s = await _defaultFStatement200L14Service.GetQueryableAsync();
                defaultFStatement200L14s = defaultFStatement200L14s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L14 = await _fStatement200L14Service.GetQueryableAsync();
                var lstfStatement200L14 = fStatement200L14.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L14s = fStatement200L14.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L14s)
                {
                    await _fStatement200L14Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L14.Count > 0)
                {
                    foreach (var item in lstfStatement200L14)
                    {
                        CrudFStatement200L14Dto crudFStatement200L14Dto = new CrudFStatement200L14Dto();
                        crudFStatement200L14Dto.Id = this.GetNewObjectId();
                        crudFStatement200L14Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L14Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L14Dto.Ord = item.Ord;
                        crudFStatement200L14Dto.Sort = item.Sort;
                        crudFStatement200L14Dto.Bold = item.Bold;
                        crudFStatement200L14Dto.Ord = item.Ord;
                        crudFStatement200L14Dto.Printable = item.Printable;
                        crudFStatement200L14Dto.GroupId = item.GroupId;
                        crudFStatement200L14Dto.Description = item.Description;
                        crudFStatement200L14Dto.Title = item.Title;
                        crudFStatement200L14Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L14Dto, TenantFStatement200L14>(crudFStatement200L14Dto);
                        await _fStatement200L14Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L14s)
                    {
                        CrudFStatement200L14Dto crudFStatement200L14Dto = new CrudFStatement200L14Dto();
                        crudFStatement200L14Dto.Id = this.GetNewObjectId();
                        crudFStatement200L14Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L14Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L14Dto.Ord = item.Ord;
                        crudFStatement200L14Dto.Sort = item.Sort;
                        crudFStatement200L14Dto.Bold = item.Bold;
                        crudFStatement200L14Dto.Ord = item.Ord;
                        crudFStatement200L14Dto.Printable = item.Printable;
                        crudFStatement200L14Dto.GroupId = item.GroupId;
                        crudFStatement200L14Dto.Description = item.Description;
                        crudFStatement200L14Dto.Title = item.Title;
                        crudFStatement200L14Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L14Dto, TenantFStatement200L14>(crudFStatement200L14Dto);
                        await _fStatement200L14Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L15") == true)
            {
                var defaultFStatement200L15s = await _defaultFStatement200L15Service.GetQueryableAsync();
                defaultFStatement200L15s = defaultFStatement200L15s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L15 = await _fStatement200L15Service.GetQueryableAsync();
                var lstfStatement200L15 = fStatement200L15.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L15s = fStatement200L15.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L15s)
                {
                    await _fStatement200L15Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L15.Count > 0)
                {
                    foreach (var item in lstfStatement200L15)
                    {
                        CrudFStatement200L15Dto crudFStatement200L15Dto = new CrudFStatement200L15Dto();
                        crudFStatement200L15Dto.Id = this.GetNewObjectId();
                        crudFStatement200L15Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L15Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L15Dto.Ord = item.Ord;
                        crudFStatement200L15Dto.Sort = item.Sort;
                        crudFStatement200L15Dto.Bold = item.Bold;
                        crudFStatement200L15Dto.Ord = item.Ord;
                        crudFStatement200L15Dto.Printable = item.Printable;
                        crudFStatement200L15Dto.GroupId = item.GroupId;
                        crudFStatement200L15Dto.Description = item.Description;
                        crudFStatement200L15Dto.Title = item.Title;
                        crudFStatement200L15Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L15Dto, TenantFStatement200L15>(crudFStatement200L15Dto);
                        await _fStatement200L15Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L15s)
                    {
                        CrudFStatement200L15Dto crudFStatement200L15Dto = new CrudFStatement200L15Dto();
                        crudFStatement200L15Dto.Id = this.GetNewObjectId();
                        crudFStatement200L15Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L15Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L15Dto.Ord = item.Ord;
                        crudFStatement200L15Dto.Sort = item.Sort;
                        crudFStatement200L15Dto.Bold = item.Bold;
                        crudFStatement200L15Dto.Ord = item.Ord;
                        crudFStatement200L15Dto.Printable = item.Printable;
                        crudFStatement200L15Dto.GroupId = item.GroupId;
                        crudFStatement200L15Dto.Description = item.Description;
                        crudFStatement200L15Dto.Title = item.Title;
                        crudFStatement200L15Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L15Dto, TenantFStatement200L15>(crudFStatement200L15Dto);
                        await _fStatement200L15Service.CreateAsync(entitys);
                    }
                }


            }

            if (dto.LstDataForYear.Contains("FStatement200L16") == true)
            {
                var defaultFStatement200L16s = await _defaultFStatement200L16Service.GetQueryableAsync();
                defaultFStatement200L16s = defaultFStatement200L16s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L16 = await _fStatement200L16Service.GetQueryableAsync();
                var lstfStatement200L16 = fStatement200L16.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L16s = fStatement200L16.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L16s)
                {
                    await _fStatement200L16Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L16.Count > 0)
                {
                    foreach (var item in lstfStatement200L16)
                    {
                        CrudFStatement200L16Dto crudFStatement200L16Dto = new CrudFStatement200L16Dto();
                        crudFStatement200L16Dto.Id = this.GetNewObjectId();
                        crudFStatement200L16Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L16Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L16Dto.Ord = item.Ord;
                        crudFStatement200L16Dto.Sort = item.Sort;
                        crudFStatement200L16Dto.Bold = item.Bold;
                        crudFStatement200L16Dto.Ord = item.Ord;
                        crudFStatement200L16Dto.Printable = item.Printable;
                        crudFStatement200L16Dto.GroupId = item.GroupId;
                        crudFStatement200L16Dto.Description = item.Description;
                        crudFStatement200L16Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L16Dto.Type = item.Type;
                        crudFStatement200L16Dto.NumberCode = item.NumberCode;
                        crudFStatement200L16Dto.Rank = item.Rank;
                        crudFStatement200L16Dto.Formular = item.Formular;
                        crudFStatement200L16Dto.ValueAcc = item.ValueAcc;
                        crudFStatement200L16Dto.PreventiveAcc = item.PreventiveAcc;
                        crudFStatement200L16Dto.ValueAmount1 = item.ValueAmount1;
                        crudFStatement200L16Dto.PreventiveAmount1 = item.PreventiveAmount1;
                        crudFStatement200L16Dto.ValueAmount2 = item.PreventiveAmount2;
                        crudFStatement200L16Dto.PreventiveAmount2 = item.PreventiveAmount2;
                        crudFStatement200L16Dto.Title = item.Title;
                        crudFStatement200L16Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L16Dto, TenantFStatement200L16>(crudFStatement200L16Dto);
                        await _fStatement200L16Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L16s)
                    {
                        CrudFStatement200L16Dto crudFStatement200L16Dto = new CrudFStatement200L16Dto();
                        crudFStatement200L16Dto.Id = this.GetNewObjectId();
                        crudFStatement200L16Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L16Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L16Dto.Ord = item.Ord;
                        crudFStatement200L16Dto.Sort = item.Sort;
                        crudFStatement200L16Dto.Bold = item.Bold;
                        crudFStatement200L16Dto.Ord = item.Ord;
                        crudFStatement200L16Dto.Printable = item.Printable;
                        crudFStatement200L16Dto.GroupId = item.GroupId;
                        crudFStatement200L16Dto.Description = item.Description;
                        crudFStatement200L16Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L16Dto.Type = item.Type;
                        crudFStatement200L16Dto.NumberCode = item.NumberCode;
                        crudFStatement200L16Dto.Rank = item.Rank;
                        crudFStatement200L16Dto.Formular = item.Formular;
                        crudFStatement200L16Dto.ValueAcc = item.ValueAcc;
                        crudFStatement200L16Dto.PreventiveAcc = item.PreventiveAcc;
                        crudFStatement200L16Dto.ValueAmount1 = item.ValueAmount1;
                        crudFStatement200L16Dto.PreventiveAmount1 = item.PreventiveAmount1;
                        crudFStatement200L16Dto.ValueAmount2 = item.PreventiveAmount2;
                        crudFStatement200L16Dto.PreventiveAmount2 = item.PreventiveAmount2;
                        crudFStatement200L16Dto.Title = item.Title;
                        crudFStatement200L16Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L16Dto, TenantFStatement200L16>(crudFStatement200L16Dto);
                        await _fStatement200L16Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L17") == true)
            {
                var defaultFStatement200L17s = await _defaultFStatement200L17Service.GetQueryableAsync();
                defaultFStatement200L17s = defaultFStatement200L17s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L17 = await _fStatement200L17Service.GetQueryableAsync();
                var lstfStatement200L17 = fStatement200L17.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L17s = fStatement200L17.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L17s)
                {
                    await _fStatement200L17Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L17.Count > 0)
                {
                    foreach (var item in lstfStatement200L17)
                    {
                        CrudFStatement200L17Dto crudFStatement200L17Dto = new CrudFStatement200L17Dto();
                        crudFStatement200L17Dto.Id = this.GetNewObjectId();
                        crudFStatement200L17Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L17Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L17Dto.Ord = item.Ord;
                        crudFStatement200L17Dto.Sort = item.Sort;
                        crudFStatement200L17Dto.Bold = item.Bold;
                        crudFStatement200L17Dto.Ord = item.Ord;
                        crudFStatement200L17Dto.Printable = item.Printable;
                        crudFStatement200L17Dto.GroupId = item.GroupId;
                        crudFStatement200L17Dto.Description = item.Description;
                        crudFStatement200L17Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L17Dto.Type = item.Type;
                        crudFStatement200L17Dto.NumberCode = item.NumberCode;
                        crudFStatement200L17Dto.Rank = item.Rank;
                        crudFStatement200L17Dto.Formular = item.Formular;
                        crudFStatement200L17Dto.Method = item.Method;
                        crudFStatement200L17Dto.Condition = item.Condition;
                        crudFStatement200L17Dto.DebitBalance1 = item.DebitBalance1;
                        crudFStatement200L17Dto.CreditBalance1 = item.CreditBalance1;
                        crudFStatement200L17Dto.Debit = item.Debit;
                        crudFStatement200L17Dto.Credit = item.Credit;
                        crudFStatement200L17Dto.DebitBalance2 = item.DebitBalance2;
                        crudFStatement200L17Dto.CreditBalance2 = item.CreditBalance2;
                        crudFStatement200L17Dto.Title = item.Title;
                        crudFStatement200L17Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L17Dto, TenantFStatement200L17>(crudFStatement200L17Dto);
                        await _fStatement200L17Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L17s)
                    {
                        CrudFStatement200L17Dto crudFStatement200L17Dto = new CrudFStatement200L17Dto();
                        crudFStatement200L17Dto.Id = this.GetNewObjectId();
                        crudFStatement200L17Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString()) + 1;
                        crudFStatement200L17Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L17Dto.Ord = item.Ord;
                        crudFStatement200L17Dto.Sort = item.Sort;
                        crudFStatement200L17Dto.Bold = item.Bold;
                        crudFStatement200L17Dto.Ord = item.Ord;
                        crudFStatement200L17Dto.Printable = item.Printable;
                        crudFStatement200L17Dto.GroupId = item.GroupId;
                        crudFStatement200L17Dto.Description = item.Description;
                        crudFStatement200L17Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L17Dto.Type = item.Type;
                        crudFStatement200L17Dto.NumberCode = item.NumberCode;
                        crudFStatement200L17Dto.Rank = item.Rank;
                        crudFStatement200L17Dto.Formular = item.Formular;
                        crudFStatement200L17Dto.Method = item.Method;
                        crudFStatement200L17Dto.Condition = item.Condition;
                        crudFStatement200L17Dto.DebitBalance1 = item.DebitBalance1;
                        crudFStatement200L17Dto.CreditBalance1 = item.CreditBalance1;
                        crudFStatement200L17Dto.Debit = item.Debit;
                        crudFStatement200L17Dto.Credit = item.Credit;
                        crudFStatement200L17Dto.DebitBalance2 = item.DebitBalance2;
                        crudFStatement200L17Dto.CreditBalance2 = item.CreditBalance2;
                        crudFStatement200L17Dto.Title = item.Title;
                        crudFStatement200L17Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L17Dto, TenantFStatement200L17>(crudFStatement200L17Dto);
                        await _fStatement200L17Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L18") == true)
            {
                var defaultFStatement200L18s = await _defaultFStatement200L18Service.GetQueryableAsync();
                defaultFStatement200L18s = defaultFStatement200L18s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);
                var fStatement200L18 = await _fStatement200L18Service.GetQueryableAsync();
                var lstfStatement200L18 = fStatement200L18.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L18s = fStatement200L18.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                foreach (var item in lstfStatement200L18s)
                {
                    await _fStatement200L18Service.DeleteAsync(item, true);
                }
                if (lstfStatement200L18.Count > 0)
                {
                    foreach (var item in lstfStatement200L18)
                    {
                        CrudFStatement200L18Dto crudFStatement200L18Dto = new CrudFStatement200L18Dto();
                        crudFStatement200L18Dto.Id = this.GetNewObjectId();
                        crudFStatement200L18Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L18Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L18Dto.Ord = item.Ord;
                        crudFStatement200L18Dto.Sort = item.Sort;
                        crudFStatement200L18Dto.Bold = item.Bold;
                        crudFStatement200L18Dto.Ord = item.Ord;
                        crudFStatement200L18Dto.Printable = item.Printable;
                        crudFStatement200L18Dto.GroupId = item.GroupId;
                        crudFStatement200L18Dto.Description = item.Description;

                        crudFStatement200L18Dto.Title = item.Title;
                        crudFStatement200L18Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L18Dto, TenantFStatement200L18>(crudFStatement200L18Dto);
                        await _fStatement200L18Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L18s)
                    {
                        CrudFStatement200L18Dto crudFStatement200L18Dto = new CrudFStatement200L18Dto();
                        crudFStatement200L18Dto.Id = this.GetNewObjectId();
                        crudFStatement200L18Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L18Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L18Dto.Ord = item.Ord;
                        crudFStatement200L18Dto.Sort = item.Sort;
                        crudFStatement200L18Dto.Bold = item.Bold;
                        crudFStatement200L18Dto.Ord = item.Ord;
                        crudFStatement200L18Dto.Printable = item.Printable;
                        crudFStatement200L18Dto.GroupId = item.GroupId;
                        crudFStatement200L18Dto.Description = item.Description;

                        crudFStatement200L18Dto.Title = item.Title;
                        crudFStatement200L18Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L18Dto, TenantFStatement200L18>(crudFStatement200L18Dto);
                        await _fStatement200L18Service.CreateAsync(entitys);
                    }
                }

            }
            if (dto.LstDataForYear.Contains("FStatement200L19") == true)
            {
                var defaultFStatement200L19s = await _defaultFStatement200L19Service.GetQueryableAsync();
                defaultFStatement200L19s = defaultFStatement200L19s.Where(p => p.UsingDecision == lsrYearCategory.UsingDecision);

                var fStatement200L19 = await _fStatement200L19Service.GetQueryableAsync();
                var lstfStatement200L19 = fStatement200L19.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year).ToList();
                var lstfStatement200L19s = fStatement200L19.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == year + 1).ToList();
                if (lstfStatement200L19s.Count > 0)
                {
                    foreach (var item in lstfStatement200L19)
                    {
                        CrudFStatement200L19Dto crudFStatement200L19Dto = new CrudFStatement200L19Dto();
                        crudFStatement200L19Dto.Id = this.GetNewObjectId();
                        crudFStatement200L19Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L19Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L19Dto.Ord = item.Ord;
                        crudFStatement200L19Dto.Sort = item.Sort;
                        crudFStatement200L19Dto.Bold = item.Bold;
                        crudFStatement200L19Dto.Ord = item.Ord;
                        crudFStatement200L19Dto.Printable = item.Printable;
                        crudFStatement200L19Dto.GroupId = item.GroupId;
                        crudFStatement200L19Dto.Description = item.Description;
                        crudFStatement200L19Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L19Dto.NumberCode = item.NumberCode;
                        crudFStatement200L19Dto.Rank = item.Rank;
                        crudFStatement200L19Dto.Formular = item.Formular;
                        crudFStatement200L19Dto.Acc = item.Acc;
                        crudFStatement200L19Dto.NV1 = item.NV1;
                        crudFStatement200L19Dto.NV2 = item.NV2;
                        crudFStatement200L19Dto.NV3 = item.NV3;
                        crudFStatement200L19Dto.NV4 = item.NV4;
                        crudFStatement200L19Dto.NV5 = item.NV5;
                        crudFStatement200L19Dto.NV6 = item.NV6;
                        crudFStatement200L19Dto.NV7 = item.NV7;
                        crudFStatement200L19Dto.Total = item.Total;
                        crudFStatement200L19Dto.Title = item.Title;
                        crudFStatement200L19Dto.OrgCode = item.OrgCode;
                        var entitys = ObjectMapper.Map<CrudFStatement200L19Dto, TenantFStatement200L19>(crudFStatement200L19Dto);
                        await _fStatement200L19Service.CreateAsync(entitys);
                    }
                }
                else
                {
                    foreach (var item in defaultFStatement200L19s)
                    {
                        CrudFStatement200L19Dto crudFStatement200L19Dto = new CrudFStatement200L19Dto();
                        crudFStatement200L19Dto.Id = this.GetNewObjectId();
                        crudFStatement200L19Dto.Year = int.Parse(_webHelper.GetCurrentYear().ToString());
                        crudFStatement200L19Dto.UsingDecision = item.UsingDecision;
                        crudFStatement200L19Dto.Ord = item.Ord;
                        crudFStatement200L19Dto.Sort = item.Sort;
                        crudFStatement200L19Dto.Bold = item.Bold;
                        crudFStatement200L19Dto.Ord = item.Ord;
                        crudFStatement200L19Dto.Printable = item.Printable;
                        crudFStatement200L19Dto.GroupId = item.GroupId;
                        crudFStatement200L19Dto.Description = item.Description;
                        crudFStatement200L19Dto.DebitOrCredit = item.DebitOrCredit;
                        crudFStatement200L19Dto.NumberCode = item.NumberCode;
                        crudFStatement200L19Dto.Rank = item.Rank;
                        crudFStatement200L19Dto.Formular = item.Formular;
                        crudFStatement200L19Dto.Acc = item.Acc;
                        crudFStatement200L19Dto.NV1 = item.NV1;
                        crudFStatement200L19Dto.NV2 = item.NV2;
                        crudFStatement200L19Dto.NV3 = item.NV3;
                        crudFStatement200L19Dto.NV4 = item.NV4;
                        crudFStatement200L19Dto.NV5 = item.NV5;
                        crudFStatement200L19Dto.NV6 = item.NV6;
                        crudFStatement200L19Dto.NV7 = item.NV7;
                        crudFStatement200L19Dto.Total = item.Total;
                        crudFStatement200L19Dto.Title = item.Title;
                        crudFStatement200L19Dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var entitys = ObjectMapper.Map<CrudFStatement200L19Dto, TenantFStatement200L19>(crudFStatement200L19Dto);
                        await _fStatement200L19Service.CreateAsync(entitys);
                    }
                }

            }
            return crud;
        }

        private async Task<CruAccountSystemDto> StandardDto(CruAccountSystemDto dto)
        {
            CruAccountSystemDto result = dto;
            result.OrgCode = _webHelper.GetCurrentOrgUnit();
            result.AccRank = 1;

            var parentAccount = await _accountSystemService.GetParentAccountByIdAsync(dto.ParentAccId);
            if (parentAccount != null)
            {
                result.AccRank = parentAccount.AccRank + 1;
                result.ParentCode = parentAccount.AccCode;
            }

            if (!string.IsNullOrEmpty(result.ParentCode) && !result.AccCode.StartsWith(result.ParentCode))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.NotStartWithParentCode),
                                    "Code is not valid.Code is not start with parent code");
            }

            string prefixName = new string('-', 2 * (result.AccRank - 1));
            result.AccNameTemp = $"{prefixName}{result.AccName}";
            result.AccNameTempE = $"{prefixName}{result.AccNameEn}";

            return result;
        }
    }
}

