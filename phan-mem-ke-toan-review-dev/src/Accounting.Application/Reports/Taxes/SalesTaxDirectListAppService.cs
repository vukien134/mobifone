using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.Helpers;
using Accounting.Reports.DebitBooks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Vouchers;
using Accounting.Reports.Taxes.NewFolder;
using Accounting.Reports.Taxes.SalesTaxDirectLists;
using Accounting.Caching;

namespace Accounting.Reports.Taxes
{
    public class SalesTaxDirectListAppService : AccountingAppService
    {
        private readonly ExciseTaxService _exciseTaxService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly AccPartnerService _accPartnerService;
        private readonly WebHelper _webHelper;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly ProductService _productService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly CircularsService _circularsService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;

        public SalesTaxDirectListAppService(ExciseTaxService exciseTaxService,
            VoucherExciseTaxService voucherExciseTaxService,
            AccPartnerService accPartnerService,
            WebHelper webHelper,
            OrgUnitService orgUnitService,
            YearCategoryService yearCategoryService,
            AccTaxDetailService accTaxDetailService,
            ProductVoucherService productVoucherService,
            AccVoucherService accVoucherService,
            TenantSettingService tenantSettingService,
            TaxCategoryService taxCategoryService,
            ProductService productService,
            CircularsService circularsService,
            IWebHostEnvironment hostingEnvironment,
            ReportTemplateService reportTemplateService,
            AccountingCacheManager accountingCacheManager
            )
        {
            _exciseTaxService = exciseTaxService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _accPartnerService = accPartnerService;
            _webHelper = webHelper;
            _orgUnitService = orgUnitService;
            _yearCategoryService = yearCategoryService;
            _accTaxDetailService = accTaxDetailService;
            _productVoucherService = productVoucherService;
            _accVoucherService = accVoucherService;
            _tenantSettingService = tenantSettingService;
            _taxCategoryService = taxCategoryService;
            _productService = productService;
            _circularsService = circularsService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        public async Task<ReportResponseDto<SalesTaxDirectListDto>> CreateDataAsync(ReportRequestDto<SalesTaxDirectListParameterDto> dto)
        {
            var reportResponse = new ReportResponseDto<SalesTaxDirectListDto>();
            var accTaxDetail = await _accTaxDetailService.GetQueryableAsync();
            var lstAccTaxDetail = accTaxDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var accPartner = await _accPartnerService.GetQueryableAsync();
            var lstAccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var taxCategory = await _taxCategoryService.GetQueryableAsync();
            var lstTaxCategory = taxCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var accVoucher = await _accVoucherService.GetQueryableAsync();
            var lstAccVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var dataTax = (from a in lstAccTaxDetail
                           join b in lstTaxCategory on a.TaxCategoryCode equals b.Code
                          join c in lstAccVoucher on a.AccVoucherId equals c.Id into ajc
                          from c in ajc.DefaultIfEmpty()
                          join d in lstProductVoucher on a.ProductVoucherId equals d.Id into ajd
                          from d in ajd.DefaultIfEmpty()
                          where ((a.CheckDuplicate ?? "") == "" || a.CheckDuplicate != "*")
                              && b.IsDirect == true
                              && a.VoucherDate >= dto.Parameters.FromDate
                              && a.VoucherDate <= dto.Parameters.ToDate
                              && ((a.DebitAcc ?? "") == "" || a.DebitAcc.StartsWith(dto.Parameters.AccCode ?? "")
                              || (a.CreditAcc ?? "") == "" || a.CreditAcc.StartsWith(dto.Parameters.AccCode ?? ""))
                              && ((dto.Parameters.FProducWorkCode ?? "") == "" || a.FProductWorkCode.StartsWith(dto.Parameters.FProducWorkCode ?? ""))
                          select new SalesTaxDirectListDataTaxDto
                          {
                              Sort = "B1",
                              Bold = "K",
                              ViewType = 2,
                              OrgCode = a.OrgCode,
                              CheckDuplicate = a.CheckDuplicate,
                              Id = c == null ? d?.Id ?? "" : c?.Id ?? "",
                              Ord0 = a.Ord0,
                              Year = a.Year,
                              VoucherCode = c == null ? d?.VoucherCode ?? "" : c?.VoucherCode ?? "",
                              ContractCode = a.ContractCode,
                              DepartmentCode = a.DepartmentCode,
                              InvoiceDate = a.InvoiceDate,
                              VoucherDate = a.VoucherDate,
                              VoucherNumber = c == null ? d?.VoucherNumber ?? "" : c?.VoucherNumber ?? "",
                              TaxCategoryCode = a.TaxCategoryCode,
                              InvoiceGroup = a.InvoiceGroup,
                              InvoiceBookCode = a.InvoiceBookCode,
                              DebitAcc = a.DebitAcc,
                              CreditAcc = a.CreditAcc,
                              InvoiceSymbol = a.InvoiceSymbol,
                              InvoiceNumber = a.InvoiceNumber,
                              PartnerCode = a.PartnerCode,
                              PartnerName = a.PartnerName,
                              Address = a.Address,
                              TaxCode = a.TaxCode,
                              ProductName = a.ProductName,
                              AmountWithoutVatCur = a.AmountWithoutVatCur,
                              AmountWithoutVat = a.AmountWithoutVat,
                              VatPercentage = a.VatPercentage,
                              Amount = a.Amount,
                              AmountCur = a.AmountCur,
                              Note = a.Note,
                              Deduct = b.Deduct,
                              OutOrIn = b.OutOrIn,
                              DebitAcc0 = a.DebitAcc,
                              CreditAcc0 = a.CreditAcc,
                          }).ToList();
            foreach(var item in dataTax)
            {
                if (item.DebitAcc == (dto.Parameters.AccCode ?? ""))
                {
                    item.DebitAcc = item.CreditAcc0;
                    item.CreditAcc = item.DebitAcc0;
                    item.AmountWithoutVat = -1*item.AmountWithoutVat;
                    item.AmountWithoutVatCur = -1*item.AmountWithoutVatCur;
                    item.Amount = -1*item.Amount;
                    item.AmountCur = -1*item.AmountCur;
                }
                item.Note = "";
            }
            var sort = "B1";
            var amountWithoutVatCur = dataTax.Sum(p => p.AmountWithoutVatCur);
            var amountWithoutVat = dataTax.Sum(p => p.AmountWithoutVat);
            var amountCur = dataTax.Sum(p => p.AmountCur);
            var amount = dataTax.Sum(p => p.Amount);
            var amountWithoutVatCur0 = dataTax.Where(p => (p.VatPercentage ?? 0) == 0).Sum(p => p.AmountWithoutVatCur);
            var amountWithoutVat0 = dataTax.Where(p => (p.VatPercentage ?? 0) == 0).Sum(p => p.AmountWithoutVat);
            var amountCur0 = dataTax.Where(p => (p.VatPercentage ?? 0) == 0).Sum(p => p.AmountCur);
            var amount0 = dataTax.Where(p => (p.VatPercentage ?? 0) == 0).Sum(p => p.Amount);
            var amountWithoutVatCur1 = dataTax.Where(p => (p.VatPercentage ?? 0) == 1).Sum(p => p.AmountWithoutVatCur);
            var amountWithoutVat1 = dataTax.Where(p => (p.VatPercentage ?? 0) == 1).Sum(p => p.AmountWithoutVat);
            var amountCur1 = dataTax.Where(p => (p.VatPercentage ?? 0) == 1).Sum(p => p.AmountCur);
            var amount1 = dataTax.Where(p => (p.VatPercentage ?? 0) == 1).Sum(p => p.Amount);
            var amountWithoutVatCur2 = dataTax.Where(p => (p.VatPercentage ?? 0) == 2).Sum(p => p.AmountWithoutVatCur);
            var amountWithoutVat2 = dataTax.Where(p => (p.VatPercentage ?? 0) == 2).Sum(p => p.AmountWithoutVat);
            var amountCur2 = dataTax.Where(p => (p.VatPercentage ?? 0) == 2).Sum(p => p.AmountCur);
            var amount2 = dataTax.Where(p => (p.VatPercentage ?? 0) == 2).Sum(p => p.Amount);
            var amountWithoutVatCur3 = dataTax.Where(p => (p.VatPercentage ?? 0) == 3).Sum(p => p.AmountWithoutVatCur);
            var amountWithoutVat3 = dataTax.Where(p => (p.VatPercentage ?? 0) == 3).Sum(p => p.AmountWithoutVat);
            var amountCur3 = dataTax.Where(p => (p.VatPercentage ?? 0) == 3).Sum(p => p.AmountCur);
            var amount3 = dataTax.Where(p => (p.VatPercentage ?? 0) == 3).Sum(p => p.Amount);
            var amountWithoutVatCur5 = dataTax.Where(p => (p.VatPercentage ?? 0) == 5).Sum(p => p.AmountWithoutVatCur);
            var amountWithoutVat5 = dataTax.Where(p => (p.VatPercentage ?? 0) == 5).Sum(p => p.AmountWithoutVat);
            var amountCur5 = dataTax.Where(p => (p.VatPercentage ?? 0) == 5).Sum(p => p.AmountCur);
            var amount5 = dataTax.Where(p => (p.VatPercentage ?? 0) == 5).Sum(p => p.Amount);
            foreach (var item in dataTax)
            {
                if ((item.VatPercentage ?? 0) == 0)
                {
                    item.Sort = "A2";
                    item.Deduct = 1;
                    item.InvoiceGroup = "1";
                }
                if (item.VatPercentage == 1)
                {
                    item.Sort = "B2";
                    item.Deduct = 1;
                    item.InvoiceGroup = "2";
                }
                if (item.VatPercentage == 5)
                {
                    item.Sort = "C2";
                    item.Deduct = 1;
                    item.InvoiceGroup = "3";
                }
                if (item.VatPercentage == 3)
                {
                    item.Sort = "D2";
                    item.Deduct = 1;
                    item.InvoiceGroup = "4";
                }
                if (item.VatPercentage == 2)
                {
                    item.Sort = "E2";
                    item.Deduct = 1;
                    item.InvoiceGroup = "5";
                }
            }

            var data = new List<SalesTaxDirectListDataTaxDto>();
            // Khong chiu thue
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "A1",
                Bold = "C",
                Deduct = 0,
                ViewType = 1,
                PartnerName = "1. Hàng hóa, dịch vụ không chịu thuế GTGT, hoặc hàng hóa dịch vụ áp thuế suất 0%",
                InvoiceGroup = "1",
            });
            data.AddRange(dataTax.Where(p => (p.VatPercentage ?? 0) == 0).ToList());
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "A3",
                Bold = "C",
                Deduct = 0,
                ViewType = 3,
                PartnerName = "TỔNG CỘNG",
                InvoiceGroup = "1",
                AmountWithoutVatCur = amountWithoutVatCur0,
                AmountWithoutVat = amountWithoutVat0,
                AmountCur = amountCur0,
                Amount = amount0,
            });
            // --Thue 1%
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "B1",
                Bold = "C",
                Deduct = 0,
                ViewType = 1,
                PartnerName = "2. Phân phối, cung cấp hàng hóa áp dụng thuế suất 1%",
                InvoiceGroup = "1",
            });
            data.AddRange(dataTax.Where(p => p.VatPercentage == 1).ToList());
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "B3",
                Bold = "C",
                Deduct = 1,
                ViewType = 3,
                PartnerName = "TỔNG CỘNG",
                InvoiceGroup = "1",
                AmountWithoutVatCur = amountWithoutVatCur1,
                AmountWithoutVat = amountWithoutVat1,
                AmountCur = amountCur1,
                Amount = amount1,
            });
            // Thue 5%
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "C1",
                Bold = "C",
                Deduct = 1,
                ViewType = 1,
                PartnerName = "3. Dịch vụ, xây dựng không báo thầu nguyên vật liệu áp dụng thuế suất 5%",
                InvoiceGroup = "1",
            });
            data.AddRange(dataTax.Where(p => p.VatPercentage == 5).ToList());
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "C3",
                Bold = "C",
                Deduct = 1,
                ViewType = 3,
                PartnerName = "TỔNG CỘNG",
                InvoiceGroup = "1",
                AmountWithoutVatCur = amountWithoutVatCur5,
                AmountWithoutVat = amountWithoutVat5,
                AmountCur = amountCur5,
                Amount = amount5,
            });
            // Thue 3%
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "D1",
                Bold = "C",
                Deduct = 1,
                ViewType = 1,
                PartnerName = "4. Sản xuât, vận tải, dịch vụ có gắn với hàng hóa, xây dựng có bao thầu nguyên vật liệu áp dụng thuế suất 3%",
                InvoiceGroup = "1",
            });
            data.AddRange(dataTax.Where(p => p.VatPercentage == 3).ToList());
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "D3",
                Bold = "C",
                Deduct = 1,
                ViewType = 3,
                PartnerName = "TỔNG CỘNG",
                InvoiceGroup = "1",
                AmountWithoutVatCur = amountWithoutVatCur3,
                AmountWithoutVat = amountWithoutVat3,
                AmountCur = amountCur3,
                Amount = amount3,
            });
            // Thue 2%
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "E1",
                Bold = "C",
                Deduct = 1,
                ViewType = 1,
                PartnerName = "5. Hoạt động kinh doanh khác áp dụng thuế suất 2%",
                InvoiceGroup = "5",
            });
            data.AddRange(dataTax.Where(p => p.VatPercentage == 3).ToList());
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "E3",
                Bold = "C",
                Deduct = 2,
                ViewType = 3,
                PartnerName = "TỔNG CỘNG",
                InvoiceGroup = "1",
                AmountWithoutVatCur = amountWithoutVatCur2,
                AmountWithoutVat = amountWithoutVat2,
                AmountCur = amountCur2,
                Amount = amount2,
            });
            // Tong phat sinh
            data.Add(new SalesTaxDirectListDataTaxDto
            {
                Sort = "F0",
                Bold = "C",
                Deduct = 1,
                ViewType = 3,
                PartnerName = "TỔNG PHÁT SINH",
                InvoiceGroup = "1",
                AmountWithoutVatCur = amountWithoutVatCur,
                AmountWithoutVat = amountWithoutVat,
                AmountCur = amountCur,
                Amount = amount,
            });
            //-----------------
            var lst = (from a in data
                      join b in lstAccPartner on a.PartnerCode equals b.Code into ajb
                      from b in ajb.DefaultIfEmpty()
                      select new SalesTaxDirectListDto
                      {
                          Sort = a.Sort,
                          Bold = a.Bold,
                          DocId = a.Id,
                          VoucherCode = a.VoucherCode,
                          Deduct = a.Deduct,
                          ViewType = a.ViewType,
                          TotalAmountWithoutVat = amountWithoutVat,
                          TotalAmountVat = amount,
                          InvoiceDate = a.InvoiceDate,
                          VatPercentage = a.VatPercentage,
                          InvoiceNumber = a.InvoiceNumber,
                          InvoiceSerial = a.InvoiceSymbol,
                          VoucherDate = a.VoucherDate,
                          VoucherNumber = a.VoucherNumber,
                          TaxCode = b?.TaxCode ?? a.TaxCode,
                          PartnerName = ((a.PartnerName ?? "") != "") ? a.PartnerName : b?.Name ?? "",
                          PartnerCode = a.PartnerCode,
                          ProductName = a.ProductName,
                          AmountWithoutVat = a.AmountWithoutVat,
                          Amount = a.Amount,
                      }).ToList();
            if (dto.Parameters.Sort == "1")
            {
                lst = lst.OrderBy(p => p.Sort).ThenBy(p => p.Deduct).ThenBy(p => p.VatPercentage).ThenBy(p => p.InvoiceDate).ThenBy(p => p.InvoiceNumber).ToList();
            } 
            else
            {
                lst = lst.OrderBy(p => p.Sort).ThenBy(p => p.Deduct).ThenBy(p => p.VatPercentage).ThenBy(p => p.InvoiceDate).ThenBy(p => p.VatPercentage).ThenBy(p => p.InvoiceNumber).ToList();
            }
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }

        public async Task<FileContentResult> PrintAsync(ReportRequestDto<SalesTaxDirectListParameterDto> dto)
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
    }
}

