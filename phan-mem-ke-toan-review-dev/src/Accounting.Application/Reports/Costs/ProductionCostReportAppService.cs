using Accounting.BaseDtos.Customines;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.CostProductions;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Products;
using Accounting.Catgories.CostProductions;
using Accounting.Catgories.FProductWorks;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.CostReports;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.ImportExports.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Reports.Costs
{
    public class ProductionCostReportAppService : AccountingAppService
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
        private readonly CostProductionAppService _costProductionAppService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly ProductionPeriodService _productionPeriodService;
        private readonly InfoZService _infoZService;
        private readonly SoTHZService _soTHZService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductService _productService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ProductionCostReportAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        CostProductionAppService costProductionAppService,
                        WorkPlaceSevice workPlaceSevice,
                        ProductionPeriodService productionPeriodService,
                        InfoZService infoZService,
                        SoTHZService soTHZService,
                        ProductVoucherService productVoucherService,
                        ProductService productService,
                        ProductVoucherDetailService productVoucherDetailService,
                        VoucherTypeService voucherTypeService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager
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
            _costProductionAppService = costProductionAppService;
            _workPlaceSevice = workPlaceSevice;
            _productionPeriodService = productionPeriodService;
            _infoZService = infoZService;
            _soTHZService = soTHZService;
            _productVoucherService = productVoucherService;
            _productService = productService;
            _productVoucherDetailService = productVoucherDetailService;
            _voucherTypeService = voucherTypeService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.ProductionCostReportView)]
        public async Task<ReportResponseDto<JsonObject>> CreateDataAsync(ReportRequestDto<ProductionCostReportParameterDto> dto)
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var year = dto.Parameters.FromDate.Value.Year;
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherType = voucherType.Where(p => p.Code == "PTP").Select(p => p.ListVoucher).FirstOrDefault();
            var workPlace = await _workPlaceSevice.GetQueryableAsync();
            var lstWorkPlace = workPlace.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productionPeriod = await _productionPeriodService.GetQueryableAsync();
            var lstProductionPeriod = productionPeriod.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var iQyearCategory = await _yearCategoryService.GetQueryableAsync();
            var yearCategory = iQyearCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
            var lstProductVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstVoucherCode = voucherType.Where(p => p.Code == "PTP").Select(p => p.ListVoucher).First();
            var lstQuantityFProduct = await _costProductionAppService.GetQuantityFProduct(new GetQuantityFProductFilterDto
            {
                Year = year,
                FromDate = dto.Parameters.FromDate.Value,
                ToDate = dto.Parameters.ToDate.Value,
                LstVoucherCode = lstVoucherCode,
                ProductionPeriodCode = dto.Parameters.ProductionPeriodCode
            });
            var infoZ = await _infoZService.GetQueryableAsync();
            var lstZ = infoZ.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                         && p.FProductWork == "S"
                                         && p.BeginM >= dto.Parameters.FromDate
                                         && p.EndM <= dto.Parameters.ToDate
                                         && p.DebitAcc.StartsWith("154")
                                         && (dto.Parameters.WorkPlaceCode == "" || p.WorkPlaceCode == dto.Parameters.WorkPlaceCode)
                                         && (dto.Parameters.ProductionPeriodCode == "" || p.ProductionPeriodCode == dto.Parameters.ProductionPeriodCode)
                                         && (dto.Parameters.FProductWorkCode == "" || p.FProductWorkCode.StartsWith(dto.Parameters.FProductWorkCode)))
                            .Select(p => ObjectMapper.Map<InfoZ, InfoZDto>(p)).ToList();
            foreach (var item in lstZ)
            {
                item.BeginQuantity = item.BeginM == dto.Parameters.FromDate ? item.BeginQuantity : 0;
                item.BeginAmount = item.BeginM == dto.Parameters.FromDate ? item.BeginAmount : 0;
                item.EndQuantity = item.EndM == dto.Parameters.ToDate ? item.EndQuantity : 0;
                item.EndAmount = item.EndM == dto.Parameters.ToDate ? item.EndAmount : 0;
            }
            // Insert tiền đầu kỳ, cuối kỳ với trường hợp theo % hoàn thành
            // ĐẦU KỲ
            lstZ.AddRange((from a in lstProductVoucher
                           join b in lstProductVoucherDetail on a.Id equals b.ProductVoucherId
                           where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == dto.Parameters.FromDate
                           && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154")
                           select new InfoZDto
                           {
                               Id = b.Id,
                               OrgCode = b.OrgCode,
                               WorkPlaceCode = b.WorkPlaceCode,
                               FProductWorkCode = b.FProductWorkCode,
                               BeginAmount = b.Amount,
                           }).ToList());
            // CUỐI KỲ
            lstZ.AddRange((from a in lstProductVoucher
                           join b in lstProductVoucherDetail on a.Id equals b.ProductVoucherId
                           where a.Year == year && a.VoucherCode == "KKD" && a.VoucherDate == dto.Parameters.ToDate
                           && b.DebitAcc.StartsWith("155") && b.CreditAcc.StartsWith("154")
                           select new InfoZDto
                           {
                               Id = b.Id,
                               OrgCode = b.OrgCode,
                               WorkPlaceCode = b.WorkPlaceCode,
                               FProductWorkCode = b.FProductWorkCode,
                               EndAmount = b.Amount,
                           }).ToList());

            var lstFProductWork = new List<QuantityFProductDto>();
            lstFProductWork.AddRange((from a in lstQuantityFProduct
                                      where (dto.Parameters.WorkPlaceCode == "" || a.WorkPlaceCode == dto.Parameters.WorkPlaceCode)
                                          && (dto.Parameters.FProductWorkCode == "" || a.FProductWorkCode.StartsWith(dto.Parameters.FProductWorkCode))
                                      select new QuantityFProductDto
                                      {
                                          OrgCode = a.OrgCode,
                                          FProductWorkCode = a.FProductWorkCode,
                                          WorkPlaceCode = a.WorkPlaceCode,
                                      }).ToList());
            lstFProductWork.AddRange((from a in lstZ
                                      select new QuantityFProductDto
                                      {
                                          OrgCode = a.OrgCode,
                                          FProductWorkCode = a.FProductWorkCode,
                                          WorkPlaceCode = a.WorkPlaceCode,
                                      }).ToList());
            lstFProductWork = lstFProductWork.GroupBy(g => new
                                {
                                    g.OrgCode,
                                    g.FProductWorkCode,
                                    g.WorkPlaceCode,
                                }).Select(p => new QuantityFProductDto
                                {
                                    OrgCode = p.Key.OrgCode,
                                    FProductWorkCode = p.Key.FProductWorkCode,
                                    WorkPlaceCode = p.Key.WorkPlaceCode,
                                }).ToList();
            var data = (from a in lstProduct
                        join b in lstFProductWork on a.FProductWorkCode equals b.FProductWorkCode
                        join c in lstQuantityFProduct on a.FProductWorkCode equals c.FProductWorkCode into ajc
                        from c in ajc.DefaultIfEmpty()
                        where dto.Parameters.ProductionPeriodCode == "" || a.ProductionPeriodCode == dto.Parameters.ProductionPeriodCode
                        select new ProductionCostReportDataDto
                        {
                            Sort = "B",
                            Bold = "K",
                            Rank = 2,
                            OrgCode = a.OrgCode,
                            Code = a.Code,
                            ProductCode0 = a.Code,
                            ProductCode = a.Code,
                            ProductName = a.Name,
                            UnitCode = a.UnitCode,
                            WorkPlaceCode = b.WorkPlaceCode,
                            ProductionPeriodCode = a.ProductionPeriodCode,
                            FProductWorkCode = a.FProductWorkCode,
                            BeginQuantity = c?.UnfinishedQuantity ?? 0,
                            BeginPercentage = c?.HTPercentage ?? 0,
                            BeginAmount = 0,
                            Quantity = c?.Quantity ?? 0,
                            Price = 0,
                            IncurredTotalCost = 0,
                            TotalZ = 0,
                            EndQuantity = c?.UnfinishedQuantity2 ?? 0,
                            EndPercentage = c?.HTPercentage2 ?? 0,
                            EndAmount = 0,
                        }).ToList();
            var totalZ = (from a in lstZ
                          group new { a } by new
                          {
                              a.OrgCode,
                              a.WorkPlaceCode,
                              a.FProductWorkCode,
                          } into gr
                          select new InfoZDto
                          {
                              OrgCode = gr.Key.OrgCode,
                              WorkPlaceCode = gr.Key.WorkPlaceCode,
                              FProductWorkCode = gr.Key.FProductWorkCode,
                              BeginAmount = gr.Sum(p => p.a.BeginAmount ?? 0),
                              Amount = gr.Sum(p => p.a.Amount ?? 0),
                              EndAmount = gr.Sum(p => p.a.EndAmount ?? 0),
                          }).ToList();
            foreach (var item in data)
            {
                var dataTotalZ = totalZ.Where(p => p.WorkPlaceCode == item.WorkPlaceCode && p.FProductWorkCode == item.FProductWorkCode).FirstOrDefault();
                item.BeginAmount = dataTotalZ?.BeginAmount ?? 0;
                item.IncurredTotalCost = dataTotalZ?.Amount ?? 0;
                item.EndAmount = dataTotalZ?.EndAmount ?? 0;
                item.TotalZ = item.BeginAmount + item.IncurredTotalCost - item.EndAmount;
                item.Price = (item.Quantity ?? 0) != 0 ? Math.Round(item.TotalZ / item.Quantity ?? 0, 0) : 0;
            }
            var soThz = await _soTHZService.GetQueryableAsync();
            var dataSoThZ = soThz.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()
                                          && p.FProductOrWork == "S"
                                          && p.UsingDecision == (yearCategory == null ? 200 : yearCategory.UsingDecision))
                                 .OrderBy(p => p.Ord).ToList();
            var jsonDataZ = new List<JsonObject>();
            var jsonData = new List<JsonObject>();
            var lstSumField = new List<string>();
            foreach (var item in lstZ)
            {
                var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = JsonNode.Parse(strJson);
                jsonDataZ.Add((JsonObject)job);
            }
            foreach (var item in data)
            {
                var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = JsonNode.Parse(strJson);
                jsonData.Add((JsonObject)job);
            }
            foreach (var itemSoThZ in dataSoThZ)
            {
                lstSumField.Add(itemSoThZ.FieldName);
                var lstField = new List<FormularDto>();
                if (itemSoThZ.DebitAcc != "" || itemSoThZ.CreditAcc != "")
                {
                    if ((itemSoThZ.TSum ?? "") != "")
                    {
                        lstField = GetFormular(itemSoThZ.TSum);
                    }
                    else
                    {
                        lstField.Add(new FormularDto { Code = "Amount", Math = "+" });
                    }

                    if (itemSoThZ.DebitAcc != "" || itemSoThZ.CreditAcc != "" || itemSoThZ.DebitSection != "" || itemSoThZ.CreditSection != "")
                    {
                        var lstGroup = jsonDataZ.Where(p => (itemSoThZ.DebitAcc == "" || (p["DebitAcc"] ?? "").ToString().StartsWith(itemSoThZ.DebitAcc))
                                                          && (itemSoThZ.CreditAcc == "" || (p["CreditAcc"] ?? "").ToString().StartsWith(itemSoThZ.CreditAcc))
                                                          && (itemSoThZ.DebitSection == "" || (p["DebitSectionCode"] ?? "").ToString().StartsWith(itemSoThZ.DebitSection))
                                                          && (itemSoThZ.CreditSection == "" || (p["CreditSectionCode"] ?? "").ToString().StartsWith(itemSoThZ.CreditSection)))
                                            .GroupBy(g => new { WorkPlaceCode = g["WorkPlaceCode"].ToString(), FProductWorkCode = g["FProductWorkCode"].ToString() })
                                            .Select(p =>
                                            {
                                                decimal amount = 0;
                                                foreach (var itemField in lstField)
                                                {
                                                    amount += p.Sum(a => decimal.Parse(a[itemField.Code].ToString(), CultureInfo.InvariantCulture)) * (itemField.Math == "+" ? 1 : -1);
                                                }
                                                return new
                                                {
                                                    p.Key.WorkPlaceCode,
                                                    p.Key.FProductWorkCode,
                                                    Amount = Math.Round(amount, 0)
                                                };
                                            }).ToList();
                        foreach (var itemData in jsonData)
                        {
                            var itemGroup = lstGroup.Where(p => p.WorkPlaceCode == itemData["WorkPlaceCode"].ToString()
                                                             && p.FProductWorkCode == itemData["FProductWorkCode"].ToString()).FirstOrDefault();
                            itemData[itemSoThZ.FieldName] = itemGroup?.Amount ?? 0;
                        }
                    }
                }
                else if ((itemSoThZ.TSum ?? "") != "")
                {
                    foreach (var itemData in jsonData)
                    {
                        var lstFieldTsum = GetFormular(itemSoThZ.TSum);
                        decimal amount = 0;
                        foreach (var itemFieldeTsum in lstFieldTsum)
                        {
                            amount += decimal.Parse(itemData[itemFieldeTsum.Code].ToString(), CultureInfo.InvariantCulture) * (itemFieldeTsum.Math == "+" ? 1 : -1);
                        }
                        itemData[itemSoThZ.FieldName] = amount;
                        //itemData[itemSoThZ.FieldName] = itemData[itemSoThZ.TSum];
                    }
                }
            }

            jsonData.AddRange(jsonData.Where(p => p["Bold"].ToString() == "K" && (p["WorkPlaceCode"] ?? "").ToString() != "")
                                      .GroupBy(g => new { WorkPlaceCode = g["WorkPlaceCode"].ToString() })
                                            .Select(p =>
                                            {
                                                var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(new ProductionCostReportDataDto());
                                                var workPlaceName = lstWorkPlace.Where(a => a.Code == p.Key.WorkPlaceCode).FirstOrDefault();
                                                var jsonItem = (JsonObject)JsonNode.Parse(strJson);
                                                jsonItem["Sort"] = "B";
                                                jsonItem["Bold"] = "B";
                                                jsonItem["Rank"] = 0;
                                                jsonItem["OrgCode"] = _webHelper.GetCurrentOrgUnit();
                                                jsonItem["ProductCode0"] = "";
                                                jsonItem["ProductCode"] = p.Key.WorkPlaceCode;
                                                jsonItem["ProductName"] = workPlaceName?.Name ?? "";
                                                jsonItem["WorkPlaceCode"] = p.Key.WorkPlaceCode;
                                                jsonItem["ProductionPeriodCode"] = "";
                                                jsonItem["BeginAmount"] = p.Sum(a => decimal.Parse(a["BeginAmount"].ToString(), CultureInfo.InvariantCulture));
                                                jsonItem["IncurredTotalCost"] = p.Sum(a => decimal.Parse(a["IncurredTotalCost"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                jsonItem["TotalZ"] = p.Sum(a => decimal.Parse(a["TotalZ"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                jsonItem["Quantity"] = p.Sum(a => decimal.Parse(a["Quantity"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                jsonItem["EndAmount"] = p.Sum(a => decimal.Parse(a["EndAmount"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                foreach (var fieldName in lstSumField)
                                                {
                                                    jsonItem[fieldName] = p.Sum(a => decimal.Parse(a[fieldName].ToString(), CultureInfo.InvariantCulture));
                                                }
                                                return jsonItem;
                                            }).ToList());
            jsonData.AddRange(jsonData.Where(p => p["Bold"].ToString() == "K")
                                      .GroupBy(g => new 
                                                  { 
                                                    WorkPlaceCode = g["WorkPlaceCode"].ToString(), 
                                                    ProductionPeriodCode = g["ProductionPeriodCode"].ToString() 
                                                  })
                                      .Select(p =>
                                        {
                                            var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(new ProductionCostReportDataDto());
                                            var productionPeriodName = lstProductionPeriod.Where(a => a.Code == p.Key.ProductionPeriodCode).FirstOrDefault();
                                            var jsonItem = (JsonObject)JsonNode.Parse(strJson);
                                            jsonItem["Sort"] = "B";
                                            jsonItem["Bold"] = "C";
                                            jsonItem["Rank"] = 1;
                                            jsonItem["OrgCode"] = _webHelper.GetCurrentOrgUnit();
                                            jsonItem["ProductCode0"] = "";
                                            jsonItem["ProductCode"] = p.Key.ProductionPeriodCode;
                                            jsonItem["ProductName"] = productionPeriodName?.Name ?? "";
                                            jsonItem["WorkPlaceCode"] = p.Key.WorkPlaceCode;
                                            jsonItem["ProductionPeriodCode"] = p.Key.ProductionPeriodCode;
                                            jsonItem["BeginAmount"] = p.Sum(a => decimal.Parse(a["BeginAmount"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                            jsonItem["IncurredTotalCost"] = p.Sum(a => decimal.Parse(a["IncurredTotalCost"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                            jsonItem["TotalZ"] = p.Sum(a => decimal.Parse(a["TotalZ"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                            jsonItem["Quantity"] = p.Sum(a => decimal.Parse(a["Quantity"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                            jsonItem["EndAmount"] = p.Sum(a => decimal.Parse(a["EndAmount"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                            foreach (var fieldName in lstSumField)
                                            {
                                                jsonItem[fieldName] = p.Sum(a => decimal.Parse((a[fieldName] ?? 0).ToString(), CultureInfo.InvariantCulture));
                                            }
                                            return jsonItem;
                                        }).ToList());
            jsonData.AddRange(jsonData.Where(p => p["Bold"].ToString() == "K").GroupBy(g => new { OrgCode = g["OrgCode"].ToString() })
                                            .Select(p =>
                                            {
                                                var strJson = Newtonsoft.Json.JsonConvert.SerializeObject(new ProductionCostReportDataDto());
                                                var jsonItem = (JsonObject)JsonNode.Parse(strJson);
                                                jsonItem["Sort"] = "A";
                                                jsonItem["Bold"] = "C";
                                                jsonItem["Rank"] = 0;
                                                jsonItem["OrgCode"] = _webHelper.GetCurrentOrgUnit();
                                                jsonItem["ProductCode0"] = "";
                                                jsonItem["ProductName"] = "Tổng cộng";
                                                jsonItem["BeginAmount"] = p.Sum(a => decimal.Parse(a["BeginAmount"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                jsonItem["IncurredTotalCost"] = p.Sum(a => decimal.Parse(a["IncurredTotalCost"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                jsonItem["TotalZ"] = p.Sum(a => decimal.Parse(a["TotalZ"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                jsonItem["Quantity"] = p.Sum(a => decimal.Parse(a["Quantity"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                jsonItem["EndAmount"] = p.Sum(a => decimal.Parse(a["EndAmount"].ToString() ?? "0", CultureInfo.InvariantCulture));
                                                foreach (var fieldName in lstSumField)
                                                {
                                                    jsonItem[fieldName] = p.Sum(a => decimal.Parse((a[fieldName] ?? 0).ToString(), CultureInfo.InvariantCulture));
                                                }
                                                return jsonItem;
                                            }).ToList());

            var reportResponse = new ReportResponseDto<JsonObject>();
            reportResponse.Data = jsonData.OrderBy(p => p["Sort"].ToString()).ThenBy(p => (p["WorkPlaceCode"] ?? "").ToString())
                                          .ThenBy(p => (p["ProductionPeriodCode"] ?? "").ToString()).ThenBy(p => (p["ProductCode0"] ?? "").ToString())
                                          .ThenBy(p => ((p["ProductCode"] ?? "").ToString())).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.ProductionCostReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ProductionCostReportParameterDto> dto)
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
        private List<FormularDto> GetFormular(string formular)
        {
            var lst = new List<FormularDto>();
            formular = formular.Replace(" ", "");
            formular = formular.Replace("+", ",+,");
            formular = formular.Replace("-", ",-,");
            formular = "+," + formular;
            var lstData = formular.Split(',').ToList();
            for (var i = 0; i < lstData.Count; i += 2)
            {
                lst.Add(new FormularDto
                {
                    Code = lstData[i + 1],
                    AccCode = lstData[i + 1],
                    Math = lstData[i],
                });
            }
            return lst;
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
                ((IDictionary<string, object>)exo).Add(setting.Key, setting.Value);
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
