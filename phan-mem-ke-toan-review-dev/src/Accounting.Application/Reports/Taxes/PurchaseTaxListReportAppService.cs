using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Vouchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Taxes
{
    public class PurchaseTaxListReportAppService : AccountingAppService
    {
        #region Privates
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly AccVoucherService _accVoucherService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public PurchaseTaxListReportAppService(AccTaxDetailService accTaxDetailService,
                            TaxCategoryService taxCategoryService,
                            WebHelper webHelper,
                            YearCategoryService yearCategoryService,
                            CircularsService circularsService,
                            OrgUnitService orgUnitService,
                            TenantSettingService tenantSettingService,
                            IWebHostEnvironment hostingEnvironment,
                            ReportTemplateService reportTemplateService,
                            ProductVoucherService productVoucherService,
                            AccVoucherService accVoucherService,
                            AccountingCacheManager accountingCacheManager
                        )
        {
            _accTaxDetailService = accTaxDetailService;
            _webHelper = webHelper;
            _taxCategoryService = taxCategoryService;
            _yearCategoryService = yearCategoryService;
            _circularsService = circularsService;
            _orgUnitService = orgUnitService;
            _tenantSettingService = tenantSettingService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _productVoucherService = productVoucherService;
            _accVoucherService = accVoucherService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.PurchaseTaxListReportView)]
        public async Task<ReportResponseDto<SalesTaxListDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var datas = await GetData(dto);
            var groupTaxs = GetGroupByTaxCategory(datas);
            int sort = dto.Parameters.Sort1 == null ? 1 : dto.Parameters.Sort1.Value;
            int i = 1;
            decimal amount = 0,
                    amountCur = 0,
                    amountWithoutVat = 0,
                    amountWithoutVatCur = 0,
                    totalAmount = 0,
                    totalAmountCur = 0;
            var dtos = new List<SalesTaxListDto>();
            foreach (var group in groupTaxs)
            {
                var item = new SalesTaxListDto()
                {
                    PartnerName = this.GetInvoiceGroupName(group.InvoiceGroup),
                    Bold = "C",
                    InvoiceGroup = group.InvoiceGroup
                };
                dtos.Add(item);
                var details = this.GetListByGroup(datas, item.InvoiceGroup, sort);
                dtos.AddRange(details);

                item = new SalesTaxListDto()
                {
                    Bold = "C",
                    PartnerName = "Tổng cộng",
                    Amount = group.Amount,
                    AmountCur = group.AmountCur,
                    AmountWithoutVat = group.AmountWithoutVat,
                    AmountWithoutVatCur = group.AmountWithoutVatCur,
                    TotalAmount = group.TotalAmount,
                    TotalAmountCur = item.TotalAmountCur,
                    InvoiceGroup = group.InvoiceGroup
                };
                dtos.Add(item);

                amount = amount + this.GetAmount(group.Amount);
                amountCur = amountCur + this.GetAmount(group.AmountCur);
                amountWithoutVat = amountWithoutVat + this.GetAmount(group.AmountWithoutVat);
                amountWithoutVatCur = amountWithoutVatCur + this.GetAmount(group.AmountWithoutVatCur);
                totalAmount = totalAmount + this.GetAmount(group.TotalAmount);
                totalAmountCur = totalAmountCur + this.GetAmount(group.TotalAmountCur);
                i++;
            }

            dtos.Add(new SalesTaxListDto()
            {
                Bold = "C",
                PartnerName = "Tổng phát sinh",
                Amount = amount,
                AmountCur = amountCur,
                AmountWithoutVat = amountWithoutVat,
                AmountWithoutVatCur = amountWithoutVatCur,
                TotalAmount = totalAmount,
                TotalAmountCur = totalAmountCur
            });
            var reportResponse = new ReportResponseDto<SalesTaxListDto>();
            reportResponse.Data = dtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.PurchaseTaxListReportPrint)]
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
        #region Privates
        private async Task<IQueryable<AccTaxDetail>> Filter(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var queryable = await _accTaxDetailService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit())
                                    && p.VoucherDate >= dto.Parameters.FromDate
                                    && p.VoucherDate <= dto.Parameters.ToDate
                                    && p.DebitAcc.StartsWith(dto.Parameters.AccCode));
            if (!string.IsNullOrEmpty(dto.Parameters.FProductWorkCode))
            {
                queryable = queryable.Where(p => p.FProductWorkCode.Equals(dto.Parameters.FProductWorkCode));
            }
            if (!string.IsNullOrEmpty(dto.Parameters.DepartmentCode))
            {
                queryable = queryable.Where(p => p.DepartmentCode.Equals(dto.Parameters.DepartmentCode));
            }
            return queryable;
        }
        private async Task<List<SalesTaxListDto>> GetData(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var queryable = await Filter(dto);
            var queryableProductVoucher = await _productVoucherService.GetQueryableAsync();
            var queryableAccVoucher = await _accVoucherService.GetQueryableAsync();

            var query = from p in queryable
                        join c in queryableProductVoucher on p.ProductVoucherId equals c.Id into dj
                        from df in dj.DefaultIfEmpty()
                        join d in queryableAccVoucher on p.AccVoucherId equals d.Id into ej
                        from ef in ej.DefaultIfEmpty()
                        select new SalesTaxListDto
                        {
                            Amount = p.Amount,
                            AmountCur = p.AmountCur,
                            AmountWithoutVat = p.AmountWithoutVat,
                            AmountWithoutVatCur = p.AmountWithoutVatCur,
                            InvoiceDate = p.InvoiceDate,
                            InvoiceLink = p.InvoiceLink,
                            InvoiceNumber = p.InvoiceNumber,
                            InvoiceSymbol = p.InvoiceSymbol,
                            PartnerName = p.PartnerName,
                            ProductName = p.ProductName,
                            TaxCode = p.TaxCode,
                            TotalAmount = p.TotalAmount,
                            TotalAmountCur = p.TotalAmountCur,
                            VatPercentage = p.VatPercentage,
                            TaxCategoryCode = p.TaxCategoryCode,
                            VoucherDate = p.VoucherDate,
                            InvoiceGroup = p.InvoiceGroup,
                            VoucherId = df != null ? df.Id :
                                        ef != null ? ef.Id : null,
                            VoucherCode = df != null ? df.VoucherCode :
                                          ef != null ? ef.VoucherCode : null
                        };

            return await AsyncExecuter.ToListAsync(query);
        }
        private List<SalesTaxListDto> GetGroupByTaxCategory(List<SalesTaxListDto> dtos)
        {
            var groupData = dtos.GroupBy(p => new
            {
                //p.Amount,
                //p.AmountCur,
                //p.AmountWithoutVat,
                //p.AmountWithoutVatCur,
                p.InvoiceGroup
            }).Select(p => new SalesTaxListDto()
            {
                InvoiceGroup = p.Key.InvoiceGroup,
                Amount = p.Sum(s => s.Amount),
                AmountCur = p.Sum(s => s.AmountCur),
                AmountWithoutVat = p.Sum(s => s.AmountWithoutVat),
                AmountWithoutVatCur = p.Sum(s => s.AmountWithoutVatCur),
                TotalAmount = p.Sum(p => p.TotalAmount),
                TotalAmountCur = p.Sum(p => p.TotalAmountCur)
            }).OrderBy(p => p.InvoiceGroup).ToList();

            return groupData;
        }
        private async Task<List<TaxCategory>> GetTaxOut()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            return await _taxCategoryService.GetByOutOrIn(orgCode, "R");
        }
        private List<SalesTaxListDto> GetListByGroup(List<SalesTaxListDto> dtos, string invoiceGroup, int sort)
        {
            var query = dtos.Where(p => p.InvoiceGroup == (invoiceGroup));
            if (sort == 1)
            {
                query = query.OrderBy(p => p.VoucherDate);
            }
            if (sort == 2)
            {
                query = query.OrderBy(p => p.InvoiceDate);
            }
            if (sort == 3)
            {
                query = query.OrderBy(p => p.VatPercentage);
            }
            return query.ToList();
        }
        private decimal GetAmount(decimal? amount)
        {
            if (amount == null) return 0;
            return amount.Value;
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
        private async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return ObjectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private string GetInvoiceGroupName(string invoiceGroup)
        {
            var name = invoiceGroup switch
            {
                "1" => "1. Hóa đơn GTGT đủ điều kiện khấu trừ thuế",
                "2" => "2. Hàng hóa, dịch vụ dùng chung cho SXKD chịu thuế và không chịu thuế đủ điều kiện khấu trừ",
                "3" => "3. Hàng hóa, dịch vụ dùng cho dự án đầu tư đủ điều kiện được khấu trừ thuế",
                _ => ""
            };
            return name;
        }
        #endregion
    }
}
