using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Localization;
using Accounting.Windows;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Volo.Abp.Domain.Entities;
using static NPOI.HSSF.UserModel.HeaderFooter;

namespace Accounting.DomainServices.Invoices
{
    public class MobifoneInvoice : IEInvoice
    {
        private InvoiceSupplierService _invoiceSupplierService;
        private InvoiceAuthService _invoiceAuthService;
        private readonly AccPartnerService _accPartnerService;
        private WebClient _webClient;
        private readonly string apiLink = "";
        private readonly string codeTax = "";
        private readonly string username = "";
        private readonly string password = "";
        private readonly string URL_LOGIN = "/api/Account/Login";
        private readonly string URL_INVOICE_TEMPLATE = "/api/System/GetDataReferencesByRefId?refId=RF00059";
        private readonly string URL_SAVE = "/api/Invoice68/SaveListHoadon78";
        private readonly string URL_DELETE = "/api/Invoice68/SaveHoadon78";
        private readonly string URL_INVOICE_GetInvoices = "/api/Invoice68/GetInvoiceFromdateTodate";
        private readonly WebHelper _webHelper;
        private readonly WarehouseService _warehouseService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly DefaultVoucherTypeService _defaultVoucherTypeService;
        public MobifoneInvoice(JsonObject model, IServiceProvider serviceProvider)
        {
            _invoiceSupplierService = serviceProvider.GetRequiredService<InvoiceSupplierService>();
            _invoiceAuthService = serviceProvider.GetRequiredService<InvoiceAuthService>();
            this._webClient = new WebClient();
            _webClient.Encoding = Encoding.UTF8;
            _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
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

            var model = JObject.Parse(dto.ToString());
            var accPartner = await _accPartnerService.GetQueryableAsync();
            var lstAccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == model["PartnerCode"].ToString()).ToList().FirstOrDefault();
            var email = "";
            if (lstAccPartner != null)
            {
                email = lstAccPartner.Email;
            }

            string token = await this.Login();
            string[] dvcs = token.Split(';');
            var idMobi = JsonObject.Parse(model["idMobi"].ToString());
            DateTime date = DateTime.Parse(model["IssuedDate"].ToString());
            string voucherDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            JArray array = new JArray();
            JObject data = new JObject();
            data.Add("cctbao_id", idMobi["InvoiceId"].ToString());
            data.Add("nlap", voucherDate);
            data.Add("sdhang", model["InvoiceNumber"].ToString());
            data.Add("dvtte", model["CurrencyCode"].ToString());
            data.Add("tgia", model["ExchangeRate"].ToString());
            data.Add("htttoan", "Tiền mặt/Chuyển khoản");
            data.Add("stknban", "");
            data.Add("tnhban", "");
            data.Add("mnmua", model["PartnerCode"].ToString());
            data.Add("mst", model["BuyerTaxCode"].ToString());
            data.Add("tnmua", model["BuyerDisplayName"].ToString());
            data.Add("email", email);
            data.Add("ten", model["PartnerName"].ToString());
            // data.Add("ten", model["partnerName0"].ToString());
            data.Add("dchi", model["BuyerAddressLine"].ToString());
            data.Add("stknmua", "");
            data.Add("tnhmua", "");
            data.Add("tgtcthue", decimal.Parse(model["TotalAmountWithoutVat"].ToString()));
            data.Add("tgtthue", decimal.Parse(model["VatAmount"].ToString()));
            data.Add("tgtttbso", decimal.Parse(model["TotalAmount"].ToString()));
            data.Add("tgtttbso_last", decimal.Parse(model["TotalAmount"].ToString()));
            data.Add("tkcktmn", decimal.Parse(model["DiscountAmount"].ToString()));
            data.Add("ttcktmai", 0);
            data.Add("tgtphi", 0);
            data.Add("mdvi", dvcs[1].ToString());
            data.Add("tthdon", 0);
            data.Add("is_hdcma", 1);

            JArray arrayDetail = new JArray();
            var productVoucherDetails = model["InvoiceAuthDetails"].ToString();
            var lstproductVoucherDetails = JArray.Parse(productVoucherDetails);

            int j = 0;
            var test = lstproductVoucherDetails[0]["ItemName"].ToString();
            foreach (var item in lstproductVoucherDetails)
            {
                string ord0 = item["Ord0"].ToString().Substring(1, 9).TrimStart(new Char[] { '0' });
                JObject dataDetail = new JObject();
                dataDetail.Add("stt", ord0);
                dataDetail.Add("ma", item["ItemCode"].ToString());
                dataDetail.Add("ten", item["ItemName"].ToString());
                dataDetail.Add("mdvtinh", item["UnitCode"].ToString());
                dataDetail.Add("sluong", decimal.Parse(item["Quantity"].ToString()));
                dataDetail.Add("dgia", decimal.Parse(item["Price"].ToString()));
                dataDetail.Add("thtien", decimal.Parse(item["TotalAmountWithoutVat"].ToString()));
                dataDetail.Add("tlckhau", decimal.Parse(item["DiscountPercentage"].ToString()));
                dataDetail.Add("stckhau", decimal.Parse(item["DiscountAmount"].ToString()));
                dataDetail.Add("tsuat", string.IsNullOrEmpty(item["VatPercentage"].ToString()) == true ? 0 : item["VatPercentage"].ToString());
                dataDetail.Add("tthue", decimal.Parse(item["VatAmount"].ToString()));
                dataDetail.Add("tgtien", decimal.Parse(item["TotalAmount"].ToString()));
                dataDetail.Add("kmai", 1);
                arrayDetail.Add(dataDetail);
            }
            JObject jsons = new JObject();
            jsons.Add("data", arrayDetail);
            JArray arrays = new JArray();
            arrays.Add(jsons);
            data.Add("details", arrays);
            array.Add(data);
            JObject resul = new JObject();
            resul.Add("data", array);
            resul.Add("editmode", 1);

            string url = apiLink + URL_INVOICE_TEMPLATE;

            using var client = this.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            var a = System.Text.RegularExpressions.Regex.Unescape(resul.ToString());
            string s = Regex.Replace(a, "(#[A-Za-z0-9]+)=\"\"", "\"").Replace("\\n", "").Replace("\\", @" """).Replace("\n", "").Replace("u0022", "");
            var bodyContent = new StringContent(s.ToString(), Encoding.UTF8, ContentTypeConst.ApplicationJson);
            var respMessage = await client.PostAsync(apiLink + URL_SAVE, bodyContent);
            respMessage.EnsureSuccessStatusCode();
            string content = await respMessage.Content.ReadAsStringAsync();
            var result = JsonObject.Parse(content);

            var josn = JsonObject.Parse(resul.ToString());
            JsonObject keyValues = new JsonObject();
            if (string.IsNullOrEmpty((result[0]["error"] ?? "").ToString()) == true)
            {
                await this.UpdateInvoiceNumber(result, model["Id"].ToString());

                keyValues.Add("json", josn);
                keyValues.Add("result", result);
            }
            else
            {
                keyValues.Add("json", josn);
                keyValues.Add("result", result);
                //throw new AccountingInvoiceException("", content, "", resul);
            }

            return keyValues;
        }

        public async Task<JsonObject> Delete(JsonObject model)
        {
            string token = await Login();
            string url = apiLink + URL_DELETE;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using var client = this.GetHttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);

            var a = Regex.Unescape(model.ToString());
            string s = Regex.Replace(a, "(#[A-Za-z0-9]+)=\"\"", "\"").Replace("\\n", "").Replace("\\", @" """).Replace("\n", "").Replace("u0022", "");
            var bodyContent = new StringContent(s.ToString(), Encoding.UTF8, ContentTypeConst.ApplicationJson);
            var respMessage = await client.PostAsync(apiLink + URL_DELETE, bodyContent);
            respMessage.EnsureSuccessStatusCode();
            string content = await respMessage.Content.ReadAsStringAsync();
            JsonObject result = (JsonObject)JsonObject.Parse(content);


            //
            // WebClient webClient = new WebClient();
            // webClient.Headers.Add("Content-Type", "application/json");
            // webClient.Headers["Authorization"] = "Bear "+ token;
            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //
            // var reponse = webClient.UploadString(url, s);
            // JsonObject keyValuePairs=(JsonObject) JsonObject.Parse(reponse.ToString());
            return result;
        }

        public async Task<JsonObject> InvoiceTemplate(JsonObject model)
        {
            string token = await this.Login();
            string url = apiLink + URL_INVOICE_TEMPLATE;

            using var client = this.GetHttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            var content = await client.GetStringAsync(url);
            JsonObject resul = new JsonObject();
            List<InvoiceDto> invoice = new List<InvoiceDto>();
            JArray jArray = JArray.Parse(content);
            foreach (var item in jArray)
            {
                InvoiceDto invoiceDto = new InvoiceDto();
                invoiceDto.InvoiceId = item["id"].ToString();
                invoiceDto.InvoiceSerial = item["khhdon"].ToString();
                invoiceDto.InvoiceSysbol = item["sdmau"].ToString();
                invoice.Add(invoiceDto);
            }
            var resulAll = JsonConvert.SerializeObject(invoice);
            var newNode = JsonNode.Parse(resulAll);
            resul.Add("Result", resulAll);
            return resul;
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
            string ma_dvcs = rootNode["ma_dvcs"].GetValue<string>();
            return $"{token};" + ma_dvcs;
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

            string token = await this.Login();
            string[] dvcs = token.Split(';');
            var idMobi = JsonObject.Parse(model["idMobi"].ToString());
            DateTime date = DateTime.Parse(model["IssuedDate"].ToString());
            string voucherDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            JArray array = new JArray();
            JObject data = new JObject();
            data.Add("cctbao_id", idMobi["InvoiceId"].ToString());
            data.Add("nlap", voucherDate);
            data.Add("sdhang", model["InvoiceNumber"].ToString());
            data.Add("dvtte", model["CurrencyCode"].ToString());
            data.Add("tgia", model["ExchangeRate"].ToString());
            data.Add("htttoan", model["PaymentMethodName"].ToString());
            data.Add("stknban", "");
            data.Add("tnhban", "");
            data.Add("mnmua", model["PartnerCode"].ToString());
            data.Add("mst", model["BuyerTaxCode"].ToString());
            data.Add("tnmua", model["BuyerDisplayName"].ToString());
            data.Add("email", email);
            data.Add("ten", model["PartnerName"].ToString());
            // data.Add("ten", model["partnerName0"].ToString());
            data.Add("dchi", model["BuyerAddressLine"].ToString());
            data.Add("stknmua", "");
            data.Add("tnhmua", "");
            data.Add("tgtcthue", decimal.Parse(model["TotalAmountWithoutVat"].ToString()));
            data.Add("tgtthue", 0);
            data.Add("tgtttbso", decimal.Parse(model["TotalAmount"].ToString()));
            data.Add("tgtttbso_last", decimal.Parse(model["TotalAmount"].ToString()));
            data.Add("tkcktmn", 0);
            data.Add("ttcktmai", 0);
            data.Add("tgtphi", 0);
            data.Add("mdvi", dvcs[1].ToString());
            data.Add("tthdon", 0);
            data.Add("is_hdcma", 1);

            JArray arrayDetail = new JArray();
            var productVoucherDetails = model["InvoiceAuthDetails"].ToString();
            var lstproductVoucherDetails = JArray.Parse(productVoucherDetails);


            //var test = lstproductVoucherDetails[0]["ItemName"].ToString();
            decimal DecreaseAmount = lstproductVoucherDetails.Sum(p => decimal.Parse(p["DecreaseAmount"].ToString()));
            if (DecreaseAmount > 0)
            {
                data.Add("tienthuegtgtgiam", DecreaseAmount);
                data.Add("giamthuebanhang20", 1);
            }
            foreach (var item in lstproductVoucherDetails)
            {
                string ord0 = item["Ord0"].ToString().Substring(1, 9).TrimStart(new Char[] { '0' });
                JObject dataDetail = new JObject();
                dataDetail.Add("stt", ord0);
                dataDetail.Add("ma", item["ItemCode"].ToString());
                dataDetail.Add("ten", item["ItemName"].ToString());
                dataDetail.Add("mdvtinh", item["UnitCode"].ToString());
                dataDetail.Add("sluong", decimal.Parse(item["Quantity"].ToString()));
                dataDetail.Add("dgia", decimal.Parse(item["Price"].ToString()));
                dataDetail.Add("thtien", decimal.Parse(item["TotalAmountWithoutVat"].ToString()));
                dataDetail.Add("tlckhau", decimal.Parse(item["DiscountPercentage"].ToString()));
                dataDetail.Add("stckhau", decimal.Parse(item["DiscountAmount"].ToString()));
                dataDetail.Add("tsuat", string.IsNullOrEmpty(item["VatPercentage"].ToString()) == true ? 0 : item["VatPercentage"].ToString());
                dataDetail.Add("tthue", 0);
                dataDetail.Add("tgtien", decimal.Parse(item["TotalAmount"].ToString())-decimal.Parse(item["VatAmount"].ToString()));                dataDetail.Add("kmai", 1);
                arrayDetail.Add(dataDetail);
            }
            JObject jsons = new JObject();
            jsons.Add("data", arrayDetail);
            JArray arrays = new JArray();
            arrays.Add(jsons);
            data.Add("details", arrays);
            array.Add(data);
            JObject resul = new JObject();
            resul.Add("data", array);
            resul.Add("editmode", 1);

            string url = apiLink + URL_INVOICE_TEMPLATE;

            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using var client = this.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            var a = System.Text.RegularExpressions.Regex.Unescape(resul.ToString());
            string s = Regex.Replace(a, "(#[A-Za-z0-9]+)=\"\"", "\"").Replace("\\n", "").Replace("\\", @" """).Replace("\n", "").Replace("u0022", "");
            var bodyContent = new StringContent(s.ToString(), Encoding.UTF8, ContentTypeConst.ApplicationJson);
            var respMessage = await client.PostAsync(apiLink + URL_SAVE, bodyContent);
            respMessage.EnsureSuccessStatusCode();
            string content = await respMessage.Content.ReadAsStringAsync();
            var result = JsonObject.Parse(content);

            var josn = JsonObject.Parse(resul.ToString());
            JsonObject keyValues = new JsonObject();
            if (string.IsNullOrEmpty((result[0]["error"] ?? "").ToString()) == true)
            {
                await this.UpdateInvoiceNumber(result, model["Id"].ToString());

                keyValues.Add("json", josn);
                keyValues.Add("result", result);
            }
            else
            {
                keyValues.Add("json", josn);
                keyValues.Add("result", result);
                //throw new AccountingInvoiceException("", content, "", resul);
            }

            return keyValues;
        }
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
            var resuls = JsonArray.Parse(result[0]["data"].ToString());
            if (resuls == null) return;
            var dto = new UpdateInvoiceNumberDto();
            dto.InvoiceNumber = (resuls["shdon"] ?? "").ToString();
            dto.InvoiceSeries = resuls["khieu"].ToString();
            dto.Id = id;
            dto.HDonID = resuls["hdon_id"].ToString();
            dto.cttb_id = resuls["cctbao_id"].ToString();
            await _invoiceAuthService.UpdateInvoiceNumberAsync(dto);
        }

        public async Task<JsonObject> UpdateInvoiceStatus(JsonObject model)
        {
            string token = await this.Login();
            using var client = this.GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bear", token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                                                    ContentTypeConst.ApplicationJson);
            var dto = new JsonObject();
            // dto.Add("tu_ngay", model["fromDate"].ToString());
            // dto.Add("den_ngay", model["toDate"].ToString());\


            // Lấy thời gian hiện tại
            DateTime currentDate = DateTime.Now;
            DateTime tuNgay = new DateTime(currentDate.Year, currentDate.Month, 1);
            dto.Add("tu_ngay", tuNgay);
            DateTime denNgay = tuNgay.AddMonths(1).AddDays(-1);
            dto.Add("den_ngay", denNgay);

            var bodyContent = new StringContent(dto.ToString(), Encoding.UTF8, ContentTypeConst.ApplicationJson);

            var respMessage = await client.PostAsync(apiLink + URL_INVOICE_GetInvoices, bodyContent);
            respMessage.EnsureSuccessStatusCode();

            string content = await respMessage.Content.ReadAsStringAsync();
            //var result = JsonNode.Parse(content);
            JArray jArray = JArray.Parse(content);
            var invoiceAuth = await _invoiceAuthService.GetQueryableAsync();
            var test = invoiceAuth.Where(auth => auth.InvoiceId != null).ToList();
            var a = invoiceAuth.ToList();
            foreach (var hoaDon in test)
            {
                if (hoaDon.InvoiceId != null)
                {
                    foreach (var item in jArray)
                    {
                        if ( item["hdon_id"].ToString() == hoaDon.InvoiceId.ToString())
                        {
                            if (item["tthai"].ToString() == InvoiceConst.CodeNotCqt)
                            {
                                var lstInvoice = invoiceAuth.Where(p =>
                                    p.InvoiceId.ToString() == item["hdon_id"].ToString());
                                foreach (var invoice in lstInvoice)
                                {
                                    invoice.Status = InvoiceConst.CodeNotCqt;
                                    invoice.InvoiceNumber = item["shdon"].ToString();
                                    await _invoiceAuthService.UpdateAsync(invoice);
                                }
                            }

                            if (item["tthai"].ToString() == InvoiceConst.InvoiceCQT)
                            {
                                var lstInvoice = invoiceAuth.Where(p =>
                                    p.InvoiceId.ToString() == item["hdon_id"].ToString());
                              
                                foreach (var invoice in lstInvoice)
                                {
                                    invoice.Status = InvoiceConst.InvoiceCQT;
                                    invoice.InvoiceNumber = item["shdon"].ToString();
                                    await _invoiceAuthService.UpdateAsync(invoice);
                                }
                            }

                            if (item["tthai"].ToString() == InvoiceConst.InvoiceNotCqt)
                            {
                                var lstInvoice = invoiceAuth.Where(p =>
                                    p.InvoiceId.ToString() == item["hdon_id"].ToString());
                                foreach (var invoice in lstInvoice)
                                {
                                    invoice.Status = InvoiceConst.InvoiceNotCqt;
                                    invoice.InvoiceNumber = item["shdon"].ToString();
                                    await _invoiceAuthService.UpdateAsync(invoice);
                                }
                            }

                            if (item["tthai"].ToString() == InvoiceConst.WaitingNumber)
                            {
                                var lstInvoice = invoiceAuth.Where(p =>
                                    p.InvoiceId.ToString() == item["hdon_id"].ToString());
                                foreach (var invoice in lstInvoice)
                                {
                                    invoice.Status = InvoiceConst.WaitingNumber;
                                    invoice.InvoiceNumber = item["shdon"].ToString();
                                    await _invoiceAuthService.UpdateAsync(invoice);
                                }
                            }

                            if (item["tthai"].ToString() == InvoiceConst.Signed)
                            {
                                var lstInvoice = invoiceAuth.Where(p =>
                                    p.InvoiceId.ToString() == item["hdon_id"].ToString());
                                foreach (var invoice in lstInvoice)
                                {
                                    invoice.Status = InvoiceConst.Signed;
                                    invoice.InvoiceNumber = item["shdon"].ToString();
                                    await _invoiceAuthService.UpdateAsync(invoice);
                                }
                            }
                        }
                    }
                }
            }


            var resul = new JsonObject();
            return resul;
        }
    }
}