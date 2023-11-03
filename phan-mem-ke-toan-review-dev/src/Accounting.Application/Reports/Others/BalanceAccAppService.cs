using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accounting.Categories.Partners;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.Cores;
using Accounting.Reports.Others;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Linq;
using NPOI.OpenXmlFormats.Shared;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Caching;

namespace Accounting.Reports.Others
{
    public class BalanceAccAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly AccountSystemService _accountSystemService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccSectionService _accSectionService;
        private readonly DepartmentService _departmentService;
        private readonly AccCaseService _accCaseService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly ProductGroupService _productGroupService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductAppService _productAppService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public BalanceAccAppService(ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccSectionService accSectionService,
                        DepartmentService departmentService,
                        AccCaseService accCaseService,
                        FProductWorkService fProductWorkService,
                        AccPartnerService accPartnerService,
                        PartnerGroupService partnerGroupService,
                        AccPartnerAppService accPartnerAppService,
                        ProductGroupService productGroupService,
                        ProductService productService,
                        WarehouseService warehouseService,
                        ProductAppService productAppService,
                        ProductVoucherService productVoucherService,
                        ProductVoucherDetailService productVoucherDetailService,
                        VoucherCategoryService voucherCategoryService,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accSectionService = accSectionService;
            _departmentService = departmentService;
            _accCaseService = accCaseService;
            _fProductWorkService = fProductWorkService;
            _accPartnerService = accPartnerService;
            _partnerGroupService = partnerGroupService;
            _accPartnerAppService = accPartnerAppService;
            _productGroupService = productGroupService;
            _productService = productService;
            _warehouseService = warehouseService;
            _productAppService = productAppService;
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _voucherCategoryService = voucherCategoryService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        public async Task<ReportResponseDto<BalanceAccDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var ord0 = "";
            var attachPartner = "";
            //Theo hợp đồng
            var attachContract = "";
            string ordRec = null;
            //Theo Khoản mục
            var attachAccSection = "";
            //Mã khoản mục
            var attachWorkPlace = "";
            //Theo ngoại tệ
            var attachCurrency = "";
            //Theo phân xưởng
            var attachProductCost = "";
            var ord0Code = "";
            decimal debitIncurred = 0;//ps_no
            decimal debitIncurredCur = 0;
            DateTime? dateNew;
            int year = 0;
            decimal residual = 0;
            decimal residualCur = 0;
            DateTime ac = (DateTime)dto.Parameters.ToDate;
            year = ac.Year;
            var dic = GetLedgerParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);
            var lstLedger = (from a in incurredData
                             where a.VoucherId == ordRec
                             select new BalanceAccDto
                             {
                                 CreationTime = a.CreationTime
                             }).Take(1).FirstOrDefault();
            dateNew = lstLedger != null ? lstLedger.CreationTime : null;
            if (dateNew == null)
            {
                dateNew = DateTime.Now;
            }
            var sterilizations = await _tenantSettingService.GetTenantSettingByKeyAsync("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            var sterilization = sterilizations.Value;

            //var lstVoucheCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            //var voucherCategory = lstVoucheCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.Parameters.VoucherCode).FirstOrDefault();
            //var voucherOrd = voucherCategory.VoucherOrd;

            var Acc = await _accountSystemService.GetQueryableAsync();
            var lstAcc = Acc.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == dto.Parameters.AccCode).ToList().FirstOrDefault();
            attachPartner = lstAcc.AttachPartner;
            attachContract = lstAcc.AttachContract;
            attachAccSection = lstAcc.AttachAccSection;
            attachWorkPlace = lstAcc.AttachWorkPlace;
            attachCurrency = lstAcc.AttachCurrency;
            attachProductCost = lstAcc.AttachProductCost;
            var attachSectionCode = lstAcc.AttachAccSection;
            var round = "K";
            var nC = "";
            var ctSp = "";

            if (attachContract != "C")
            {
                dto.Parameters.CurrencyCode = "*";
            }
            if (attachPartner != "C")
            {
                dto.Parameters.PartnerCode = null;
            }
            if (attachContract != "C")
            {
                dto.Parameters.ContractCode = null;

            }
            if (attachProductCost != "C")
            {
                dto.Parameters.FProductWorkCode = null;
            }
            if (attachSectionCode != null)
            {
                dto.Parameters.SectionCode = null;
            }
            if (attachWorkPlace != "C")
            {
                dto.Parameters.WorkPlaceCode = null;
            }

            var dic2 = GetLedgerParameter2(dto.Parameters);

            var openingBalance = await GetOpeningBalance(dic2);
            residual = (decimal)(openingBalance.Debit - openingBalance.Credit);
            residualCur = (decimal)(openingBalance.DebitCur - openingBalance.CreditCur);

            var dic3 = GetLedgerParameter2(dto.Parameters);


            var reusulIncurredData = await GetIncurredData(dic3);
            var reusulDebit = (from a in incurredData
                               where
                                 String.Compare(a.Status, "2") < 0
                                 && (sterilization != "C" || a.CheckDuplicate != "C")
                                 && a.CheckDuplicate0 != "C"
                                 && dto.Parameters.AccCode.Contains(a.DebitAcc) == true

                               select new BalanceAccDto
                               {
                                   VoucherId = a.VoucherId,
                                   VoucherDate = a.VoucherDate,
                                   CreationTime = a.CreationTime,
                                   Amount = a.Amount,
                                   DebitAmountCur = a.DebitAmountCur
                               }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.VoucherCode))
            {
                reusulDebit = (from a in reusulIncurredData
                               where a.VoucherCode == dto.Parameters.VoucherCode

                               select new BalanceAccDto
                               {
                                   VoucherId = a.VoucherId,
                                   VoucherDate = a.VoucherDate,
                                   CreationTime = a.CreationTime,
                                   Amount = a.Amount,
                                   DebitAmountCur = a.DebitAmountCur
                               }).ToList();
            }
            reusulDebit = (from a in reusulDebit
                           where (a.VoucherDate < dto.Parameters.ToDate)
                           select new BalanceAccDto
                           {
                               Amount = a.Amount,
                               DebitAmountCur = a.DebitAmountCur,
                               VoucherDate = a.VoucherDate
                           }).ToList();

            var reusulDebit2 = (from a in reusulDebit
                                where (a.VoucherDate == dto.Parameters.FromDate && (a.CreationTime <= dateNew))
                                select new BalanceAccDto
                                {
                                    Amount = a.Amount,
                                    DebitAmountCur = a.DebitAmountCur
                                }).ToList();

            reusulDebit.AddRange(reusulDebit2);
            //tinh Ben NO
            debitIncurred = (decimal)reusulDebit.Select(p => p.Amount).Sum();
            debitIncurredCur = (decimal)reusulDebit.Select(p => p.DebitAmountCur).Sum();



            var reusulCredit = (from a in incurredData
                                where String.Compare(a.Status, "2") < 0
                                  && (a.CheckDuplicate != "N")
                                  // && a.CheckDuplicate0 != "N"
                                  && dto.Parameters.AccCode.Contains(a.CreditAcc) == true

                                select new BalanceAccDto
                                {
                                    VoucherId = a.VoucherId,
                                    VoucherDate = a.VoucherDate,
                                    CreationTime = a.CreationTime,
                                    Amount = a.Amount,
                                    DebitAmountCur = a.DebitAmountCur
                                }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.VoucherCode))
            {
                reusulCredit = (from a in incurredData
                                where a.VoucherCode == dto.Parameters.VoucherCode


                                select new BalanceAccDto
                                {
                                    VoucherId = a.VoucherId,
                                    VoucherDate = a.VoucherDate,
                                    CreationTime = a.CreationTime,
                                    Amount = a.Amount,
                                    DebitAmountCur = a.DebitAmountCur
                                }).ToList();
            }
            reusulCredit = (from a in reusulCredit
                            where (a.VoucherDate < dto.Parameters.ToDate)
                            select new BalanceAccDto
                            {
                                Amount = a.Amount,
                                CreditAmountCur = a.CreditAmountCur
                            }).ToList();

            var reusulCredit2 = (from a in reusulCredit
                                 where (dateNew == null || a.CreationTime <= dateNew || (dateNew == a.CreationTime && a.VoucherId == ordRec))
                                 select new BalanceAccDto
                                 {
                                     Amount = a.Amount,
                                     CreditAmountCur = a.CreditAmountCur
                                 }).ToList();
            reusulCredit.AddRange(reusulCredit2);
            var creditIncurred = (decimal)reusulCredit.Select(p => p.Amount).Sum();
            var creditIncurredCur = (decimal)reusulCredit.Select(p => p.CreditAmountCur).Sum();

            residual = residual + debitIncurred - creditIncurred;
            residualCur = residualCur + debitIncurredCur - creditIncurredCur;
            BalanceAccDto crud = new BalanceAccDto();
            crud.PartnerCode = dto.Parameters.PartnerCode;
            crud.ContractCode = dto.Parameters.ContractCode;
            crud.WorkPlaceCode = dto.Parameters.WorkPlaceCode;
            crud.FProductWorkCode = dto.Parameters.FProductWorkCode;
            crud.SectionCode = dto.Parameters.SectionCode;
            crud.Residual = residual;
            crud.ResidualCur = residualCur;
            List<BalanceAccDto> balanceAccDtos = new List<BalanceAccDto>();
            balanceAccDtos.Add(crud);
            string orgCode = _webHelper.GetCurrentOrgUnit();



            var reportResponse = new ReportResponseDto<BalanceAccDto>();
            reportResponse.Data = balanceAccDtos;
            //reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            //reportResponse.RequestParameter = dto.Parameters;
            //reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            //reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
            //                        _webHelper.GetCurrentYear());
            return reportResponse;
        }

        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
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
        private async Task<AccountBalanceDto> GetOpeningBalance(Dictionary<string, object> dic)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime toDate = Convert.ToDateTime(dic[LedgerParameterConst.ToDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, toDate);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, yearCategory.Year);
            }
            dic[LedgerParameterConst.Year] = yearCategory.Year;
            dic[LedgerParameterConst.FromDate] = null;

            var openingBalances = await _reportDataService.GetAccountBalancesAsync(dic);
            var balances = new AccountBalanceDto()
            {
                Debit = openingBalances.Sum(p => p.Debit),
                Credit = openingBalances.Sum(p => p.Credit),
                DebitCur = openingBalances.Sum(p => p.DebitCur),
                CreditCur = openingBalances.Sum(p => p.CreditCur)
            };

            return balances;
        }
        private async Task<List<BalanceAccDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetLedger(dic);
            var incurredData = warehouseBook.Select(p => new BalanceAccDto()
            {
                Sort = 0,
                Sort0 = 1,
                Bold = "K",
                OrgCode = p.OrgCode,
                Year = p.Year,
                VoucherNumber = p.VoucherNumber,
                VoucherDate = p.VoucherDate,
                Ord0 = p.Ord0,
                GroupCode = null,
                Description = p.Description,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                ExchangeRate = p.ExchangeRate,
                Quantity = p.Quantity,
                Amount0 = p.Amount0,
                AmountCur0 = p.AmountCur0,
                VoucherId = p.VoucherId,
                Id = p.Id,
                VoucherCode = p.VoucherCode,
                FProductWorkCode = p.FProductWorkCode,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                Price = p.Price,
                PriceCur = p.PriceCur,
                BusinessAcc = p.BusinessAcc,
                PartnerCode = p.PartnerCode,
                PartnerName = null,
                Unit = p.UnitCode,
                CaseName = null,
                WarehouseCode = p.WarehouseCode,
                WarehouseName = null,
                //ProductGroupCode = p.ProductGroupCode,
                ProductCode = p.ProductCode,
                //ProductName = p.pro
                TransWarehouseCode = p.TransWarehouseCode,
                Note = p.Note,
                VoucherGroup = p.VoucherGroup,
                AccName = null,
                DepartmentCode = p.DepartmentCode,
                DepartmentName = null,
                Status = p.Status,
                CheckDuplicate = p.CheckDuplicate,
                SectionCode = p.SectionCode,
                DebitContractCode = p.DebitContractCode,
                DebitWorkPlaceCode = p.DebitWorkPlaceCode,
                DebitSectionCode = p.DebitSectionCode,
                DebitFProductWorkCode = p.DebitFProductWorkCode,
                DebitAmountCur = p.DebitAmountCur,
                CreditAmountCur = p.CreditAmountCur,
                CreationTime = p.CreationTime,
                ContractCode = p.ContractCode,
                CheckDuplicate0 = p.CheckDuplicate0,
                Amount = p.Amount0

            }).ToList();


            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetLedger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }

        private Dictionary<string, object> GetLedgerParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");


            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(LedgerParameterConst.PartnerCode, dto.PartnerCode);
            }
            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(LedgerParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            }


            return dic;
        }
        private Dictionary<string, object> GetLedgerParameter2(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);

            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(LedgerParameterConst.PartnerCode, dto.PartnerCode);
            }
            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(LedgerParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            }


            return dic;
        }
        private Dictionary<string, object> GetWarehouseBookParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            //dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }

            //if (!string.IsNullOrEmpty(dto.EndVoucherNumber))
            //{
            //    dic.Add(WarehouseBookParameterConst.EndVoucherNumber, dto.EndVoucherNumber);
            //}
            //if (!string.IsNullOrEmpty(dto.BeginVoucherNumber))
            //{
            //    dic.Add(WarehouseBookParameterConst.BeginVoucherNumber, dto.BeginVoucherNumber);
            //}
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            }
            //if (!string.IsNullOrEmpty(dto.DepartmentCode))
            //{
            //    dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
            //}
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(WarehouseBookParameterConst.SectionCode, dto.SectionCode);
            }
            //if (!string.IsNullOrEmpty(dto.PartnerGroup))
            //{
            //    dic.Add(WarehouseBookParameterConst.PartnerGroup, dto.PartnerGroup);
            //}
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
            }
            if (!string.IsNullOrEmpty(dto.ContractCode))
            {
                dic.Add(WarehouseBookParameterConst.ContractCode, dto.ContractCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(WarehouseBookParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }

            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(WarehouseBookParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
            }
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

