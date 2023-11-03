using Accounting.BaseDtos;
using Accounting.Caching;
using Accounting.Categories.Partners;
using Accounting.Catgories.Partners;
using Accounting.Catgories.Products;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Reports.Others
{
    public class CategoryReportAppService : BaseReportAppService
    {
        #region Fields
        private readonly AccPartnerService _accPartnerService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly ProductService _productService;
        private readonly ProductGroupService _productGroupService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public CategoryReportAppService(WebHelper webHelper,
                    ReportTemplateService reportTemplateService,
                    IWebHostEnvironment hostingEnvironment,
                    YearCategoryService yearCategoryService,
                    TenantSettingService tenantSettingService,
                    OrgUnitService orgUnitService,
                    CircularsService circularsService,
                    IStringLocalizer<AccountingResource> localizer,
                    AccPartnerService accPartnerService,
                    ProductService productService,
                    PartnerGroupService partnerGroupService,
                    ProductGroupService productGroupService,
                    AccountingCacheManager accountingCacheManager
            ) : base(webHelper, reportTemplateService, hostingEnvironment, yearCategoryService,
                            tenantSettingService, orgUnitService, circularsService, localizer)
        {
            _accPartnerService = accPartnerService;
            _productService = productService;
            _partnerGroupService = partnerGroupService;
            _productGroupService = productGroupService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        public async Task<ReportResponseDto<JsonObject>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var data = new List<JsonObject>();
            if (dto.Parameters.Type.Equals("Partner"))
            {
                data = await this.GetPartnersAsync(dto.Parameters);
            }
            if (dto.Parameters.Type.Equals("Product"))
            {
                data = await this.GetProductsAsync(dto.Parameters);
            }

            var response = await this.CreateReportResponseDto<JsonObject>(data, dto);
            return response;
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
        #region Privates
        private async Task<List<JsonObject>> GetPartnersAsync(ReportBaseParameterDto dto)
        {
            var queryablePartnerGroup = await _partnerGroupService.GetQueryableAsync();
            var queryablePartner = await _accPartnerService.GetQueryableAsync();
            queryablePartner = queryablePartner.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));
            if (!string.IsNullOrEmpty(dto.PartnerGroup))
            {
                queryablePartner = queryablePartner.Where(p => p.PartnerGroupId.Equals(dto.PartnerGroup));
            }
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                queryablePartner = queryablePartner.Where(p => p.Code.Equals(dto.PartnerCode));
            }

            var query = from c in queryablePartner
                        join d in queryablePartnerGroup on c.PartnerGroupId equals d.Id into gj
                        from df in gj.DefaultIfEmpty()
                        orderby c.Code
                        select new AccPartnerDto()
                        {
                            Code = c.Code,
                            Name = c.Name,
                            Address = c.Address,
                            Fax = c.Fax,
                            Email = c.Email,
                            Tel = c.Tel,
                            Note = c.Note,
                            OtherContact = c.OtherContact,
                            Representative = c.Representative,
                            TaxCode = c.TaxCode,
                            PartnerGroupCode = df.Code,
                            PartnerGroupName = df.Name
                        };

            var entities = await AsyncExecuter.ToListAsync(query);
            var data = new List<JsonObject>();
            foreach(var item in entities)
            {
                var obj = new JsonObject();

                obj.Add("code", item.Code);
                obj.Add("name", item.Name);
                obj.Add("address", item.Address);
                obj.Add("tel", item.Tel);
                obj.Add("taxCode", item.TaxCode);
                obj.Add("fax", item.Fax);
                obj.Add("email", item.Email);
                obj.Add("note", item.Note);
                obj.Add("otherContact", item.OtherContact);
                obj.Add("representative", item.Representative);
                obj.Add("partnerGroupCode", item.PartnerGroupCode);
                obj.Add("partnerGroupName", item.PartnerGroupName);

                data.Add(obj);
            }

            return data;
        }
        private async Task<List<JsonObject>> GetProductsAsync(ReportBaseParameterDto dto)
        {
            var queryableProductGroup = await _productGroupService.GetQueryableAsync();
            var queryable = await _productService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));
            if (!string.IsNullOrEmpty(dto.ProductGroupId))
            {
                queryable = queryable.Where(p => p.ProductGroupId.Equals(dto.ProductGroupId));
            }
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                queryable = queryable.Where(p => p.Code.Equals(dto.ProductCode));
            }

            var query = from c in queryable
                        join d in queryableProductGroup on c.ProductGroupId equals d.Id into gj
                        from df in gj.DefaultIfEmpty()
                        orderby c.Code
                        select new ProductDto()
                        {
                            Code = c.Code,
                            Name = c.Name,
                            UnitCode = c.UnitCode,
                            Specification = c.Specification,
                            VatPercentage = c.VatPercentage,
                            DiscountAcc = c.DiscountAcc,
                            ProductAcc = c.ProductAcc,
                            ProductCostAcc = c.ProductCostAcc,
                            RevenueAcc = c.RevenueAcc,
                            SaleReturnsAcc = c.SaleReturnsAcc,
                            Note = c.Note,
                            Barcode = c.Barcode,
                            ProductGroupName = df.Name,
                            ProductGroupCode = df.Code,
                            ProductType = c.ProductType
                        };
            var entities = await AsyncExecuter.ToListAsync(query);

            var data = new List<JsonObject>();
            foreach (var item in entities)
            {
                var obj = new JsonObject();

                obj.Add("code", item.Code);
                obj.Add("name", item.Name);
                obj.Add("unitCode", item.UnitCode);
                obj.Add("specification", item.Specification);
                obj.Add("vatPercentage", item.VatPercentage);
                obj.Add("discountAcc", item.DiscountAcc);
                obj.Add("productAcc", item.ProductAcc);
                obj.Add("productCostAcc", item.ProductCostAcc);
                obj.Add("revenueAcc", item.RevenueAcc);
                obj.Add("saleReturnsAcc", item.SaleReturnsAcc);
                obj.Add("productGroupCode", item.ProductGroupCode);
                obj.Add("productGroupName", item.ProductGroupName);
                obj.Add("productType", item.ProductType);
                obj.Add("productTypeName", this.GetProductTypeName(item.ProductType));
                obj.Add("note", item.Note);
                obj.Add("barcode", item.Barcode);

                data.Add(obj);
            }

            return data;
        }
        private string GetProductTypeName(string productType)
        {
            string name = productType switch
            {
                "H" => "Hàng hóa",
                "T" => "Thành phẩm",
                "D" => "Dịch vụ",
                "0" => "Trung gian",
                "V" => "Vật tư",
                _ => null
            };
            return name;
        }
        #endregion
    }
}
