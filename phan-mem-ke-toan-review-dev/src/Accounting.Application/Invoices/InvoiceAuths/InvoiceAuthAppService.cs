using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Excels;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Invoices.InvoiceAuths;
using Accounting.Permissions;
using Accounting.Reports;
using Accounting.Vouchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Accounting.DomainServices.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Volo.Abp.Uow;
using MongoDB.Bson;

namespace Accounting.Invoice.InvoiceAuths
{
    public class InvoiceAuthAppService : AccountingAppService, IInvoiceAuthAppService
    {
        #region Field
        private readonly string URL_LOGIN = "/api/Account/Login";
        private readonly string URL_DELETE = "/api/Invoice68/SaveHoadon78";
        private WebClient _webClient;
        private readonly InvoiceAuthService _invoiceAuthService;
        private readonly InvoiceAuthDetailService _invoiceAuthDetailService;
        private readonly InvoiceSupplierService _invoiceSupplierService;
        private readonly LedgerService _ledgerService;
        private readonly AccPartnerService _accPartnerService;
        private readonly InvoiceBookService _invoiceBookService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailReceiptService _productVoucherDetailReceiptService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly UserService _userService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly ProductService _productService;
        private readonly InvoiceAppService _invoiceAppService;
        private readonly IConfiguration _config;
        private readonly InvoiceService _invoiceService;
     
        #endregion
        #region Ctor
        public InvoiceAuthAppService(InvoiceAuthService invoiceAuthService,
                                InvoiceAuthDetailService invoiceAuthDetailService,
                                InvoiceSupplierService invoiceSupplierService,
                                LedgerService ledgerService,
                                AccPartnerService accPartnerService,
                                InvoiceBookService invoiceBookService,
                                WarehouseBookService warehouseBookService,
                                AccTaxDetailService accTaxDetailService,
                                VoucherExciseTaxService voucherExciseTaxService,
                                ProductVoucherService productVoucherService,
                                ProductVoucherDetailReceiptService productVoucherDetailReceiptService,
                                ProductVoucherDetailService productVoucherDetailService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ExcelService excelService,
                                TenantSettingService tenantSettingService,
                                ProductService productService,
                                InvoiceAppService invoiceAppService,
                                IServiceProvider serviceProvider,
                                InvoiceService invoiceService
                                )
        {
            _invoiceAuthService = invoiceAuthService;
            _invoiceAuthDetailService = invoiceAuthDetailService;
            _invoiceSupplierService = invoiceSupplierService;
            _ledgerService = ledgerService;
            _accPartnerService = accPartnerService;
            _invoiceBookService = invoiceBookService;
            _warehouseBookService = warehouseBookService;
            _accTaxDetailService = accTaxDetailService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _productVoucherService = productVoucherService;
            _productVoucherDetailReceiptService = productVoucherDetailReceiptService;
            _productVoucherDetailService = productVoucherDetailService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _tenantSettingService = tenantSettingService;
            _productService = productService;
            _invoiceAppService = invoiceAppService;
            this._webClient = new WebClient();
            _webClient.Encoding = Encoding.UTF8;
            _invoiceService = invoiceService;
            _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        }
        #endregion

        public async Task<InvoiceAuthDto> CreateAsync(CrudInvoiceAuthDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudInvoiceAuthDto, InvoiceAuth>(dto);
            var result = await _invoiceAuthService.CreateAsync(entity);
            throw new Exception("Không thể thực hiện thêm mới hóa đơn.\n Vui lòng Tạo hóa đơn điện tử từ các Phiếu bán hàng hoặc Hóa đơn bán hàng/dịch vụ trên hệ thống. Xin cảm ơn!");
            return ObjectMapper.Map<InvoiceAuth, InvoiceAuthDto>(result);
        }

        public async Task<JsonObject> DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var queryable = await _invoiceAuthService.GetQueryableAsync();

            var item = queryable.Where(auth => auth.Id == id).FirstOrDefault();
            // string result = "";
            JsonObject response = new JsonObject();
            if (item != null)
            {
               
                    string InvoiceID = item.InvoiceId.ToString();
                    string ID = item.InvoiceCodeId.ToString();
                    WebClient webClient = new WebClient();
                    var model = new JObject();
                    JObject data = new JObject();
                    
                    data.Add("hdon_id", InvoiceID);
                    data.Add("cctbao_id", ID);
                    JArray array = new JArray();
                    array.Add(data);
                    var test = JArray.Parse(array.ToString());
                    JsonObject node = new JsonObject();
                    var jsonObject = new Dictionary<string, object>();
                    model.Add("editmode",3);
                    model.Add("data", array);
                    var invoice = await _invoiceAppService.Delete((JsonObject)JsonObject.Parse(model.ToString()));
                    
                   JArray jArrayjArray = new JArray();
                    
                var value = invoice["ok"];
                if ( value != null)
                {
                    await _invoiceAuthService.DeleteAsync(id);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Add("message", "Thành công!");
                    // result = "Thành công!";
                }
                else
                {
                    if (item.Status == "Chờ ký" && item.InvoiceId != null)
                    {
                        jArrayjArray.Add(item.InvoiceId);
                        //result = "Không thể xóa hóa đơn đã phát hành!";
                        throw new Exception("Không thể xóa hóa đơn đã phát hành!");
                    }
                    else
                    {
                        await _invoiceAuthService.DeleteAsync(id);
                        await CurrentUnitOfWork.SaveChangesAsync();
                        response.Add("message", "Thành công!");
                        // result = "thành công!";
                    }
                }
            }
            return response;
        }

        public async Task<string> Login(string link, string username, string password)
        {
            string url = link + URL_LOGIN;

            var model = new JsonObject();
            model.Add("username", username);
            model.Add("password", password);
           
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            try
            {
                var response =  webClient.UploadString(url, "POST", model.ToString());
                var tokens = JObject.Parse(response);
                string token = tokens["token"].ToString();
                string dvcs = tokens["ma_dvcs"].ToString();
                return "Bear " + token + ";" + dvcs;
            }
            catch (Exception e)
            {
                throw new Exception(url);
            }
           
        }
        


        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }

        public async Task DeleteListByProductVoucherIdAsync(List<string> productVoucherId)
        {
            await _licenseBusiness.CheckExpired();
            var invoiceAuthDetail = await _invoiceAuthDetailService.GetQueryableAsync();
            var lstInvoiceAuthDetail = invoiceAuthDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && productVoucherId.Contains(p.InvoiceAuthId));
            await _invoiceAuthService.DeleteManyAsync(productVoucherId, true);
            await _invoiceAuthDetailService.DeleteManyAsync(lstInvoiceAuthDetail, true);
        }

        public Task<PageResultDto<InvoiceAuthDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        public async Task<PageResultDto<InvoiceAuthDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<InvoiceAuthDto>();
            var query = await Filter(dto);
            var querysort = query.OrderByDescending(p => p.IssuedDate).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<InvoiceAuth, InvoiceAuthDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<InvoiceAuthDetailDto>> GetListInvoiceAuthDetailAsync(string invoiceAuthId)
        {
            var invoiceAuthDetails = await _invoiceAuthDetailService.GetByInvoiceAuthIdAsync(invoiceAuthId);
            var dtos = invoiceAuthDetails.Select(p => ObjectMapper.Map<InvoiceAuthDetail, InvoiceAuthDetailDto>(p)).ToList();
            return dtos;
        }

        public async Task UpdateAsync(string id, CrudInvoiceAuthDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _invoiceAuthService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                var invoiceAuthDetails = await _invoiceAuthDetailService.GetByInvoiceAuthIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (invoiceAuthDetails != null)
                {
                    await _invoiceAuthDetailService.DeleteManyAsync(invoiceAuthDetails);
                }
                await _invoiceAuthService.UpdateAsync(entity);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public async Task<InvoiceAuthDto> GetByIdAsync(string invoiceAuthId)
        {
            var invoiceAuth = await _invoiceAuthService.GetAsync(invoiceAuthId);
            return ObjectMapper.Map<InvoiceAuth, InvoiceAuthDto>(invoiceAuth);
        }

        public async Task<ResultDto> VoucherCreateInvoiceAsync(CreateInvoiceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var invoiceAuth = await _invoiceAuthService.GetQueryableAsync();
            var lstInvoiceAuth = invoiceAuth.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var invoiceSupplier = await _invoiceSupplierService.GetSupplierActive();
            var accPartner = await _accPartnerService.GetQueryableAsync();
            var lstAccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var accTaxDetail = await _accTaxDetailService.GetQueryableAsync();
            var lstAccTaxDetail = accTaxDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && dto.ListId.Contains(p.ProductVoucherId));
            var invoiceBook = await _invoiceBookService.GetQueryableAsync();
            var lstInvoiceBook = invoiceBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucer = await _productVoucherService.GetQueryableAsync();
            var lstProductVoucer = productVoucer.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && dto.ListId.Contains(p.Id));
            var productVoucerDetail = await _productVoucherDetailService.GetQueryableAsync();
            var lstProductVoucerDetail = productVoucerDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && dto.ListId.Contains(p.ProductVoucherId));
            var productVoucherDetailReceipt = await _productVoucherDetailReceiptService.GetQueryableAsync();
            var lstProductVoucherDetailReceipt = productVoucherDetailReceipt.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && lstProductVoucerDetail.Select(p => p.Id).Contains(p.ProductVoucherDetailId));

            // xóa hóa đơn cũ
            await DeleteListByProductVoucherIdAsync(dto.ListId);
            var dataInvoiceAuth = (from a in lstProductVoucer
                                   join b in lstAccPartner on a.PartnerCode0 equals b.Code into ajb
                                   from b in ajb.DefaultIfEmpty()
                                   join c in lstAccTaxDetail on a.Id equals c.ProductVoucherId into ajc
                                   from c in ajc.DefaultIfEmpty()
                                   join d in lstInvoiceBook on new { InvoiceBookCode = (c == null ? "" : c.InvoiceBookCode) } equals new { InvoiceBookCode = d.Code } into cjd
                                   from d in cjd.DefaultIfEmpty()
                                   where ((c == null ? "" : c.InvoiceBookCode) == "" || c.Ord0.Substring(c.Ord0.Length - 1, 1) == "1")
                                   select new CrudInvoiceAuthDto
                                   {
                                       Id = a.Id,
                                       IssuedDate = a.VoucherDate,
                                       CurrencyCode = a.CurrencyCode,
                                       ExchangeRate = a.ExchangeRate,
                                       InvoiceTemplate = d == null ? "" : d.InvoiceTemplate ?? "",
                                       InvoiceSeries = d == null ? "" : d.InvoiceSerial ?? "",
                                       AdjustmentType = 1,
                                       BuyerDisplayName = a.Representative,
                                       BuyerLegalName = c == null ? b.Name : c.PartnerName ?? "",
                                       BuyerTaxCode = c == null ? b.TaxCode : c.TaxCode ?? "",
                                       BuyerAddressLine = c == null ? b.Address : c.Address ?? "",
                                       BuyerEmail = b == null ? b.Email : b.Email ?? "",
                                       PaymentMethodName = "TM/CK",
                                       DiscountAmount = (a.ExchangeRate != 1 ? a.TotalDiscountAmountCur ?? 0 : a.TotalDiscountAmount ?? 0),
                                       Status = dto.Status,
                                       OrgCode = _webHelper.GetCurrentOrgUnit(),
                                       VoucherCode = a.VoucherCode,
                                       TotalAmountWithoutVat = (a.ExchangeRate != 1 ? a.TotalAmountWithoutVatCur ?? 0 : a.TotalAmountWithoutVat ?? 0),
                                       VatAmount = (a.ExchangeRate != 1 ? a.TotalVatAmountCur ?? 0 : a.TotalVatAmount ?? 0),
                                       TotalAmount = (a.ExchangeRate != 1 ? a.TotalAmountCur ?? 0 : a.TotalAmount ?? 0),
                                       SupplierCode = invoiceSupplier.Code,
                                       InvoiceNumber = c == null ? null : c.InvoiceNumber,
                                       VatPercentage = c == null ? 0 : c.VatPercentage ?? 0,
                                       OtherDepartment = a.OtherDepartment,
                                       BusinessCode = a.BusinessCode,
                                       CommandNumber = a.CommandNumber,
                                       Representative = a.Representative,
                                       DepartmentCode = a.DepartmentCode,
                                       PartnerName = a.PartnerName0,
                                       PartnerCode = a.PartnerCode0,
                                       SellerAddressLine = a.Address,
                                       Note = a.Description,
                                       Year = a.Year,
                                       ImportDate = a.ImportDate,
                                       ExportDate = a.ExportDate
                                   }).ToList();
            var dataInvoiceAuthDetail = (from a in lstProductVoucerDetail
                                         join b in lstProductVoucherDetailReceipt on a.Id equals b.ProductVoucherDetailId
                                         join c in lstProductVoucer on a.ProductVoucherId equals c.Id
                                         select new CrudInvoiceAuthDetailDto
                                         {
                                             Id = a.Id,
                                             InvoiceAuthId = a.ProductVoucherId,
                                             Ord0 = a.Ord0,
                                             ItemCode = a.ProductCode,
                                             ItemName = a.ProductName ?? a.ProductName0,
                                             UnitCode = a.UnitCode,
                                             UnitName = a.UnitCode,
                                             Price = (c.ExchangeRate != 1 ? (dto.Type == 1 ? a.PriceCur2 : a.PriceCur) : (dto.Type == 1 ? a.Price2 : a.Price)),
                                             Quantity = a.Quantity,
                                             TotalAmountWithoutVat = (c.ExchangeRate != 1 ? (dto.Type == 1 ? a.AmountCur2 : a.AmountCur) : (dto.Type == 1 ? a.Amount2 : a.Amount)),
                                             DiscountPercentage = b.DiscountPercentage,
                                             DiscountAmount = (c.ExchangeRate != 1 ? b.DiscountAmountCur : b.DiscountAmount),
                                             TaxCategoryCode = a.TaxCategoryCode,
                                             VatPercentage = b.VatPercentage,
                                             VatAmount = (c.ExchangeRate != 1 ? b.VatAmountCur : b.VatAmount),
                                             TotalAmount = (c.ExchangeRate != 1 ? (dto.Type == 1 ? a.AmountCur2 : a.AmountCur) - b.DiscountAmountCur + b.VatAmountCur : (dto.Type == 1 ? a.Amount2 : a.Amount) - b.DiscountAmount + b.VatAmount),
                                             OrgCode = a.OrgCode,
                                             Promotion = false,
                                             WarehouseCode = a.WarehouseCode,
                                             DecreaseAmount = a.DecreaseAmount,
                                             DecreasePercentage = a.DecreasePercentage,
                                         }).ToList();
            var lstInvoiceAuthAdd = dataInvoiceAuth.Select(p => ObjectMapper.Map<CrudInvoiceAuthDto, InvoiceAuth>(p)).ToList();
            var lstInvoiceAuthDetailAdd = dataInvoiceAuthDetail.Select(p => ObjectMapper.Map<CrudInvoiceAuthDetailDto, InvoiceAuthDetail>(p)).ToList();
            foreach (var item in lstInvoiceAuthAdd)
            {
                item.AmountByWord = GetAmountToWord(item.TotalAmount ?? 0);
            }
            await _invoiceAuthService.CreateManyAsync(lstInvoiceAuthAdd, true);
            await _invoiceAuthDetailService.CreateManyAsync(lstInvoiceAuthDetailAdd, true);
            var res = new ResultDto();
            res.Ok = true;
            return res;
        }

        #region Private
        private decimal GetVoucherNumber(string VoucherNumber)
        {
            string[] numbers = Regex.Split(VoucherNumber, @"\D+");
            if (string.IsNullOrEmpty(VoucherNumber) == false)
            {
                return decimal.Parse(string.Join("", numbers));
            }
            else
            {
                return 0;
            }
        }

        private async Task<IQueryable<InvoiceAuth>> Filter(PageRequestDto dto)
        {
            var queryable = await _invoiceAuthService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            queryable = await this.GetFilterRows(queryable, dto);
            queryable = await this.GetFilterAdvanced(queryable, dto);
            return queryable;
        }
        private async Task<IQueryable<InvoiceAuth>> GetFilterAdvanced(IQueryable<InvoiceAuth> queryable, PageRequestDto dto)
        {
            if (dto.FilterAdvanced == null) return queryable;
            string orgCode = _webHelper.GetCurrentOrgUnit();

            bool hasJoinPartner = false,
                 hasJoinProduct = false;

            var queryablePartner = await _accPartnerService.GetQueryableAsync();
            queryablePartner = queryablePartner.Where(p => p.OrgCode.Equals(orgCode));
            var queryableProduct = await _productService.GetQueryableAsync();
            queryableProduct = queryableProduct.Where(p => p.OrgCode.Equals(orgCode));
            var queryableInvoiceDetail = await _invoiceAuthDetailService.GetQueryableAsync();
            queryableInvoiceDetail = queryableInvoiceDetail.Where(p => p.OrgCode == orgCode);

            foreach (var item in dto.FilterAdvanced)
            {
                if (item.Value == null) continue;
                if (string.IsNullOrEmpty(item.Value.ToString())) continue;

                if (item.ColumnName.Equals("fromDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime fromDate = Convert.ToDateTime(item.Value.ToString());
                    queryable = queryable.Where(p => p.IssuedDate >= fromDate);
                }
                if (item.ColumnName.Equals("toDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime toDate = Convert.ToDateTime(item.Value.ToString());
                    queryable = queryable.Where(p => p.IssuedDate <= toDate);
                }
                if (item.ColumnName.Equals("partnerGroupId"))
                {
                    hasJoinPartner = true;
                    queryablePartner = queryablePartner.Where(p => p.PartnerGroupId.Equals(item.Value.ToString()));
                    //var test = queryablePartner.ToList();
                }
                if (item.ColumnName.Equals("productGroupId"))
                {
                    hasJoinProduct = true;
                    queryableProduct = queryableProduct.Where(p => p.ProductGroupId.Equals(item.Value.ToString()));
                }

                if (item.ColumnName.Equals("beginVoucherNumber"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = queryable.Where(p => string.IsNullOrEmpty(p.InvoiceNumber) == false);
                        foreach (var items in queryable.OrderBy(p => p.InvoiceNumber))
                        {
                            items.InvoiceNumber = GetVoucherNumber(items.InvoiceNumber).ToString();

                        }
                        var para = GetVoucherNumber(item.Value.ToString());

                        try
                        {
                            queryable = queryable.Where(p => Convert.ToDecimal(p.InvoiceNumber) >= para);
                        }
                        catch (Exception ex)
                        {

                        }



                    }

                }
                if (item.ColumnName.Equals("endVoucherNumber"))
                {
                    queryable = queryable.Where(p => string.IsNullOrEmpty(p.InvoiceNumber) == false);
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        foreach (var items in queryable.OrderBy(p => p.InvoiceNumber))
                        {
                            items.InvoiceNumber = GetVoucherNumber(items.InvoiceNumber).ToString();

                        }
                        var para = GetVoucherNumber(item.Value.ToString());
                        queryable = queryable.Where(p => Convert.ToDecimal(p.InvoiceNumber) <= para);

                    }
                }
                if (item.ColumnName.Equals("beginAmount"))
                {
                    if (decimal.Parse(item.Value.ToString()) != 0)
                    {
                        queryable = queryable.Where(p => p.TotalAmount >= decimal.Parse(item.Value.ToString()));
                    }
                }
                if (item.ColumnName.Equals("columnName"))
                {
                    if (decimal.Parse(item.Value.ToString()) != 0)
                    {
                        queryable = queryable.Where(p => p.TotalAmount <= decimal.Parse(item.Value.ToString()));
                    }
                }


                if (item.ColumnName.Equals("partnerCode0"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where c.PartnerCode == item.Value.ToString()
                                    select c;
                    }

                }

                if (item.ColumnName.Equals("currencyCode"))
                {
                    queryable = from c in queryable
                                where c.CurrencyCode == item.Value.ToString()
                                select c;

                }

                if (item.ColumnName.Equals("productCode"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        var result = (from a in queryableInvoiceDetail
                                      where a.ItemCode == item.Value.ToString()
                                      select new InvoiceAuthDetail
                                      {
                                          InvoiceAuthId = a.InvoiceAuthId
                                      }).Distinct();

                        queryable = from c in queryable
                                    join d in result on c.Id equals d.InvoiceAuthId

                                    select c;

                    }
                }
                if (item.ColumnName.Equals("creationTime"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where c.CreationTime == DateTime.Parse(item.Value.ToString())
                                    select c;

                    }
                }

                if (item.ColumnName.Equals("status"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where c.Status == item.Value.ToString()
                                    select c;

                    }
                }


            }

            if (hasJoinPartner == true && hasJoinProduct == true)
            {
                return queryable;
            }
            else
            {
                if (hasJoinPartner)
                {
                    var test = queryable.ToList();
                    var queryablePartners = queryablePartner.ToList();
                    return from c in queryable
                           join d in queryablePartner on c.PartnerCode equals d.Code
                           select c;
                }
                if (hasJoinProduct)
                {
                    return from c in queryable
                           join d in queryableInvoiceDetail on c.Id equals d.InvoiceAuthId
                           join e in queryableProduct on d.ItemCode equals e.Code
                           select c;
                }
                return queryable;
            }
        }
        private async Task<string> GetSymbolSeparateNumber()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var currencyFormats = await _tenantSettingService.GetNumberSeparateSymbol(orgCode);
            foreach (var item in currencyFormats)
            {
                if (item.Key.Equals(CurrencyConst.SymbolSeparateDecimal)) return item.Value;
            }
            return null;
        }
        private decimal GetNumberDecimal(string value, string tenantDecimalSymbol)
        {
            string decimalSymbol = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            if (decimalSymbol.Equals(tenantDecimalSymbol)) return Convert.ToDecimal(value);
            value = value.Replace(tenantDecimalSymbol, decimalSymbol);
            return Convert.ToDecimal(value);
        }
        private async Task<IQueryable<InvoiceAuth>> GetFilterRows(IQueryable<InvoiceAuth> queryable, PageRequestDto dto)
        {
            if (dto.FilterRows == null) return queryable;
            string tenantDecimalSymbol = await this.GetSymbolSeparateNumber();
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnType.Equals(ColumnType.Decimal) || item.ColumnType.Equals(ColumnType.Number))
                {
                    string value = item.Value.ToString();
                    if (value.StartsWith(OperatorFilter.GreaterThan))
                    {
                        value = value.Replace(OperatorFilter.GreaterThan, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.GreaterThan);
                    }
                    else if (value.StartsWith(OperatorFilter.GreaterThanOrEqual))
                    {
                        value = value.Replace(OperatorFilter.GreaterThanOrEqual, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.GreaterThanOrEqual);
                    }
                    else if (value.StartsWith(OperatorFilter.LessThan))
                    {
                        value = value.Replace(OperatorFilter.LessThan, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.LessThan);
                    }
                    else if (value.StartsWith(OperatorFilter.LessThanOrEqual))
                    {
                        value = value.Replace(OperatorFilter.LessThanOrEqual, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.LessThanOrEqual);
                    }
                    else if (value.StartsWith(OperatorFilter.NumEqual))
                    {
                        value = value.Replace(OperatorFilter.NumEqual, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.Equal);
                    }
                }
                else if (item.ColumnType.Equals(ColumnType.Date))
                {
                    var obj = JsonObject.Parse(item.Value.ToString());
                    if (obj["start"] != null)
                    {
                        DateTime value = Convert.ToDateTime(obj["start"].ToString());
                        queryable = queryable.Where(item.ColumnName, value, FilterOperator.GreaterThanOrEqual);
                    }
                    if (obj["end"] != null)
                    {
                        DateTime value = Convert.ToDateTime(obj["end"].ToString());
                        queryable = queryable.Where(item.ColumnName, value, FilterOperator.LessThanOrEqual);
                    }
                }
                else
                {
                    queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
                }
            }
            return queryable;
        }

        private string GetAmountToWord(decimal inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber == "" ? "0" : sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "mốt " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (suffix ? " đồng" : "");
        }

        public async Task<ResultDto> InvoiceAuthUpdate(JsonObject dto)
        {

            await _invoiceAppService.UpdateInvoiceStatus(dto);

            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }





        #endregion
    }
}
