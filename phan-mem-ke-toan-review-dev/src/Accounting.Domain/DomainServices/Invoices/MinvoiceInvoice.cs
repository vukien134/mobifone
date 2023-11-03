using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Windows;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Cmp;
using static NPOI.HSSF.Util.HSSFColor;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Accounting.DomainServices.Invoices
{
    public class MinvoiceInvoice : IEInvoice
    {
        private InvoiceSupplierService _invoiceSupplierService;
        private InvoiceAuthService _invoiceAuthService;
        private readonly AccPartnerService _accPartnerService;
        private readonly string apiLink = "";
        private readonly string codeTax = "";
        private readonly string username = "";
        private readonly string password = "";
        private readonly WebHelper _webHelper;
        private readonly WarehouseService _warehouseService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly IConfiguration _config;
        private readonly DefaultVoucherTypeService _defaultVoucherTypeService;
        private const string URL_INVOICE_TEMPLATE = "/api/Invoice68/GetTypeInvoiceSeries?type";
        private const string URL_LOGIN = "/api/Account/Login";
        private const string URL_SAVE = "/api/InvoiceAPI78/Save";
        private const string TAB_ID = "TAB00188";
        private const string WINDOW_ID = "WIN00187";
        private const string URL_INVOICE_GetInvoices = "/api/InvoiceApi78/GetInvoices";
        public MinvoiceInvoice(JsonObject model, IServiceProvider serviceProvider)
        {
            _invoiceSupplierService = serviceProvider.GetRequiredService<InvoiceSupplierService>();
            _invoiceAuthService = serviceProvider.GetRequiredService<InvoiceAuthService>();
            apiLink = model["apiLink"].ToString();
            username = model["username"].ToString();
            password = model["password"].ToString();
            _accPartnerService = serviceProvider.GetRequiredService<AccPartnerService>();
            _webHelper = serviceProvider.GetRequiredService<WebHelper>();
            _warehouseService = serviceProvider.GetRequiredService<WarehouseService>();
            _voucherTypeService = serviceProvider.GetRequiredService<VoucherTypeService>();
            _config = serviceProvider.GetRequiredService<IConfiguration>();
            _defaultVoucherTypeService = serviceProvider.GetRequiredService<DefaultVoucherTypeService>();
        }

        public async Task<JsonObject> Create(JsonObject dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string partnerCode = dto["PartnerCode"].ToString();
            string voucherCode = dto["VoucherCode"].ToString();
            var model = dto;
            var accPartner = await _accPartnerService.GetAccPartnerByCodeAsync(partnerCode, orgCode);
            var email = accPartner?.Email ?? "";
            string vatPercentage = (model["VatPercentage"] ?? "").ToString();

            var vatPercentages = vatPercentage.Split('.');
            var invoiceNumber = (model["InvoiceNumber"] ?? "").ToString();

            var voucherType = await _voucherTypeService.GetByCodeAsync("000");
            var lstDefaultVoucherType = await _defaultVoucherTypeService.GetQueryableAsync();
            string voucherCode2 = "";
            if (voucherType != null)
            {
                voucherCode2 = voucherType?.ListVoucher ?? "";
            }
            else
            {
                voucherCode2 = lstDefaultVoucherType.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;

            }

            var priceVoucher = voucherCode2.Contains(voucherCode) ? 1 : 0;
            var idMobi = JsonObject.Parse(dto["idMobi"].ToString());

            var wareHouse = await _warehouseService.GetQueryableAsync();
            var lstWareHouse = await _warehouseService.GetListAsync(orgCode);

            string token = await this.Login();
            using var client = this.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                                                    ContentTypeConst.ApplicationJson);

            var lstproductVoucherDetails = (JsonArray)dto["InvoiceAuthDetails"];

            var wareHouseImportCode = "";
            var wareHouseExportCode = "";
            var wareHouseImport = "";
            var wareHouseExport = "";

            if (!string.IsNullOrEmpty(wareHouseImportCode))
            {
                wareHouseImport = lstWareHouse.Where(p => p.Code == wareHouseImportCode).ToList().FirstOrDefault().Address;
            }

            if (!string.IsNullOrEmpty(wareHouseExportCode))
            {
                wareHouseExport = lstWareHouse.Where(p => p.Code == wareHouseExportCode).ToList().FirstOrDefault().Address;
            }


            var jsons = new JsonObject();

            DateTime date = DateTime.Parse(model["IssuedDate"].ToString());
            string voucherDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            jsons.Add("inv_invoiceNumber", model["BuyerTaxCode"].ToString());
            jsons.Add("inv_invoiceSeries", idMobi["InvoiceSerial"].ToString());
            jsons.Add("inv_invoiceIssuedDate", voucherDate);
            //jsons.Add("so_benh_an", model["InvoiceNumber"].ToString());
            jsons.Add("inv_currencyCode", model["CurrencyCode"].ToString());
            jsons.Add("inv_exchangeRate", model["ExchangeRate"].ToString());
            jsons.Add("inv_buyerDisplayName", model["BuyerDisplayName"].ToString() ?? null);
            jsons.Add("ma_dt", model["PartnerCode"].ToString() ?? null);
            jsons.Add("inv_buyerLegalName", model["BuyerLegalName"].ToString());
            jsons.Add("inv_buyerTaxCode", model["BuyerTaxCode"].ToString());
            jsons.Add("inv_buyerAddressLine", model["BuyerAddressLine"].ToString() ?? null);
            jsons.Add("inv_buyerEmail", email ?? null);
            jsons.Add("inv_paymentMethodName", "TM/CK");
            //jsons.Add("ma_bp", model["DepartmentCode"].ToString());
            jsons.Add("inv_TotalAmountWithoutVat", model["TotalAmountWithoutVat"].ToString() ?? null);
            jsons.Add("inv_vatAmount", model["VatAmount"].ToString() ?? null);
            jsons.Add("inv_discountAmount", model["DiscountAmount"].ToString() ?? null);
            jsons.Add("inv_TotalAmount", model["TotalAmount"].ToString() ?? null);
            jsons.Add("so_hop_dong", model["VoucherCode"].ToString() == "PDC" ? model["VoucherNumber"].ToString() : null);
            // json.Add("inv_TotalAmount", model["totalAmount"].ToString() ?? null);
            //if (!string.IsNullOrEmpty(model["ImportDate"].ToString()))
            //{
            //    jsons.Add("ngay_nhap", model["ImportDate"].ToString() ?? null);
            //}
            //if (!string.IsNullOrEmpty(model["ExportDate"].ToString()))
            //{
            //    jsons.Add("ngay_xuat", model["ExportDate"].ToString() ?? null);
            //}

            //jsons.Add("phuong_tien", model["Vehicle"].ToString() ?? null);
            jsons.Add("nguoi_van_chuyen", voucherCode == "PDC" ? model["Representative"].ToString() : null);
            jsons.Add("kho_nhap", wareHouseImport);
            jsons.Add("kho_xuat", wareHouseExport);
            // jsons.Add("phong_ban", model["OtherDepartment"].ToString() ?? null);
            //jsons.Add("ve_viec", model["BusinessCode"].ToString() ?? null);
            //jsons.Add("lenh_dieu_dong", model["CommandNumber"].ToString() ?? null);
            jsons.Add("ngay_ct", voucherDate);

            var arr = new JsonArray();
            var arrDetails = new JsonArray();
            var objDetail = new JsonObject();

            var arrDataDetail = new JsonArray();
            //lstproductVoucherDetails.OrderBy(p => p.[""]).ToList();

            foreach (var item in lstproductVoucherDetails)
            {

                string ord0 = item["Ord0"].ToString().Substring(1, 9).TrimStart(new Char[] { '0' });
                var objDetails = new JsonObject();
                objDetails.Add("tchat", 1);
                objDetails.Add("stt_rec0", ord0);
                objDetails.Add("inv_itemCode", item["ItemCode"].ToString());
                objDetails.Add("inv_itemName", model["VoucherCode"].ToString() == "PDV" ? item["ItemName"].ToString() : item["ItemName"].ToString());
                objDetails.Add("inv_unitCode", item["UnitCode"].ToString());
                objDetails.Add("inv_quantity", item["Quantity"].ToString());
                objDetails.Add("inv_unitPrice", model["CurrencyCode"].ToString() == "VND" ? (priceVoucher == 1 ? item["Price"].ToString() : item["Price"].ToString()) : item["Price"].ToString());
                objDetails.Add("inv_discountPercentage", item["DiscountPercentage"].ToString());
                objDetails.Add("inv_discountAmount", item["DiscountAmount"].ToString());
                objDetails.Add("inv_TotalAmountWithoutVat", model["CurrencyCode"].ToString() == "VND" ? (priceVoucher == 1 ? item["TotalAmountWithoutVat"].ToString() : item["TotalAmountWithoutVat"].ToString()) : item["amount"].ToString());
                objDetails.Add("ma_thue", vatPercentages[0]);
                objDetails.Add("inv_vatAmount", item["VatAmount"].ToString());
                objDetails.Add("inv_TotalAmount", item["TotalAmount"].ToString());

                arrDataDetail.Add(objDetails);

            }

            var obj = new JsonObject();
            obj.Add("tab_id", TAB_ID);
            obj.Add("data", arrDataDetail);
            var objResul = new JsonObject();
            arrDetails.Add(obj);
            jsons.Add("details", arrDetails);
            objResul.Add("windowid", WINDOW_ID);


            int editmode = string.IsNullOrEmpty(invoiceNumber) == true ? 1 : 2;
            objResul.Add("editmode", editmode);

            arr.Add(jsons);
            objResul.Add("data", arr);

            var bodyContent = new StringContent(objResul.ToString(), Encoding.UTF8, ContentTypeConst.ApplicationJson);
            var respMessage = await client.PostAsync(apiLink + URL_SAVE, bodyContent);
            respMessage.EnsureSuccessStatusCode();
            string content = await respMessage.Content.ReadAsStringAsync();
            var result = JsonNode.Parse(content);
            JsonObject keyValuePairs = new JsonObject();
            if (string.IsNullOrEmpty((result["error"] ?? "").ToString()) == true)
            {
                await this.UpdateInvoiceNumber(result, model["Id"].ToString());
                keyValuePairs.Add("json", objResul);
                keyValuePairs.Add("result", result);
            }
            else
            {
                keyValuePairs.Add("json", objResul);
                keyValuePairs.Add("result", result);
            }



            return keyValuePairs;
        }
        public async Task<JsonObject> CreateHKD(JsonObject dto)
        {

            var model = JObject.Parse(dto.ToString());
            var accPartner = await _accPartnerService.GetQueryableAsync();
            var lstAccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == model["PartnerCode"].ToString()).ToList().FirstOrDefault();
            var email = "";
            if (lstAccPartner != null)
            {
                email = lstAccPartner.Email;
            }
            var lstVoucherType = await _voucherTypeService.GetQueryableAsync();
            var voucherCode2 = lstVoucherType.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            var priceVoucher = 0;
            if (voucherCode2.Contains(model["VoucherCode"].ToString()) == true)
            {
                priceVoucher = 1;
            }


            var wareHouse = await _warehouseService.GetQueryableAsync();
            var lstWareHouse = wareHouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();



            var productVoucherDetails = model["InvoiceAuthDetails"].ToString();
            var lstproductVoucherDetails = JArray.Parse(productVoucherDetails);

            var wareHouseImportCode = "";
            var wareHouseExportCode = "";
            var wareHouseImport = "";
            var wareHouseExport = "";

            string token = await this.Login();
            using var client = this.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                                                    ContentTypeConst.ApplicationJson);

            if (!string.IsNullOrEmpty(wareHouseImportCode))
            {
                wareHouseImport = lstWareHouse.Where(p => p.Code == wareHouseImportCode).ToList().FirstOrDefault().Address;
            }

            if (!string.IsNullOrEmpty(wareHouseExportCode))
            {
                wareHouseExport = lstWareHouse.Where(p => p.Code == wareHouseExportCode).ToList().FirstOrDefault().Address;
            }
            var idMobi = JsonObject.Parse(dto["idMobi"].ToString());


            JObject jsons = new JObject();

            DateTime date = DateTime.Parse(model["IssuedDate"].ToString());
            string voucherDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            jsons.Add("inv_invoiceNumber", model["BuyerTaxCode"].ToString());
            jsons.Add("inv_invoiceSeries", idMobi["InvoiceSerial"].ToString());
            jsons.Add("inv_invoiceIssuedDate", voucherDate);
            //jsons.Add("so_benh_an", model["InvoiceNumber"].ToString());
            jsons.Add("inv_currencyCode", model["CurrencyCode"].ToString());
            jsons.Add("inv_exchangeRate", model["ExchangeRate"].ToString());
            jsons.Add("inv_buyerDisplayName", model["BuyerDisplayName"].ToString() ?? null);
            jsons.Add("ma_dt", model["PartnerCode"].ToString() ?? null);
            jsons.Add("inv_buyerLegalName", model["BuyerLegalName"].ToString());
            jsons.Add("inv_buyerTaxCode", model["BuyerTaxCode"].ToString());
            jsons.Add("inv_buyerAddressLine", model["BuyerAddressLine"].ToString() ?? null);
            jsons.Add("inv_buyerEmail", email ?? null);
            jsons.Add("inv_paymentMethodName", "TM/CK");
            //jsons.Add("ma_bp", model["DepartmentCode"].ToString());
            jsons.Add("inv_TotalAmountWithoutVat", model["TotalAmountWithoutVat"].ToString() ?? null);
            jsons.Add("inv_vatAmount", model["VatAmount"].ToString() ?? null);
            jsons.Add("inv_discountAmount", model["DiscountAmount"].ToString() ?? null);
            jsons.Add("inv_TotalAmount", model["TotalAmount"].ToString() ?? null);
            jsons.Add("so_hop_dong", model["VoucherCode"].ToString() == "PDC" ? model["VoucherNumber"].ToString() : null);


            jsons.Add("ngay_ct", voucherDate);
            JArray arr = new JArray();
            JArray arrDetails = new JArray();
            JObject objDetail = new JObject();

            JArray arrDataDetail = new JArray();
            // lstproductVoucherDetails.OrderBy(p => p["Ord0"]).ToList();

            decimal? amountDiscount = 0;
            foreach (var item in lstproductVoucherDetails)
            {
                amountDiscount += decimal.Parse(item["DecreaseAmount"].ToString());
            }
            if (amountDiscount > 0)
            {
                foreach (var item in lstproductVoucherDetails)
                {
                    string ord0 = item["Ord0"].ToString().Substring(1, 9).TrimStart(new Char[] { '0' });

                    JObject objDetails = new JObject();
                    objDetails.Add("tchat", 4);
                    objDetails.Add("stt_rec0", ord0);
                    objDetails.Add("inv_itemCode", item["ItemCode"].ToString());
                    objDetails.Add("inv_itemName", "Giảm " + amountDiscount + " tương ứng " + item["DecreasePercentage"].ToString() + "% mức tỉ lệ % để tính thuế giá trị gia tăng theo nghị  quyết số 43/2022/QH15");
                    objDetails.Add("inv_unitCode", null);
                    objDetails.Add("inv_quantity", 0);
                    objDetails.Add("inv_unitPrice", 0);
                    objDetails.Add("inv_discountPercentage", null);
                    objDetails.Add("inv_discountAmount", 0);
                    objDetails.Add("inv_TotalAmountWithoutVat", -amountDiscount);
                    objDetails.Add("ma_thue", null);
                    objDetails.Add("inv_vatAmount", 0);
                    objDetails.Add("inv_TotalAmount", -amountDiscount);

                    arrDataDetail.Add(objDetails);

                }
            }
            else
            {
                foreach (var item in lstproductVoucherDetails)
                {
                    string ord0 = item["Ord0"].ToString().Substring(1, 9).TrimStart(new Char[] { '0' });

                    JObject objDetails = new JObject();
                    objDetails.Add("tchat", 1);
                    objDetails.Add("stt_rec0", ord0);
                    objDetails.Add("inv_itemCode", item["ItemCode"].ToString());
                    objDetails.Add("inv_itemName", model["VoucherCode"].ToString() == "PDV" ? item["ItemName"].ToString() : item["ItemName"].ToString());
                    objDetails.Add("inv_unitCode", item["UnitCode"].ToString());
                    objDetails.Add("inv_quantity", decimal.Parse(item["Quantity"].ToString()));
                    objDetails.Add("inv_unitPrice", decimal.Parse(model["CurrencyCode"].ToString() == "VND" ? (priceVoucher == 1 ? item["Price"].ToString() : item["Price"].ToString()) : item["Price"].ToString()));
                    objDetails.Add("inv_discountPercentage", item["DiscountPercentage"].ToString());
                    objDetails.Add("inv_discountAmount", decimal.Parse(item["DiscountAmount"].ToString()));
                    objDetails.Add("inv_TotalAmountWithoutVat", decimal.Parse(model["CurrencyCode"].ToString() == "VND" ? (priceVoucher == 1 ? item["TotalAmountWithoutVat"].ToString() : item["TotalAmountWithoutVat"].ToString()) : item["amount"].ToString()));
                    objDetails.Add("ma_thue", item["VatPercentage"].ToString());
                    objDetails.Add("inv_vatAmount", decimal.Parse(item["VatAmount"].ToString()));
                    objDetails.Add("inv_TotalAmount", decimal.Parse(item["TotalAmount"].ToString()));

                    arrDataDetail.Add(objDetails);

                }
            }



            JObject obj = new JObject();
            obj.Add("tab_id", "TAB00188");
            obj.Add("data", arrDataDetail);
            JObject objResul = new JObject();
            arrDetails.Add(obj);
            jsons.Add("details", arrDetails);
            objResul.Add("windowid", "WIN00187");
            if (string.IsNullOrEmpty(model["InvoiceNumber"].ToString()))
            {
                objResul.Add("editmode", 1);
            }
            else
            {
                objResul.Add("editmode", 2);
            }

            arr.Add(jsons);
            objResul.Add("data", arr);
            JObject jsonss = JObject.Parse(objResul.ToString());

            jsonss.ToString().Replace(@"\\\", "");
            var a = System.Text.RegularExpressions.Regex.Unescape(jsonss.ToString());
            string s = Regex.Replace(a, "(#[A-Za-z0-9]+)=\"\"", "\"").Replace("\\n", "").Replace("\\", @" """).Replace("\n", "").Replace("u0022", "");
            var bodyContent = new StringContent(objResul.ToString(), Encoding.UTF8, ContentTypeConst.ApplicationJson);
            var respMessage = await client.PostAsync(apiLink + URL_SAVE, bodyContent);
            respMessage.EnsureSuccessStatusCode();
            string content = await respMessage.Content.ReadAsStringAsync();
            var result = JsonNode.Parse(content);
            JsonObject keyValuePairs = new JsonObject();
            var json = JsonObject.Parse(objResul.ToString());
            if (string.IsNullOrEmpty((result["error"] ?? "").ToString()) == true)
            {
                await this.UpdateInvoiceNumber(result, model["Id"].ToString());
                keyValuePairs.Add("json", json);
                keyValuePairs.Add("result", result);
            }
            else
            {
                keyValuePairs.Add("json", json);
                keyValuePairs.Add("result", result);
            }
            return keyValuePairs;
        }

        public Task<JsonObject> Delete(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonObject> InvoiceTemplate(JsonObject model)
        {
            string token = await this.Login();
            string url = apiLink + URL_INVOICE_TEMPLATE;

            using var client = this.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            var content = await client.GetStringAsync(url);
            var rootNode = JsonNode.Parse(content);

            List<InvoiceDto> invoice = new List<InvoiceDto>();
            if (rootNode["data"] != null)
            {
                var items = (JsonArray)rootNode["data"];
                foreach (var item in items)
                {
                    InvoiceDto invoiceDto = new InvoiceDto();
                    invoiceDto.InvoiceId = item["id"].ToString();
                    invoiceDto.InvoiceSerial = item["khhdon"].ToString();
                    invoiceDto.InvoiceSysbol = null;
                    invoice.Add(invoiceDto);
                }

            }

            JsonObject result = new JsonObject();
            var resulAll = System.Text.Json.JsonSerializer.Serialize(invoice);
            result.Add("Result", resulAll);

            return result;

        }

        public async Task<string> Login()
        {
            string url = apiLink + URL_LOGIN;

            var model = new JsonObject();
            model.Add("username", username);
            model.Add("password", password);

            using var httpClient = this.GetHttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", ContentTypeConst.ApplicationJson);
            var content = new StringContent(model.ToJsonString(), Encoding.UTF8,
                            ContentTypeConst.ApplicationJson);
            var respMessage = await httpClient.PostAsync(url, content);
            respMessage.EnsureSuccessStatusCode();
            var respContent = await respMessage.Content.ReadAsStringAsync();

            var rootNode = JsonNode.Parse(respContent);
            string token = rootNode["token"].GetValue<string>();
            return $"{token};VP";
        }

        public byte[] Preview(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> SaveXmlInvoice(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> Update(JsonObject model)
        {
            throw new NotImplementedException();
        }
        #region Privates        
        private HttpClient GetHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            string proxyAddress = _config.GetValue<string>("Proxy:Proxyaddress");
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                bool useDefaultCredentials = _config.GetValue<bool>("Proxy:Usesystemdefault");
                bool byPassOnLocal = _config.GetValue<bool>("Proxy:Bypassonlocal");
                var proxy = new WebProxy(proxyAddress);
                proxy.BypassProxyOnLocal = byPassOnLocal;
                handler.Proxy = proxy;
                handler.UseDefaultCredentials = useDefaultCredentials;
            }

            var client = new HttpClient(handler: handler, disposeHandler: true);
            return client;
        }
        private async Task UpdateInvoiceNumber(JsonNode result, string id)
        {
            if (result["data"].ToString() == null) return;
            var test = JObject.Parse(result["data"].ToString());
            var dto = new UpdateInvoiceNumberDto();
            dto.InvoiceNumber = test["inv_invoiceNumber"].ToString();
            dto.InvoiceSeries = test["inv_invoiceSeries"].ToString();
            dto.Id = id;
            dto.HDonID = test["inv_invoiceId"].ToString();
            dto.cttb_id = test["inv_invoiceCodeId"].ToString();

            try
            {
                await _invoiceAuthService.UpdateInvoiceNumberAsync(dto);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<JsonObject> UpdateInvoiceStatus(JsonObject model)
        {
            string token = await this.Login();
            using var client = this.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                                                    ContentTypeConst.ApplicationJson);
            var dto = new JsonObject();
            dto.Add("tuNgay", model["fromDate"].ToString());
            dto.Add("denNgay", model["toDate"].ToString());
            dto.Add("khieu", model["InvoiceSerial"].ToString());
            var bodyContent = new StringContent(dto.ToString(), Encoding.UTF8, ContentTypeConst.ApplicationJson);
            var respMessage = await client.PostAsync(apiLink + URL_INVOICE_GetInvoices, bodyContent);
            respMessage.EnsureSuccessStatusCode();
            string content = await respMessage.Content.ReadAsStringAsync();
            var result = JsonObject.Parse(content);
            var invoiceAuth = await _invoiceAuthService.GetQueryableAsync();
            if (result["data"] != null)
            {
                var items = (JsonArray)result["data"];
                foreach (var item in items)
                {

                    if ((item["is_success"] ?? "").ToString() == "1")
                    {
                        var lstInvoice = invoiceAuth.Where(p => p.InvoiceNumber == item["shdon"].ToString() && p.InvoiceSeries == item["inv_invoiceSeries"].ToString());
                        foreach (var invoice in lstInvoice)
                        {
                            invoice.Status = "Đã ký";
                            await _invoiceAuthService.UpdateAsync(invoice);
                        }

                    }

                }

            }
            var resul = new JsonObject();
            resul.Add("err", "");
            resul.Add("Oke", true);
            return resul;

        }


        #endregion
    }
}
