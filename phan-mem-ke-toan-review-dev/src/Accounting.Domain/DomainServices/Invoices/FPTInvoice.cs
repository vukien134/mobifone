using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Licenses;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.Invoices
{
    public class FPTInvoice : IEInvoice
    {
        private InvoiceSupplierService _invoiceSupplierService;
        private InvoiceAuthService _invoiceAuthService;
        private string TOKEN_HEADER = "";
        private string _username = "";
        private string _password = "";
        private string _host = "";
        private string _apiURL = "";
        private string _lang = "";

        private string _taxcode = "";
        private string _checkCircular = "";

        // private string _partnerTOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiIwMTAyMjM2Mjc2LmFkbWluIiwibWFpbCI6InRodXlodDIwQGZwdC5jb20udm4iLCJmbiI6ImFkbWluIiwib3UiOjEsIm9uIjoiQ8OUTkcgVFkgQ-G7lCBQSOG6pk4gVkFDT00iLCJvcmciOnsib24iOiJDw5RORyBUWSBD4buUIFBI4bqmTiBWQUNPTSIsIm90IjoiMDEwMjIzNjI3NiIsImFkZCI6ImFiYywgUGjGsOG7nW5nIE3hu7kgVGjhuqFuaCwgVGjDoG5oIHBo4buRIExvbmcgWHV5w6puLCBBbiBHaWFuZyIsInRlbCI6IjA5MTQwMDE0MDgiLCJtYWlsIjoidGh1eWh0MjBAZnB0LmNvbS52biIsImFjYyI6IiIsImJhbmsiOiIifSwic2NoZW1hIjoiaTAxMDIyMzYyNzYiLCJ0YXhjIjoiMDEwMjIzNjI3NiIsInN0YXR1cyI6MiwibXN0IjoiMDEwMjIzNjI3NiIsInRheCI6IjAxMDIyMzYyNzYiLCJzYyI6MCwic2N0IjowLCJuYSI6MiwibnAiOjUsIm5xIjoyLCJpdHlwZSI6IjAxR1RLVCIsInJvbGUiOlsxLDIsMyw0LDUsNiw3LDgsOSwxMCwxMSwxMiwxMywxNF0sInBhdGgiOlsiL291cyIsIi9lZGkiLCIvZW1hIiwiL2V0ZSIsIi91c2UiLCIvYWRzIiwiL2NvbCIsIi9kdGwiLCIveGxzIiwiL29yYyIsIi9leGMiLCIvbG9jIiwiL2ducyIsIi9zZXIiLCIvdGJxIiwiL2ludiIsIi9leGMiLCIvZ25zIiwiL3NlYSIsIi9zZXEiLCIvYXBwIiwiL3NhdiIsIi9ycHQiLCIvc2VhIiwiL2FkaiIsIi9zZWEiLCIvaW52IiwiL2ducyIsIi9yZXAiLCIvc2VhIiwiL2ludiIsIi9nbnMiLCIvc2VhIiwiL2ljYSIsIi9zZXIiLCIvc2FwIiwiL3NlciIsIi9zY2EiLCIvc2VxIiwiL2RzcyJdLCJpYXQiOjE2MDkxMjAyMjEsImV4cCI6MTYwOTIwNjYyMX0.qd66oRSil8N5XMJGk6KHqOGpavdhSbBfQe2UtTsZ3XA";


        private string URL_LOGIN = "/c_signin";
        private string URL_CREATE_INVOICE = "/create-invoice";
        private string URL_UPDATE_INVOICE = "/update-invoice";
        private string URL_SEACH_INVOICE = "/search-invoice";


        private WebClient _webClient = null;
        public FPTInvoice()
        {

        }
        public FPTInvoice(JsonObject model, IServiceProvider serviceProvider)
        {
            _invoiceSupplierService = serviceProvider.GetRequiredService<InvoiceSupplierService>();
            _invoiceAuthService = serviceProvider.GetRequiredService<InvoiceAuthService>();
            _webClient = new WebClient();
            _webClient.Encoding = System.Text.Encoding.UTF8;
            _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            this._host = model["apiLink"].ToString();
            this._username = model["username"].ToString(); ;
            this._password = model["password"].ToString(); ;
            this._taxcode = model["codeTax"].ToString(); ;
            this._checkCircular = model["checkCircular"].ToString();
            this.Login();
        }
        public async Task<JsonObject> Create(JsonObject model)
        {

            JsonObject res = new JsonObject();
            if (_checkCircular == "K")
            {
                JsonObject postData = new JsonObject();
                postData.Add("sid", model["id"].ToString());
                if (model["ContractDate"].ToString() == null)
                {
                    postData.Add("idt", DateTime.Now.ToString("yyyy/MM/dd hh:ss"));
                }
                else { postData.Add("idt", model["ContractDate"].ToString()); }

                postData.Add("type", model["InvoiceType"].ToString());
                postData.Add("form", model["InvoiceTemplate"].ToString());
                postData.Add("serial", model["InvoiceSeries"].ToString());
                postData.Add("seq", model["InvoiceNumber"].ToString());
                postData.Add("bcode", model["PartnerCode"].ToString());
                postData.Add("bname", model["BuyerLegalName"].ToString());
                postData.Add("buyer", model["BuyerDisplayName"].ToString());
                postData.Add("btax", model["BuyerTaxCode"].ToString());
                postData.Add("baddr", model["BuyerAddressLine"].ToString());
                postData.Add("btel", model["BuyerMobile"].ToString());
                postData.Add("bmail", model["BuyerEmail"].ToString());
                postData.Add("paym", model["PaymentMethodName"].ToString());
                postData.Add("curr", model["CurrencyCode"].ToString());
                postData.Add("exrt", Convert.ToInt32(model["ExchangeRate"].ToString()));
                postData.Add("bacc", model["BuyerBankAccount"].ToString());
                postData.Add("bbank", model["BuyerBankName"].ToString());
                postData.Add("note", model["InvoiceNote"].ToString());
                postData.Add("sumv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("sum", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("vatv", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("vat", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("word", model["AmountByWord"].ToString());
                postData.Add("totalv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));

                postData.Add("total", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("tradeamount", Convert.ToDecimal(model["DiscountAmount"].ToString()));
                postData.Add("discount", model["DiscountAmount"].ToString());
                postData.Add("aun", 2);
                postData.Add("sign", -1);
                postData.Add("type_ref", 1);
                postData.Add("listnum", "");
                postData.Add("listdt", null);
                postData.Add("sendtype", 1);
                JsonArray postDetails = new JsonArray();

                JsonArray items = (JsonArray)model["details"];
                foreach (JsonObject item in items)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("line", item["Ord0"].ToString());
                    postItem.Add("type", "");
                    if (item["VatPercentage"].ToString() == "")
                    {
                        postItem.Add("vrt", 0);
                    }
                    else
                    {
                        postItem.Add("vrt", item["VatPercentage"].ToString());
                    }
                    postItem.Add("code", item["ItemCode"].ToString());
                    postItem.Add("name", item["ItemName"].ToString());
                    postItem.Add("unit", item["UnitCode"].ToString());
                    postItem.Add("price", Convert.ToDecimal(item["Price"].ToString()));
                    postItem.Add("quantity", Convert.ToInt32(item["Quantity"].ToString()));
                    postItem.Add("perdiscount", Convert.ToInt32(item["DiscountPercentage"].ToString()));
                    postItem.Add("amtdiscount", Convert.ToInt32(item["DiscountAmount"].ToString()));

                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("vat", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("total", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postDetails.Add(postItem);


                }
                postData.Add("items", postDetails);

                postData.Add("stax", this._taxcode);
                string url = this._host + this.URL_CREATE_INVOICE;

                JsonObject userandpass = new JsonObject();
                userandpass.Add("username", _username);
                userandpass.Add("password", _password);

                JsonObject dataGuiqua = new JsonObject();
                dataGuiqua.Add("user", userandpass);
                dataGuiqua.Add("inv", postData);
                JsonObject iteam = new JsonObject();

                string msg = dataGuiqua.ToString();
                string result = "";
                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                WebException _ex = null;
                try
                {
                    result = _webClient.UploadString(url, dataGuiqua.ToString());
                    res.Add("ok", true);
                    res.Add("invoice", result);
                    JsonObject rss = (JsonObject)result;
                    string InvoiceSeries = rss["serial"].ToString();
                    string InvoiceTemplate = rss["form"].ToString();
                    string InvoiceNumber = rss["seq"].ToString();
                    Guid id = Guid.Parse(model["id"].ToString());

                    //var db = EngineContext.Current.Resolve<IDbContext>();
                    //string sql = "UPDATE inv_invoiceauth SET InvoiceSeries=@PR_0,InvoiceTemplate=@PR_1,InvoiceNumber=@PR_2 WHERE id=@PR_3";

                    //db.ExectueNonQuery(sql, InvoiceSeries, InvoiceTemplate, InvoiceNumber, id);
                    //string so_hd = "UPDATE PSTHUE SET SO_HD = @PR_0 WHERE DPHV_id=@PR_1";
                    //db.ExectueNonQuery(so_hd, InvoiceNumber, id);
                }
                catch (WebException ex)
                {
                    _ex = ex;
                    using (Stream stream = ex.Response.GetResponseStream())
                    {
                        MemoryStream ms = new MemoryStream();
                        stream.CopyTo(ms);

                        byte[] bytes = ms.ToArray();
                        ms.Close();

                        string error = System.Text.Encoding.UTF8.GetString(bytes);
                        res.Add("ok", false);
                        res.Add("error", error);

                    }
                }
            }
            else
            {
                JsonObject postData = new JsonObject();
                postData.Add("sid", model["id"].ToString());
                if (model["ContractDate"].ToString() == null)
                {
                    postData.Add("idt", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                }
                else { postData.Add("idt", model["ContractDate"].ToString()); }

                postData.Add("type", model["InvoiceType"].ToString());
                postData.Add("form", model["InvoiceTemplate"].ToString());
                postData.Add("serial", model["InvoiceSeries"].ToString());
                postData.Add("seq", model["InvoiceNumber"].ToString());
                postData.Add("bcode", model["PartnerCode"].ToString());
                postData.Add("bname", model["BuyerLegalName"].ToString());
                postData.Add("buyer", model["BuyerDisplayName"].ToString());
                postData.Add("btax", model["BuyerTaxCode"].ToString());
                postData.Add("baddr", model["BuyerAddressLine"].ToString());
                postData.Add("btel", model["BuyerMobile"].ToString());
                postData.Add("bmail", model["BuyerEmail"].ToString());
                postData.Add("paym", model["PaymentMethodName"].ToString());
                postData.Add("curr", model["CurrencyCode"].ToString());
                postData.Add("exrt", Convert.ToInt32(model["ExchangeRate"].ToString()));
                postData.Add("bacc", model["BuyerBankAccount"].ToString());
                postData.Add("bbank", model["BuyerBankName"].ToString());
                postData.Add("note", model["InvoiceNote"].ToString());
                postData.Add("sumv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("sum", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("vatv", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("vat", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("word", model["AmountByWord"].ToString());
                postData.Add("totalv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("total", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("discount", model["DiscountAmount"].ToString());
                postData.Add("aun", 2);
                postData.Add("sign", -1);
                JsonArray postDetails = new JsonArray();

                JsonArray items = (JsonArray)model["details"];
                foreach (JsonObject item in items)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("line", item["Ord0"].ToString());
                    postItem.Add("type", "");
                    if (item["VatPercentage"].ToString() == "")
                    {
                        postItem.Add("vrt", 0);
                    }
                    else
                    {
                        postItem.Add("vrt", item["VatPercentage"].ToString());
                    }

                    postItem.Add("code", item["ItemCode"].ToString());
                    postItem.Add("name", item["ItemName"].ToString());
                    postItem.Add("unit", item["UnitCode"].ToString());
                    postItem.Add("price", Convert.ToDecimal(item["Price"].ToString()));
                    postItem.Add("quantity", Convert.ToInt32(item["Quantity"].ToString()));
                    postItem.Add("perdiscount", Convert.ToInt32(item["DiscountPercentage"].ToString()));
                    postItem.Add("amtdiscount", Convert.ToInt32(item["DiscountAmount"].ToString()));
                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("vat", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("total", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postDetails.Add(postItem);


                }
                postData.Add("items", postDetails);

                postData.Add("stax", this._taxcode);
                string url = this._host + this.URL_CREATE_INVOICE;

                JsonObject userandpass = new JsonObject();
                userandpass.Add("username", _username);
                userandpass.Add("password", _password);

                JsonObject dataGuiqua = new JsonObject();
                dataGuiqua.Add("lang", "vi");
                dataGuiqua.Add("user", userandpass);
                dataGuiqua.Add("inv", postData);
                JsonObject iteam = new JsonObject();

                string msg = dataGuiqua.ToString();
                string result = "";
                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                WebException _ex = null;
                try
                {
                    result = _webClient.UploadString(url, dataGuiqua.ToString());
                    res.Add("ok", true);
                    res.Add("invoice", result);
                    JsonObject rss = (JsonObject)result;
                    string InvoiceSeries = rss["serial"].ToString();
                    string InvoiceTemplate = rss["form"].ToString();
                    string InvoiceNumber = rss["seq"].ToString();
                    Guid id = Guid.Parse(model["id"].ToString());

                    //var db = EngineContext.Current.Resolve<IDbContext>();
                    //string sql = "UPDATE inv_invoiceauth SET InvoiceSeries=@PR_0,InvoiceTemplate=@PR_1,InvoiceNumber=@PR_2 WHERE id=@PR_3";

                    //db.ExectueNonQuery(sql, InvoiceSeries, InvoiceTemplate, InvoiceNumber, id);
                    //string so_hd = "UPDATE PSTHUE SET SO_HD = @PR_0 WHERE DPHV_id=@PR_1";
                    //db.ExectueNonQuery(so_hd, InvoiceNumber, id);
                    //string invoice = "UPDATE DPHV SET INVOICE = @PR_0 WHERE id=@PR_1";
                    //db.ExectueNonQuery(invoice, InvoiceNumber, id);
                }
                catch (WebException ex)
                {
                    _ex = ex;
                    using (Stream stream = ex.Response.GetResponseStream())
                    {
                        MemoryStream ms = new MemoryStream();
                        stream.CopyTo(ms);

                        byte[] bytes = ms.ToArray();
                        ms.Close();

                        string error = System.Text.Encoding.UTF8.GetString(bytes);
                        res.Add("ok", false);
                        res.Add("error", error);

                    }
                }
            }

            return res;
        }

        public async Task<JsonObject> Delete(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonObject> GetTemplateInvoice(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<string> Login()
        {
            string urls = this._host + URL_LOGIN;
            JsonObject param = new JsonObject();
            param.Add("username", _username);
            param.Add("password", _password);
            param.Add("lang", "vi");
            _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string result = _webClient.UploadString(urls, param.ToString());
            TOKEN_HEADER = result;
            return Task.FromResult(TOKEN_HEADER);
        }
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public byte[] Preview(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonObject> SaveXmlInvoice(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonObject> Update(JsonObject model)
        {

            JsonObject res = new JsonObject();
            if (_checkCircular == "K")
            {
                JsonObject postData = new JsonObject();
                postData.Add("sid", model["id"].ToString());
                if (model["ContractDate"].ToString() == null)
                {
                    postData.Add("idt", DateTime.Now.ToString("yyyy/MM/dd hh:ss"));
                }
                else { postData.Add("idt", model["ContractDate"].ToString()); }

                postData.Add("type", model["InvoiceType"].ToString());
                postData.Add("form", model["InvoiceTemplate"].ToString());
                postData.Add("serial", model["InvoiceSeries"].ToString());
                postData.Add("seq", model["InvoiceNumber"].ToString());
                postData.Add("bcode", model["PartnerCode"].ToString());
                postData.Add("bname", model["BuyerLegalName"].ToString());
                postData.Add("buyer", model["BuyerDisplayName"].ToString());
                postData.Add("btax", model["BuyerTaxCode"].ToString());
                postData.Add("baddr", model["BuyerAddressLine"].ToString());
                postData.Add("btel", model["BuyerMobile"].ToString());
                postData.Add("bmail", model["BuyerEmail"].ToString());
                postData.Add("paym", model["PaymentMethodName"].ToString());
                postData.Add("curr", model["CurrencyCode"].ToString());
                postData.Add("exrt", Convert.ToInt32(model["ExchangeRate"].ToString()));
                postData.Add("bacc", model["BuyerBankAccount"].ToString());
                postData.Add("bbank", model["BuyerBankName"].ToString());
                postData.Add("note", model["InvoiceNote"].ToString());
                postData.Add("sumv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("sum", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("vatv", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("vat", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("word", model["AmountByWord"].ToString());
                postData.Add("totalv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));

                postData.Add("total", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("tradeamount", Convert.ToDecimal(model["DiscountAmount"].ToString()));
                postData.Add("discount", model["DiscountAmount"].ToString());
                postData.Add("aun", 2);
                postData.Add("sign", -1);
                postData.Add("type_ref", 1);
                postData.Add("listnum", "");
                postData.Add("listdt", null);
                postData.Add("sendtype", 1);
                JsonArray postDetails = new JsonArray();

                JsonArray items = (JsonArray)model["details"];
                foreach (JsonObject item in items)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("line", item["Ord0"].ToString());
                    postItem.Add("type", "");
                    if (item["VatPercentage"].ToString() == "")
                    {
                        postItem.Add("vrt", 0);
                    }
                    else
                    {
                        postItem.Add("vrt", item["VatPercentage"].ToString());
                    }
                    postItem.Add("code", item["ItemCode"].ToString());
                    postItem.Add("name", item["ItemName"].ToString());
                    postItem.Add("unit", item["UnitCode"].ToString());
                    postItem.Add("price", Convert.ToDecimal(item["Price"].ToString()));
                    postItem.Add("quantity", Convert.ToInt32(item["Quantity"].ToString()));
                    postItem.Add("perdiscount", Convert.ToInt32(item["DiscountPercentage"].ToString()));
                    postItem.Add("amtdiscount", Convert.ToInt32(item["DiscountAmount"].ToString()));

                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("vat", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("total", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postDetails.Add(postItem);


                }
                postData.Add("items", postDetails);

                postData.Add("stax", this._taxcode);
                string url = this._host + this.URL_UPDATE_INVOICE;

                JsonObject userandpass = new JsonObject();
                userandpass.Add("username", _username);
                userandpass.Add("password", _password);

                JsonObject dataGuiqua = new JsonObject();
                dataGuiqua.Add("user", userandpass);
                dataGuiqua.Add("inv", postData);
                JsonObject iteam = new JsonObject();

                string msg = dataGuiqua.ToString();
                string result = "";
                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                WebException _ex = null;
                try
                {
                    result = _webClient.UploadString(url, dataGuiqua.ToString());
                    res.Add("ok", true);
                    res.Add("invoice", result);

                }
                catch (WebException ex)
                {
                    _ex = ex;
                    using (Stream stream = ex.Response.GetResponseStream())
                    {
                        MemoryStream ms = new MemoryStream();
                        stream.CopyTo(ms);

                        byte[] bytes = ms.ToArray();
                        ms.Close();

                        string error = System.Text.Encoding.UTF8.GetString(bytes);
                        res.Add("ok", false);
                        res.Add("error", error);

                    }
                }
            }
            else
            {
                JsonObject postData = new JsonObject();
                postData.Add("sid", model["id"].ToString());
                if (model["ContractDate"].ToString() == null)
                {
                    postData.Add("idt", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                }
                else { postData.Add("idt", model["ContractDate"].ToString()); }

                postData.Add("type", model["InvoiceType"].ToString());
                postData.Add("form", model["InvoiceTemplate"].ToString());
                postData.Add("serial", model["InvoiceSeries"].ToString());
                postData.Add("seq", model["InvoiceNumber"].ToString());
                postData.Add("bcode", model["PartnerCode"].ToString());
                postData.Add("bname", model["BuyerLegalName"].ToString());
                postData.Add("buyer", model["BuyerDisplayName"].ToString());
                postData.Add("btax", model["BuyerTaxCode"].ToString());
                postData.Add("baddr", model["BuyerAddressLine"].ToString());
                postData.Add("btel", model["BuyerMobile"].ToString());
                postData.Add("bmail", model["BuyerEmail"].ToString());
                postData.Add("paym", model["PaymentMethodName"].ToString());
                postData.Add("curr", model["CurrencyCode"].ToString());
                postData.Add("exrt", Convert.ToInt32(model["ExchangeRate"].ToString()));
                postData.Add("bacc", model["BuyerBankAccount"].ToString());
                postData.Add("bbank", model["BuyerBankName"].ToString());
                postData.Add("note", model["InvoiceNote"].ToString());
                postData.Add("sumv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("sum", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("vatv", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("vat", Convert.ToInt32(model["VatAmount"].ToString()));
                postData.Add("word", model["AmountByWord"].ToString());
                postData.Add("totalv", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("total", Convert.ToDecimal(model["TotalAmountWithoutVat"].ToString()));
                postData.Add("discount", model["DiscountAmount"].ToString());
                postData.Add("aun", 2);
                postData.Add("sign", -1);
                JsonArray postDetails = new JsonArray();

                JsonArray items = (JsonArray)model["details"];
                foreach (JsonObject item in items)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("line", item["Ord0"].ToString());
                    postItem.Add("type", "");
                    if (item["VatPercentage"].ToString() == "")
                    {
                        postItem.Add("vrt", 0);
                    }
                    else
                    {
                        postItem.Add("vrt", item["VatPercentage"].ToString());
                    }

                    postItem.Add("code", item["ItemCode"].ToString());
                    postItem.Add("name", item["ItemName"].ToString());
                    postItem.Add("unit", item["UnitCode"].ToString());
                    postItem.Add("price", Convert.ToDecimal(item["Price"].ToString()));
                    postItem.Add("quantity", Convert.ToInt32(item["Quantity"].ToString()));
                    postItem.Add("perdiscount", Convert.ToInt32(item["DiscountPercentage"].ToString()));
                    postItem.Add("amtdiscount", Convert.ToInt32(item["DiscountAmount"].ToString()));
                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("vat", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postItem.Add("total", Convert.ToInt32(item["TotalAmountWithoutVat"].ToString()));
                    postDetails.Add(postItem);


                }
                postData.Add("items", postDetails);

                postData.Add("stax", this._taxcode);
                string url = this._host + this.URL_UPDATE_INVOICE;

                JsonObject userandpass = new JsonObject();
                userandpass.Add("username", _username);
                userandpass.Add("password", _password);

                JsonObject dataGuiqua = new JsonObject();
                dataGuiqua.Add("lang", "vi");
                dataGuiqua.Add("user", userandpass);
                dataGuiqua.Add("inv", postData);
                JsonObject iteam = new JsonObject();

                string msg = dataGuiqua.ToString();
                string result = "";
                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                WebException _ex = null;
                try
                {
                    result = _webClient.UploadString(url, dataGuiqua.ToString());
                    res.Add("ok", true);
                    res.Add("invoice", result);

                }
                catch (WebException ex)
                {
                    _ex = ex;
                    using (Stream stream = ex.Response.GetResponseStream())
                    {
                        MemoryStream ms = new MemoryStream();
                        stream.CopyTo(ms);

                        byte[] bytes = ms.ToArray();
                        ms.Close();

                        string error = System.Text.Encoding.UTF8.GetString(bytes);
                        res.Add("ok", false);
                        res.Add("error", error);

                    }
                }
            }

            return res;


        }

        public Task<JsonObject> InvoiceTemplate(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> CreateHKD(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> UpdateInvoiceStatus(JsonObject model)
        {
            throw new NotImplementedException();
        }
    }
}