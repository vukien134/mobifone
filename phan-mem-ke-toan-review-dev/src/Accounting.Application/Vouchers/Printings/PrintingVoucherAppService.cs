using Accounting.Catgories.ProductVouchers;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Vouchers;
using Accounting.DomainServices.Windows;
using Accounting.Helpers;
using Accounting.Helpers.Currencies;
using Accounting.Vouchers.AccVouchers;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Accounting.Report;
using Volo.Abp.ObjectMapping;
using Accounting.Catgories.OrgUnits;
using Accounting.Categories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.DomainServices.Categories.Others;
using Accounting.Categories.Others;
using Accounting.Catgories.Others.Currencies;
using Microsoft.Extensions.Localization;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Authorization;
using Accounting.Exceptions;
using Volo.Abp.Settings;
using Accounting.DomainServices.Users;
using Accounting.Catgories.Partners;
using Accounting.Catgories.Accounts;
using Accounting.Caching;

namespace Accounting.Vouchers.Printings
{
    public class PrintingVoucherAppService : AccountingAppService, IPrintingVoucherAppService
    {
        #region Fields
        private readonly VoucherTemplateService _voucherTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly WebHelper _webHelper;
        private readonly WindowService _windowService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CurrencyService _currencyService;
        private readonly LedgerService _ledgerService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly AccVoucherDetailService _accVoucherDetailService;
        private readonly MenuAccountingService _menuAccountingService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly UserService _userService;
        private readonly AccountSystemService _accountSystemService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public PrintingVoucherAppService(VoucherTemplateService voucherTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        AccVoucherService accVoucherService,
                        ProductVoucherService productVoucherService,
                        ProductVoucherDetailService productVoucherDetailService,
                        VoucherCategoryService voucherCategoryService,
                        DefaultVoucherCategoryService defaultVoucherCategoryService,
                        WebHelper webHelper,
                        WindowService windowService,
                        OrgUnitService orgUnitService,
                        CurrencyService currencyService,
                        LedgerService ledgerService,
                        TenantSettingService tenantSettingService,
                        YearCategoryService yearCategoryService,
                        CircularsService circularsService,
                        AccTaxDetailService accTaxDetailService,
                        AccVoucherDetailService accVoucherDetailService,
                        MenuAccountingService menuAccountingService,
                        IStringLocalizer<AccountingResource> localizer,
                        UserService userService,
                        AccountSystemService accountSystemService,
                        AccountingCacheManager accountingCacheManager
                        )
        {
            _voucherTemplateService = voucherTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _accVoucherService = accVoucherService;
            _productVoucherService = productVoucherService;
            _voucherCategoryService = voucherCategoryService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _webHelper = webHelper;
            _windowService = windowService;
            _orgUnitService = orgUnitService;
            _currencyService = currencyService;
            _ledgerService = ledgerService;
            _tenantSettingService = tenantSettingService;
            _yearCategoryService = yearCategoryService;
            _circularsService = circularsService;
            _accTaxDetailService = accTaxDetailService;
            _accVoucherDetailService = accVoucherDetailService;
            _productVoucherDetailService = productVoucherDetailService;
            _menuAccountingService = menuAccountingService;
            _localizer = localizer;
            _userService = userService;
            _accountSystemService = accountSystemService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        public async Task<FileContentResult> CreateAsync(PrintingVoucherRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.VoucherTemplateId) && dto.LstVoucherId == null)
            {
                throw new ArgumentNullException(nameof(dto.VoucherTemplateId));
            }

            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionCreate);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            byte[] result = await this.CreateSingleReport(dto);
            if (result == null)
            {
                result = await this.CreateMultiReport(dto);
            }

            return new FileContentResult(result, MIMETYPE.GetContentType(dto.TypePrint.ToLower()))
            {
                FileDownloadName = $"Voucher.{dto.TypePrint}"
            };
        }
        public Task<object> GetDataAsync(string voucherCode, string voucherId)
        {
            return GetDataVoucher(voucherCode, voucherId);
        }
        #region Private
        private async Task<object> GetDataVoucher(string voucherCode, string voucherId)
        {
            string voucherKind = await this.GetVoucherKind(voucherCode);
            object result = voucherKind switch
            {
                "KT" => await GetDataAccVoucher(voucherId),
                "HV" => await GetDataProductVoucher(voucherId),
                _ => null
            };
            return result;
        }
        private async Task<List<PrintingAccVoucherDto>> GetDataAccVoucher(string voucherId)
        {
            var result = new List<PrintingAccVoucherDto>();
            var accVoucher = await _accVoucherService.GetAsync(voucherId);
            var dto = ObjectMapper.Map<AccVoucher, PrintingAccVoucherDto>(accVoucher);
            var currency = await _currencyService.GetCurrencyByCodeAsync(dto.CurrencyCode, _webHelper.GetCurrentOrgUnit());
            var currencyTranslator = new VietnameseMoneyTranslator("VND", currency.OddCurrencyVN, "", 0);
            dto.AmountByWord = currencyTranslator.ToWords((double)dto.TotalAmount);
            dto.VoteMaker = await this.GetVoteMaker(dto.CreatorId);
            dto.PrintingVoucherAccounts = await GetPrintingVoucherAccounts(voucherId);
            dto.OrgUnitDto = await GetOrgUnitDto(accVoucher.OrgCode);
            dto.Circulars = await GetCircularsDto(accVoucher.OrgCode, accVoucher.Year);
            dto.TenantSetting = await GetTenantSetting(accVoucher.OrgCode);
            dto.Currency = ObjectMapper.Map<Currency, CurrencyDto>(currency);
            dto.AccTaxDetails = await GetAccTaxDetails(voucherId);
            dto.AccVoucherDetails = await GetAccVoucherDetails(voucherId);
            dto.PrintingAccVoucherDetails = GetPrintingAccVoucherDetails(dto);
            var lstaccVoucher = await _accVoucherService.GetQueryableAsync();
            var accvouchers = lstaccVoucher.Where(p => p.Id == voucherId).ToList();
            List<AccountSystemDto> accs = new List<AccountSystemDto>();
            if (accvouchers.Count > 0)
            {
                var accvoucherDetail = accvouchers.FirstOrDefault().AccVoucherDetails;

                foreach (var item in accvoucherDetail)
                {
                    var acc = await this.getBank(item.CreditAcc);
                    accs.AddRange(acc);
                }
            }
            dto.accountSystemDtos = accs;
            result.Add(dto);
            return result;
        }
        private async Task<List<PrintingProductVoucherDto>> GetDataProductVoucher(string voucherId)
        {
            var result = new List<PrintingProductVoucherDto>();
            var productVoucher = await _productVoucherService.GetAsync(voucherId);
            var dto = ObjectMapper.Map<ProductVoucher, PrintingProductVoucherDto>(productVoucher);
            var currency = await _currencyService.GetCurrencyByCodeAsync(dto.CurrencyCode, _webHelper.GetCurrentOrgUnit());
            var currencyTranslator = new VietnameseMoneyTranslator("VND", currency.OddCurrencyVN, "", 0);
            dto.AmountByWord = currencyTranslator.ToWords((double)dto.TotalAmount);
            dto.OrgUnitDto = await GetOrgUnitDto(productVoucher.OrgCode);
            dto.Currency = ObjectMapper.Map<Currency, CurrencyDto>(currency);
            dto.TenantSetting = await GetTenantSetting(productVoucher.OrgCode);
            dto.PrintingVoucherAccounts = await GetPrintingVoucherAccounts(voucherId);
            dto.Circulars = await GetCircularsDto(productVoucher.OrgCode, productVoucher.Year);
            dto.AccTaxDetails = await GetProductAccTaxDetails(voucherId);
            dto.ProductVoucherDetails = await GetProductVoucherDetails(voucherId);
            dto.VoteMaker = await this.GetVoteMaker(dto.CreatorId);

            result.Add(dto);
            return result;
        }
        private async Task<List<AccountSystemDto>> getBank(string acc)
        {
            var accountSystems = await _accountSystemService.GetQueryableAsync();
            var lsaccountSystemst = accountSystems.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.AccCode == acc && p.Year == _webHelper.GetCurrentYear()).ToList();
            List<AccountSystemDto> accs = new List<AccountSystemDto>();
            foreach (var item in lsaccountSystemst)
            {
                AccountSystemDto account = new AccountSystemDto();
                account.BankAccountNumber = item.BankAccountNumber;
                account.BankName = item.BankName;
                accs.Add(account);
            }
            return accs;
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Voucher,
                                        templateFile);
            if (!File.Exists(filePath))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Printing, ErrorCode.FileNotFound),
                        $"Template File ['{filePath}'] not found ");
            }
            return filePath;
        }
        private async Task<List<PrintingVoucherAccount>> GetPrintingVoucherAccounts(string voucherId)
        {
            var ledgers = await _ledgerService.GetByAccVoucherIdAsync(voucherId);

            var groupByDebit = ledgers.GroupBy(p => p.DebitAcc)
                                        .Select(p => new PrintingVoucherAccount()
                                        {
                                            AccCode = p.Key,
                                            Type = "Nợ",
                                            Amount = p.Sum(y => y.Amount)
                                        }).ToList<PrintingVoucherAccount>();

            var groupByCredit = ledgers.GroupBy(p => p.CreditAcc)
                                        .Select(p => new PrintingVoucherAccount()
                                        {
                                            AccCode = p.Key,
                                            Type = "Có",
                                            Amount = p.Sum(y => y.Amount)
                                        }).ToList<PrintingVoucherAccount>();

            groupByDebit.AddRange(groupByCredit);
            return groupByDebit;
        }
        private async Task<OrgUnitDto> GetOrgUnitDto(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return ObjectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
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
        private async Task<List<AccTaxDetailDto>> GetAccTaxDetails(string id)
        {
            var taxDetails = await _accTaxDetailService.GetByAccVoucherIdAsync(id);
            return taxDetails.Select(p => ObjectMapper.Map<AccTaxDetail, AccTaxDetailDto>(p))
                        .ToList();
        }
        private async Task<List<AccTaxDetailDto>> GetProductAccTaxDetails(string id)
        {
            var taxDetails = await _accTaxDetailService.GetByProductIdAsync(id);
            return taxDetails.Select(p => ObjectMapper.Map<AccTaxDetail, AccTaxDetailDto>(p))
                        .ToList();
        }
        private async Task<List<AccVoucherDetailDto>> GetAccVoucherDetails(string id)
        {
            var details = await _accVoucherDetailService.GetByAccVoucherIdAsync(id);
            return details.Select(p => ObjectMapper.Map<AccVoucherDetail, AccVoucherDetailDto>(p))
                    .ToList();
        }
        private List<PrintingAccVoucherDetail> GetPrintingAccVoucherDetails(PrintingAccVoucherDto dto)
        {
            var lst = new List<PrintingAccVoucherDetail>();

            var lstAccVoucherDetail = dto.AccVoucherDetails;
            foreach (var item in lstAccVoucherDetail)
            {
                lst.Add(new PrintingAccVoucherDetail()
                {
                    Id = item.Id,
                    DebitAcc = item.DebitAcc,
                    CreditAcc = item.CreditAcc,
                    Amount = item.Amount,
                    AmountCur = item.AmountCur,
                    PartnerCode = item.PartnerCode,
                    PartnerName = item.PartnerName,
                    Note = item.Note
                });
            }

            var lstAccTaxDetail = dto.AccTaxDetails;
            foreach (var item in lstAccTaxDetail)
            {
                lst.Add(new PrintingAccVoucherDetail()
                {
                    Id = item.Id,
                    DebitAcc = item.DebitAcc,
                    CreditAcc = item.CreditAcc,
                    Amount = item.Amount,
                    AmountCur = item.AmountCur,
                    PartnerCode = item.PartnerCode,
                    PartnerName = item.PartnerName,
                    Note = item.Note
                });
            }

            return lst;
        }
        private async Task<List<ProductVoucherDetailDto>> GetProductVoucherDetails(string voucherId)
        {
            List<ProductVoucherDetail> details = await _productVoucherDetailService.GetByProductIdAsync(voucherId);
            var decreaseAmount = details.Sum(p => p.DecreaseAmount);
            long integerAmount = (long)decreaseAmount; // Ép kiểu sang kiểu long (số nguyên dài)
            
            if (integerAmount > 0)
            {
                ProductVoucherDetail productVoucherDetail = new ProductVoucherDetail();
                productVoucherDetail.ProductName = "Giảm " + integerAmount +
                                                   " tương ứng 20% mức tỉ lệ % để tính thuế giá trị gia tăng theo Nghị quyết số 101/2023/QH15";
                productVoucherDetail.Amount = integerAmount;
                details.Add(productVoucherDetail);
            }
           
            return details.Select(p => ObjectMapper.Map<ProductVoucherDetail, ProductVoucherDetailDto>(p))
                        .ToList();
        }
        private async Task<byte[]> CreateMultiReport(PrintingVoucherRequestDto dto)
        {
            if (dto.LstVoucherId == null) return null;

            var voucherTemplate = await _voucherTemplateService.GetByIdAsync(dto.VoucherTemplateId);
            string templateFile = this.GetFileTemplatePath(voucherTemplate.FileTemplate);
            var window = await _windowService.GetByIdAsync(voucherTemplate.WindowId);
            var currencyFormats = await _accountingCacheManager.GetCurrencyFormats(_webHelper.GetCurrentOrgUnit());

            var renderOption = new RenderOption()
            {
                TypePrint = dto.TypePrint,
                TemplateFile = templateFile,
                CurrencyFormats = currencyFormats
            };
            var render = new ReportRender(renderOption);

            for (int i = 0; i < dto.LstVoucherId.Length; i++)
            {
                string voucherId = dto.LstVoucherId[i];
                var dataVoucher = await GetDataVoucher(window.VoucherCode, voucherId);
                renderOption.DataSource = dataVoucher;
                var reportDocument = render.CreateDocument();
                render.CreateMultiReport(reportDocument);
            }
            return render.ExportMultiReport();
        }
        private async Task<byte[]> CreateSingleReport(PrintingVoucherRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.VoucherId)) return null;
            var voucherTemplate = await _voucherTemplateService.GetByIdAsync(dto.VoucherTemplateId);
            var window = await _windowService.GetByIdAsync(voucherTemplate.WindowId);
            var dataVoucher = await GetDataVoucher(window.VoucherCode, dto.VoucherId);
            var currencyFormats = await _accountingCacheManager.GetCurrencyFormats(_webHelper.GetCurrentOrgUnit());

            var renderOption = new RenderOption()
            {
                DataSource = dataVoucher,
                TypePrint = dto.TypePrint,
                TemplateFile = GetFileTemplatePath(voucherTemplate.FileTemplate),
                CurrencyFormats = currencyFormats
            };
            var render = new ReportRender(renderOption);
            return render.Execute();
        }
        private async Task<bool> IsGrantPermission(string menuAccountingId, string action)
        {
            var menuAccounting = await _menuAccountingService.FindAsync(menuAccountingId);
            if (menuAccounting == null) return false;
            string permissionName = menuAccounting.ViewPermission.Replace(AccountingPermissions.ActionView,
                                                    action);
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            return result.Succeeded;
        }
        private async Task<string> GetVoteMaker(Guid? creatorId)
        {
            if (creatorId == null) return null;
            var user = await _userService.GetIdentityUserByIdAsync(creatorId.Value);
            if (user == null) return null;
            return user.Name;
        }
        private async Task<string> GetVoucherKind(string voucherCode)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var voucherCategory = await _voucherCategoryService.GetByCodeAsync(voucherCode, orgCode);
            if (voucherCategory != null) return voucherCategory.VoucherKind;
            var defaultVoucherCategory = await _defaultVoucherCategoryService.GetByCodeAsync(voucherCode);
            if (defaultVoucherCategory != null) return defaultVoucherCategory.VoucherKind;
            return null;
        }
        #endregion
    }
}
