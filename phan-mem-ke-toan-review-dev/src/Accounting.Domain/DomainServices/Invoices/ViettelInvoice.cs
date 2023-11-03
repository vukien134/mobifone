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
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.Invoices
{
    public class ViettelInvoice : IEInvoice
    {
        #region Fields
        private YearCategoryService _yearCategoryService;
        private WebHelper _webHelper;
        private InvoiceSupplierService _invoiceSupplierService;
        private InvoiceAuthService _invoiceAuthService;
        private string Url_Login = "https://api-vinvoice.viettel.vn/auth/login";
        private readonly string Url_CreateInvoice = "/InvoiceAPI/InvoiceWS/createInvoice/";
        private readonly string Url_CreateInvoiceUsbTokenGetHash = "/InvoiceAPI/InvoiceWS/createInvoiceUsbTokenGetHash/";
        private readonly string Url_CreateInvoiceUsbTokenInsertSignature = "/InvoiceAPI/InvoiceWS/createInvoiceUsbTokenInsertSignature";
        private readonly string Url_GetInvoiceRepresentationFile = "/InvoiceAPI/InvoiceUtilsWS/getInvoiceRepresentationFile";
        private readonly string Url_GetInvoiceDraftPreview = "/InvoiceAPI/InvoiceUtilsWS/createInvoiceDraftPreview/";
        private readonly string Url_GetInvoiceFilePortal = "/InvoiceAPI/InvoiceUtilsWS/getInvoiceFilePortal";
        private readonly string Url_CancelTransactionInvoice = "/InvoiceAPI/InvoiceWS/cancelTransactionInvoice";

        private readonly string apiLink = "";
        private readonly string codeTax = "";
        private readonly string username = "";
        private readonly string password = "";
        private readonly string authorization = "";
        private readonly string certificateSerial = null;
        private readonly WebClient webClient;

        public ViettelInvoice(JsonObject model, IServiceProvider serviceProvider)
        {
            _invoiceSupplierService = serviceProvider.GetRequiredService<InvoiceSupplierService>();
            _invoiceAuthService = serviceProvider.GetRequiredService<InvoiceAuthService>();
            codeTax = model["codeTax"].ToString();
            apiLink = model["apiLink"].ToString();
            username = model["username"].ToString();
            password = model["password"].ToString();
            //---------------------------------------------------------------------------
            certificateSerial = string.IsNullOrEmpty(model["certificateSerial"] == null ? null : model["certificateSerial"].ToString())
                    ? null
                    : model["certificateSerial"].ToString();
            webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            Login();
        }
        #endregion
        #region Ctor
        public async Task<JsonObject> Create(JsonObject model)
        {
            return certificateSerial == null ? await CreateInvoiceByHSM(model) : await CreateInvoiceByUsbToken(model);
        }

        private async Task<JsonObject> CreateInvoiceByHSM(JsonObject model)
        {
            JsonObject postData = PrepareJsonData(model);

            string url = apiLink + Url_CreateInvoice + codeTax;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string response = "";
            JsonObject Jres = new JsonObject();
            try
            {
                response = webClient.UploadString(url, "POST", postData.ToString());
                Jres = (JsonObject)response;
            }
            catch (WebException e)
            {
                using (WebResponse resp = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)resp;
                    using (Stream data = resp.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string err = reader.ReadToEnd();
                        JsonObject errj = (JsonObject)err;
                        Jres.Add("errorCode", errj["code"].ToString());
                        Jres.Add("description", errj["data"].ToString());
                    }
                }
            }

            if (Jres["errorCode"] == null)
            {
                JsonObject invoiceInfo = (JsonObject)Jres["result"];
                JsonObject res = await UpdateInvoiceNumberAndSetResult(model, invoiceInfo);
                return res;
            }
            else
            {
                return new JsonObject
                {
                    { "ok", false },
                    { "error", Jres["description"].ToString() }
                };
            }
        }

        private async Task<JsonObject> CreateInvoiceByUsbToken(JsonObject model)
        {
            JsonObject objHash = CreateInvoiceUsbTokenGetHash(model);

            if (Convert.ToBoolean(objHash["ok"].ToString()) == true)
            {
                var postData = new JsonObject
                {
                    { "supplierTaxCode", codeTax },
                    { "templateCode", model["InvoiceTemplate"].ToString() },
                    { "hashString", objHash["hashString"].ToString() },
                    { "signature", certificateSerial }
                };

                var jsonObjects = CreateInvoiceUsbTokenInsertSignature(postData);

                if (Convert.ToBoolean(objHash["ok"].ToString()) == true)
                {
                    var invoiceInfo = (JsonObject)jsonObjects["result"];
                    var obj = await UpdateInvoiceNumberAndSetResult(model, invoiceInfo);
                    return obj;
                }
                else
                {
                    return new JsonObject
                    {
                        { "ok", false },
                        { "error", jsonObjects["description"].ToString() }
                    };
                }
            }
            return objHash;
        }

        public Task<JsonObject> Delete(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> InvoiceTemplate(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<string> Login()
        {
            string url = Url_Login;
            var postData = new JsonObject
            {
                {"username", username },
                {"password", password }
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string respone = webClient.UploadString(url, postData.ToString());
            var Data = JsonObject.Parse(respone);
            webClient.Headers.Add(HttpRequestHeader.Cookie, "access_token=" + Data["access_token"].ToString() + ";session_token=" + Data["refresh_token"].ToString());
            return Task.FromResult(string.Empty);
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
        #endregion
        #region Private
        private JsonObject PrepareJsonData(JsonObject model)
        {
            JsonObject jsonData = new JsonObject();

            var paymentStatus = true;
            var invoiceIssuedDate = PrepareDate(model["IssuedDate"].ToString(), "yyyy-MM-dd");
            var date_y = int.Parse(PrepareDate(model["IssuedDate"].ToString(), "yyyy"));
            var date_m = int.Parse(PrepareDate(model["IssuedDate"].ToString(), "MM"));
            var date_d = int.Parse(PrepareDate(model["IssuedDate"].ToString(), "dd"));
            Int64 TimeNow = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            Int64 time2 = (int)DateTime.UtcNow.Subtract(new DateTime(date_y, date_m, date_d)).TotalSeconds;
            Int64 time = (TimeNow - time2) * 1000;
            var currencyCode = model["CurrencyCode"].ToString();
            Guid id = Guid.Parse(model["InvoiceId"].ToString());

            var objInvoice = new JsonObject
            {
                { "transactionUuid", id },
                { "invoiceType", model["InvoiceType"].ToString() },
                { "templateCode", model["InvoiceTemplate"].ToString() },
                { "currencyCode", currencyCode },
                { "exchangeRate", model["ExchangeRate"].ToString() },
                { "adjustmentType", Convert.ToInt32(model["AdjustmentType"].ToString()) },
                { "paymentStatus", paymentStatus },
                { "paymentType", model["PaymentMethodName"].ToString() },
                { "paymentTypeName", model["PaymentMethodName"].ToString() },
                { "userName", model["USER_NEW"] },

                { "invoiceSeries", model["InvoiceSeries"].ToString() },
                { "invoiceIssuedDate", time },
                { "certificateSerial", certificateSerial }
            };

            var objBuyer = new JsonObject
            {
                { "buyerName", model["BuyerDisplayName"].ToString() },
                { "buyerLegalName", model["BuyerLegalName"].ToString() },
                { "buyerTaxCode", model["BuyerTaxCode"].ToString() },
                { "buyerAddressLine", model["BuyerAddressLine"].ToString() },
                { "buyerPhoneNumber", model["BuyerMobile"].ToString() },
                { "buyerEmail", model["BuyerEmail"].ToString() }
            };

            JsonArray postDataDetails = new JsonArray();
            List<JsonObject> Idetail = model["details"].AsArray().Select(x => (JsonObject)x).ToList();
            Idetail.ToList().ForEach(
                item =>
                {
                    postDataDetails.Add(new JsonObject
                    {
                        { "lineNumber", Convert.ToInt32(item["Ord0"]) },
                        { "selection", 1},
                        { "itemCode", item["ItemCode"].ToString() },
                        { "itemName", item["ItemName"].ToString() },
                        { "itemNote", item["ItemNote"] == null ? null : item["ItemNote"].ToString() },
                        { "unitName", item["UnitName"].ToString() },
                        { "unitPrice", Convert.ToDecimal(item["Price"]) },
                        { "quantity", Convert.ToDecimal(item["Quantity"]) },
                        { "itemTotalAmountWithoutTax", Convert.ToDecimal(item["TotalAmountWithoutVat"]) },
                        { "taxPercentage", GetTaxRateID(item["VatPercentage"].ToString()) },
                        { "taxAmount", Convert.ToDecimal(item["VatAmount"]) },
                        { "discount", 0 },
                        { "itemDiscount", 0 },
                        { "itemTotalAmountAfterDiscount", Convert.ToDecimal(item["inv_itemTotalAmountAfterDiscount"]) },
                        { "itemTotalAmountWithTax", Convert.ToDecimal(item["inv_TotalAmountWithTax"]) },
                        { "batchNo", item["inv_batchNo"] == null ? null : item["inv_batchNo"].ToString() },
                        { "expDate", item["inv_expDate"] == null ? null : item["inv_expDate"].ToString() }
                    });

                    if (Convert.ToDecimal(item["DiscountAmount"]) != 0)
                    {
                        postDataDetails.Add(new JsonObject
                        {
                            { "selection", 3},
                            { "itemName", "Chiết khấu"},
                            { "itemTotalAmountWithoutTax", Convert.ToDecimal(item["DiscountAmount"])},
                            { "taxPercentage", "0" },
                            { "taxAmount", "0" },
                            { "discount", "0" },
                            { "itemDiscount", "0" }
                        });
                    }
                }
            );

            List<TaxBreakdowns> ItaxBreakdowns = Idetail.GroupBy(x => x["VatPercentage"].ToString())
                .Select(y => new TaxBreakdowns
                {
                    taxPercentage = GetTaxRateID(y.First()["VatPercentage"].ToString()),
                    taxableAmount = y.Sum(a => Convert.ToDecimal(a["TotalAmountWithoutVat"])),
                    taxAmount = y.Sum(a => Convert.ToDecimal(a["VatAmount"])),
                    taxableAmountPos = y.Sum(a => Convert.ToDecimal(a["TotalAmountWithoutVat"])) >= 0,
                    taxAmountPos = y.Sum(a => Convert.ToDecimal(a["VatAmount"])) >= 0
                }).ToList();
            var arrTaxBreakdowns = new JsonArray();
            foreach (var taxBreakdown in ItaxBreakdowns)
            {
                arrTaxBreakdowns.Add(taxBreakdown);
            }
            var objSummary = new JsonObject();

            #region objSummary
            var sumOfTotalLineAmountWithoutTax = Convert.ToDecimal(model["TotalAmountWithoutVat"]);
            var totalAmountWithoutTax = Convert.ToDecimal(model["TotalAmountWithoutVat"]);
            var totalTaxAmount = Convert.ToDecimal(model["VatAmount"]);
            var totalAmountWithTax = Convert.ToDecimal(model["TotalAmount"]);
            var discountAmount = Convert.ToDecimal(model["DiscountAmount"]);
            var taxPercentage = model["VatPercentage"] == null ? null : model["VatPercentage"].ToString();

            objSummary.Add("sumOfTotalLineAmountWithoutTax", sumOfTotalLineAmountWithoutTax);
            objSummary.Add("totalAmountWithoutTax", totalAmountWithoutTax);
            objSummary.Add("totalTaxAmount", totalTaxAmount);
            objSummary.Add("totalAmountWithTax", totalAmountWithTax);

            objSummary.Add("discountAmount", discountAmount);
            objSummary.Add("settlementDiscountAmount", 0); // tổng CK thanh toán
            objSummary.Add("taxPercentage", GetTaxRateID(taxPercentage));

            objSummary.Add("isTotalAmountPos", totalAmountWithTax >= 0);
            objSummary.Add("isTotalTaxAmountPos", totalTaxAmount >= 0);
            objSummary.Add("isTotalAmtWithoutTaxPos", totalAmountWithoutTax >= 0);
            objSummary.Add("isDiscountAmtPos", discountAmount >= 0);

            if (currencyCode == "VND")
            {
                objSummary.Add("totalAmountWithTaxFrn", Convert.ToDecimal(model["TotalAmountWithoutVat"])); // NT
            }
            #endregion objSummary

            var payments = new JsonArray
            {
                new JsonObject{ {"paymentMethodName", model["PaymentMethodName"].ToString()} }
            };

            jsonData = new JsonObject
            {
                { "generalInvoiceInfo", objInvoice },
                { "buyerInfo", objBuyer },
                { "payments", payments },
                { "itemInfo", postDataDetails },
                { "summarizeInfo", objSummary },
                { "taxBreakdowns", arrTaxBreakdowns }
            };

            return jsonData;
        }

        private string PrepareDate(string strDate, string dateFormat)
        {
            DateTime date = DateTime.Parse(strDate);
            return date.ToString(dateFormat);
        }

        private int GetTaxRateID(string taxRate)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            dic.Add("0", 0);
            dic.Add("5", 5);
            dic.Add("10", 10);
            dic.Add("8", 8);
            dic.Add("-1", -1);

            return taxRate.Length > 0 ? dic[taxRate] : -1;
        }

        private class TaxBreakdowns
        {
            public Decimal taxPercentage { get; set; }
            public Decimal taxableAmount { get; set; }
            public Decimal taxAmount { get; set; }
            public bool taxableAmountPos { get; set; }
            public bool taxAmountPos { get; set; }
        }

        private async Task<JsonObject> UpdateInvoiceNumberAndSetResult(JsonObject model, JsonObject invoiceInfo)
        {
            var invoiceNo = invoiceInfo["invoiceNo"].ToString();
            var invoiceNumber = invoiceNo.Substring(6);
            var invoiceSeries = invoiceNo.Substring(0, 6);
            var reservationCode = invoiceInfo["reservationCode"].ToString();
            var transactionID = invoiceInfo["transactionID"].ToString();
            var supplierTaxCode = invoiceInfo["supplierTaxCode"].ToString();
            string id = model["id"].ToString();
            var updateInvoiceNumberDto = new UpdateInvoiceNumberDto
            {
                InvoiceSeries = invoiceSeries,
                InvoiceNumber = invoiceNumber,
                ReservationCode = reservationCode,
                TransactionID = transactionID,
                Id = id
            };
            await _invoiceAuthService.UpdateInvoiceNumberAsync(updateInvoiceNumberDto);
            return new JsonObject
            {
                { "ok", true },
                { "templateCode", model["templateCode"] },
                { "invoiceSeries", invoiceSeries },
                { "invoiceNumber", invoiceNumber },
                { "supplierTaxCode", supplierTaxCode },
                { "invoiceNo", invoiceNo },
                { "transactionID", transactionID },
                { "reservationCode", reservationCode }
            };
        }

        private JsonObject CreateInvoiceUsbTokenGetHash(JsonObject model)
        {
            var postData = PrepareJsonData(model);

            string url = apiLink + Url_CreateInvoiceUsbTokenGetHash + codeTax;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string response = webClient.UploadString(url, "POST", postData.ToString());

            var Jres = JsonObject.Parse(response);
            var objMessage = new JsonObject();

            if (Jres["errorCode"].ToString() == "200")
            {
                var invoiceInfo = (JsonObject)Jres["result"];
                if (invoiceInfo["hashString"] == null)
                {
                    objMessage.Add("ok", false);
                    objMessage.Add("error", "ID chứng từ đã được tạo hoá đơn");
                    objMessage.Add("InvoiceInfo", invoiceInfo);
                }
                else
                {
                    objMessage.Add("ok", true);
                    objMessage.Add("hashString", invoiceInfo["hashString"].ToString());
                }
            }
            else
            {
                objMessage.Add("ok", false);
                objMessage.Add("error", Jres["description"].ToString());
            }
            return objMessage;
        }

        private JsonObject CreateInvoiceUsbTokenInsertSignature(JsonObject postData)
        {
            string url = apiLink + Url_CreateInvoiceUsbTokenInsertSignature;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string response = webClient.UploadString(url, postData.ToString());

            var Jres = (JsonObject)response;
            var JsonObject = new JsonObject();

            if (Jres["errorCode"].ToString() == "200")
            {
                JsonObject.Add("ok", true);
                JsonObject.Add("result", Jres["result"]);
            }
            else
            {
                JsonObject.Add("ok", false);
                JsonObject.Add("error", Jres["description"].ToString());
            }

            return JsonObject;
        }

        public Task<JsonObject> CreateHKD(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> UpdateInvoiceStatus(JsonObject model)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
